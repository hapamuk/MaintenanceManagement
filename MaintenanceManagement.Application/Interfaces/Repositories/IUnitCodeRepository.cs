using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IUnitCodeRepository
    {
        /// <summary>
        /// Birim kodunu veritabanı transaction'ı
        /// içerisinde otomatik oluşturur.
        ///
        /// Birim kaydı ve sayaç artışı birlikte
        /// kaydedilir.
        /// </summary> 
        Task<Unit> CreateWithNextCodeAsync(
            Unit unit,
            CancellationToken cancellationToken = default);
    }
}
