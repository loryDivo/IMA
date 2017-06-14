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

        SKPoint LeftTopPixelCoordinate { get; }
        SKPoint LeftBottomPixelCoordinate { get; }
        SKPoint RightTopPixelCoordinate { get; }
        SKPoint RightBottomPixelCoordinate { get; }

        void CalculateVertexCoordinate();
    }
}
