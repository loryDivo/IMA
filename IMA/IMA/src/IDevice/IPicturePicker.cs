using System.IO;
using System.Threading.Tasks;

namespace IMA.src.IDevice
{
    /*
     *  Interaccia per implementazione galleria di ogni singola piattaforma
     */
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
        string GetImageRealPath();
    }
}
