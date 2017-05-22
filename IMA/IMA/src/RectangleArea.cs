using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace IMA.src
{
    public class RectangleArea
    {

        private float radiousOfCircleRect = 20;
        private float topRect;
        private float bottomRect;
        private float leftRect;
        private float rightRect;
        private float offSetRectWidth = 100;
        private float offSetRectHeight = 100;

        private SKPoint leftTopCoordinatePixelRect;
        private SKPoint leftBottomCoordinatePixelRect;
        private SKPoint rightTopCoordinatePixelRect;
        private SKPoint rightBottomCoordinatePixelRect;
        private SKPoint oneRadiousCoordinatePixel;
        private SKPoint coordinatePixelDetected;

        private Boolean drawRectArea = false;

        private Boolean panMove = false;
        private Boolean resizeMove = false;

        private ResizeInfo resizeInfo;

        public RectangleArea()
        {

        }

        public float RadiousOfCircleRect { get => radiousOfCircleRect; set => radiousOfCircleRect = value; }
        public float TopRect { get => topRect; set => topRect = value; }
        public float BottomRect { get => bottomRect; set => bottomRect = value; }
        public float LeftRect { get => leftRect; set => leftRect = value; }
        public float RightRect { get => rightRect; set => rightRect = value; }
        public float OffSetRectWidth { get => offSetRectWidth; set => offSetRectWidth = value; }
        public float OffSetRectHeight { get => offSetRectHeight; set => offSetRectHeight = value; }
        public bool DrawRectArea { get => drawRectArea; set => drawRectArea = value; }
        public bool PanMove { get => panMove; set => panMove = value; }
        public bool ResizeMove { get => resizeMove; set => resizeMove = value; }
        public ResizeInfo ResizeInfo { get => resizeInfo; set => resizeInfo = value; }
        public SKPoint LeftTopCoordinatePixelRect { get => leftTopCoordinatePixelRect; set => leftTopCoordinatePixelRect = value; }
        public SKPoint LeftBottomCoordinatePixelRect { get => leftBottomCoordinatePixelRect; set => leftBottomCoordinatePixelRect = value; }
        public SKPoint RightTopCoordinatePixelRect { get => rightTopCoordinatePixelRect; set => rightTopCoordinatePixelRect = value; }
        public SKPoint RightBottomCoordinatePixelRect { get => rightBottomCoordinatePixelRect; set => rightBottomCoordinatePixelRect = value; }
        public SKPoint OneRadiousCoordinatePixel { get => oneRadiousCoordinatePixel; set => oneRadiousCoordinatePixel = value; }
        public SKPoint CoordinatePixelDetected { get => coordinatePixelDetected; set => coordinatePixelDetected = value; }
    }
}
