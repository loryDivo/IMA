using IMA.src.IDevice;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using TouchTracking;
using Xamarin.Forms;
using System.Collections.Generic;
using IMA.src;

namespace IMA
{
    public class ImageActionTools : ContentPage
    {
        

        private Display display;
        private Grid gridLayout;

        private BitMapArea bitMapArea;
        private SKRect rectangleBitMapImg;
        private SKPaint paintResizeRectSelectionArea;
        private SKCanvasView canvasBitMap;

        private RectangleArea rectangleArea;
        private SKRect rectSelectionArea;
        private SKPaint paintRectSelectionArea;

        private TouchEffect touchEffect;
        private List<long> touchId;
        private PinchGestureRecognizer pinchGesture;
        private PanGestureRecognizer panGesture;

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
            display = new Display();
            gridLayout = new Grid();

            bitMapArea = new BitMapArea(bitMap);
            canvasBitMap = new SKCanvasView();
            canvasBitMap.PaintSurface += OnCanvasViewBitMapImgSurface;

            rectangleArea = new RectangleArea();

            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;


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

            display.AspectRatio = Math.Min((float)info.Width / bitMapArea.BitMap.Width,
                        (float)info.Height / bitMapArea.BitMap.Height);

            display.ScaleHeight = info.Height / rectangleBitMapImg.Height;
            display.ScaleWidth = info.Width / rectangleBitMapImg.Width;

            GetBitMapSize(info);
            rectangleBitMapImg = new SKRect(bitMapArea.LeftBitMapImg, bitMapArea.TopBitMapImg, bitMapArea.RightBitMapImg, bitMapArea.BottomBitMapImg);
            canvas.DrawBitmap(bitMapArea.BitMap, rectangleBitMapImg);

            if (!DisplayChange(canvas))
            {
                if (rectangleArea.DrawRectArea && !rectangleArea.PanMove && !rectangleArea.ResizeMove)
                {
                    canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
                }
                else if (rectangleArea.DrawRectArea && rectangleArea.PanMove && !rectangleArea.ResizeMove)
                {
                    canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
                    rectangleArea.PanMove = false;
                }
                else if (rectangleArea.DrawRectArea && rectangleArea.ResizeMove && !rectangleArea.PanMove)
                {
                    paintResizeRectSelectionArea = new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill,
                        Color = SKColors.LightSkyBlue,
                    };
                    canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.LeftRect, rectangleArea.TopRect, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.LeftRect, rectangleArea.BottomRect, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.RightRect, rectangleArea.TopRect, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.RightRect, rectangleArea.BottomRect, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    rectangleArea.ResizeMove = false;
                }
            }
        }

        private Boolean DisplayChange(SKCanvas canvas)
        {
            if (display.PrevRatio == 0)
            {
                display.PrevRatio = display.AspectRatio;
                if (rectangleArea.DrawRectArea && !rectangleArea.PanMove && !rectangleArea.ResizeMove)
                {
                    DrawRectArea(canvas, true);
                }
                return true;
            }
            else if (display.AspectRatio != display.PrevRatio)
            {
                if (rectangleArea.DrawRectArea && !rectangleArea.PanMove && !rectangleArea.ResizeMove)
                {
                    DrawRectArea(canvas, false);
                }
                display.PrevRatio = display.AspectRatio;
                return true;
            }
            return false;
        }

        private void GetBitMapSize(SKImageInfo info)
        {
            bitMapArea.LeftBitMapImg = (info.Width - display.AspectRatio * bitMapArea.BitMap.Width) / 2;
            bitMapArea.TopBitMapImg = (info.Height - display.AspectRatio * bitMapArea.BitMap.Height) / 2;
            bitMapArea.RightBitMapImg = bitMapArea.LeftBitMapImg + display.AspectRatio * bitMapArea.BitMap.Width;
            bitMapArea.BottomBitMapImg = bitMapArea.TopBitMapImg + display.AspectRatio * bitMapArea.BitMap.Height;
        }

        private void DrawRectArea(SKCanvas canvas, Boolean defaultRectPosition)
        {
            if (defaultRectPosition)
            {
                rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
            }
            else
            {
                rectangleArea.OffSetRectHeight = (rectangleArea.BottomRect - rectangleArea.TopRect) * display.ScaleHeight;
                rectangleArea.OffSetRectWidth = (rectangleArea.RightRect - rectangleArea.LeftRect) * display.ScaleWidth;
                rectangleArea.TopRect = rectangleArea.OffSetRectHeight + rectangleArea.TopRect;
                rectangleArea.LeftRect = rectangleArea.OffSetRectWidth + rectangleArea.LeftRect;
            }
            rectangleArea.RightRect = rectangleArea.LeftRect + rectangleArea.OffSetRectWidth;
            rectangleArea.BottomRect = rectangleArea.TopRect + rectangleArea.OffSetRectHeight;

            rectSelectionArea = new SKRect(rectangleArea.LeftRect, rectangleArea.TopRect, rectangleArea.RightRect, rectangleArea.BottomRect);
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
            Navigation.PushAsync(new Sender(this, bitMapArea.BitMapDirectorySource, rectCoordinate));
            
        }

        private Rectangle DeterminateRectCoordinate()
        {
            if (rectangleArea.DrawRectArea)
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
            if (!rectangleArea.DrawRectArea)
            {

                rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                rectangleArea.RightRect = rectangleArea.LeftRect + rectangleArea.OffSetRectWidth;
                rectangleArea.BottomRect = rectangleArea.TopRect + rectangleArea.OffSetRectHeight;

                rectSelectionArea = new SKRect(rectangleArea.LeftRect, rectangleArea.TopRect, rectangleArea.RightRect, rectangleArea.BottomRect);
                paintRectSelectionArea = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 5

                };

                rectangleArea.DrawRectArea = true;

                touchId = new List<long>();
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
                rectangleArea.DrawRectArea = false;
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
                        rectangleArea.ResizeInfo = CheckIfResize(args.Location);
                        if (rectangleArea.ResizeInfo != ResizeInfo.none)
                        {
                            ResizeRect(rectangleArea.CoordinatePixelDetected, rectangleArea.ResizeInfo);
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

            rectangleArea.LeftTopCoordinatePixelRect = new SKPoint(rectangleArea.LeftRect, rectangleArea.TopRect);
            rectangleArea.RightTopCoordinatePixelRect = new SKPoint(rectangleArea.RightRect, rectangleArea.TopRect);
            rectangleArea.LeftBottomCoordinatePixelRect = new SKPoint(rectangleArea.LeftRect, rectangleArea.BottomRect);
            rectangleArea.RightBottomCoordinatePixelRect = new SKPoint(rectangleArea.RightRect, rectangleArea.BottomRect);

            rectangleArea.OneRadiousCoordinatePixel = new SKPoint(rectangleArea.LeftRect + rectangleArea.RadiousOfCircleRect, rectangleArea.TopRect + rectangleArea.RadiousOfCircleRect);

            rectangleArea.CoordinatePixelDetected = ConvertToPixel(new Point(coordinateDetected.X, coordinateDetected.Y));

            ResizeInfo resizeDetected = MovimentMethods.CheckIfResize(rectangleArea.CoordinatePixelDetected, rectangleArea.OneRadiousCoordinatePixel,
                                                                     rectangleArea.LeftTopCoordinatePixelRect, rectangleArea.LeftBottomCoordinatePixelRect,
                                                                     rectangleArea.RightTopCoordinatePixelRect, rectangleArea.RightBottomCoordinatePixelRect);


            return resizeDetected;
        }

        private void ResizeRect(SKPoint coordinatePixelDetected, ResizeInfo resizeDetected)
        {

            MovimentInfo resizeMovimentDetected = MovimentMethods.GetResizeInfo(coordinatePixelDetected.X, coordinatePixelDetected.Y,
                                                                            bitMapArea.LeftBitMapImg, bitMapArea.TopBitMapImg, bitMapArea.RightBitMapImg, bitMapArea.BottomBitMapImg);

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
            rectSelectionArea.Top = rectangleArea.TopRect;
            rectSelectionArea.Right = rectangleArea.RightRect;
            rectSelectionArea.Left = rectangleArea.LeftRect;
            rectSelectionArea.Bottom = rectangleArea.BottomRect;
            rectangleArea.OffSetRectHeight = rectangleArea.BottomRect - rectangleArea.TopRect;
            rectangleArea.OffSetRectWidth = rectangleArea.RightRect - rectangleArea.LeftRect;
            rectangleArea.ResizeMove = true;
            canvasBitMap.InvalidateSurface();
        }

        private void LeftTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {

            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideLeft:
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    rectangleArea.TopRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.OverTop:
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    rectangleArea.LeftRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideLeftOverTop:
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.LeftRect = coordinatePixelDetected.X;
                    rectangleArea.TopRect = coordinatePixelDetected.Y;
                    break;

            }
        }


        private void LeftBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {

            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideLeft:
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    rectangleArea.BottomRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.UnderBottom:
                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    rectangleArea.LeftRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideLeftUnderBottom:
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.LeftRect = coordinatePixelDetected.X;
                    rectangleArea.BottomRect = coordinatePixelDetected.Y;
                    break;
            }
        }

        private void RightTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {
            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideRight:
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.TopRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.OverTop:
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    rectangleArea.RightRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideRightOverTop:
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.RightRect = coordinatePixelDetected.X;
                    rectangleArea.TopRect = coordinatePixelDetected.Y;
                    break;
            }
        }

        private void RightBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {
            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideRight:
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.BottomRect = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.UnderBottom:
                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    rectangleArea.RightRect = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideRightOverTop:
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.RightRect = coordinatePixelDetected.X;
                    rectangleArea.BottomRect = coordinatePixelDetected.Y;
                    break;
            }
        }



        private void RectangleMoveArea(Point pointMove)
        {

            SKPoint pixelMove = ConvertToPixel(pointMove);
            MovimentInfo actualMoviment = MovimentMethods.GetMovimentInfo(pixelMove.X, pixelMove.Y,
                                                                          bitMapArea.LeftBitMapImg, bitMapArea.TopBitMapImg, bitMapArea.RightBitMapImg,
                                                                          bitMapArea.BottomBitMapImg, rectangleArea.OffSetRectWidth, rectangleArea.OffSetRectHeight);

            switch (actualMoviment)
            {
                case MovimentInfo.InsideArea:

                    rectangleArea.BottomRect = pixelMove.Y + rectangleArea.OffSetRectHeight / 2;
                    rectangleArea.TopRect = rectangleArea.BottomRect - rectangleArea.OffSetRectHeight;
                    rectangleArea.RightRect = pixelMove.X + rectangleArea.OffSetRectWidth / 2;
                    rectangleArea.LeftRect = rectangleArea.RightRect - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideRightOverTop:

                    rectangleArea.BottomRect = bitMapArea.TopBitMapImg + rectangleArea.OffSetRectHeight;
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.LeftRect = rectangleArea.RightRect - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideRight:
                    rectangleArea.BottomRect = pixelMove.Y + rectangleArea.OffSetRectHeight / 2;
                    rectangleArea.TopRect = rectangleArea.BottomRect - rectangleArea.OffSetRectHeight;
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.LeftRect = rectangleArea.RightRect - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeftOverTop:

                    rectangleArea.BottomRect = bitMapArea.TopBitMapImg + rectangleArea.OffSetRectHeight;
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    rectangleArea.RightRect = bitMapArea.LeftBitMapImg + rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OverTop:

                    rectangleArea.BottomRect = bitMapArea.TopBitMapImg + rectangleArea.OffSetRectHeight;
                    rectangleArea.TopRect = bitMapArea.TopBitMapImg;
                    rectangleArea.RightRect = pixelMove.X + rectangleArea.OffSetRectWidth / 2;
                    rectangleArea.LeftRect = rectangleArea.RightRect - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeftUnderBottom:

                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    rectangleArea.TopRect = rectangleArea.BottomRect - rectangleArea.OffSetRectHeight;
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    rectangleArea.RightRect = bitMapArea.LeftBitMapImg + rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.UnderBottom:

                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    rectangleArea.TopRect = rectangleArea.BottomRect - rectangleArea.OffSetRectHeight;
                    rectangleArea.RightRect = pixelMove.X + rectangleArea.OffSetRectWidth / 2;
                    rectangleArea.LeftRect = rectangleArea.RightRect - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeft:

                    rectangleArea.BottomRect = pixelMove.Y + rectangleArea.OffSetRectHeight / 2;
                    rectangleArea.TopRect = rectangleArea.BottomRect - rectangleArea.OffSetRectHeight;
                    rectangleArea.LeftRect = bitMapArea.LeftBitMapImg;
                    rectangleArea.RightRect = bitMapArea.LeftBitMapImg + rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideRightUnderBottom:

                    rectangleArea.BottomRect = bitMapArea.BottomBitMapImg;
                    rectangleArea.TopRect = bitMapArea.BottomBitMapImg - rectangleArea.OffSetRectHeight;
                    rectangleArea.RightRect = bitMapArea.RightBitMapImg;
                    rectangleArea.LeftRect = bitMapArea.RightBitMapImg - rectangleArea.OffSetRectWidth;
                    break;

            }

            rectSelectionArea.Top = rectangleArea.TopRect;
            rectSelectionArea.Right = rectangleArea.RightRect;
            rectSelectionArea.Left = rectangleArea.LeftRect;
            rectSelectionArea.Bottom = rectangleArea.BottomRect;
            rectangleArea.PanMove = true;
            canvasBitMap.InvalidateSurface();

        }


    }
}
