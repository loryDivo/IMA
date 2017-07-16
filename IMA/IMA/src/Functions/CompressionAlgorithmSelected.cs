
namespace IMA.src
{

    /*
     * Classe enum per scelta degli algoritmi di compressione
     */

    public enum CompressionAlgorithmSelected
    {
        WEBPAlgorithm,
        JPEGAlgorithm,
        none,
    }
    public static class CompressionAlgorithmSelectedMethods
    {
        public static CompressionAlgorithmSelected AlgorithmSelected(object algorithmSelected)
        {
            if(algorithmSelected == null)
            {
                return CompressionAlgorithmSelected.none;
            }
            if (algorithmSelected.Equals("WEBPAlgorithm"))
            {
                return CompressionAlgorithmSelected.WEBPAlgorithm;
            }
            if (algorithmSelected.Equals("JPEGAlgorithm"))
            {
                return CompressionAlgorithmSelected.JPEGAlgorithm;
            }
            return CompressionAlgorithmSelected.none;
        }
    }
}
