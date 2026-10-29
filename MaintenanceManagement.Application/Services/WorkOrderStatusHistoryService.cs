using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class WorkOrderStatusHistoryService
        : IWorkOrderStatusHistoryService
    {
        #region Fields

        private readonly IWorkOrderStatusHistoryRepository
            _workOrderStatusHistoryRepository;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderStatusHistoryService(
            IWorkOrderStatusHistoryRepository
                workOrderStatusHistoryRepository,
            IMapper mapper)
        {
            _workOrderStatusHistoryRepository =
                workOrderStatusHistoryRepository;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün iş emri durum geçmişlerini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var histories =
                await _workOrderStatusHistoryRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre durum geçmişi detayını getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderStatusHistoryDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var history =
                await _workOrderStatusHistoryRepository
                    .GetByIdWithDetailsAsync(
                        id,
                        cancellationToken);

            if (history == null)
            {
                return null;
            }

            return _mapper.Map<
                GetByIdWorkOrderStatusHistoryDto>(
                    history);
        }

        #endregion

        #region GetByWorkOrderId

        /// <summary>
        /// İş emrinin bütün durum geçmişini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByWorkOrderIdAsync(
                        workOrderId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByChangedByUserId

        /// <summary>
        /// Kullanıcının gerçekleştirdiği
        /// durum değişikliklerini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByChangedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                userId,
                "Kullanıcı");

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByChangedByUserIdAsync(
                        userId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByResponsibleUnitId

        /// <summary>
        /// Sorumlu organizasyon biriminin
        /// iş emri durum geçmişini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Sorumlu Birim");

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByResponsibleUnitIdAsync(
                        unitId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByTeamId

        /// <summary>
        /// Takımın iş emirlerine ait
        /// durum geçmişlerini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                teamId,
                "Takım");

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByTeamIdAsync(
                        teamId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByPreviousStatus

        /// <summary>
        /// Önceki duruma göre geçmiş kayıtlarını getirir.
        ///
        /// null kullanıldığında ilk oluşturulma
        /// kayıtları sorgulanabilir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByPreviousStatusAsync(
                WorkStatusEnum? previousStatus,
                CancellationToken cancellationToken = default)
        {
            ValidateNullableStatus(previousStatus);

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByPreviousStatusAsync(
                        previousStatus,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByNewStatus

        /// <summary>
        /// Geçilen yeni duruma göre
        /// geçmiş kayıtlarını getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByNewStatusAsync(
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default)
        {
            ValidateStatus(newStatus);

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByNewStatusAsync(
                        newStatus,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByStatusTransition

        /// <summary>
        /// Belirli bir önceki durumdan
        /// yeni duruma yapılan geçişleri getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByStatusTransitionAsync(
                WorkStatusEnum? previousStatus,
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default)
        {
            ValidateNullableStatus(previousStatus);

            ValidateStatus(newStatus);

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByStatusTransitionAsync(
                        previousStatus,
                        newStatus,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByWorkOrderAndNewStatus

        /// <summary>
        /// İş emrinin belirtilen yeni
        /// duruma geçtiği kayıtları getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByWorkOrderAndNewStatusAsync(
                int workOrderId,
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateStatus(newStatus);

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByWorkOrderAndNewStatusAsync(
                        workOrderId,
                        newStatus,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// UTC tarih aralığında yapılan
        /// bütün durum değişikliklerini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByDateRangeAsync(
                        startDateUtc,
                        endDateUtc,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetByWorkOrderAndDateRange

        /// <summary>
        /// Belirli iş emrinin UTC tarih
        /// aralığındaki durum geçmişini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByWorkOrderAndDateRangeAsync(
                int workOrderId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var histories =
                await _workOrderStatusHistoryRepository
                    .GetByWorkOrderAndDateRangeAsync(
                        workOrderId,
                        startDateUtc,
                        endDateUtc,
                        cancellationToken);

            return _mapper.Map<
                List<ResultWorkOrderStatusHistoryDto>>(
                    histories);
        }

        #endregion

        #region GetFirstHistoryByWorkOrderId

        /// <summary>
        /// İş emrinin ilk durum geçmişini getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderStatusHistoryDto?>
            GetFirstHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var history =
                await _workOrderStatusHistoryRepository
                    .GetFirstHistoryByWorkOrderIdAsync(
                        workOrderId,
                        cancellationToken);

            if (history == null)
            {
                return null;
            }

            // Detay alanlarını eksiksiz doldurmak için
            // ilişkileri yükleyen sorguyu kullanıyoruz.
            return await GetByIdAsync(
                history.Id,
                cancellationToken);
        }

        #endregion

        #region GetLatestHistoryByWorkOrderId

        /// <summary>
        /// İş emrinin son durum geçmişini getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderStatusHistoryDto?>
            GetLatestHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var history =
                await _workOrderStatusHistoryRepository
                    .GetLatestHistoryByWorkOrderIdAsync(
                        workOrderId,
                        cancellationToken);

            if (history == null)
            {
                return null;
            }

            return await GetByIdAsync(
                history.Id,
                cancellationToken);
        }

        #endregion

        #region HasHistory

        /// <summary>
        /// İş emrinin en az bir durum geçmişi
        /// kaydı bulunuyor mu kontrol eder.
        /// </summary>
        public async Task<bool> HasHistoryAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _workOrderStatusHistoryRepository
                .HasHistoryAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region HasReachedStatus

        /// <summary>
        /// İş emri geçmişte belirli bir
        /// duruma geçmiş mi kontrol eder.
        /// </summary>
        public async Task<bool> HasReachedStatusAsync(
            int workOrderId,
            WorkStatusEnum status,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateStatus(status);

            return await _workOrderStatusHistoryRepository
                .HasReachedStatusAsync(
                    workOrderId,
                    status,
                    cancellationToken);
        }

        #endregion

        #region GetHistoryCountByWorkOrderId

        /// <summary>
        /// İş emrine ait durum geçmişi
        /// kayıt sayısını getirir.
        /// </summary>
        public async Task<int> GetHistoryCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _workOrderStatusHistoryRepository
                .GetHistoryCountByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// ID değerinin geçerli olduğunu doğrular.
        /// </summary>
        private static void ValidateId(
            int id,
            string fieldName)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    $"Geçerli bir {fieldName} seçiniz.");
            }
        }

        /// <summary>
        /// İş emri durumunun tanımlı
        /// bir enum değeri olduğunu kontrol eder.
        /// </summary>
        private static void ValidateStatus(
            WorkStatusEnum status)
        {
            if (!Enum.IsDefined(
                    typeof(WorkStatusEnum),
                    status))
            {
                throw new ValidationException(
                    "Geçersiz iş emri durumu.");
            }
        }

        /// <summary>
        /// Nullable önceki durumu doğrular.
        /// İlk durum geçmişi için null geçerlidir.
        /// </summary>
        private static void ValidateNullableStatus(
            WorkStatusEnum? status)
        {
            if (status.HasValue)
            {
                ValidateStatus(status.Value);
            }
        }

        /// <summary>
        /// UTC tarih aralığı doğrulaması.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Başlangıç ve bitiş tarihleri " +
                    "UTC formatında olmalıdır.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç tarihinden " +
                    "önce olamaz.");
            }
        }

        #endregion
    }
}
