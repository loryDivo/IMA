using System.IO;
using System.Threading.Tasks;

namespace IMA.src.IDevice
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
        string GetImageRealPath();
    }
}
