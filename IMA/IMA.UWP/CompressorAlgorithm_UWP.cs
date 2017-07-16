using System.Runtime.InteropServices;
using IMA.src;
using IMA.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(CompressorAlgorithm_UWP))]
namespace IMA.UWP
{
  /*
  * Classe wrapping compressione UWP
  */
    public class CompressorAlgorithm_UWP : ICompressorAlgorithm
    {
        public int CallWEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality)
        {
            int result = -1;
            result = WEBPCompressorAlgorithm(imageSource, imageDestination, quality);
            return result;
        }

        [DllImport("WEPAlgorithmTools.UWP.dll", EntryPoint = "WEBPEncode")]

        public static extern int WEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality);

    }
}
