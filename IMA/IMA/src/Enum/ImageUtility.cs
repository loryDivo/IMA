
namespace IMA.src
{
    public enum ImageExtension
    {
        JPEG,
        WEBP,
        none,
    }
    public static class ImageExtensionMethods{
        public static ImageExtension GetImageExtension(CompressionAlgorithmSelected compressorAlgorithmSelected)
        {
            switch (compressorAlgorithmSelected)
            {
                case (CompressionAlgorithmSelected.WEBPAlgorithm):
                    return ImageExtension.WEBP;

                case (CompressionAlgorithmSelected.JPEGAlgorithm):
                    return ImageExtension.JPEG;
            }
            return ImageExtension.none;
        }
        public static string GetImageExtensionString(this ImageExtension imageExtension)
        {
            switch (imageExtension)
            {
                case ImageExtension.WEBP:
                    return ".webp";
                case ImageExtension.JPEG:
                    return ".jpg";
            }
            return "error";
        }
    }
}
