using System.Runtime.InteropServices;
using IMA.src;
using IMA.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(CompressorAlgorithm_UWP))]
namespace IMA.UWP
{
    public class CompressorAlgorithm_UWP : ICompressorAlgorithm
    {
        public int CallWEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality)
        {
            int result = -1;
            result = WEBPCompressorAlgorithm(imageSource, imageDestination, quality);
            return result;
        }
        public bool CallJPEGCompressorAlgorithm(string imageSource, string imageDestination, string quality)
        {
            
            return true;
        }

        [DllImport("WEPAlgorithmTools.UWP.dll", EntryPoint = "WEBPEncode")]

        public static extern int WEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality);

        public string removeDoubleBackslashes(string input)
        {
            char[] separator = new char[1] { '\\' };
            string result = "";
            string[] subResult = input.Split(separator);
            for (int i = 0; i <= subResult.Length - 1; i++)
            {
                result = i < subResult.Length - 1 ? result + subResult[i] + "\\" : result + subResult[i];
            }
            return result;
        }
    }
}
