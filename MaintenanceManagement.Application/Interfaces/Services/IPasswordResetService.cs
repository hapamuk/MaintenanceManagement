using MaintenanceManagement.Application.DTOS.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IPasswordResetService
    {
        Task RequestAsync(
            ForgotPasswordUserDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> ResetAsync(
            ResetPasswordUserDto dto,
            CancellationToken cancellationToken = default);
    }
}
