using System.Threading;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public interface IVideoService
    {
        Task SendVideoChunkAsync(byte[] chunk, CancellationToken cancellationToken = default);
    }
}
