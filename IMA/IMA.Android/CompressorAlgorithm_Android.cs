using IMA.src;
using IMA.Droid;
using System.Runtime.InteropServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(CompressorAlgorithm_Android))]
namespace IMA.Droid
{
    public class CompressorAlgorithm_Android : ICompressorAlgorithm
    {
        public int CallCompressorAlgorithm(string imageSource, string imageDestination)
        {
            MainActivity activity = Forms.Context as MainActivity;
            int result = -1;
            activity.RunOnUiThread(() =>
            {
                result = WEBPCompressorAlgorithm(imageSource, imageDestination);
            });
            return result;
        }
        [DllImport("libWEBPAlgorithmTools", EntryPoint = "WEBPEncode")]

        public static extern int WEBPCompressorAlgorithm(string imageSource, string imageDestination);
    }
}