using System;
using System.Runtime.InteropServices;
using IMA.src;
using IMA.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(CompressorAlgorithm_UWP))]
namespace IMA.UWP
{
    public class CompressorAlgorithm_UWP : ICompressorAlgorithm
    {
        public int CallCompressorAlgorithm(string imageSource, string imageDestination)
        {
            int result = -1;
            result = WEBPCompressorAlgorithm(imageSource, imageDestination);
            return result;
        }
        [DllImport("WEPAlgorithmTools.UWP.dll", EntryPoint = "WEBPEncode")]

        public static extern int WEBPCompressorAlgorithm(string imageSource, string imageDestination);
    }
}
