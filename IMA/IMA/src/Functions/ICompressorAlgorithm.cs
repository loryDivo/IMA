
namespace IMA.src
{
    /*
     * Interfaccia wrapping di chiamata ad algoritmo WebP di compressione
     */
    public interface ICompressorAlgorithm
    {
        int CallWEBPCompressorAlgorithm(string imageSource, string imageDestination, string quality);
    }
}
