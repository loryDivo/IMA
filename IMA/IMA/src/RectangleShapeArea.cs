using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace IMA.src
{
    public interface RectangleShapeArea
    {
        float Top { get; set; }
        float Left { get; set; }
        float Right { get; set; }
        float Bottom { get; set; }

        SKPoint LeftTopPixelCoordinate { get; set; }
        SKPoint LeftBottomPixelCoordinate { get; set; }
        SKPoint RightTopPixelCoordinate { get; set; }
        SKPoint RightBottomPixelCoordinate { get; set; }

        SKPoint ScaleLeftTopPixelCoordinate { get; set; }
        SKPoint ScaleLeftBottomPixelCoordinate { get; set; }
        SKPoint ScaleRightTopPixelCoordinate { get; set; }
        SKPoint ScaleRightBottomPixelCoordinate { get; set; }
        
        void CalculateVertexCoordinate();
    }
}
