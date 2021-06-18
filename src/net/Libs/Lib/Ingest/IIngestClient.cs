using System.Threading.Tasks;
using Lib.Model;

namespace Lib.Ingest
{
    public interface IIngestClient
    {
        Task<Image> Ingest(Image image);
    }
}