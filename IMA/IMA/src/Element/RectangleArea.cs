using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
namespace IMA.src
{
    /*
     * Oggetto rappresentante la singola bounding box
     * possiede tutti gli attributi per disegnarla su schermo
     */ 
    public class RectangleArea : RectangleShapeArea
    {
        private String id;

        private float radiousOfCircleRect = 20;
        private float topRect;
        private float bottomRect;
        private float leftRect;
        private float rightRect;
        private float offSetRectWidth = 100;
        private float offSetRectHeight = 100;

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

        private SKPoint positionIDRect;

        private float scaleTopPixelCoordinate;
        private float scaleBottomPixelCoordinate;
        private float scaleLeftPixelCoordinate;
        private float scaleRightPixelCoordinate;

        private bool rectangleSelected = false;
        private SKPaint paintRect;
        private SKPaint paintIDRect;

        public RectangleArea(String idRect)
        {
            this.id = idRect;

            byte[] buffer = new byte[4];
            new Random().NextBytes(buffer);
            this.paintRect = new SKPaint
            {
                Color = new SKColor(buffer[0], buffer[1], buffer[2], buffer[3]),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5,
            };

            this.paintIDRect = new SKPaint
            {
                Color = new SKColor(0, 0, 0),
                TextSize = 15,
            };
        }

        public float RadiousOfCircleRect { get => radiousOfCircleRect; set => radiousOfCircleRect = value; }
        public float Top { get => topRect; set => topRect = value; }
        public float Bottom { get => bottomRect; set => bottomRect = value; }
        public float Left { get => leftRect; set => leftRect = value; }
        public float Right { get => rightRect; set => rightRect = value; }
        public float OffSetRectWidth { get => offSetRectWidth; set => offSetRectWidth = value; }
        public float OffSetRectHeight { get => offSetRectHeight; set => offSetRectHeight = value; }
        public bool ResizeMove { get => resizeMove; set => resizeMove = value; }
        public ResizeInfo ResizeInfo { get => resizeInfo; set => resizeInfo = value; }
        public SKPoint LeftTopPixelCoordinate { get => leftTopPixelCoordinateRect; }
        public SKPoint LeftBottomPixelCoordinate { get => leftBottomPixelCoordinateRect; }
        public SKPoint RightTopPixelCoordinate { get => rightTopPixelCoordinateRect; }
        public SKPoint RightBottomPixelCoordinate { get => rightBottomPixelCoordinateRect; }
        public SKPoint OneRadiousPixelCoordinate { get => oneRadiousPixelCoordinate; }
        public SKPoint PixelCoordinateDetected { get => coordinatePixelDetected; }
        public SKPoint ScaleLeftTopPixelCoordinate { get => scaleLeftTopPixelCoordinateRect; }
        public SKPoint ScaleLeftBottomPixelCoordinate { get => scaleLeftBottomPixelCoordinateRect; }
        public SKPoint ScaleRightTopPixelCoordinate { get => scaleRightTopPixelCoordinateRect; }
        public SKPoint ScaleRightBottomPixelCoordinate { get => scaleRightBottomPixelCoordinateRect; }
        public SKPaint PaintRect { get => paintRect; }
        public bool RectangleSelected { get => rectangleSelected; set => rectangleSelected = value; }
        public String Id { get => id; }
        public SKPaint PaintIDRect { get => paintIDRect; }
        public SKPoint PositionIDRect { get => positionIDRect; }

        public void CalculateVertexCoordinate()
        {
            leftTopPixelCoordinateRect = new SKPoint(Left, Top);
            rightTopPixelCoordinateRect = new SKPoint(Right, Top);
            leftBottomPixelCoordinateRect = new SKPoint(Left, Bottom);
            rightBottomPixelCoordinateRect = new SKPoint(Right, Bottom);
        }

        public void CalculateNewPositionIDRect()
        {
            positionIDRect = new SKPoint(this.rightRect - OffSetRectWidth / 2 - (paintIDRect.MeasureText(id) / 2), this.topRect - 10);
        }

        public void CalculateScaleVertexCoordinate(BitMapArea bitMapArea)
        {
            scaleTopPixelCoordinate = topRect - bitMapArea.Top;
            scaleBottomPixelCoordinate = bottomRect - bitMapArea.Top;
            scaleLeftPixelCoordinate = leftRect - bitMapArea.Left;
            scaleRightPixelCoordinate = rightRect - bitMapArea.Left;
            CorrectSideRect();

            scaleLeftTopPixelCoordinateRect = new SKPoint(scaleLeftPixelCoordinate * bitMapArea.ScaleWidth, (scaleTopPixelCoordinate * bitMapArea.ScaleHeight));
            scaleLeftBottomPixelCoordinateRect = new SKPoint(scaleLeftPixelCoordinate * bitMapArea.ScaleWidth, scaleBottomPixelCoordinate * bitMapArea.ScaleHeight);
            scaleRightTopPixelCoordinateRect = new SKPoint(scaleRightPixelCoordinate * bitMapArea.ScaleWidth, scaleTopPixelCoordinate * bitMapArea.ScaleHeight);
            scaleRightBottomPixelCoordinateRect = new SKPoint(scaleRightPixelCoordinate * bitMapArea.ScaleWidth, scaleBottomPixelCoordinate * bitMapArea.ScaleHeight);
        }

        private void CorrectSideRect()
        {
            float temp = 0;
            if(scaleTopPixelCoordinate > scaleBottomPixelCoordinate)
            {
                temp = scaleTopPixelCoordinate;
                scaleTopPixelCoordinate = scaleBottomPixelCoordinate;
                scaleBottomPixelCoordinate = temp;
            }
            if(scaleLeftPixelCoordinate > scaleRightPixelCoordinate)
            {
                temp = scaleLeftPixelCoordinate;
                scaleLeftPixelCoordinate = scaleRightPixelCoordinate;
                scaleRightPixelCoordinate = temp;
            }
        }

        public void CalculateOneRadiousCoordinate(Point coordinateDetected, SKCanvasView canvasBitMap)
        {
            oneRadiousPixelCoordinate = new SKPoint(Left + RadiousOfCircleRect, Top + RadiousOfCircleRect);
            coordinatePixelDetected = UtilityFunctions.ConvertToPixel(new Point(coordinateDetected.X, coordinateDetected.Y), canvasBitMap);
        }

        public bool CheckIfInAnyRectangle(SKPoint pointPixel)
        {
            return pointPixel.X >= leftRect && pointPixel.X <= rightRect
                && pointPixel.Y >= topRect && pointPixel.Y <= Bottom;
        }

        public bool EqualsID(String idNewRect)
        {
            return id.Equals(idNewRect);
        }

    }
}
