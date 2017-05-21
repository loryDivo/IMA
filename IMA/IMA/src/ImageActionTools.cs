using IMA.src.IDevice;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using TouchTracking;
using Xamarin.Forms;
using System.Collections.Generic;

namespace IMA
{
    public class ImageActionTools : ContentPage
    {
        
        private Boolean drawRectArea = false;
        private Grid gridLayout;
        private SKCanvasView canvasBitMap;
        private SKBitmap bitMap;
        private string bitMapDirectorySource;


        private float topBitMapImg;
        private float bottomBitMapImg;
        private float leftBitMapImg;
        private float rightBitMapImg;
        private SKRect rectangleBitMapImg;
        private SKPaint paintResizeRectSelectionArea;

        private float radiousOfCircleRect = 20;
        private float topRect;
        private float bottomRect;
        private float leftRect;
        private float rightRect;
        private float offSetRectWidth = 100;
        private float offSetRectHeight = 100;
        private SKRect rectSelectionArea;
        private SKPaint paintRectSelectionArea;
        private Boolean panMove = false;
        private Boolean resizeMove = false;
        private TouchEffect touchEffect;
        private PinchGestureRecognizer pinchGesture;
        private PanGestureRecognizer panGesture;
        private List<long> touchId;

        private float prevRatio;
        private float aspectRatio;
        private float scaleWidth;
        private float scaleHeight;

        private SKPoint leftTopCoordinatePixelRect;
        private SKPoint leftBottomCoordinatePixelRect;
        private SKPoint rightTopCoordinatePixelRect;
        private SKPoint rightBottomCoordinatePixelRect;
        private SKPoint oneRadiousCoordinatePixel;
        private SKPoint coordinatePixelDetected;

        private ResizeInfo resizeInfo;

        //PINCH
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;
        private const double OVERSHOOT = 0.15;
        private double StartScale;
        private double LastScale;
        //PAN
        private double StartX, StartY;

        public ImageActionTools(SKBitmap bitMap)
        {
            InizializeComponent(bitMap);
            ToolbarAdd();
        }

        private void InizializeComponent(SKBitmap bitMap)
        {
            touchId = new List<long>();
            this.gridLayout = new Grid();
            this.bitMapDirectorySource = DependencyService.Get<IPicturePicker>().GetImageRealPath();
            this.bitMap = bitMap;

            rectangleBitMapImg = new SKRect();

            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;

            canvasBitMap = new SKCanvasView();
            canvasBitMap.PaintSurface += OnCanvasViewBitMapImgSurface;

            pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            gridLayout.GestureRecognizers.Add(pinchGesture);
            panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            gridLayout.GestureRecognizers.Add(panGesture);
            gridLayout.Children.Add(canvasBitMap);
            Content = gridLayout;

        }

        private void ToolbarAdd()
        {
            ToolbarItem rectanglePortion = new ToolbarItem()
            {
                Icon = "rectangleSelection.png",
                Command = new Command(this.ShowRectangleIntoImageArea),
            };

            this.ToolbarItems.Add(rectanglePortion);

            ToolbarItem sendFile = new ToolbarItem()
            {
                Icon = "sendImage.png",
                Command = new Command(this.SendFileToCompressor),
            };

            this.ToolbarItems.Add(sendFile);

        }
    
        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    LastScale = e.Scale;
                    StartScale = Scale;
                    AnchorX = e.ScaleOrigin.X;
                    AnchorY = e.ScaleOrigin.Y;
                    break;

                case GestureStatus.Running:
                    if (e.Scale < 0 || Math.Abs(LastScale - e.Scale) > (LastScale * 1.3) - LastScale)
                    {                   
                        return;
                    }
                    LastScale = e.Scale;
                    var current = Scale + (e.Scale - 1) * StartScale;
                    Scale = Clamp(current, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
                    break;

                case GestureStatus.Completed:
                    if (Scale > MAX_SCALE)
                    {
                        Scale = MAX_SCALE;
                    }
                    else if (Scale < MIN_SCALE)
                    {

                        Scale = MIN_SCALE;
                    }
                    break;

            }
        }

        private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0)
                return minimum;
            else if (value.CompareTo(maximum) > 0)
                return maximum;
            else
                return value;
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    StartX = (1 - AnchorX) * Width;
                    StartY = (1 - AnchorY) * Height;
                    break;
                case GestureStatus.Running:
                    AnchorX = Clamp(1 - (StartX + e.TotalX) / Width, 0, 1);
                    AnchorY = Clamp(1 - (StartY + e.TotalY) / Height, 0, 1);
                    break;
            }
        }
        private void OnCanvasViewBitMapImgSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            aspectRatio = Math.Min((float)info.Width / bitMap.Width,
                        (float)info.Height / bitMap.Height);

            scaleHeight = info.Height / rectangleBitMapImg.Height;
            scaleWidth = info.Width / rectangleBitMapImg.Width;

            GetBitMapSize(info);
            rectangleBitMapImg = new SKRect(leftBitMapImg, topBitMapImg, rightBitMapImg, bottomBitMapImg);
            canvas.DrawBitmap(bitMap, rectangleBitMapImg);

            if (!DisplayChange(canvas))
            {
                if (drawRectArea && !panMove && !resizeMove)
                {
                    canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
                }
                else if (drawRectArea && panMove && !resizeMove)
                {
                    canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
                    panMove = false;
                }
                else if (drawRectArea && resizeMove && !panMove)
                {
                    paintResizeRectSelectionArea = new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill,
                        Color = SKColors.LightSkyBlue,
                    };
                    canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
                    canvas.DrawCircle(leftRect, topRect, radiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(leftRect, bottomRect, radiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rightRect, topRect, radiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rightRect, bottomRect, radiousOfCircleRect, paintResizeRectSelectionArea);
                    resizeMove = false;
                }
            }
        }

        private Boolean DisplayChange(SKCanvas canvas)
        {
            if (prevRatio == 0)
            {
                prevRatio = aspectRatio;
                if (drawRectArea && !panMove && !resizeMove)
                {
                    DrawRectArea(canvas, true);
                }
                return true;
            }
            else if (aspectRatio != prevRatio)
            {
                if (drawRectArea && !panMove && !resizeMove)
                {
                    DrawRectArea(canvas, false);
                }
                prevRatio = aspectRatio;
                return true;
            }
            return false;
        }

        private void GetBitMapSize(SKImageInfo info)
        {
            leftBitMapImg = (info.Width - aspectRatio * bitMap.Width) / 2;
            topBitMapImg = (info.Height - aspectRatio * bitMap.Height) / 2;
            rightBitMapImg = leftBitMapImg + aspectRatio * bitMap.Width;
            bottomBitMapImg = topBitMapImg + aspectRatio * bitMap.Height;
        }

        private void DrawRectArea(SKCanvas canvas, Boolean defaultRectPosition)
        {
            if (defaultRectPosition)
            {
                topRect = topBitMapImg;
                leftRect = leftBitMapImg;
            }
            else
            {
                offSetRectHeight = (bottomRect - topRect) * scaleHeight;
                offSetRectWidth = (rightRect - leftRect) * scaleWidth;
                topRect = offSetRectHeight + topRect;
                leftRect = offSetRectWidth + leftRect;
            }
            rightRect = leftRect + offSetRectWidth;
            bottomRect = topRect + offSetRectHeight;

            rectSelectionArea = new SKRect(leftRect, topRect, rightRect, bottomRect);
            canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasBitMap.CanvasSize.Width * pt.X / canvasBitMap.Width),
                               (float)(canvasBitMap.CanvasSize.Height * pt.Y / canvasBitMap.Height));
        }

        private void SendFileToCompressor()
        {
            Rectangle rectCoordinate;
            rectCoordinate = DeterminateRectCoordinate();
            Navigation.PushAsync(new Sender(this, bitMapDirectorySource, rectCoordinate));
            
        }

        private Rectangle DeterminateRectCoordinate()
        {
            if (drawRectArea)
            {
                return new Rectangle();
                // determina cordinate
            }
            else
            {
                DisplayAlert("Nessuna selezione", "Non è stata selezionata nessuna area, il server elaborerà tutta la foto", "OK");
                return Rectangle.Zero;
            }
        }

        private void ShowRectangleIntoImageArea()
        {
            if (!drawRectArea)
            {

                topRect = topBitMapImg;
                leftRect = leftBitMapImg;
                rightRect = leftRect + offSetRectWidth;
                bottomRect = topRect + offSetRectHeight;

                rectSelectionArea = new SKRect(leftRect, topRect, rightRect, bottomRect);
                paintRectSelectionArea = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 5

                };

                drawRectArea = true;

                touchEffect = new TouchEffect();
                touchEffect.TouchAction += OnTouchEffectAction;
                gridLayout.Effects.Add(touchEffect);
                gridLayout.GestureRecognizers.Remove(pinchGesture);
                gridLayout.GestureRecognizers.Remove(panGesture);
            }
            else
            {
                canvasBitMap.Effects.Remove(touchEffect);
                gridLayout.GestureRecognizers.Add(pinchGesture);
                gridLayout.GestureRecognizers.Add(panGesture);
                drawRectArea = false;
            }
            canvasBitMap.InvalidateSurface();
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {

                case TouchActionType.Pressed:
                    if (!touchId.Contains(args.Id)){
                        touchId.Add(args.Id);
                    }
                    break;

                case TouchActionType.Moved:
                    if (touchId.Contains(args.Id))
                    {
                        resizeInfo = CheckIfResize(args.Location);
                        if (resizeInfo != ResizeInfo.none)
                        {
                            ResizeRect(coordinatePixelDetected, resizeInfo);
                        }
                        else
                        {
                            RectangleMoveArea(args.Location);
                        }
                    }
                    break;
                case TouchActionType.Released:
                    if (touchId.Contains(args.Id))
                    {
                        touchId.Remove(args.Id);
                    }
                    break;
            }
        }

        private ResizeInfo CheckIfResize(Point coordinateDetected)
        {

            leftTopCoordinatePixelRect = new SKPoint(leftRect, topRect);
            rightTopCoordinatePixelRect = new SKPoint(rightRect, topRect);
            leftBottomCoordinatePixelRect = new SKPoint(leftRect, bottomRect);
            rightBottomCoordinatePixelRect = new SKPoint(rightRect, bottomRect);

            oneRadiousCoordinatePixel = new SKPoint(leftRect + radiousOfCircleRect, topRect + radiousOfCircleRect);

            coordinatePixelDetected = ConvertToPixel(new Point(coordinateDetected.X, coordinateDetected.Y));

            ResizeInfo resizeDetected = MovimentMethods.CheckIfResize(coordinatePixelDetected, oneRadiousCoordinatePixel,
                                                                     leftTopCoordinatePixelRect, leftBottomCoordinatePixelRect,
                                                                     rightTopCoordinatePixelRect, rightBottomCoordinatePixelRect);


            return resizeDetected;
        }

        private void ResizeRect(SKPoint coordinatePixelDetected, ResizeInfo resizeDetected)
        {

            MovimentInfo resizeMovimentDetected = MovimentMethods.GetResizeInfo(coordinatePixelDetected.X, coordinatePixelDetected.Y,
                                                                            leftBitMapImg, topBitMapImg, rightBitMapImg, bottomBitMapImg);

            switch (resizeDetected)
            {
                case ResizeInfo.LeftTopAround:
                    LeftTopResizeCase(coordinatePixelDetected, resizeMovimentDetected);
                    break;

                case ResizeInfo.LeftBottomAround:
                    LeftBottomResizeCase(coordinatePixelDetected, resizeMovimentDetected);
                    break;

                case ResizeInfo.RightTopAround:
                    RightTopResizeCase(coordinatePixelDetected, resizeMovimentDetected);
                    break;

                case ResizeInfo.RightBottomAround:
                    RightBottomResizeCase(coordinatePixelDetected, resizeMovimentDetected);
                    break;

            }
            rectSelectionArea.Top = topRect;
            rectSelectionArea.Right = rightRect;
            rectSelectionArea.Left = leftRect;
            rectSelectionArea.Bottom = bottomRect;
            offSetRectHeight = bottomRect - topRect;
            offSetRectWidth = rightRect - leftRect;
            resizeMove = true;
            canvasBitMap.InvalidateSurface();
        }

        private void LeftTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {

            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideLeft:
                    leftRect = leftBitMapImg;
                    topRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.OverTop:
                    topRect = topBitMapImg;
                    leftRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideLeftOverTop:
                    topRect = topBitMapImg;
                    leftRect = leftBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    leftRect = coordinatePixelDetected.X;
                    topRect = coordinatePixelDetected.Y;
                    break;

            }
        }


        private void LeftBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {

            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideLeft:
                    leftRect = leftBitMapImg;
                    bottomRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.UnderBottom:
                    bottomRect = bottomBitMapImg;
                    leftRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideLeftUnderBottom:
                    leftRect = leftBitMapImg;
                    bottomRect = bottomBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    leftRect = coordinatePixelDetected.X;
                    bottomRect = coordinatePixelDetected.Y;
                    break;
            }
        }

        private void RightTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {
            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideRight:
                    rightRect = rightBitMapImg;
                    topRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.OverTop:
                    topRect = topBitMapImg;
                    rightRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideRightOverTop:
                    rightRect = rightBitMapImg;
                    topRect = topBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    rightRect = coordinatePixelDetected.X;
                    topRect = coordinatePixelDetected.Y;
                    break;
            }
        }

        private void RightBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {
            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideRight:
                    rightRect = rightBitMapImg;
                    bottomRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.UnderBottom:
                    bottomRect = bottomBitMapImg;
                    rightRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideRightOverTop:
                    rightRect = rightBitMapImg;
                    bottomRect = bottomBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    rightRect = coordinatePixelDetected.X;
                    bottomRect = coordinatePixelDetected.Y;
                    break;
            }
        }



        private void RectangleMoveArea(Point pointMove)
        {

            SKPoint pixelMove = ConvertToPixel(pointMove);
            MovimentInfo actualMoviment = MovimentMethods.GetMovimentInfo(pixelMove.X, pixelMove.Y,
                                                                          leftBitMapImg, topBitMapImg, rightBitMapImg,
                                                                          bottomBitMapImg, offSetRectWidth, offSetRectHeight);

            switch (actualMoviment)
            {
                case MovimentInfo.InsideArea:

                    bottomRect = pixelMove.Y + offSetRectHeight / 2;
                    topRect = bottomRect - offSetRectHeight;
                    rightRect = pixelMove.X + offSetRectWidth / 2;
                    leftRect = rightRect - offSetRectWidth;
                    break;

                case MovimentInfo.OutSideRightOverTop:

                    bottomRect = topBitMapImg + offSetRectHeight;
                    topRect = topBitMapImg;
                    rightRect = rightBitMapImg;
                    leftRect = rightRect - offSetRectWidth;
                    break;

                case MovimentInfo.OutSideRight:
                    bottomRect = pixelMove.Y + offSetRectHeight / 2;
                    topRect = bottomRect - offSetRectHeight;
                    rightRect = rightBitMapImg;
                    leftRect = rightRect - offSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeftOverTop:

                    bottomRect = topBitMapImg + offSetRectHeight;
                    topRect = topBitMapImg;
                    leftRect = leftBitMapImg;
                    rightRect = leftBitMapImg + offSetRectWidth;
                    break;

                case MovimentInfo.OverTop:

                    bottomRect = topBitMapImg + offSetRectHeight;
                    topRect = topBitMapImg;
                    rightRect = pixelMove.X + offSetRectWidth / 2;
                    leftRect = rightRect - offSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeftUnderBottom:

                    bottomRect = bottomBitMapImg;
                    topRect = bottomRect - offSetRectHeight;
                    leftRect = leftBitMapImg;
                    rightRect = leftBitMapImg + offSetRectWidth;
                    break;

                case MovimentInfo.UnderBottom:

                    bottomRect = bottomBitMapImg;
                    topRect = bottomRect - offSetRectHeight;
                    rightRect = pixelMove.X + offSetRectWidth / 2;
                    leftRect = rightRect - offSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeft:

                    bottomRect = pixelMove.Y + offSetRectHeight / 2;
                    topRect = bottomRect - offSetRectHeight;
                    leftRect = leftBitMapImg;
                    rightRect = leftBitMapImg + offSetRectWidth;
                    break;

                case MovimentInfo.OutSideRightUnderBottom:

                    bottomRect = bottomBitMapImg;
                    topRect = bottomBitMapImg - offSetRectHeight;
                    rightRect = rightBitMapImg;
                    leftRect = rightBitMapImg - offSetRectWidth;
                    break;

            }

            rectSelectionArea.Top = topRect;
            rectSelectionArea.Right = rightRect;
            rectSelectionArea.Left = leftRect;
            rectSelectionArea.Bottom = bottomRect;
            panMove = true;
            canvasBitMap.InvalidateSurface();

        }


    }
}
