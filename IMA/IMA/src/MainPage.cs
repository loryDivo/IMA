using System;
using SkiaSharp;
using Xamarin.Forms;
using System.IO;
using IMA.src.IDevice;
using IMA.src;

namespace IMA
{
    public class MainPage : ContentPage
    {
        private Image imageDefault;
        private SKBitmap bitMap;
        private Boolean loadBitMap = false;
        public MainPage()
        {

            Label homeTitle = new Label
            {
                Text = "IMA",
                TextColor = Color.Black,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };

            Label descriptionTitle = new Label
            {
                Text = "Image Management Application",
                TextColor = Color.Black,
                FontSize = 10,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };

            Button btnChooseImg = new Button
            {
                Text = "Choose image",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            btnChooseImg.Clicked += OnButtonClickedChoosePhotos;

            Button btnTapImg = new Button
            {
                Text = "Tap image",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            btnTapImg.Clicked += OnButtonClickedTapPhotos;

            imageDefault = new Image
            {
                Source = ImageSource.FromFile("mare.jpg"),
                VerticalOptions = LayoutOptions.CenterAndExpand,

            };

            StackLayout mainLayout = new StackLayout();
            ScrollView scrollView = new ScrollView();

            mainLayout.Spacing = 10;
            mainLayout.Children.Add(homeTitle);
            mainLayout.Children.Add(descriptionTitle);
            mainLayout.Children.Add(imageDefault);
            mainLayout.Children.Add(btnTapImg);
            mainLayout.Children.Add(btnChooseImg);

            Content = mainLayout;

        }

        async void OnButtonClickedChoosePhotos(object sender, EventArgs e)
        {
            IPicturePicker picturePicker = DependencyService.Get<IPicturePicker>();
            using (Stream stream = await picturePicker.GetImageStreamAsync())
            {
                if (stream != null)
                {
                    loadBitMap = true;
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        stream.CopyTo(memStream);
                        memStream.Seek(0, SeekOrigin.Begin);

                        using (SKManagedStream skStream = new SKManagedStream(memStream))
                        {
                            bitMap = SKBitmap.Decode(skStream);
                        }
                    }
                }
                else
                {
                    loadBitMap = false;
                }
            }
            CallImageActionTools();

        }
        //TODO REFACTORING
        async void OnButtonClickedTapPhotos(object sender, EventArgs e)
        {
            /*
           await CrossMedia.Current.Initialize();

            Image imageTapped = new Image();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                //DisplayAlert("No Camera", "There is no camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                SaveToAlbum = true,
            });
            if (file == null)
            {
                return;
            }

            imageTapped.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
            CallImageActionTools();
            */
            
        }

        private void CallImageActionTools()
        {
            if (loadBitMap)
            {
                Navigation.PushAsync(new ImageActionTools(bitMap));
            }
            else
            {
                DisplayAlert("Could not load image", "ERROR: Could not load image, try again", "OK");
            }
        }
    }
}
