using System;
using IMA.IOS;
using IMA.src.IDevice;
using Foundation;
using UIKit;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PicturePicker_IOS))]
namespace IMA.IOS
{
    public class PicturePicker_IOS : IPicturePicker
    {
        TaskCompletionSource<Stream> taskCompletionSource;
        UIImagePickerController imagePicker;
        private string imagePath;
        public Task<Stream> GetImageStreamAsync()
        {
            // Create and define UIImagePickerController
            imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
            };

            // Set event handlers
            imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            imagePicker.Canceled += OnImagePickerCancelled;

            // Present UIImagePickerController;
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentModalViewController(imagePicker, true);

            // Return Task object
            taskCompletionSource = new TaskCompletionSource<Stream>();
            return taskCompletionSource.Task;
        }

        void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        {
            imagePath = DetectFullPath(args.ReferenceUrl.AbsoluteString);
            
            UIImage image = args.EditedImage ?? args.OriginalImage;
            
            if (image != null)
            {
                // Convert UIImage to .NET Stream object
                NSData data = image.AsJPEG(1);
                Stream stream = data.AsStream();

                // Set the Stream as the completion of the Task
                taskCompletionSource.SetResult(stream);
            }
            else
            {
                taskCompletionSource.SetResult(null);
            }
            imagePicker.DismissModalViewController(true);
        }

        void OnImagePickerCancelled(object sender, EventArgs args)
        {
            taskCompletionSource.SetResult(null);
            imagePicker.DismissModalViewController(true);
        }

        public string DetectFullPath(string fileName)
        {
            //Any old images with a full file path stored should have the front part removed
            if (fileName.Contains("/"))
            {
                fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
            }
            var docsDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            return System.IO.Path.Combine(docsDir, fileName);
        }
        public string GetImageRealPath()
        {
            return imagePath;
        }
    }
}