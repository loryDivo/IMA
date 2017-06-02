
using System;

namespace IMA.src
{
    public interface ICompressorAlgorithm
    {
        int CallWEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality);
    }
}
