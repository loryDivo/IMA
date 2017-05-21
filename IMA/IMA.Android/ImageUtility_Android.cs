using IMA.Droid;
using Android.Graphics;
using Xamarin.Forms;
using IMA.src;

[assembly : Dependency(typeof(ImageUtility_Android))]
namespace IMA.Droid
{
    class ImageUtility_Android : ImageUtility
    {
        public Size GetImageSize(string fileName)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            fileName = fileName.Replace(".png", "").Replace(".jpg", "");
            var resField = typeof(Resource.Drawable).GetField(fileName);
            var resID = (int)resField.GetValue(null);

            BitmapFactory.DecodeResource(Forms.Context.Resources, resID, options);
            return new Size((double)options.OutWidth, (double)options.OutHeight);
        } 
    }
}