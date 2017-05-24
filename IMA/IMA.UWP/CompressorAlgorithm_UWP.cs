using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using IMA.src;
using IMA.UWP;
using Windows.Storage;
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
