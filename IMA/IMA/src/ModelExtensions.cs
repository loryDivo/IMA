using SkiaSharp;

namespace IMA
{
    public static class ModelExtensions
    {
        public static float GetMidX(this SKRect rect)
        {
            return (rect.Left + rect.Width / 2);
        }

        public static float GetMidY(this SKRect rect)
        {
            return (rect.Top + rect.Height / 2);
        }

        public static SKRect GetAspectRectangle(this SKSize imgSize, SKRect fullRect)
        {
            SKSize aspectSize = imgSize;
            float imgAspect = aspectSize.Width / aspectSize.Height;
            float fullRectAspect = fullRect.Width / fullRect.Height;
            if (fullRectAspect > imgAspect)
            {
                aspectSize.Height = fullRect.Height;
                aspectSize.Width = aspectSize.Height * imgAspect;
            }
            else
            {
                aspectSize.Width = fullRect.Width;
                aspectSize.Height = aspectSize.Width / imgAspect;
            }
            float aspectLeft = fullRect.GetMidX() - (aspectSize.Width / 2);
            float aspectTop = fullRect.GetMidY() - (aspectSize.Height / 2);
            return (SKRect.Create(new SKPoint(aspectLeft, aspectTop), aspectSize));
        }
    }
}
