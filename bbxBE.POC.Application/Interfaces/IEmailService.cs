using bbxBE.POC.Application.DTOs.Email;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}