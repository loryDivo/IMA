using IMA.IOS;
using IMA.src;
using System.Runtime.InteropServices;
[assembly: Xamarin.Forms.Dependency(typeof(CompressorAlgorithm_IOS))]
namespace IMA.IOS
{
    public class CompressorAlgorithm_IOS : ICompressorAlgorithm
    {
        public int CallCompressorAlgorithm(string imageSource, string imageDestination)
        {
            int result = -1;
            result = WEBPCompressorAlgorithm(imageSource, imageDestination);
            return result;
        }
        [DllImport("__Internal", EntryPoint = "WEBPEncode")]

        public static extern int WEBPCompressorAlgorithm(string imageSource, string imageDestination);
    }
}
