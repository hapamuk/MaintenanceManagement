using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrderFilesDtos;
using MaintenanceManagement.Application.Interfaces;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class WorkOrderFileService : IWorkOrderFileService
    {
        #region Constants

        private const string CreatePermission =
            "WORK_ORDER_CREATE";

        private const string ExecutePermission =
            "WORK_ORDER_EXECUTE";

        private const long MaxSingleFileSizeBytes =
            20L * 1024 * 1024;

        private const long MaxTotalFileSizeBytes =
            100L * 1024 * 1024;

        private const int MaxFileCountPerWorkOrder = 20;

        #endregion

        #region Fields

        private readonly IWorkOrderFileRepository
            _workOrderFileRepository;

        private readonly IWorkOrderRepository
            _workOrderRepository;

        private readonly IWorkOrderPersonnelRepository
            _workOrderPersonnelRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderFileService(
            IWorkOrderFileRepository workOrderFileRepository,
            IWorkOrderRepository workOrderRepository,
            IWorkOrderPersonnelRepository workOrderPersonnelRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _workOrderFileRepository = workOrderFileRepository;

            _workOrderRepository = workOrderRepository;

            _workOrderPersonnelRepository =
                workOrderPersonnelRepository;

            _userRepository = userRepository;

            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var files = await _workOrderFileRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkOrderFileDto>>(files);
        }

        #endregion

        #region GetById

        public async Task<GetByIdWorkOrderFileDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var file = await _workOrderFileRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            return file == null
                ? null
                : _mapper.Map<GetByIdWorkOrderFileDto>(file);
        }

        #endregion

        #region GetByIdAndWorkOrderId

        public async Task<GetByIdWorkOrderFileDto?>
            GetByIdAndWorkOrderIdAsync(
                int id,
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(id, "Dosya");

            ValidateId(workOrderId, "İş Emri");

            var file = await _workOrderFileRepository
                .GetByIdAndWorkOrderIdAsync(
                    id,
                    workOrderId,
                    cancellationToken);

            if (file == null)
            {
                return null;
            }

            return await GetByIdAsync(
                file.Id,
                cancellationToken);
        }

        #endregion

        #region GetByWorkOrderId

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var files = await _workOrderFileRepository
                .GetFilesByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderFileDto>>(files);
        }

        #endregion

        #region GetByUploadedByUserId

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByUploadedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var files = await _workOrderFileRepository
                .GetFilesByUploadedByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderFileDto>>(files);
        }

        #endregion

        #region GetByWorkOrderAndUser

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            ValidateId(userId, "Kullanıcı");

            var files = await _workOrderFileRepository
                .GetFilesByWorkOrderAndUserAsync(
                    workOrderId,
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderFileDto>>(files);
        }

        #endregion

        #region GetByResponsibleUnitId

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var files = await _workOrderFileRepository
                .GetFilesByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderFileDto>>(files);
        }

        #endregion

        #region GetByContentType

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByContentTypeAsync(
                string contentType,
                CancellationToken cancellationToken = default)
        {
            string normalizedType = ValidateRequiredText(
                contentType,
                100,
                "İçerik Türü");

            // Doğrulanmamış özel Repository imzası
            // eklememek için mevcut detay sorgusu
            // üzerinden filtreleme yapılıyor.
            var files = await _workOrderFileRepository
                .GetAllWithDetailsAsync(cancellationToken);

            var filteredFiles = files
                .Where(x => string.Equals(
                    x.ContentType,
                    normalizedType,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            return _mapper.Map<List<ResultWorkOrderFileDto>>(
                filteredFiles);
        }

        #endregion

        #region GetByDateRange

        public async Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var files = await _workOrderFileRepository
                .GetAllWithDetailsAsync(cancellationToken);

            var filteredFiles = files
                .Where(x =>
                    x.CreatedAt >= startDateUtc &&
                    x.CreatedAt <= endDateUtc)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return _mapper.Map<List<ResultWorkOrderFileDto>>(
                filteredFiles);
        }

        #endregion

        #region GetByFilePath

        public async Task<GetByIdWorkOrderFileDto?>
            GetByFilePathAsync(
                string filePath,
                CancellationToken cancellationToken = default)
        {
            string normalizedPath =
                ValidateStoragePath(filePath);

            var files = await _workOrderFileRepository
                .GetAllWithDetailsAsync(cancellationToken);

            var file = files.FirstOrDefault(x =>
                string.Equals(
                    x.FilePath,
                    normalizedPath,
                    StringComparison.Ordinal));

            return file == null
                ? null
                : await GetByIdAsync(
                    file.Id,
                    cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Önceden güvenli depolama alanına yüklenen
        /// dosyanın metadata kaydını oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkOrderFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.WorkOrderId, "İş Emri");

            ValidateId(currentUserId, "Kullanıcı");

            // =====================================
            // 2. DOSYA BİLGİLERİ
            // =====================================

            string fileName = ValidateFileName(dto.FileName);

            string filePath = ValidateStoragePath(dto.FilePath);

            string? contentType = NormalizeOptionalText(
                dto.ContentType,
                100,
                "İçerik Türü");

            ValidateFileSize(dto.FileSizeBytes);

            // =====================================
            // 3. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await GetWorkOrderOrThrowAsync(
                dto.WorkOrderId,
                cancellationToken);

            // =====================================
            // 4. YÜKLEME YETKİSİNİ DOĞRULA
            // =====================================

            await ValidateFileWritePermissionAsync(
                workOrder,
                currentUserId,
                cancellationToken);

            // =====================================
            // 5. BENZERSİZ DEPOLAMA YOLU
            // =====================================

            bool existsPath = await ExistsFilePathAsync(
                filePath,
                null,
                cancellationToken);

            if (existsPath)
            {
                throw new InvalidOperationException(
                    "Bu depolama yoluna ait başka " +
                    "bir dosya kaydı bulunmaktadır.");
            }

            // =====================================
            // 6. DOSYA SAYISI VE TOPLAM BOYUT
            // =====================================

            var existingFiles = await _workOrderFileRepository
                .GetFilesByWorkOrderIdAsync(
                    dto.WorkOrderId,
                    cancellationToken);

            if (existingFiles.Count() >= MaxFileCountPerWorkOrder)
            {
                throw new ValidationException(
                    "Bir iş emrine en fazla 20 " +
                    "dosya eklenebilir.");
            }

            long currentTotalSize = existingFiles
                .Sum(x => x.FileSizeBytes);

            if (currentTotalSize < 0 ||
                currentTotalSize > MaxTotalFileSizeBytes ||
                dto.FileSizeBytes >
                    MaxTotalFileSizeBytes - currentTotalSize)
            {
                throw new ValidationException(
                    "İş emrinin toplam dosya boyutu " +
                    "100 MB sınırını aşamaz.");
            }

            // =====================================
            // 7. DTO -> ENTITY
            // =====================================

            var file = _mapper.Map<WorkOrderFile>(dto);

            file.WorkOrderId = dto.WorkOrderId;

            file.FileName = fileName;

            file.FilePath = filePath;

            file.ContentType = contentType;

            file.FileSizeBytes = dto.FileSizeBytes;

            // Bu alan DTO'dan alınmaz.
            file.UploadedByUserId = currentUserId;

            // =====================================
            // 8. REPOSITORY KAYDI
            // =====================================

            var createdFile = await _workOrderFileRepository
                .CreateAsync(
                    file,
                    cancellationToken);

            return createdFile.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Dosyanın yalnızca görünen adını günceller.
        /// Fiziksel dosyanın adı ve yolu değişmez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkOrderFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Dosya");

            ValidateId(currentUserId, "Kullanıcı");

            string fileName = ValidateFileName(dto.FileName);

            // =====================================
            // 2. MEVCUT DOSYAYI GETİR
            // =====================================

            var existingFile = await _workOrderFileRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingFile == null)
            {
                return false;
            }

            // =====================================
            // 3. DOSYAYI YÜKLEYEN KULLANICI
            // =====================================

            if (existingFile.UploadedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Dosyanın adını yalnızca dosyayı " +
                    "yükleyen kullanıcı değiştirebilir.");
            }

            // =====================================
            // 4. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await GetWorkOrderOrThrowAsync(
                existingFile.WorkOrderId,
                cancellationToken);

            // =====================================
            // 5. DÜZENLEME YETKİSİ
            // =====================================

            await ValidateFileWritePermissionAsync(
                workOrder,
                currentUserId,
                cancellationToken);

            // =====================================
            // 6. MEVCUT ENTITY'Yİ GÜNCELLE
            // =====================================

            _mapper.Map(dto, existingFile);

            existingFile.FileName = fileName;

            // WorkOrderId, FilePath, ContentType,
            // FileSizeBytes, UploadedByUserId,
            // CreatedAt ve ilişkiler korunur.

            // =====================================
            // 7. GÜNCELLEME
            // =====================================

            return await _workOrderFileRepository
                .UpdateAsync(
                    existingFile,
                    cancellationToken);
        }

        #endregion

        #region ExistsFilePath

        public async Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedPath =
                ValidateStoragePath(filePath);

            ValidateExcludedId(excludedId);

            var files = await _workOrderFileRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return files.Any(x =>
                x.Id != excludedId &&
                string.Equals(
                    x.FilePath,
                    normalizedPath,
                    StringComparison.Ordinal));
        }

        #endregion

        #region ExistsFileNameInWorkOrder

        public async Task<bool> ExistsFileNameInWorkOrderAsync(
            int workOrderId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            ValidateExcludedId(excludedId);

            string normalizedName = ValidateFileName(fileName);

            var files = await _workOrderFileRepository
                .GetFilesByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return files.Any(x =>
                x.Id != excludedId &&
                string.Equals(
                    x.FileName,
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region HasFiles

        public async Task<bool> HasFilesAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var files = await _workOrderFileRepository
                .GetFilesByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return files.Any();
        }

        #endregion

        #region GetFileCountByWorkOrderId

        public async Task<int> GetFileCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var files = await _workOrderFileRepository
                .GetFilesByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return files.Count();
        }

        #endregion

        #region GetTotalFileSizeByWorkOrderId

        public async Task<long> GetTotalFileSizeByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var files = await _workOrderFileRepository
                .GetFilesByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return files.Sum(x => x.FileSizeBytes);
        }

        #endregion

        #region Private Validation Methods

        private static void ValidateDto<T>(T dto)
            where T : class
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

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

        private static void ValidateExcludedId(int? excludedId)
        {
            if (excludedId.HasValue && excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak kayıt ID geçersiz.");
            }
        }

        private static string ValidateRequiredText(
            string? value,
            int maxLength,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(
                    $"{fieldName} zorunludur.");
            }

            string normalized = value.Trim();

            if (normalized.Length > maxLength)
            {
                throw new ValidationException(
                    $"{fieldName} en fazla " +
                    $"{maxLength} karakter olabilir.");
            }

            if (normalized.Any(char.IsControl))
            {
                throw new ValidationException(
                    $"{fieldName} kontrol karakteri içeremez.");
            }

            return normalized;
        }

        private static string? NormalizeOptionalText(
            string? value,
            int maxLength,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return ValidateRequiredText(
                value,
                maxLength,
                fieldName);
        }

        private static string ValidateFileName(
            string? fileName)
        {
            string normalized = ValidateRequiredText(
                fileName,
                255,
                "Dosya Adı");

            if (normalized == "." ||
                normalized == ".." ||
                normalized.Contains('/') ||
                normalized.Contains('\\') ||
                normalized.IndexOfAny(
                    new[] { ':', '*', '?', '"', '<', '>', '|' }) >= 0)
            {
                throw new ValidationException(
                    "Dosya adı geçersiz karakterler içeriyor.");
            }

            return normalized;
        }

        private static string ValidateStoragePath(
            string? filePath)
        {
            string normalized = ValidateRequiredText(
                filePath,
                1000,
                "Dosya Yolu");

            // FilePath fiziksel mutlak yol değil,
            // güvenilir depolama anahtarı olmalıdır.
            if (normalized.StartsWith('/') ||
                normalized.Contains('\\') ||
                normalized.Contains(':') ||
                normalized.Contains('?') ||
                normalized.Contains('#'))
            {
                throw new ValidationException(
                    "Dosya yolu göreli bir depolama " +
                    "anahtarı olmalıdır.");
            }

            string[] segments = normalized.Split('/');

            if (segments.Any(x =>
                string.IsNullOrWhiteSpace(x) ||
                x == "." ||
                x == ".." ||
                x.Any(char.IsControl)))
            {
                throw new ValidationException(
                    "Dosya yolu geçersiz klasör " +
                    "bileşenleri içeriyor.");
            }

            return normalized;
        }

        private static void ValidateFileSize(long fileSizeBytes)
        {
            if (fileSizeBytes <= 0)
            {
                throw new ValidationException(
                    "Dosya boyutu sıfırdan büyük olmalıdır.");
            }

            if (fileSizeBytes > MaxSingleFileSizeBytes)
            {
                throw new ValidationException(
                    "Tek dosya boyutu 20 MB sınırını aşamaz.");
            }
        }

        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Tarihler UTC olarak gönderilmelidir.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        private async Task<WorkOrder> GetWorkOrderOrThrowAsync(
            int workOrderId,
            CancellationToken cancellationToken)
        {
            var workOrder = await _workOrderRepository
                .GetByIdAsync(
                    workOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                throw new InvalidOperationException(
                    "İş emri bulunamadı.");
            }

            return workOrder;
        }

        private async Task ValidateUserAsync(
            int userId,
            CancellationToken cancellationToken)
        {
            ValidateId(userId, "Kullanıcı");

            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null ||
                !user.IsActive ||
                !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir veya " +
                    "sistem erişimi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Yeni iş emrinde oluşturma yetkilisi,
        /// devam eden iş emrinde ise aktif
        /// atanmış personel dosya ekleyebilir.
        /// </summary>
        private async Task ValidateFileWritePermissionAsync(
            WorkOrder workOrder,
            int currentUserId,
            CancellationToken cancellationToken)
        {
            await ValidateUserAsync(
                currentUserId,
                cancellationToken);

            string permissionCode;

            switch (workOrder.Status)
            {
                case WorkStatusEnum.New:
                    permissionCode = CreatePermission;
                    break;

                case WorkStatusEnum.InProgress:
                    permissionCode = ExecutePermission;

                    bool isAssigned =
                        await _workOrderPersonnelRepository
                            .IsUserAssignedAsync(
                                workOrder.Id,
                                currentUserId,
                                cancellationToken);

                    if (!isAssigned)
                    {
                        throw new UnauthorizedAccessException(
                            "Dosya eklemek için iş emrine " +
                            "aktif personel olarak atanmış " +
                            "olmanız gerekmektedir.");
                    }

                    break;

                default:
                    throw new InvalidOperationException(
                        "Onay bekleyen, tamamlanmış veya " +
                        "iptal edilmiş iş emirlerinin " +
                        "dosyaları değiştirilemez.");
            }

            bool hasPermission =
                await _roleWorkSubTypePermissionService
                    .HasUserActivePermissionAsync(
                        currentUserId,
                        workOrder.ResponsibleUnitId,
                        workOrder.WorkSubTypeId,
                        permissionCode,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu iş emrine " +
                    "dosya ekleme veya güncelleme " +
                    "yetkisi bulunmamaktadır.");
            }
        }

        #endregion
    }
}
