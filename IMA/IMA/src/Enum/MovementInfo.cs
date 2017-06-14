using SkiaSharp;
using System;

namespace IMA
{
    public enum MovimentInfo
    {
        InsideArea,
        OutSideRight,
        OutSideLeft,
        OverTop,
        UnderBottom,
        OutSideRightOverTop,
        OutSideLeftOverTop,
        OutSideLeftUnderBottom,
        OutSideRightUnderBottom,
        none,
    }

    public enum ResizeInfo
    {
        LeftTopAround,
        LeftBottomAround,
        RightTopAround,
        RightBottomAround,
        none,
    }


    public static class MovimentMethods
    {

        public static ResizeInfo CheckIfResize(SKPoint coordinateDetected, SKPoint oneRadiousCoordinate,
                                               SKPoint leftTopCoordinate, SKPoint leftBottomCoordinate,
                                               SKPoint rightTopCoordinate, SKPoint rightBottomCoordinate)
        {
            double distanceLeftTopCoordinate = 0;
            double distanceLeftBottomCoordinate = 0;
            double distanceRightTopCoordinate = 0;
            double distanceRightBottomCoordinate = 0;

            double distanceRadiousCenter = 0;

            double minimumDistance = 0;

            distanceLeftTopCoordinate = EuclideanDistance(leftTopCoordinate, coordinateDetected);
            distanceLeftBottomCoordinate = EuclideanDistance(leftBottomCoordinate, coordinateDetected);
            distanceRightTopCoordinate = EuclideanDistance(rightTopCoordinate, coordinateDetected);
            distanceRightBottomCoordinate = EuclideanDistance(rightBottomCoordinate, coordinateDetected);

            distanceRadiousCenter = EuclideanDistance(leftTopCoordinate, oneRadiousCoordinate);

            minimumDistance = Math.Min(distanceLeftTopCoordinate, 
                              Math.Min(distanceLeftBottomCoordinate, 
                              Math.Min(distanceRightTopCoordinate, distanceRightBottomCoordinate)));
            
            if(minimumDistance <= distanceRadiousCenter)
            {
                return CheckMinDistanceCoordinate(minimumDistance, distanceLeftTopCoordinate,
                                                  distanceLeftBottomCoordinate, distanceRightTopCoordinate,
                                                  distanceRightBottomCoordinate);
            }
            else
            {
                return ResizeInfo.none;
            }

        }

        public static double EuclideanDistance(SKPoint point1, SKPoint point2)
        {
            return Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
        }

        public static float EuclideanDistance(float coordinate1, float coordinate2)
        {
            return (float)Math.Sqrt(Math.Pow((coordinate1 - coordinate2), 2));
        }

        private static ResizeInfo CheckMinDistanceCoordinate(double minimumDistanceDetected, double distanceLeftTopCoordinate,
                                                             double distanceLeftBottomCoordinate, double distanceRightTopCoordinate,
                                                             double distanceRightBottomCoordinate)
        {
            if (minimumDistanceDetected == distanceLeftTopCoordinate)
            {
                return ResizeInfo.LeftTopAround;
            }
            else if (minimumDistanceDetected == distanceLeftBottomCoordinate)
            {
                return ResizeInfo.LeftBottomAround;
            }
            else if (minimumDistanceDetected == distanceRightTopCoordinate)
            {
                return ResizeInfo.RightTopAround;
            }
            else
            {
                return ResizeInfo.RightBottomAround;
            }
        
        }


        public static MovimentInfo GetMovimentInfo( float XPixel, float YPixel, 
                                                    float left, float top, float right, 
                                                    float bottom, float offSetWidth, float offSetHeight)
        {
            if (XPixel + offSetWidth / 2<= right
                && XPixel - offSetWidth / 2 >= left
               && YPixel - offSetHeight / 2 >= top
                && YPixel + offSetHeight / 2 <= bottom)
            {
                return MovimentInfo.InsideArea;
            }

            else if (XPixel + offSetWidth / 2 > right)
            {
                if (YPixel - offSetHeight / 2 < top)
                {
                    return MovimentInfo.OutSideRightOverTop;

                }
                else if (YPixel + offSetHeight / 2 > bottom)
                {
                    return MovimentInfo.OutSideRightUnderBottom;
                }
                else
                {
                    return MovimentInfo.OutSideRight;
                }
            }

            else if (YPixel - offSetHeight / 2 < top)
            {
                if (XPixel - offSetWidth / 2 < left)
                {
                    return MovimentInfo.OutSideLeftOverTop;

                }

                else
                {
                    return MovimentInfo.OverTop;

                }
            }

            else if (YPixel + offSetHeight / 2 > bottom)
            {
                if (XPixel - offSetWidth / 2 < left)
                {
                    return MovimentInfo.OutSideLeftUnderBottom;
                }
                else
                {
                    return MovimentInfo.UnderBottom;
                }
            }

            else if (XPixel - offSetWidth / 2 < left)
            {
                return MovimentInfo.OutSideLeft;
            }

            return MovimentInfo.none;
        }


        public static MovimentInfo GetResizeInfo(float XPixel, float YPixel,
                                                 float left, float top, float right, float bottom)
        {
            if (XPixel <= right
                && XPixel >= left
               && YPixel >= top
                && YPixel <= bottom)
            {
                return MovimentInfo.InsideArea;
            }

            else if (XPixel > right)
            {
                if (YPixel < top)
                {
                    return MovimentInfo.OutSideRightOverTop;

                }
                else if (YPixel > bottom)
                {
                    return MovimentInfo.OutSideRightUnderBottom;
                }
                else
                {
                    return MovimentInfo.OutSideRight;
                }
            }

            else if (YPixel < top)
            {
                if (XPixel < left)
                {
                    return MovimentInfo.OutSideLeftOverTop;

                }

                else
                {
                    return MovimentInfo.OverTop;

                }
            }

            else if (YPixel > bottom)
            {
                if (XPixel < left)
                {
                    return MovimentInfo.OutSideLeftUnderBottom;
                }
                else
                {
                    return MovimentInfo.UnderBottom;
                }
            }

            else if (XPixel < left)
            {
                return MovimentInfo.OutSideLeft;
            }

            return MovimentInfo.none;
        }

    }

}
