using System.IO;
using System.Threading.Tasks;
using Lib.Model;

namespace Lib.Media
{
    public interface IImageClient
    {
        Task<Image> GetImage(Image image);
        Task<Image> GetRandomImage();
        Task<Stream> GetImageStream(Image image);
        Task<Stream> GetRandomImageStream(string url);
    }
}