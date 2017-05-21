using Xamarin.Forms;

namespace IMA
{
    public static class UtilityFunctions
    {
        public static Size GetDisplayResolution()
        {
            Size resInfoSize = new Size();
            resInfoSize.Height = DependencyService.Get<IDisplay>().Height;
            resInfoSize.Width = DependencyService.Get<IDisplay>().Width;
            return resInfoSize;
        }
    }
}
