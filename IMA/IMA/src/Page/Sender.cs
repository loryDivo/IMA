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
    /*
    * Pagina dedicata alla compressione e invio
    */
    public class Sender : ContentPage
    {

        private static string defaultTempDirectory = System.IO.Path.GetTempPath();
        private static string imageCompressDirectory = defaultTempDirectory + "compress";
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

    /*
    * Avvio compressione e invio
    */

        private void OnButtonClickProcessImage(object sender, EventArgs e)
        {
            try
            {
                CompressionAlgorithmSelected compressionAlgorithmSelected = CompressionAlgorithmSelectedMethods.AlgorithmSelected(algorithmList.SelectedItem);
                CompressImage(compressionAlgorithmSelected, Convert.ToString(qualityAlgorithmEntry.Value));
                SendFileToServer(compressionAlgorithmSelected);
            }
            catch(Exception ex)
            {
                DisplayAlert("Errore", ex.Message, "OK");
            }
        }

    /*
    * Avvio compressione in base ad algoritmo scelto
    */

        private void CompressImage(CompressionAlgorithmSelected info, string quality)
        {
            switch (info)
            {
                case CompressionAlgorithmSelected.WEBPAlgorithm:
                    int resultWEBP = DependencyService.Get<ICompressorAlgorithm>().CallWEBPCompressorAlgorithm(imageSource, imageCompressDirectory + ".webp", quality);
                    if (resultWEBP != 0)
                    {
                        throw new Exception("Errore di compressione");
                    }
                    break;
                case CompressionAlgorithmSelected.JPEGAlgorithm:
                    if(!JPEGCompressorAlgorithm.JPEGCompressor(imageSource, imageCompressDirectory + ".jpg", Convert.ToInt32(quality)))
                    {
                        throw new Exception("Errore di compressione");
                    }
                    break;
                case CompressionAlgorithmSelected.none:
                    throw new Exception("Errore selezione algoritmo di compressione'");
            }
        }

    /*
    * Invio file al server in base ad algoritmo scelto
    */

        private void SendFileToServer(CompressionAlgorithmSelected compressionAlgorithmSelected)
        {
            ImageExtension imageExtension = ImageExtensionMethods.GetImageExtension(compressionAlgorithmSelected);
            try
            {
                CreateJSONFiles();
                Send(imageExtension);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async void Send(ImageExtension imageExtension)
        {
            string extensionImage = ImageExtensionMethods.GetImageExtensionString(imageExtension);
            try
            {
                var upImageBytes = File.ReadAllBytes(imageCompressDirectory + extensionImage);
                var upJSONBytes = File.ReadAllBytes(JSONFileDirectory);

                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                ByteArrayContent imageContent = new ByteArrayContent(upImageBytes);
                string imageSendName = "image" + extensionImage;
                content.Add(imageContent, "image", imageSendName);

                ByteArrayContent JSONContent = new ByteArrayContent(upJSONBytes);
                content.Add(JSONContent, "coordinate", "coordinate.txt");

                var response =
                    await client.PostAsync(URLSend, content);

                var responsestr = response.Content.ReadAsStringAsync().Result;
                
            }
            catch (Exception e)
            {
                await DisplayAlert("Errore invio file", "Errore connessione con il server " + e.Source, "OK");
            }
        }

    /*
    * Creazione file JSON con informazioni bounding box
    */

        private void CreateJSONFiles()
        { 
            try
            {
                if (!File.Exists(defaultTempDirectory))
                {
                    CreateJSON();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Errore creazione file di testo " + e.Source);
            }
        }

        private void CreateJSON()
        {
            FileStream fs = null;
            fs = File.Create(JSONFileDirectory);
            StreamWriter sw = new StreamWriter(fs);
            if (CoordinateRectangleExist())
            {
                List<JSONRectangleMapping> jsonRectangleMapping = new List<JSONRectangleMapping>();
                foreach (RectangleArea rectangleArea in allRectangleCoordinate)
                {
                    jsonRectangleMapping.Add(new JSONRectangleMapping(rectangleArea.Id, rectangleArea.ScaleLeftTopPixelCoordinate, rectangleArea.ScaleLeftBottomPixelCoordinate,
                                                rectangleArea.ScaleRightTopPixelCoordinate, rectangleArea.ScaleRightBottomPixelCoordinate));
                }
                StringBuilder str = new StringBuilder();
                str.Append("[");
                int count = 0;
                foreach (JSONRectangleMapping jsonRectangleMap in jsonRectangleMapping)
                {
                    count++;
                    str.Append(jsonRectangleMap.CreateJSONString());
                    if (count != jsonRectangleMapping.Count)
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

        private bool CoordinateRectangleExist()
        {
            return allRectangleCoordinate != null;
        }

    }
}

