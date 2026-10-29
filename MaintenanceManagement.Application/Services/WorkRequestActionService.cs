using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkRequestActionDtos;
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
    public class WorkRequestActionService
         : IWorkRequestActionService
    {
        private readonly IWorkRequestActionRepository
            _workRequestActionRepository;

        private readonly IMapper _mapper;

        public WorkRequestActionService(
            IWorkRequestActionRepository workRequestActionRepository,
            IMapper mapper)
        {
            _workRequestActionRepository =
                workRequestActionRepository;

            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün iş talebi işlem geçmişini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var actions = await _workRequestActionRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetById

        /// <summary>
        /// İşlem kaydını talep ve kullanıcı
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<GetByIdWorkRequestActionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var action = await _workRequestActionRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (action == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkRequestActionDto>(
                action);
        }

        #endregion

        #region GetByWorkRequestId

        /// <summary>
        /// Belirtilen iş talebinin işlem geçmişini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            var actions = await _workRequestActionRepository
                .GetActionsByWorkRequestIdAsync(
                    workRequestId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetByRequestNumber

        /// <summary>
        /// Talep numarasına göre işlem geçmişini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByRequestNumberAsync(
                string requestNumber,
                CancellationToken cancellationToken = default)
        {
            string normalizedNumber = ValidateRequiredText(
                requestNumber,
                "Talep Numarası");

            var actions = await _workRequestActionRepository
                .GetActionsByRequestNumberAsync(
                    normalizedNumber,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetByUserId

        /// <summary>
        /// Kullanıcının gerçekleştirdiği
        /// iş talebi işlemlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                userId,
                "Kullanıcı");

            var actions = await _workRequestActionRepository
                .GetActionsByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetByResponsibleUnitId

        /// <summary>
        /// Sorumlu organizasyon birimine ait
        /// iş taleplerinin işlem geçmişini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Sorumlu Organizasyon Birimi");

            var actions = await _workRequestActionRepository
                .GetActionsByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetByType

        /// <summary>
        /// İşlem türüne göre hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByTypeAsync(
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default)
        {
            ValidateActionType(actionType);

            var actions = await _workRequestActionRepository
                .GetActionsByTypeAsync(
                    actionType,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetByRequestAndType

        /// <summary>
        /// Belirli iş talebinin seçilen işlem
        /// türündeki kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByRequestAndTypeAsync(
                int workRequestId,
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            ValidateActionType(actionType);

            var actions = await _workRequestActionRepository
                .GetActionsByRequestAndTypeAsync(
                    workRequestId,
                    actionType,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// UTC tarih aralığında gerçekleşen
        /// işlemleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var actions = await _workRequestActionRepository
                .GetActionsByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestActionDto>>(
                actions);
        }

        #endregion

        #region GetLatestByRequestId

        /// <summary>
        /// İş talebinin en son hareketini getirir.
        /// </summary>
        public async Task<GetByIdWorkRequestActionDto?>
            GetLatestByRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            var action = await _workRequestActionRepository
                .GetLatestActionByRequestIdAsync(
                    workRequestId,
                    cancellationToken);

            if (action == null)
            {
                return null;
            }

            return await GetByIdAsync(
                action.Id,
                cancellationToken);
        }

        #endregion

        #region GetLatestByType

        /// <summary>
        /// İş talebinin ilgili işlem türündeki
        /// son kaydını getirir.
        /// </summary>
        public async Task<GetByIdWorkRequestActionDto?>
            GetLatestByTypeAsync(
                int workRequestId,
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            ValidateActionType(actionType);

            var action = await _workRequestActionRepository
                .GetLatestActionByTypeAsync(
                    workRequestId,
                    actionType,
                    cancellationToken);

            if (action == null)
            {
                return null;
            }

            return await GetByIdAsync(
                action.Id,
                cancellationToken);
        }

        #endregion

        #region HasActions

        /// <summary>
        /// İş talebinde işlem geçmişi var mı?
        /// </summary>
        public async Task<bool> HasActionsAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            return await _workRequestActionRepository
                .HasActionsAsync(
                    workRequestId,
                    cancellationToken);
        }

        #endregion

        #region HasActionType

        /// <summary>
        /// İş talebinde belirtilen işlem türü
        /// mevcut mu kontrol eder.
        /// </summary>
        public async Task<bool> HasActionTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            ValidateActionType(actionType);

            return await _workRequestActionRepository
                .HasActionTypeAsync(
                    workRequestId,
                    actionType,
                    cancellationToken);
        }

        #endregion

        #region GetActionCountByRequestId

        /// <summary>
        /// İş talebine ait toplam işlem sayısını getirir.
        /// </summary>
        public async Task<int> GetActionCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            return await _workRequestActionRepository
                .GetActionCountByRequestIdAsync(
                    workRequestId,
                    cancellationToken);
        }

        #endregion

        #region GetActionCountByType

        /// <summary>
        /// İş talebinin belirli türdeki
        /// işlem sayısını getirir.
        /// </summary>
        public async Task<int> GetActionCountByTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workRequestId,
                "İş Talebi");

            ValidateActionType(actionType);

            return await _workRequestActionRepository
                .GetActionCountByTypeAsync(
                    workRequestId,
                    actionType,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// ID değerinin sıfırdan büyük olduğunu doğrular.
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
        /// Zorunlu metin alanlarını doğrular
        /// ve baştaki/sondaki boşlukları kaldırır.
        /// </summary>
        private static string ValidateRequiredText(
            string? value,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(
                    $"{fieldName} zorunludur.");
            }

            return value.Trim();
        }

        /// <summary>
        /// Yalnızca mevcut işlem türlerine
        /// izin verir.
        /// </summary>
        private static void ValidateActionType(
            WorkRequestActionTypeEnum actionType)
        {
            if (!Enum.IsDefined(
                    typeof(WorkRequestActionTypeEnum),
                    actionType))
            {
                throw new ValidationException(
                    "Geçersiz iş talebi işlem türü.");
            }
        }

        /// <summary>
        /// Tarih aralığının UTC ve doğru
        /// sıralamada olduğunu doğrular.
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
                    "UTC olarak gönderilmelidir.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        #endregion
    }
}
