using System;
using System.Diagnostics;
using System.IO;
using BitMiracle.LibJpeg;
using BitMiracle.LibJpeg.Classic;

    /*
     * Classe per chiamata ad algoritmo JPEG di compressione
     */

public static class JPEGCompressorAlgorithm
{

    public static bool JPEGCompressor(string imageSource, string imageDestination, int quality)
    {
        try
        {
            CompressionParameters compressParam = new CompressionParameters();
            compressParam.Quality = quality;
            using (Stream streamDestination = File.Create(imageDestination))
            {
                JpegImage jpegCompressor = new JpegImage(File.Open(imageSource, FileMode.Open));
                jpegCompressor.WriteJpeg(streamDestination, compressParam);
            }
        }
        catch(Exception e)
        {
            return false;
        }
        return true;
    }
}