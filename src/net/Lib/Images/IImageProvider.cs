using System;
using System.Threading.Tasks;
using Lib.Model;

namespace Lib.Images
{
    public interface IImageProvider
    {
        Task<Image> GetImage();
    }
}