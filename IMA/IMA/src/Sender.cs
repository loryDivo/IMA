using System;
using System.IO;
using System.Threading.Tasks;
using IMA.src;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;

namespace IMA
{
    public class Sender : ContentPage
    {

        private static string defaultTempDirectory = System.IO.Path.GetTempPath();
        private static string imageCompressDirectory = defaultTempDirectory + "compress.webp";
        private string txtFileDirectory = defaultTempDirectory + "coordinate.txt";
        private static string zipFileDirectory = defaultTempDirectory + "file.zip";
        private static string URLSend = "http://127.0.0.1:5000/upload";

        private string imageSource;
        private RectangleArea rectangleCoordinate;

        private ListView algorithmList;
        private Entry qualityAlgorithmEntry;
        private StackLayout senderLayout;

        public Sender(Page imageActionTools, string imageSource, RectangleArea rectangleCoordinate)
        {
            Navigation.RemovePage(imageActionTools);
            this.imageSource = imageSource;
            this.rectangleCoordinate = rectangleCoordinate;
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

            qualityAlgorithmEntry = new Entry
            {
                Placeholder = "enter quality"
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
            Boolean processComplete = CompressImage(CompressionAlgorithmSelectedMethods.AlgorithmSelected(algorithmList.SelectedItem), qualityAlgorithmEntry.Text);
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
            if (CheckCorrectQualityValue(quality))
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
                        return DependencyService.Get<ICompressorAlgorithm>().CallJPEGCompressorAlgorithm(imageSource, imageCompressDirectory, quality);
                    case CompressionAlgorithmSelected.none:
                        DisplayAlert("Errore selezione algoritmo di compressione'", "Selezionare algoritmo di compressione", "OK");
                        return false;
                }
            }
            DisplayAlert("Errore selezione qualita'", "Selezionare qualita' valida", "OK");
            return false;
        }

        private bool CheckCorrectQualityValue(string qualityValue)
        {
            return true ? qualityValue != null && Convert.ToInt32(qualityValue) >= 0 && Convert.ToInt32(qualityValue) <= 100
            : false;
        }

        private bool SendFileToServer()
        {
            try
            {
                CreateTxtFile();
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
                var upTxtBytes = File.ReadAllBytes(txtFileDirectory);

                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                ByteArrayContent imageContent = new ByteArrayContent(upImageBytes);
                content.Add(imageContent, "image", "image.webp");

                ByteArrayContent txtContent = new ByteArrayContent(upTxtBytes);
                content.Add(txtContent, "coordinate", "coordinate.txt");

                var response =
                    await client.PostAsync(URLSend, content);

                var responsestr = response.Content.ReadAsStringAsync().Result;
                
            }
            catch (Exception e)
            {
                throw new Exception("Errore invio file");
            }
        }

        private void CreateTxtFile()
        {
            FileStream fs = null;
            try
            {
                if (!File.Exists(defaultTempDirectory))
                {
                    fs = File.Create(txtFileDirectory);
                    StreamWriter sw = new StreamWriter(fs);
                    if (CoordinateRectangleExist())
                    {
                        sw.WriteLine("Coordinae left top " + rectangleCoordinate.ScaleLeftTopPixelCoordinate);
                        sw.WriteLine("Coordinae left bottom " + rectangleCoordinate.ScaleLeftBottomPixelCoordinate);
                        sw.WriteLine("Coordinae right top " + rectangleCoordinate.ScaleRightTopPixelCoordinate);
                        sw.WriteLine("Coordinae right bottom " + rectangleCoordinate.ScaleRightBottomPixelCoordinate);
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

        private bool CoordinateRectangleExist()
        {
            return rectangleCoordinate != null;
        }

    }
}

