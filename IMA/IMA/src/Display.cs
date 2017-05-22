using System;
using System.Collections.Generic;
using System.Text;

namespace IMA.src
{
    public class Display
    {
        private float prevRatio;
        private float aspectRatio;
        private float scaleWidth;
        private float scaleHeight;

        public Display()
        {

        }

        public float PrevRatio { get => prevRatio; set => prevRatio = value; }
        public float AspectRatio { get => aspectRatio; set => aspectRatio = value; }
        public float ScaleWidth { get => scaleWidth; set => scaleWidth = value; }
        public float ScaleHeight { get => scaleHeight; set => scaleHeight = value; }
    }
}
