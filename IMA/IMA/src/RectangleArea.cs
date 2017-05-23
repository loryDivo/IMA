using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace IMA.src
{
    public class RectangleArea : RectangleShapeArea
    {

        private float radiousOfCircleRect = 20;
        private float topRect;
        private float bottomRect;
        private float leftRect;
        private float rightRect;
        private float offSetRectWidth = 100;
        private float offSetRectHeight = 100;


        private Boolean drawRectArea = false;

        private Boolean panMove = false;

        private ResizeInfo resizeInfo;
        private Boolean resizeMove = false;

        private SKPoint leftTopPixelCoordinateRect;
        private SKPoint leftBottomPixelCoordinateRect;
        private SKPoint rightTopPixelCoordinateRect;
        private SKPoint rightBottomPixelCoordinateRect;
        private SKPoint oneRadiousPixelCoordinate;
        private SKPoint coordinatePixelDetected;

        private SKPoint scaleLeftTopPixelCoordinateRect;
        private SKPoint scaleLeftBottomPixelCoordinateRect;
        private SKPoint scaleRightTopPixelCoordinateRect;
        private SKPoint scaleRightBottomPixelCoordinateRect;

        private float scaleTopPixelCoordinate;
        private float scaleBottomPixelCoordinate;
        private float scaleLeftPixelCoordinate;
        private float scaleRightPixelCoordinate;

        public RectangleArea()
        {

        }

        public float RadiousOfCircleRect { get => radiousOfCircleRect; set => radiousOfCircleRect = value; }
        public float Top { get => topRect; set => topRect = value; }
        public float Bottom { get => bottomRect; set => bottomRect = value; }
        public float Left { get => leftRect; set => leftRect = value; }
        public float Right { get => rightRect; set => rightRect = value; }
        public float OffSetRectWidth { get => offSetRectWidth; set => offSetRectWidth = value; }
        public float OffSetRectHeight { get => offSetRectHeight; set => offSetRectHeight = value; }
        public bool DrawRectArea { get => drawRectArea; set => drawRectArea = value; }
        public bool PanMove { get => panMove; set => panMove = value; }
        public bool ResizeMove { get => resizeMove; set => resizeMove = value; }
        public ResizeInfo ResizeInfo { get => resizeInfo; set => resizeInfo = value; }
        public SKPoint LeftTopPixelCoordinate { get => leftTopPixelCoordinateRect; set => leftTopPixelCoordinateRect = value; }
        public SKPoint LeftBottomPixelCoordinate { get => leftBottomPixelCoordinateRect; set => leftBottomPixelCoordinateRect = value; }
        public SKPoint RightTopPixelCoordinate { get => rightTopPixelCoordinateRect; set => rightTopPixelCoordinateRect = value; }
        public SKPoint RightBottomPixelCoordinate { get => rightBottomPixelCoordinateRect; set => rightBottomPixelCoordinateRect = value; }
        public SKPoint OneRadiousPixelCoordinate { get => oneRadiousPixelCoordinate; set => oneRadiousPixelCoordinate = value; }
        public SKPoint PixelCoordinateDetected { get => coordinatePixelDetected; set => coordinatePixelDetected = value; }
        public SKPoint ScaleLeftTopPixelCoordinate { get => scaleLeftTopPixelCoordinateRect; set => scaleLeftTopPixelCoordinateRect = value; }
        public SKPoint ScaleLeftBottomPixelCoordinate { get => scaleLeftBottomPixelCoordinateRect; set => scaleLeftBottomPixelCoordinateRect = value; }
        public SKPoint ScaleRightTopPixelCoordinate { get => scaleRightTopPixelCoordinateRect; set => scaleRightTopPixelCoordinateRect = value; }
        public SKPoint ScaleRightBottomPixelCoordinate { get => scaleRightBottomPixelCoordinateRect; set => scaleRightBottomPixelCoordinateRect = value; }
       
        public void CalculateVertexCoordinate()
        {
            leftTopPixelCoordinateRect = new SKPoint(Left, Top);
            rightTopPixelCoordinateRect = new SKPoint(Right, Top);
            leftBottomPixelCoordinateRect = new SKPoint(Left, Bottom);
            rightBottomPixelCoordinateRect = new SKPoint(Right, Bottom);
        }

        public void CalculateScaleVertexCoordinate(BitMapArea bitMapArea)
        {
            scaleTopPixelCoordinate = topRect - bitMapArea.Top;
            scaleBottomPixelCoordinate = bottomRect - bitMapArea.Top;
            scaleLeftPixelCoordinate = leftRect - bitMapArea.Left;
            scaleRightPixelCoordinate = rightRect - bitMapArea.Left;

            scaleLeftTopPixelCoordinateRect = new SKPoint(scaleLeftPixelCoordinate * bitMapArea.ScaleWidth, (scaleTopPixelCoordinate * bitMapArea.ScaleHeight));
            scaleLeftBottomPixelCoordinateRect = new SKPoint(scaleLeftPixelCoordinate * bitMapArea.ScaleWidth, scaleBottomPixelCoordinate * bitMapArea.ScaleHeight);
            scaleRightTopPixelCoordinateRect = new SKPoint(scaleRightPixelCoordinate * bitMapArea.ScaleWidth, scaleTopPixelCoordinate * bitMapArea.ScaleHeight);
            scaleRightBottomPixelCoordinateRect = new SKPoint(scaleRightPixelCoordinate * bitMapArea.ScaleWidth, scaleBottomPixelCoordinate * bitMapArea.ScaleHeight);
        }

        public void CalculateOneRadiousCoordinate(Point coordinateDetected, SKCanvasView canvasBitMap)
        {
            OneRadiousPixelCoordinate = new SKPoint(Left + RadiousOfCircleRect, Top + RadiousOfCircleRect);
            PixelCoordinateDetected = UtilityFunctions.ConvertToPixel(new Point(coordinateDetected.X, coordinateDetected.Y), canvasBitMap);
        }


    }
}
