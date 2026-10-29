using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkRequestFileDtos;
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
    public class WorkRequestFileService : IWorkRequestFileService
    {
        #region Constants

        // Önerilen varsayılan limitler.
        private const long MaxSingleFileSizeBytes =
            20L * 1024 * 1024;

        private const long MaxTotalFileSizeBytes =
            100L * 1024 * 1024;

        private const int MaxFileCountPerRequest = 20;

        #endregion

        #region Fields

        private readonly IWorkRequestFileRepository
            _workRequestFileRepository;

        private readonly IWorkRequestRepository
            _workRequestRepository;

        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkRequestFileService(
            IWorkRequestFileRepository workRequestFileRepository,
            IWorkRequestRepository workRequestRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _workRequestFileRepository = workRequestFileRepository;
            _workRequestRepository = workRequestRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Tüm dosya kayıtlarını ilişkileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var files = await _workRequestFileRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkRequestFileDto>>(
                files);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Dosya kaydının detaylarını getirir.
        /// </summary>
        public async Task<GetByIdWorkRequestFileDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var file = await _workRequestFileRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            return file == null
                ? null
                : _mapper.Map<GetByIdWorkRequestFileDto>(file);
        }

        #endregion

        #region GetByIdAndWorkRequestId

        /// <summary>
        /// Dosyanın belirtilen iş talebine ait
        /// olup olmadığını kontrol ederek detayını getirir.
        /// </summary>
        public async Task<GetByIdWorkRequestFileDto?>
            GetByIdAndWorkRequestIdAsync(
                int id,
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(id, "Dosya");
            ValidateId(workRequestId, "İş Talebi");

            var file = await _workRequestFileRepository
                .GetByIdAndWorkRequestIdAsync(
                    id,
                    workRequestId,
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

        #region GetByWorkRequestId

        /// <summary>
        /// İş talebinin bütün dosyalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            var files = await _workRequestFileRepository
                .GetFilesByWorkRequestIdAsync(
                    workRequestId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestFileDto>>(
                files);
        }

        #endregion

        #region GetByUploadedByUserId

        /// <summary>
        /// Kullanıcının yüklediği dosyaları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByUploadedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var files = await _workRequestFileRepository
                .GetFilesByUploadedByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestFileDto>>(
                files);
        }

        #endregion

        #region GetByRequestAndUser

        /// <summary>
        /// Belirli bir iş talebinde kullanıcının
        /// yüklediği dosyaları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByRequestAndUserAsync(
                int workRequestId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");
            ValidateId(userId, "Kullanıcı");

            var files = await _workRequestFileRepository
                .GetFilesByRequestAndUserAsync(
                    workRequestId,
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestFileDto>>(
                files);
        }

        #endregion

        #region GetByContentType

        /// <summary>
        /// MIME türüne göre dosyaları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByContentTypeAsync(
                string contentType,
                CancellationToken cancellationToken = default)
        {
            string normalizedContentType =
                ValidateRequiredText(
                    contentType,
                    100,
                    "İçerik türü");

            var files = await _workRequestFileRepository
                .GetFilesByContentTypeAsync(
                    normalizedContentType,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestFileDto>>(
                files);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// UTC tarih aralığına göre dosyaları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
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
                    "Bitiş tarihi başlangıç tarihinden " +
                    "önce olamaz.");
            }

            var files = await _workRequestFileRepository
                .GetFilesByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestFileDto>>(
                files);
        }

        #endregion

        #region GetByFilePath

        /// <summary>
        /// Depolama anahtarına göre dosya getirir.
        /// </summary>
        public async Task<GetByIdWorkRequestFileDto?>
            GetByFilePathAsync(
                string filePath,
                CancellationToken cancellationToken = default)
        {
            string normalizedPath = ValidateStoragePath(filePath);

            var file = await _workRequestFileRepository
                .GetByFilePathAsync(
                    normalizedPath,
                    cancellationToken);

            return file == null
                ? null
                : await GetByIdAsync(file.Id, cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Güvenli depolama işlemi tamamlanan
        /// dosyanın metadata kaydını oluşturur.
        ///
        /// FilePath kullanıcı girdisi olarak değil,
        /// güvenilir depolama katmanının ürettiği
        /// benzersiz göreli anahtar olarak alınmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkRequestFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.WorkRequestId, "İş Talebi");
            ValidateId(currentUserId, "Kullanıcı");

            // =====================================
            // 2. DOSYA METADATA DOĞRULAMASI
            // =====================================

            string fileName = ValidateFileName(dto.FileName);

            string filePath = ValidateStoragePath(dto.FilePath);

            string? contentType = NormalizeOptionalText(
                dto.ContentType,
                100,
                "İçerik türü");

            if (dto.FileSizeBytes <= 0)
            {
                throw new ValidationException(
                    "Dosya boyutu sıfırdan büyük olmalıdır.");
            }

            if (dto.FileSizeBytes > MaxSingleFileSizeBytes)
            {
                throw new ValidationException(
                    "Tek dosya boyutu 20 MB sınırını aşamaz.");
            }

            // =====================================
            // 3. KULLANICI DOĞRULAMASI
            // =====================================

            await ValidateUserAsync(
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. TALEP VE SAHİPLİK KONTROLÜ
            // =====================================

            await ValidateEditableWorkRequestAsync(
                dto.WorkRequestId,
                currentUserId,
                cancellationToken);

            // =====================================
            // 5. DEPOLAMA YOLU BENZERSİZLİĞİ
            // =====================================

            bool existsPath = await _workRequestFileRepository
                .ExistsFilePathAsync(
                    filePath,
                    null,
                    cancellationToken);

            if (existsPath)
            {
                throw new InvalidOperationException(
                    "Bu depolama yoluna ait başka " +
                    "bir dosya kaydı bulunmaktadır.");
            }

            // Aynı FileName'e farklı dosyalar sahip olabilir.
            // Bu nedenle FileName benzersizliği
            // oluşturma işlemine engel yapılmaz.

            // =====================================
            // 6. DOSYA SAYISI SINIRI
            // =====================================

            int fileCount = await _workRequestFileRepository
                .GetFileCountByRequestIdAsync(
                    dto.WorkRequestId,
                    cancellationToken);

            if (fileCount >= MaxFileCountPerRequest)
            {
                throw new ValidationException(
                    "Bir iş talebine en fazla 20 " +
                    "dosya eklenebilir.");
            }

            // =====================================
            // 7. TOPLAM DOSYA BOYUTU
            // =====================================

            long currentTotalSize = await _workRequestFileRepository
                .GetTotalFileSizeByRequestIdAsync(
                    dto.WorkRequestId,
                    cancellationToken);

            if (currentTotalSize < 0 ||
                currentTotalSize > MaxTotalFileSizeBytes ||
                dto.FileSizeBytes >
                    MaxTotalFileSizeBytes - currentTotalSize)
            {
                throw new ValidationException(
                    "İş talebine eklenen dosyaların " +
                    "toplam boyutu 100 MB sınırını aşamaz.");
            }

            // =====================================
            // 8. DTO -> ENTITY
            // =====================================

            var file = _mapper.Map<WorkRequestFile>(dto);

            file.WorkRequestId = dto.WorkRequestId;
            file.FileName = fileName;
            file.FilePath = filePath;
            file.ContentType = contentType;
            file.FileSizeBytes = dto.FileSizeBytes;

            // Bu alan istemciden alınmaz.
            file.UploadedByUserId = currentUserId;

            // =====================================
            // 9. KAYIT
            // =====================================

            var createdFile = await _workRequestFileRepository
                .CreateAsync(
                    file,
                    cancellationToken);

            return createdFile.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Gönderilmemiş iş talebindeki dosyanın
        /// yalnızca görünen adını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkRequestFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Dosya");
            ValidateId(currentUserId, "Kullanıcı");

            string fileName = ValidateFileName(dto.FileName);

            // =====================================
            // 1. DOSYAYI GETİR
            // =====================================

            var existingFile = await _workRequestFileRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingFile == null)
            {
                return false;
            }

            // =====================================
            // 2. KULLANICI DOĞRULAMASI
            // =====================================

            await ValidateUserAsync(
                currentUserId,
                cancellationToken);

            // =====================================
            // 3. YÜKLEYEN KULLANICI KONTROLÜ
            // =====================================

            if (existingFile.UploadedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Dosya adını yalnızca dosyayı " +
                    "yükleyen kullanıcı değiştirebilir.");
            }

            // =====================================
            // 4. İŞ TALEBİ KONTROLÜ
            // =====================================

            await ValidateEditableWorkRequestAsync(
                existingFile.WorkRequestId,
                currentUserId,
                cancellationToken);

            // =====================================
            // 5. DTO -> MEVCUT ENTITY
            // =====================================

            _mapper.Map(dto, existingFile);

            existingFile.FileName = fileName;

            // WorkRequestId, FilePath, ContentType,
            // FileSizeBytes, UploadedByUserId ve
            // tarihler korunur.

            // =====================================
            // 6. GÜNCELLEME
            // =====================================

            return await _workRequestFileRepository
                .UpdateAsync(
                    existingFile,
                    cancellationToken);
        }

        #endregion

        #region ExistsFilePath

        /// <summary>
        /// Dosyanın depolama yolu benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedPath = ValidateStoragePath(filePath);

            ValidateExcludedId(excludedId);

            return await _workRequestFileRepository
                .ExistsFilePathAsync(
                    normalizedPath,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsFileNameInRequest

        /// <summary>
        /// Aynı talepte aynı görünen dosya adı var mı?
        /// Bilgi amaçlı kontrol sağlar.
        /// </summary>
        public async Task<bool> ExistsFileNameInRequestAsync(
            int workRequestId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            string normalizedName = ValidateFileName(fileName);

            ValidateExcludedId(excludedId);

            return await _workRequestFileRepository
                .ExistsFileNameInRequestAsync(
                    workRequestId,
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasFiles

        /// <summary>
        /// İş talebine bağlı dosya bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasFilesAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            return await _workRequestFileRepository
                .HasFilesAsync(
                    workRequestId,
                    cancellationToken);
        }

        #endregion

        #region GetFileCountByRequestId

        /// <summary>
        /// Talebin toplam dosya sayısını getirir.
        /// </summary>
        public async Task<int> GetFileCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            return await _workRequestFileRepository
                .GetFileCountByRequestIdAsync(
                    workRequestId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalFileSizeByRequestId

        /// <summary>
        /// Talebin toplam dosya boyutunu bayt olarak getirir.
        /// </summary>
        public async Task<long> GetTotalFileSizeByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            return await _workRequestFileRepository
                .GetTotalFileSizeByRequestIdAsync(
                    workRequestId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations doğrulamalarını gerçekleştirir.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var context = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                context,
                validateAllProperties: true);
        }

        /// <summary>
        /// Pozitif ID kontrolü.
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
        /// Hariç tutulacak kayıt ID doğrulaması.
        /// </summary>
        private static void ValidateExcludedId(int? excludedId)
        {
            if (excludedId.HasValue && excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak dosya ID geçersiz.");
            }
        }

        /// <summary>
        /// Zorunlu metin doğrulaması.
        /// </summary>
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

        /// <summary>
        /// İsteğe bağlı metin doğrulaması.
        /// </summary>
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

        /// <summary>
        /// Dosyanın yalnızca görünen adını doğrular.
        /// Klasör veya dosya sistemi yolu içeremez.
        /// </summary>
        private static string ValidateFileName(
            string? fileName)
        {
            string normalized = ValidateRequiredText(
                fileName,
                255,
                "Dosya adı");

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

        /// <summary>
        /// Güvenilir depolama altyapısının oluşturduğu
        /// göreli dosya anahtarının biçimini doğrular.
        ///
        /// Fiziksel dosya varlığını veya içeriğini
        /// doğruladığı anlamına gelmez.
        /// </summary>
        private static string ValidateStoragePath(
            string? filePath)
        {
            string normalized = ValidateRequiredText(
                filePath,
                1000,
                "Dosya yolu");

            // Bu tasarımda FilePath, sunucu tarafından
            // oluşturulan göreli ve / ayracını kullanan
            // bir depolama anahtarıdır.
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

            var segments = normalized.Split('/');

            if (segments.Any(segment =>
                string.IsNullOrWhiteSpace(segment) ||
                segment == "." ||
                segment == ".." ||
                segment.Any(char.IsControl)))
            {
                throw new ValidationException(
                    "Dosya yolu geçersiz klasör " +
                    "bileşenleri içeriyor.");
            }

            return normalized;
        }

        /// <summary>
        /// Kullanıcının mevcut, aktif ve
        /// sistem erişimine sahip olduğunu kontrol eder.
        /// </summary>
        private async Task ValidateUserAsync(
            int userId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Kullanıcı bulunamadı.");
            }

            if (!user.IsActive || !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir veya " +
                    "sistem erişimi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Talebin mevcut, ilgili kullanıcıya ait
        /// ve henüz gönderilmemiş olduğunu doğrular.
        /// </summary>
        private async Task ValidateEditableWorkRequestAsync(
            int workRequestId,
            int currentUserId,
            CancellationToken cancellationToken)
        {
            var request = await _workRequestRepository
                .GetByIdAsync(
                    workRequestId,
                    cancellationToken);

            if (request == null)
            {
                throw new InvalidOperationException(
                    "İş talebi bulunamadı.");
            }

            if (request.RequestedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Bu iş talebine dosya ekleme veya " +
                    "dosya adını değiştirme yetkiniz yok.");
            }

            if (request.Status != WorkRequestStatusEnum.Draft ||
                request.SubmittedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Yalnızca henüz gönderilmemiş " +
                    "yeni iş taleplerinin dosyaları " +
                    "düzenlenebilir.");
            }
        }

        #endregion
    }
}
