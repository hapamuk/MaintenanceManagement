using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IPasswordResetEmailSender
    {
        Task SendAsync(
            string email,
            string resetUrl,
            CancellationToken cancellationToken = default);
    }
}
