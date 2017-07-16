using IMA.src.IDevice;
using SkiaSharp;
using Xamarin.Forms;

namespace IMA.src
{

    /*
     * Rappresentazione dell'immagine principale
     * Sono presenti i vari attributi che rappresentano le coordinate dell'immagine sia 
     * su schermo che reali (per compressione)
     */
    public class BitMapArea : RectangleShapeArea
    {
        private string bitMapDirectorySource;
        private float topBitMapImg;
        private float bottomBitMapImg;
        private float leftBitMapImg;
        private float rightBitMapImg;

        private SKPoint leftTopPixelCoordinateBitMap;
        private SKPoint leftBottomPixelCoordinateBitMap;
        private SKPoint rightTopPixelCoordinateBitMap;
        private SKPoint rightBottomPixelCoordinateBitMap;

        private SKBitmap bitMap;

        private float height;
        private float width;

        private float scaleHeight;
        private float scaleWidth;
        private float previousScaleHeight;
        private float previousScaleWidth;

        public BitMapArea(SKBitmap bitMap)
        {
            this.bitMap = bitMap;
            this.bitMapDirectorySource = DependencyService.Get<IPicturePicker>().GetImageRealPath();
        }

        public string BitMapDirectorySource { get => bitMapDirectorySource; }
        public SKBitmap BitMap { get => bitMap; }
        public float Top { get => topBitMapImg; set => topBitMapImg = value; }
        public float Left { get => leftBitMapImg; set => leftBitMapImg = value; }
        public float Right { get => rightBitMapImg; set => rightBitMapImg = value; }
        public float Bottom { get => bottomBitMapImg; set => bottomBitMapImg = value; }
        public SKPoint LeftTopPixelCoordinate { get => leftTopPixelCoordinateBitMap; }
        public SKPoint LeftBottomPixelCoordinate { get => leftBottomPixelCoordinateBitMap; }
        public SKPoint RightTopPixelCoordinate { get => rightTopPixelCoordinateBitMap; }
        public SKPoint RightBottomPixelCoordinate { get => rightBottomPixelCoordinateBitMap; }
        public float Height { get => height; }
        public float Width { get => width; }
        public float ScaleHeight { get => scaleHeight; }
        public float ScaleWidth { get => scaleWidth; }
        public float PreviousScaleHeight { get => previousScaleHeight; }
        public float PreviousScaleWidth { get => previousScaleWidth; }

        /*
         * Calcolo coordinate vertici
         */
        public void CalculateVertexCoordinate()
        {
            leftTopPixelCoordinateBitMap = new SKPoint(Left, Top);
            rightTopPixelCoordinateBitMap = new SKPoint(Right, Top);
            leftBottomPixelCoordinateBitMap = new SKPoint(Left, Bottom);
            rightBottomPixelCoordinateBitMap = new SKPoint(Right, Bottom);
        }

        /*
         * Calcolo grandezza bitmap
         */
        public void CalculateBitMapSize(SKImageInfo info, Display display)
        {
            Left = (info.Width - display.AspectRatio * BitMap.Width) / 2;
            Top = (info.Height - display.AspectRatio * BitMap.Height) / 2;
            Right = Left + display.AspectRatio * BitMap.Width;
            Bottom = Top + display.AspectRatio * BitMap.Height;
            width = Right - Left;
            height = Bottom - Top;

            previousScaleHeight = scaleHeight;
            previousScaleWidth = scaleWidth;
            scaleHeight = (float)BitMap.Height / (float)Height;
            scaleWidth = (float)BitMap.Width / (float)Width;
        }

    }
}
