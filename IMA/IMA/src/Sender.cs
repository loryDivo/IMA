using System;
using IMA.src;
using Xamarin.Forms;

namespace IMA
{
    public class Sender : ContentPage
    {

        private static string defaultTempDirectory = System.IO.Path.GetTempPath();
        private static string imageCompressDirectory = defaultTempDirectory + "compress.webp";

        private string imageSource;
        private Rectangle rectCoordinate;
        private StackLayout senderLayout;
        public Sender(Page imageActionTools, string imageSource, Rectangle rectangleCoordinate)
        {
            Navigation.RemovePage(imageActionTools);
            this.imageSource = imageSource;
            this.rectCoordinate = rectangleCoordinate;
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
            senderLayout.Children.Add(btnProcessImage);

            Content = senderLayout;

        }

        private void OnButtonClickProcessImage(object sender, EventArgs e)
        {
            Boolean processComplete = CompressImage();
            if (!processComplete)
            {
                DisplayAlert("Errore di compressione", "Vi è stato un errore di compressione", "OK");
               
            }
            else
            {
                bool sendFile;
                sendFile = SendFileToServer();
                if (!sendFile)
                {
                    
                    var userResponse = DisplayAlert("Errore di invio", "Vi è stato un errore di invio", "OK");
                    if (userResponse.IsCompleted)
                    {
                        Navigation.RemovePage(this);
                    }
                }
            }
        }

        private bool CompressImage()
        {
            int result = DependencyService.Get<ICompressorAlgorithm>().CallCompressorAlgorithm(imageSource, imageCompressDirectory);
            if(result == 0)
            {
                return true;
            }
            
            return false;
        }

        private bool SendFileToServer()
        {
            return false;
        }
    }
}

