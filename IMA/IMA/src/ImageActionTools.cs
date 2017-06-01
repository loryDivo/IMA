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

            display.CalculateDisplayVertexPixelCoordinate(info);

            display.AspectRatio = Math.Min((float)info.Width / bitMapArea.BitMap.Width,
                        (float)info.Height / bitMapArea.BitMap.Height);

            bitMapArea.CalculateBitMapSize(info, display);

            rectangleBitMapImg = new SKRect(bitMapArea.Left, bitMapArea.Top, bitMapArea.Right, bitMapArea.Bottom);

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
                    canvas.DrawCircle(rectangleArea.Left, rectangleArea.Top, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.Left, rectangleArea.Bottom, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.Right, rectangleArea.Top, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.Right, rectangleArea.Bottom, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
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

        private void DrawRectArea(SKCanvas canvas, Boolean defaultRectPosition)
        {
            if (defaultRectPosition)
            {
                rectangleArea.Top = bitMapArea.Top;
                rectangleArea.Left = bitMapArea.Left;
            }
            else
            {
                //Resize rect
            }
            rectangleArea.Right = rectangleArea.Left + rectangleArea.OffSetRectWidth;
            rectangleArea.Bottom = rectangleArea.Top + rectangleArea.OffSetRectHeight;

            rectSelectionArea = new SKRect(rectangleArea.Left, rectangleArea.Top, rectangleArea.Right, rectangleArea.Bottom);
            canvas.DrawRect(rectSelectionArea, paintRectSelectionArea);
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasBitMap.CanvasSize.Width * pt.X / canvasBitMap.Width),
                               (float)(canvasBitMap.CanvasSize.Height * pt.Y / canvasBitMap.Height));
        }

        private void SendFileToCompressor()
        {
            if (ScaleRectCoordinate())
            {
                Navigation.PushAsync(new Sender(this, bitMapArea.BitMapDirectorySource, rectangleArea));
            }
            else
            {
                Navigation.PushAsync(new Sender(this, bitMapArea.BitMapDirectorySource, null));
            }
        }

        private bool ScaleRectCoordinate()
        {
            if (rectangleArea.DrawRectArea)
            {
                rectangleArea.CalculateScaleVertexCoordinate(bitMapArea);
                return true;
            }
            else
            {
                DisplayAlert("Nessuna selezione", "Non è stata selezionata nessuna area, il server elaborerà tutta la foto", "OK");
                return false;
            }
        }

        private void ShowRectangleIntoImageArea()
        {
            if (!rectangleArea.DrawRectArea)
            {

                rectangleArea.Top = bitMapArea.Top;
                rectangleArea.Left = bitMapArea.Left;
                rectangleArea.Right = rectangleArea.Left + rectangleArea.OffSetRectWidth;
                rectangleArea.Bottom = rectangleArea.Top + rectangleArea.OffSetRectHeight;

                rectSelectionArea = new SKRect(rectangleArea.Left, rectangleArea.Top, rectangleArea.Right, rectangleArea.Bottom);
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
                            ResizeRect(rectangleArea.PixelCoordinateDetected, rectangleArea.ResizeInfo);
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

            rectangleArea.CalculateVertexCoordinate();
            rectangleArea.CalculateOneRadiousCoordinate(coordinateDetected, canvasBitMap);

            ResizeInfo resizeDetected = MovimentMethods.CheckIfResize(rectangleArea.PixelCoordinateDetected, rectangleArea.OneRadiousPixelCoordinate,
                                                                     rectangleArea.LeftTopPixelCoordinate, rectangleArea.LeftBottomPixelCoordinate,
                                                                     rectangleArea.RightTopPixelCoordinate, rectangleArea.RightBottomPixelCoordinate);

            return resizeDetected;
        }

        private void ResizeRect(SKPoint coordinatePixelDetected, ResizeInfo resizeDetected)
        {

            MovimentInfo resizeMovimentDetected = MovimentMethods.GetResizeInfo(coordinatePixelDetected.X, coordinatePixelDetected.Y,
                                                                            bitMapArea.Left, bitMapArea.Top, bitMapArea.Right, bitMapArea.Bottom);

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
            rectSelectionArea.Top = rectangleArea.Top;
            rectSelectionArea.Right = rectangleArea.Right;
            rectSelectionArea.Left = rectangleArea.Left;
            rectSelectionArea.Bottom = rectangleArea.Bottom;
            rectangleArea.OffSetRectHeight = rectangleArea.Bottom - rectangleArea.Top;
            rectangleArea.OffSetRectWidth = rectangleArea.Right - rectangleArea.Left;
            rectangleArea.ResizeMove = true;
            canvasBitMap.InvalidateSurface();
        }

        private void LeftTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {

            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideLeft:
                    rectangleArea.Left = bitMapArea.Left;
                    rectangleArea.Top = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.OverTop:
                    rectangleArea.Top = bitMapArea.Top;
                    rectangleArea.Left = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideLeftOverTop:
                    rectangleArea.Top = bitMapArea.Top;
                    rectangleArea.Left = bitMapArea.Left;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.Left = coordinatePixelDetected.X;
                    rectangleArea.Top = coordinatePixelDetected.Y;
                    break;

            }
        }


        private void LeftBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {

            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideLeft:
                    rectangleArea.Left = bitMapArea.Left;
                    rectangleArea.Bottom = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.UnderBottom:
                    rectangleArea.Bottom = bitMapArea.Bottom;
                    rectangleArea.Left = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideLeftUnderBottom:
                    rectangleArea.Left = bitMapArea.Left;
                    rectangleArea.Bottom = bitMapArea.Bottom;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.Left = coordinatePixelDetected.X;
                    rectangleArea.Bottom = coordinatePixelDetected.Y;
                    break;
            }
        }

        private void RightTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {
            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideRight:
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Top = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.OverTop:
                    rectangleArea.Top = bitMapArea.Top;
                    rectangleArea.Right = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideRightOverTop:
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Top = bitMapArea.Top;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.Right = coordinatePixelDetected.X;
                    rectangleArea.Top = coordinatePixelDetected.Y;
                    break;
            }
        }

        private void RightBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected)
        {
            switch (resizeMovimentDetected)
            {
                case MovimentInfo.OutSideRight:
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Bottom = coordinatePixelDetected.Y;
                    break;

                case MovimentInfo.UnderBottom:
                    rectangleArea.Bottom = bitMapArea.Bottom;
                    rectangleArea.Right = coordinatePixelDetected.X;
                    break;

                case MovimentInfo.OutSideRightOverTop:
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Bottom = bitMapArea.Bottom;
                    break;

                case MovimentInfo.InsideArea:
                    rectangleArea.Right = coordinatePixelDetected.X;
                    rectangleArea.Bottom = coordinatePixelDetected.Y;
                    break;
            }
        }



        private void RectangleMoveArea(Point pointMove)
        {

            SKPoint pixelMove = ConvertToPixel(pointMove);
            MovimentInfo actualMoviment = MovimentMethods.GetMovimentInfo(pixelMove.X, pixelMove.Y,
                                                                          bitMapArea.Left, bitMapArea.Top, bitMapArea.Right,
                                                                          bitMapArea.Bottom, rectangleArea.OffSetRectWidth, rectangleArea.OffSetRectHeight);

            switch (actualMoviment)
            {
                case MovimentInfo.InsideArea:

                    rectangleArea.Bottom = pixelMove.Y + rectangleArea.OffSetRectHeight / 2;
                    rectangleArea.Top = rectangleArea.Bottom - rectangleArea.OffSetRectHeight;
                    rectangleArea.Right = pixelMove.X + rectangleArea.OffSetRectWidth / 2;
                    rectangleArea.Left = rectangleArea.Right - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideRightOverTop:

                    rectangleArea.Bottom = bitMapArea.Top + rectangleArea.OffSetRectHeight;
                    rectangleArea.Top = bitMapArea.Top;
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Left = rectangleArea.Right - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideRight:
                    rectangleArea.Bottom = pixelMove.Y + rectangleArea.OffSetRectHeight / 2;
                    rectangleArea.Top = rectangleArea.Bottom - rectangleArea.OffSetRectHeight;
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Left = rectangleArea.Right - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeftOverTop:

                    rectangleArea.Bottom = bitMapArea.Top + rectangleArea.OffSetRectHeight;
                    rectangleArea.Top = bitMapArea.Top;
                    rectangleArea.Left = bitMapArea.Left;
                    rectangleArea.Right = bitMapArea.Left + rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OverTop:

                    rectangleArea.Bottom = bitMapArea.Top + rectangleArea.OffSetRectHeight;
                    rectangleArea.Top = bitMapArea.Top;
                    rectangleArea.Right = pixelMove.X + rectangleArea.OffSetRectWidth / 2;
                    rectangleArea.Left = rectangleArea.Right - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeftUnderBottom:

                    rectangleArea.Bottom = bitMapArea.Bottom;
                    rectangleArea.Top = rectangleArea.Bottom - rectangleArea.OffSetRectHeight;
                    rectangleArea.Left = bitMapArea.Left;
                    rectangleArea.Right = bitMapArea.Left + rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.UnderBottom:

                    rectangleArea.Bottom = bitMapArea.Bottom;
                    rectangleArea.Top = rectangleArea.Bottom - rectangleArea.OffSetRectHeight;
                    rectangleArea.Right = pixelMove.X + rectangleArea.OffSetRectWidth / 2;
                    rectangleArea.Left = rectangleArea.Right - rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideLeft:

                    rectangleArea.Bottom = pixelMove.Y + rectangleArea.OffSetRectHeight / 2;
                    rectangleArea.Top = rectangleArea.Bottom - rectangleArea.OffSetRectHeight;
                    rectangleArea.Left = bitMapArea.Left;
                    rectangleArea.Right = bitMapArea.Left + rectangleArea.OffSetRectWidth;
                    break;

                case MovimentInfo.OutSideRightUnderBottom:

                    rectangleArea.Bottom = bitMapArea.Bottom;
                    rectangleArea.Top = bitMapArea.Bottom - rectangleArea.OffSetRectHeight;
                    rectangleArea.Right = bitMapArea.Right;
                    rectangleArea.Left = bitMapArea.Right - rectangleArea.OffSetRectWidth;
                    break;

            }

            rectSelectionArea.Top = rectangleArea.Top;
            rectSelectionArea.Right = rectangleArea.Right;
            rectSelectionArea.Left = rectangleArea.Left;
            rectSelectionArea.Bottom = rectangleArea.Bottom;
            rectangleArea.PanMove = true;
            canvasBitMap.InvalidateSurface();

        }


    }
}
