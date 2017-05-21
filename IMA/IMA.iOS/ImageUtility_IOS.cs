using System;
using Xamarin.Forms;
using IMA.src;
using IMA.IOS;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(ImageUtility_IOS))]
namespace IMA.IOS
{
    class ImageUtility_IOS : ImageUtility
    {
        public Size GetImageSize(String fileName)
        {
            UIImage image = UIImage.FromFile(fileName);
            return new Size((double)image.Size.Width, (double)image.Size.Height);
        }
    }
}
