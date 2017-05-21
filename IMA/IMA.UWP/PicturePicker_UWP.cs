using IMA.src.IDevice;
using IMA.UWP;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Xamarin.Forms;

[assembly: Dependency(typeof(PicturePicker_UWP))]
namespace IMA.UWP
{
    public class PicturePicker_UWP : IPicturePicker
    {
        private string imagePath;
        public async Task<Stream> GetImageStreamAsync()
        {
            // Create and initialize the FileOpenPicker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };

            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            // Get a file and return a Stream
            StorageFile storageFile = await openPicker.PickSingleFileAsync();
            imagePath = storageFile.Path;
            if (storageFile == null)
            {
                return null;
            }

            IRandomAccessStreamWithContentType raStream = await storageFile.OpenReadAsync();
            return raStream.AsStreamForRead();
        }
        public string GetImageRealPath()
        {
            return imagePath;
        }

    }
}