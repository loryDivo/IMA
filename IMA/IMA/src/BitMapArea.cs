using System;
using System.Collections.Generic;
using System.Text;
using IMA.src.IDevice;
using SkiaSharp;
using Xamarin.Forms;

namespace IMA.src
{
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

        private SKPoint scaleLeftTopPixelCoordinateBitMap;
        private SKPoint scaleLeftBottomPixelCoordinateBitMap;
        private SKPoint scaleRightTopPixelCoordinateBitMap;
        private SKPoint scaleRightBottomPixelCoordinateBitMap;

        private float scaleOffSetTop;
        private float scaleOffSetLeft;
        private float scaleOffSetRight;
        private float scaleOffSetBottom;

        private SKBitmap bitMap;

        private float height;
        private float width;

        private float scaleHeight;
        private float scaleWidth;

        public BitMapArea(SKBitmap bitMap)
        {
            this.bitMap = bitMap;
            this.BitMapDirectorySource = DependencyService.Get<IPicturePicker>().GetImageRealPath();
        }

        public string BitMapDirectorySource { get => bitMapDirectorySource; set => bitMapDirectorySource = value; }
        public SKBitmap BitMap { get => bitMap; set => bitMap = value; }
        public float Top { get => topBitMapImg; set => topBitMapImg = value; }
        public float Left { get => leftBitMapImg; set => leftBitMapImg = value; }
        public float Right { get => rightBitMapImg; set => rightBitMapImg = value; }
        public float Bottom { get => bottomBitMapImg; set => bottomBitMapImg = value; }
        public SKPoint LeftTopPixelCoordinate { get => leftTopPixelCoordinateBitMap; set => leftTopPixelCoordinateBitMap = value; }
        public SKPoint LeftBottomPixelCoordinate { get => leftBottomPixelCoordinateBitMap; set => leftBottomPixelCoordinateBitMap = value; }
        public SKPoint RightTopPixelCoordinate { get => rightTopPixelCoordinateBitMap; set => rightTopPixelCoordinateBitMap = value; }
        public SKPoint RightBottomPixelCoordinate { get => rightBottomPixelCoordinateBitMap; set => rightBottomPixelCoordinateBitMap = value; }
        public SKPoint ScaleLeftTopPixelCoordinate { get => scaleLeftTopPixelCoordinateBitMap; set => scaleLeftTopPixelCoordinateBitMap = value; }
        public SKPoint ScaleLeftBottomPixelCoordinate { get => scaleLeftBottomPixelCoordinateBitMap; set => scaleLeftBottomPixelCoordinateBitMap = value; }
        public SKPoint ScaleRightTopPixelCoordinate { get => scaleRightTopPixelCoordinateBitMap; set => scaleRightTopPixelCoordinateBitMap = value; }
        public SKPoint ScaleRightBottomPixelCoordinate { get => scaleRightBottomPixelCoordinateBitMap; set => scaleRightBottomPixelCoordinateBitMap = value; }
        public float ScaleOffSetTop { get => scaleOffSetTop; set => scaleOffSetTop = value; }
        public float ScaleOffSetLeft { get => scaleOffSetLeft; set => scaleOffSetLeft = value; }
        public float ScaleOffSetRight { get => scaleOffSetRight; set => scaleOffSetRight = value; }
        public float ScaleOffSetBottom { get => scaleOffSetBottom; set => scaleOffSetBottom = value; }
        public float Height { get => height; set => height = value; }
        public float Width { get => width; set => width = value; }
        public float ScaleHeight { get => scaleHeight; set => scaleHeight = value; }
        public float ScaleWidth { get => scaleWidth; set => scaleWidth = value; }

        public void CalculateVertexCoordinate()
        {
            leftTopPixelCoordinateBitMap = new SKPoint(Left, Top);
            rightTopPixelCoordinateBitMap = new SKPoint(Right, Top);
            leftBottomPixelCoordinateBitMap = new SKPoint(Left, Bottom);
            rightBottomPixelCoordinateBitMap = new SKPoint(Right, Bottom);
        }

        public void CalculateBitMapSize(SKImageInfo info, Display display)
        {
            Left = (info.Width - display.AspectRatio * BitMap.Width) / 2;
            Top = (info.Height - display.AspectRatio * BitMap.Height) / 2;
            Right = Left + display.AspectRatio * BitMap.Width;
            Bottom = Top + display.AspectRatio * BitMap.Height;
            Width = Right - Left;
            Height = Bottom - Top;
            scaleHeight = (float)BitMap.Height / (float)Height;
            scaleWidth = (float)BitMap.Width / (float)Width;
        }

    }
}
