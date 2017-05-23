using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace IMA
{
    public static class UtilityFunctions
    {
        public static Size GetDisplayResolution()
        {
            Size resInfoSize = new Size();
            resInfoSize.Height = DependencyService.Get<IDisplay>().Height;
            resInfoSize.Width = DependencyService.Get<IDisplay>().Width;
            return resInfoSize;
        }
        public static SKPoint ConvertToPixel(Point pt, SKCanvasView canvasBitMap)
        {
            return new SKPoint((float)(canvasBitMap.CanvasSize.Width * pt.X / canvasBitMap.Width),
                               (float)(canvasBitMap.CanvasSize.Height * pt.Y / canvasBitMap.Height));
        }
    }
}
