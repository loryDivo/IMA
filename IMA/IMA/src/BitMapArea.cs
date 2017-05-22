using System;
using System.Collections.Generic;
using System.Text;
using IMA.src.IDevice;
using SkiaSharp;
using Xamarin.Forms;

namespace IMA.src
{
    public class BitMapArea
    {
        private string bitMapDirectorySource;
        private float topBitMapImg;
        private float bottomBitMapImg;
        private float leftBitMapImg;
        private float rightBitMapImg;
        private SKBitmap bitMap;

        public BitMapArea(SKBitmap bitMap)
        {
            this.bitMap = bitMap;
            this.BitMapDirectorySource = DependencyService.Get<IPicturePicker>().GetImageRealPath();
        }

        public float TopBitMapImg { get => topBitMapImg; set => topBitMapImg = value; }
        public float BottomBitMapImg { get => bottomBitMapImg; set => bottomBitMapImg = value; }
        public float LeftBitMapImg { get => leftBitMapImg; set => leftBitMapImg = value; }
        public float RightBitMapImg { get => rightBitMapImg; set => rightBitMapImg = value; }
        public string BitMapDirectorySource { get => bitMapDirectorySource; set => bitMapDirectorySource = value; }
        public SKBitmap BitMap { get => bitMap; set => bitMap = value; }
    }
}
