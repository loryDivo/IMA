using System;
using System.Collections.Generic;
using System.Text;

namespace IMA
{
    /*
     * Classe enum per orientamento device
     */
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
