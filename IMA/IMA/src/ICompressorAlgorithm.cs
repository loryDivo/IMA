
using System;

namespace IMA.src
{
    public interface ICompressorAlgorithm
    {
        int CallCompressorAlgorithm(string imageSource, string imageDestination);
    }
}
