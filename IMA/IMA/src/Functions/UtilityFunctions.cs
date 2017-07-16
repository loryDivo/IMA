using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace IMA
{
    /*
     * Classe per calcolo varie funzionalità
     */
    public static class UtilityFunctions
    {
        public static SKPoint ConvertToPixel(Point pt, SKCanvasView canvasBitMap)
        {
            return new SKPoint((float)(canvasBitMap.CanvasSize.Width * pt.X / canvasBitMap.Width),
                               (float)(canvasBitMap.CanvasSize.Height * pt.Y / canvasBitMap.Height));
        }
    }
}
