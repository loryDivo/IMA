using System;
using System.Collections.Generic;
using System.Text;

namespace IMA
{
    public enum DeviceOrientations
    {
        Undefined,
        Landscape,
        Portraint,
    }

    public interface IDeviceOrientation
    {
        DeviceOrientations GetOrientation();
    }

}
