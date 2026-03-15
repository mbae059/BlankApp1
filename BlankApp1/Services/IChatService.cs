using BlankApp1.Models.DTO;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public interface IChatService
    {
        Task SendMessageAsync(MessagePayLoad messagePayLoad);
    }
}
