using System;
using System.IO;
using System.Threading.Tasks;
using IMA.src;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;

namespace IMA
{
    public class Sender : ContentPage
    {

        private static string defaultTempDirectory = System.IO.Path.GetTempPath();
        private static string imageCompressDirectory = defaultTempDirectory + "compress.webp";
        private string JSONFileDirectory = defaultTempDirectory + "coordinate.txt";
        private static string URLSend = "http://127.0.0.1:5000/upload";

        private string imageSource;
        private List<RectangleArea> allRectangleCoordinate;

        private ListView algorithmList;
        private Slider qualityAlgorithmEntry;
        private StackLayout senderLayout;

        public Sender(Page imageActionTools, string imageSource, List<RectangleArea> allRectangleCoordinate)
        {
            this.imageSource = imageSource;
            this.allRectangleCoordinate = allRectangleCoordinate;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Label informationAlgorithm = new Label
            {
                Text = "Image information",
                TextColor = Color.Black,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };

            Label imageSourceText = new Label
            {
                Text = "Image source " + imageSource,
                TextColor = Color.Black,
                FontSize = 10,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };

            Label imageDestinationText = new Label
            {
                Text = "Image Destination " + defaultTempDirectory,
                TextColor = Color.Black,
                FontSize = 10,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };

            Label algorithmWriterText = new Label
            {
                Text = "Select compression algorithm",
                TextColor = Color.Black,
                FontSize = 10,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };


            List<String> algorithmName = new List<String>
            {
                "WEBPAlgorithm",
                "JPEGAlgorithm",
            };

            algorithmList = new ListView
            {
                ItemsSource = algorithmName,

            };

            Label qualityAlgorithmLevelText = new Label
            {
                Text = "Write quality of algorithm",
                TextColor = Color.Black,
                FontSize = 10,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };

            qualityAlgorithmEntry = new Slider
            {
                Maximum = 100,
                Minimum = 0,
                Value = 50,
            };

            Button btnProcessImage = new Button
            {
                Text = "Process Image",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            btnProcessImage.Clicked += OnButtonClickProcessImage;

            senderLayout = new StackLayout();
            senderLayout.Children.Add(imageSourceText);
            senderLayout.Children.Add(imageDestinationText);
            senderLayout.Children.Add(algorithmWriterText);
            senderLayout.Children.Add(algorithmList);
            senderLayout.Children.Add(qualityAlgorithmLevelText);
            senderLayout.Children.Add(qualityAlgorithmEntry);
            senderLayout.Children.Add(btnProcessImage);

            Content = senderLayout;

        }

        private void OnButtonClickProcessImage(object sender, EventArgs e)
        {
            Boolean processComplete = CompressImage(CompressionAlgorithmSelectedMethods.AlgorithmSelected(algorithmList.SelectedItem), Convert.ToString(qualityAlgorithmEntry.Value));
            Task userResponse;
            if (!processComplete)
            {
                userResponse = DisplayAlert("Errore di compressione", "Vi è stato un errore di compressione", "OK");
                if (userResponse.IsCompleted)
                {
                    Navigation.RemovePage(this);
                }
            }
            else
            {
                bool sendFile;
                sendFile = SendFileToServer();
                if (!sendFile)
                {
                    userResponse = DisplayAlert("Errore di invio", "Vi è stato un errore di invio", "OK");
                    if (userResponse.IsCompleted)
                    {
                        Navigation.RemovePage(this);
                    }
                }
            }
        }

        private bool CompressImage(CompressionAlgorithmSelected info, string quality)
        {
            switch (info)
            {
                case CompressionAlgorithmSelected.WEBPAlgorithm:
                    int resultWEBP = DependencyService.Get<ICompressorAlgorithm>().CallWEBPCompressorAlgorithm(imageSource, imageCompressDirectory, quality);
                    if (resultWEBP == 0)
                    {
                        return true;
                    }
                    return false;
                case CompressionAlgorithmSelected.JPEGAlgorithm:
                    return JPEGCompressorAlgorithm.JPEGCompressor(imageSource, imageCompressDirectory, Convert.ToInt32(quality));
                case CompressionAlgorithmSelected.none:
                    DisplayAlert("Errore selezione algoritmo di compressione'", "Selezionare algoritmo di compressione", "OK");
                    return false;
            }
            DisplayAlert("Errore selezione qualita'", "Selezionare qualita' valida", "OK");
            return false;
        }

        private bool SendFileToServer()
        {
            try
            {
                CreateJSONFiles();
                Send();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        private async void Send()
        {
            try
            {
                var upImageBytes = File.ReadAllBytes(imageCompressDirectory);
                var upJSONBytes = File.ReadAllBytes(JSONFileDirectory);

                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                ByteArrayContent imageContent = new ByteArrayContent(upImageBytes);
                content.Add(imageContent, "image", "image.webp");

                ByteArrayContent JSONContent = new ByteArrayContent(upJSONBytes);
                content.Add(JSONContent, "coordinate", "coordinate.txt");

                var response =
                    await client.PostAsync(URLSend, content);

                var responsestr = response.Content.ReadAsStringAsync().Result;
                
            }
            catch (Exception e)
            {
                throw new Exception("Errore invio file");
            }
        }

        private void CreateJSONFiles()
        {
            if (CoordinateRectangleExist())
            {
                List<JSONRectangleMapping> jsonRectangleMapping = new List<JSONRectangleMapping>();
                foreach (RectangleArea rectangleArea in allRectangleCoordinate)
                {
                    jsonRectangleMapping.Add(new JSONRectangleMapping(rectangleArea.Id, rectangleArea.ScaleLeftTopPixelCoordinate, rectangleArea.ScaleLeftBottomPixelCoordinate,
                                                rectangleArea.ScaleRightTopPixelCoordinate, rectangleArea.ScaleRightBottomPixelCoordinate));
                }
                FileStream fs = null;
                try
                {
                    if (!File.Exists(defaultTempDirectory))
                    {
                        fs = File.Create(JSONFileDirectory);
                        StreamWriter sw = new StreamWriter(fs);
                        if (CoordinateRectangleExist())
                        {
                            StringBuilder str = new StringBuilder();
                            str.Append("[");
                            int count = 0;
                            foreach (JSONRectangleMapping jsonRectangleMap in jsonRectangleMapping)
                            {
                                count++;
                                str.Append(jsonRectangleMap.CreateJSONString());
                                if(count != jsonRectangleMapping.Count)
                                {
                                    str.Append(",");
                                }
                            }
                            str.Append("]");
                            sw.WriteLine(str);
                        }
                        else
                        {
                            sw.WriteLine("User not select rectangle coordinate");
                        }
                        sw.Dispose();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Errore creazione file di testo");
                }
            }
            else
            {
                DisplayAlert("Nessun rettangolo creato", "Nessun rettangolo creato, verrà procecssata l'intera immagine", "OK");
            }
        }

        private bool CoordinateRectangleExist()
        {
            return allRectangleCoordinate != null;
        }

    }
}

