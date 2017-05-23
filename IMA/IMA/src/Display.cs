﻿using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace IMA.src
{
    public class Display
    {
        private float prevRatio;
        private float aspectRatio;
        private float left;
        private float right;
        private float bottom;
        private float top;

        public Display()
        {

        }

        public float PrevRatio { get => prevRatio; set => prevRatio = value; }
        public float AspectRatio { get => aspectRatio; set => aspectRatio = value; }
        public float Left { get => left; set => left = value; }
        public float Right { get => right; set => right = value; }
        public float Bottom { get => bottom; set => bottom = value; }
        public float Top { get => top; set => top = value; }

        public void CalculateDisplayVertexPixelCoordinate(SKImageInfo info)
        {
            left = 0;
            right = left + info.Width;
            top = 0;
            bottom = top + info.Height;
        }

    }
}
