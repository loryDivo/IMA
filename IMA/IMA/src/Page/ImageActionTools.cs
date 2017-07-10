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

        private TouchEffect touchEffect;
        private List<long> allTouchEffect;

        private List<RectangleArea> allRectangleArea;

        private bool enablePanAndZoom = false;
        private PinchGestureRecognizer pinchGesture;
        private PanGestureRecognizer panGesture;

        private StackLayout rectangleInsertIDLayout;
        private Entry rectangleIDText;
        private Button btnConfirmID;

        private bool removeRectangleState = false;

        //PINCH
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;
        private const double OVERSHOOT = 0.15;
        private double startScale;
        private double lastScale;
        private double actualScale;
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

            allRectangleArea = new List<RectangleArea>();
            allTouchEffect = new List<long>();

            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;

            pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;

            touchEffect = new TouchEffect();
            touchEffect.TouchAction += OnTouchEffectAction;
            touchEffect.Capture = true;
            gridLayout.Effects.Add(touchEffect);

            gridLayout.Children.Add(canvasBitMap);

            Content = gridLayout;

        }

        private void ToolbarAdd()
        {
            ToolbarItem enableZoomAndPan = new ToolbarItem()
            {
                Icon = "PanZoom.png",
                Command = new Command(this.EnableZoomAndPan),
            };

            this.ToolbarItems.Add(enableZoomAndPan);


            ToolbarItem rectanglePortion = new ToolbarItem()
            {
                Icon = "rectangleSelection.png",
                Command = new Command(this.AddRectangleIntoImageArea),
            };

            this.ToolbarItems.Add(rectanglePortion);

            ToolbarItem removeRectangle = new ToolbarItem()
            {
                Icon = "rectangleRemove.png",
                Command = new Command(this.RemoveRectangleSelectedIntoImageArea),
            };

            this.ToolbarItems.Add(removeRectangle);

            ToolbarItem sendFile = new ToolbarItem()
            {
                Icon = "sendImage.png",
                Command = new Command(this.SendFileToCompressor),
            };

            this.ToolbarItems.Add(sendFile);

        }

        private void EnableZoomAndPan()
        {
            gridLayout = new Grid();
            gridLayout.Children.Add(canvasBitMap);

            if (enablePanAndZoom)
            {
                touchEffect = new TouchEffect();
                touchEffect.TouchAction += OnTouchEffectAction;
                touchEffect.Capture = true;
                gridLayout.Effects.Add(touchEffect);
                canvasBitMap.GestureRecognizers.Remove(panGesture);
                canvasBitMap.GestureRecognizers.Remove(pinchGesture);
                enablePanAndZoom = false;
            }
            else
            {
                gridLayout.Effects.Remove(touchEffect);
                canvasBitMap.GestureRecognizers.Add(panGesture);
                canvasBitMap.GestureRecognizers.Add(pinchGesture);
                enablePanAndZoom = true;
            }
            Content = gridLayout;
        }

        private void AddRectangleIntoImageArea()
        {
            removeRectangleState = false;
            actualScale = Scale;
            Scale = MIN_SCALE;

            rectangleInsertIDLayout = new StackLayout();

            rectangleIDText = new Entry
            {
                Placeholder = "write rectangle ID",
                WidthRequest = 250,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
            };

            Button btnConfirmID = new Button
            {
                Text = "Confirm",
                Font = Font.SystemFontOfSize(NamedSize.Large),
                BorderWidth = 1,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.Center,
            };

            btnConfirmID.Clicked += OnButtonConfirmIDRectangleClicked;
            rectangleInsertIDLayout.Children.Add(rectangleIDText);
            rectangleInsertIDLayout.Children.Add(btnConfirmID);
            rectangleInsertIDLayout.BackgroundColor = Color.Gray;

            gridLayout.Children.Add(rectangleInsertIDLayout);
        }

        private void RemoveRectangleSelectedIntoImageArea()
        {
            if (removeRectangleState)
            {
                removeRectangleState = false;
            }
            else
            {
                removeRectangleState = true;
                DisplayAlert("Selezionare rettangolo da rimuovere", "Selezionare rettangolo da rimuovere", "OK");
            }
        }

        private void SendFileToCompressor()
        {
            if (ScaleRectCoordinate())
            {
                Navigation.PushAsync(new Sender(this, bitMapArea.BitMapDirectorySource, allRectangleArea));
            }
            else
            {
                Navigation.PushAsync(new Sender(this, bitMapArea.BitMapDirectorySource, null));
            }
        }


        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    lastScale = e.Scale;
                    startScale = Scale;
                    AnchorX = e.ScaleOrigin.X;
                    AnchorY = e.ScaleOrigin.Y;
                    break;

                case GestureStatus.Running:
                    if (e.Scale < 0 || Math.Abs(lastScale - e.Scale) > (lastScale * 1.3) - lastScale)
                    {                   
                        return;
                    }
                    lastScale = e.Scale;
                    var current = Scale + (e.Scale - 1) * startScale;
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

        private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0)
                return minimum;
            else if (value.CompareTo(maximum) > 0)
                return maximum;
            else
                return value;
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

            DisplayChange(canvas);

            foreach (RectangleArea rectangleArea in allRectangleArea)
            {
                SKRect rectSelectionArea = new SKRect(rectangleArea.Left, rectangleArea.Top, rectangleArea.Right, rectangleArea.Bottom);

                if (rectangleArea.ResizeMove)
                {
                    paintResizeRectSelectionArea = new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill,
                        Color = SKColors.LightSkyBlue,
                    };
                    canvas.DrawCircle(rectangleArea.Left, rectangleArea.Top, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.Left, rectangleArea.Bottom, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.Right, rectangleArea.Top, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    canvas.DrawCircle(rectangleArea.Right, rectangleArea.Bottom, rectangleArea.RadiousOfCircleRect, paintResizeRectSelectionArea);
                    rectangleArea.ResizeMove = false;
                }
                rectangleArea.CalculateNewPositionIDRect();
                canvas.DrawRect(rectSelectionArea, rectangleArea.PaintRect);
                canvas.DrawText(rectangleArea.Id, rectangleArea.PositionIDRect.X, rectangleArea.PositionIDRect.Y, rectangleArea.PaintIDRect);
            }
        }

        private void DisplayChange(SKCanvas canvas)
        {
            if (display.PrevRatio == 0)
            {
                display.PrevRatio = display.AspectRatio;
            }
            else if (display.AspectRatio != display.PrevRatio)
            {
                // CHANGE SCALE ?????
                RescaleRectangleCoordinate();
                display.PrevRatio = display.AspectRatio;
            }
        }

        private void RescaleRectangleCoordinate()
        {
            allRectangleArea.RemoveRange(0, allRectangleArea.Count);
        }

        private bool ScaleRectCoordinate()
        {
            if (allRectangleArea.Count > 0)
            {
                foreach (RectangleArea rectangleArea in allRectangleArea)
                {
                    rectangleArea.CalculateScaleVertexCoordinate(bitMapArea);
                }
                return true;
            }
            else
            {
                DisplayAlert("Nessuna selezione", "Non è stata selezionata nessuna area, il server elaborerà tutta la foto", "OK");
                return false;
            }
        }

        private void OnButtonConfirmIDRectangleClicked(object sender, EventArgs e)
        {
            if(rectangleIDText.Text != null && !DupplicatedIDValue(rectangleIDText.Text))
            {
                gridLayout.Children.Remove(rectangleInsertIDLayout);

                RectangleArea rectangleArea = new RectangleArea(rectangleIDText.Text);

                rectangleArea.Top = bitMapArea.Top;
                rectangleArea.Left = bitMapArea.Left;
                rectangleArea.Right = rectangleArea.Left + rectangleArea.OffSetRectWidth;
                rectangleArea.Bottom = rectangleArea.Top + rectangleArea.OffSetRectHeight;
                allRectangleArea.Add(rectangleArea);

                Scale = actualScale;
                canvasBitMap.InvalidateSurface();
            }
            else
            {
                DisplayAlert("ID non valido", "Inserire un ID valido", "OK");
            }
        }

        private bool DupplicatedIDValue(string idNewRect)
        {
            foreach (RectangleArea rectangleArea in allRectangleArea)
            {
                if (rectangleArea.EqualsID(idNewRect))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    foreach(RectangleArea rectangleArea in allRectangleArea)
                    {
                        if (rectangleArea.CheckIfInAnyRectangle(UtilityFunctions.ConvertToPixel(args.Location, canvasBitMap)) && !allTouchEffect.Contains(args.Id))
                        {
                            allTouchEffect.Add(args.Id);
                            if (removeRectangleState)
                            {
                                allRectangleArea.Remove(rectangleArea);
                                break;
                            }
                            else
                            {
                                rectangleArea.RectangleSelected = true;
                            }
                        }
                    }
                    if (removeRectangleState)
                    {
                        canvasBitMap.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    foreach (RectangleArea rectangleArea in allRectangleArea)
                    {
                        if (allTouchEffect.Contains(args.Id) && rectangleArea.RectangleSelected)
                        {
                            rectangleArea.ResizeInfo = CheckIfResize(args.Location, rectangleArea);
                            if (rectangleArea.ResizeInfo != ResizeInfo.none)
                            {
                                ResizeRect(rectangleArea.PixelCoordinateDetected, rectangleArea.ResizeInfo, rectangleArea);
                            }
                            else if (rectangleArea.CheckIfInAnyRectangle(UtilityFunctions.ConvertToPixel(args.Location, canvasBitMap)))
                            {
                                RectangleMoveArea(args.Location, rectangleArea);
                            }
                        }
                    }
                    break;
                case TouchActionType.Released:
                    if (allTouchEffect.Contains(args.Id))
                    {
                        foreach (RectangleArea rectangleArea in allRectangleArea)
                        {
                            if (rectangleArea.RectangleSelected)
                            {
                                rectangleArea.RectangleSelected = false;
                            }
                        }
                        allTouchEffect.Remove(args.Id);
                    }
                    break;
            }
        }

        private ResizeInfo CheckIfResize(Point coordinateDetected, RectangleArea rectangleArea)
        {

            rectangleArea.CalculateVertexCoordinate();
            rectangleArea.CalculateOneRadiousCoordinate(coordinateDetected, canvasBitMap);

            ResizeInfo resizeDetected = MovimentMethods.CheckIfResize(rectangleArea.PixelCoordinateDetected, rectangleArea.OneRadiousPixelCoordinate,
                                                                     rectangleArea.LeftTopPixelCoordinate, rectangleArea.LeftBottomPixelCoordinate,
                                                                     rectangleArea.RightTopPixelCoordinate, rectangleArea.RightBottomPixelCoordinate);

            return resizeDetected;
        }

        private void ResizeRect(SKPoint coordinatePixelDetected, ResizeInfo resizeDetected, RectangleArea rectangleArea)
        {

            MovimentInfo resizeMovimentDetected = MovimentMethods.GetResizeInfo(coordinatePixelDetected.X, coordinatePixelDetected.Y,
                                                                            bitMapArea.Left, bitMapArea.Top, bitMapArea.Right, bitMapArea.Bottom);

            switch (resizeDetected)
            {
                case ResizeInfo.LeftTopAround:
                    LeftTopResizeCase(coordinatePixelDetected, resizeMovimentDetected, rectangleArea);
                    break;

                case ResizeInfo.LeftBottomAround:
                    LeftBottomResizeCase(coordinatePixelDetected, resizeMovimentDetected, rectangleArea);
                    break;

                case ResizeInfo.RightTopAround:
                    RightTopResizeCase(coordinatePixelDetected, resizeMovimentDetected, rectangleArea);
                    break;

                case ResizeInfo.RightBottomAround:
                    RightBottomResizeCase(coordinatePixelDetected, resizeMovimentDetected, rectangleArea);
                    break;

            }
            rectangleArea.OffSetRectHeight = rectangleArea.Bottom - rectangleArea.Top;
            rectangleArea.OffSetRectWidth = rectangleArea.Right - rectangleArea.Left;
            rectangleArea.ResizeMove = true;
            canvasBitMap.InvalidateSurface();
        }

        private void LeftTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected, RectangleArea rectangleArea)
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


        private void LeftBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected, RectangleArea rectangleArea)
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

        private void RightTopResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected, RectangleArea rectangleArea)
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

        private void RightBottomResizeCase(SKPoint coordinatePixelDetected, MovimentInfo resizeMovimentDetected, RectangleArea rectangleArea)
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



        private void RectangleMoveArea(Point pointMove, RectangleArea rectangleArea)
        {

            SKPoint pixelMove = UtilityFunctions.ConvertToPixel(pointMove, canvasBitMap);
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
            canvasBitMap.InvalidateSurface();
        }
    }
}
