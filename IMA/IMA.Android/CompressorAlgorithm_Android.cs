using IMA.src;
using IMA.Droid;
using System.Runtime.InteropServices;
using Xamarin.Forms;
using Android.Graphics;
using System.IO;
using System;

[assembly: Dependency(typeof(CompressorAlgorithm_Android))]
namespace IMA.Droid
{
    public class CompressorAlgorithm_Android : ICompressorAlgorithm
    {
        public int CallWEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality)
        {
            MainActivity activity = Forms.Context as MainActivity;
            int result = -1;
            activity.RunOnUiThread(() =>
            {
                result = WEBPCompressorAlgorithm(imageSource, imageDestination);
            });
            return result;
        }

        public bool CallJPEGCompressorAlgorithm(string imageSource, string imageDestination, string quality)
        {
            Bitmap bitMap = BitmapFactory.DecodeFile(imageSource);
            bool result = false;
            using (Stream stream = File.Create(imageDestination))
            {
                result = bitMap.Compress(Bitmap.CompressFormat.Jpeg, Convert.ToInt32(quality), stream);
            }
            return result;
        }

        [DllImport("libWEBPAlgorithmTools", EntryPoint = "WEBPEncode")]

        public static extern int WEBPCompressorAlgorithm(string imageSource, string imageDestination);
    }
}