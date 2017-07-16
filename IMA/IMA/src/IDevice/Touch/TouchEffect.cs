using Xamarin.Forms;

namespace TouchTracking
{
    /*
     * Classe che richiama gli effect di ogni singola piattaforma (per ricavare coordinate touch)
     */
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("XamarinDocs.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}
