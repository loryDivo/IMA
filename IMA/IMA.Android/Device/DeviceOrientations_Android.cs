using Android.Content;
using Android.Runtime;
using Android.Views;
using IMA.Droid;
using Xamarin.Forms;

[assembly:Dependency (typeof (DeviceOrientations_Android))]

namespace IMA.Droid
{
  /*
  * Classe orientamento device Android
  */
    public class DeviceOrientations_Android : IDeviceOrientation
    {
        public DeviceOrientations_Android()
        {

        }
        public static void Init()
        {

        }

        public DeviceOrientations GetOrientation()
        {
            IWindowManager windowsManager = Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            var rotation = windowsManager.DefaultDisplay.Rotation;
            bool isLandScape = rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270;
            return isLandScape ? DeviceOrientations.Landscape : DeviceOrientations.Portraint;

        }

    }
}