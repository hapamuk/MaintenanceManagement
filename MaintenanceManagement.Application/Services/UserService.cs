using AutoMapper;
using MaintenanceManagement.Application.DTOS.UserDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordResetService _passwordResetService;

        public UserService(
            IUserRepository userRepository,
            IUnitRepository unitRepository,
            IMapper mapper,
            IPasswordResetService passwordResetService)
        {
            _userRepository = userRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
            _passwordResetService = passwordResetService;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün kullanıcıları
        /// ilişkili birim bilgileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _userRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultUserDto>>(users);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif kullanıcıları listeler.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserDto>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _userRepository
                .GetActiveUsersAsync(cancellationToken);

            return _mapper.Map<List<ResultUserDto>>(users);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Kullanıcının detaylarını getirir.
        /// </summary>
        public async Task<GetByIdUserDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return null;

            var user = await _userRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            return user == null
                ? null
                : _mapper.Map<GetByIdUserDto>(user);
        }

        #endregion

        #region GetByEmail

        /// <summary>
        /// E-posta adresine göre kullanıcıyı bulur
        /// ve detay bilgilerini getirir.
        /// </summary>
        public async Task<GetByIdUserDto?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalizedEmail = NormalizeOptional(
                email,
                true);

            var user = await _userRepository
                .GetByEmailAsync(
                    normalizedEmail!,
                    cancellationToken);

            if (user == null)
                return null;

            return await GetByIdAsync(
                user.Id,
                cancellationToken);
        }

        #endregion

        #region GetByUsername

        /// <summary>
        /// Kullanıcı adına göre kullanıcı detayını getirir.
        /// </summary>
        public async Task<GetByIdUserDto?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var normalizedUsername = NormalizeOptional(
                username,
                true);

            var user = await _userRepository
                .GetByUsernameAsync(
                    normalizedUsername!,
                    cancellationToken);

            if (user == null)
                return null;

            return await GetByIdAsync(
                user.Id,
                cancellationToken);
        }

        #endregion

        #region GetByUnit

        /// <summary>
        /// Belirtilen birimle ilişkili kullanıcıları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserDto>> GetByUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Birim");

            var users = await _userRepository
                .GetUsersByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserDto>>(users);
        }

        #endregion

        #region GetByTeam

        /// <summary>
        /// Belirtilen ekibe bağlı kullanıcıları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserDto>> GetByTeamAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Ekip");

            var users = await _userRepository
                .GetUsersByTeamIdAsync(
                    teamId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserDto>>(users);
        }

        #endregion

        #region GetByRole

        /// <summary>
        /// Belirtilen role atanmış kullanıcıları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserDto>> GetByRoleAsync(
            int roleId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            var users = await _userRepository
                .GetUsersByRoleIdAsync(
                    roleId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserDto>>(users);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni kullanıcı oluşturur.
        ///
        /// Sisteme erişim yetkisi bulunan kullanıcılar için
        /// kullanıcı adı veya e-posta ve parola zorunludur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateUserDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateUserFields(
                dto.FirstName,
                dto.LastName,
                dto.PrimaryUnitId);

            string? email = NormalizeOptional(
                dto.Email,
                true);

            string? username = NormalizeOptional(
                dto.Username,
                true);

            // Sisteme giriş hakkı bulunan kullanıcının
            // bir giriş bilgisi ve parolası olmalıdır.
            if (dto.HasSystemAccess)
            {
                ValidateLoginIdentifiers(
                    username,
                    email);

                ValidateNewPassword(dto.Password);

                if (dto.Password != dto.ConfirmPassword)
                {
                    throw new ValidationException(
                        "Parola ve parola tekrarı eşleşmiyor.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ValidationException(
                    "Sistem erişimi olmayan kullanıcıya " +
                    "parola tanımlanamaz.");
            }

            // Seçilen birimin mevcut ve aktif olduğunu doğrula.
            await ValidatePrimaryUnitAsync(
                dto.PrimaryUnitId,
                cancellationToken);

            // E-posta ve kullanıcı adı benzersizliği.
            await ValidateUniquenessAsync(
                email,
                username,
                null,
                cancellationToken);

            // DTO -> Entity
            var user = _mapper.Map<User>(dto);

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();

            user.Email = email;
            user.Username = username;

            user.Phone = NormalizeOptional(dto.Phone);
            user.Title = NormalizeOptional(dto.Title);

            user.PrimaryUnitId = dto.PrimaryUnitId;

            // Sisteme erişim ve aktiflik birbirinden bağımsızdır.
            user.HasSystemAccess = dto.HasSystemAccess;
            user.IsActive = true;

            // Açık parola kesinlikle veritabanına kaydedilmez.
            user.PasswordHash = dto.HasSystemAccess
                ? BCrypt.Net.BCrypt.HashPassword(
                    dto.Password!,
                    workFactor: 12)
                : null;

            // CreatedAt ve SaveChanges işlemleri
            // mevcut GenericRepository tarafından yapılır.
            var createdUser = await _userRepository
                .CreateAsync(
                    user,
                    cancellationToken);

            return createdUser.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut kullanıcı bilgilerinin güncellenmesini sağlar.
        /// Parola, rol, aktiflik ve sistem erişimi korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
    UpdateUserDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Kullanıcı");

            ValidateUserFields(
                dto.FirstName,
                dto.LastName,
                dto.PrimaryUnitId);

            // =====================================
            // 2. KULLANICIYI GETİR
            // =====================================

            var existingUser = await _userRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingUser == null)
            {
                return false;
            }

            // =====================================
            // 3. GİRİŞ BİLGİLERİNİ NORMALİZE ET
            // =====================================

            string? email = NormalizeOptional(
                dto.Email,
                true);

            string? username = NormalizeOptional(
                dto.Username,
                true);

            // =====================================
            // 4. SİSTEM ERİŞİMİ KONTROLÜ
            // =====================================

            if (existingUser.HasSystemAccess)
            {
                ValidateLoginIdentifiers(
                    username,
                    email);
            }

            // =====================================
            // 5. ORGANİZASYON BİRİMİNİ DOĞRULA
            // =====================================

            await ValidatePrimaryUnitAsync(
                dto.PrimaryUnitId,
                cancellationToken);

            // =====================================
            // 6. BENZERSİZLİK KONTROLÜ
            // =====================================

            await ValidateUniquenessAsync(
                email,
                username,
                existingUser.Id,
                cancellationToken);

            // =====================================
            // 7. ŞİFRE DEĞİŞİKLİĞİ KONTROLÜ
            // =====================================

            bool hasNewPassword =
                !string.IsNullOrEmpty(dto.NewPassword);

            bool hasConfirmPassword =
                !string.IsNullOrEmpty(dto.ConfirmNewPassword);

            // Şifre alanlarından yalnızca biri
            // doldurulmuşsa güncelleme yapılmaz.
            if (hasNewPassword != hasConfirmPassword)
            {
                throw new ValidationException(
                    "Yeni şifre ve şifre tekrarı " +
                    "birlikte girilmelidir.");
            }

            string? newPasswordHash = null;

            if (hasNewPassword)
            {
                // Sistem erişimi olmayan kullanıcıya
                // bu ekran üzerinden şifre atanmaz.
                if (!existingUser.HasSystemAccess)
                {
                    throw new ValidationException(
                        "Sistem erişimi olmayan kullanıcının " +
                        "şifresi güncellenemez.");
                }

                // Mevcut parola uzunluk kurallarını kullan.
                ValidateNewPassword(dto.NewPassword);

                if (dto.NewPassword != dto.ConfirmNewPassword)
                {
                    throw new ValidationException(
                        "Yeni şifreler eşleşmiyor.");
                }

                // Yeni şifre mevcut şifreyle aynı olmamalı.
                if (!string.IsNullOrWhiteSpace(
                    existingUser.PasswordHash))
                {
                    bool isSamePassword =
                        BCrypt.Net.BCrypt.Verify(
                            dto.NewPassword!,
                            existingUser.PasswordHash);

                    if (isSamePassword)
                    {
                        throw new ValidationException(
                            "Yeni şifre mevcut şifreyle " +
                            "aynı olamaz.");
                    }
                }

                // Yeni şifreyi güvenli biçimde hash'le.
                newPasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.NewPassword!,
                        workFactor: 12);
            }

            // =====================================
            // 8. KULLANICI BİLGİLERİNİ GÜNCELLE
            // =====================================

            _mapper.Map(dto, existingUser);

            existingUser.FirstName =
                dto.FirstName.Trim();

            existingUser.LastName =
                dto.LastName.Trim();

            existingUser.Email = email;

            existingUser.Username = username;

            existingUser.Phone =
                NormalizeOptional(dto.Phone);

            existingUser.Title =
                NormalizeOptional(dto.Title);

            existingUser.PrimaryUnitId =
                dto.PrimaryUnitId;

            // =====================================
            // 9. ŞİFREYİ GÜNCELLE
            // =====================================

            // Yeni şifre oluşturulmadıysa
            // PasswordHash değerine dokunulmaz.
            if (newPasswordHash != null)
            {
                existingUser.PasswordHash =
                    newPasswordHash;
            }

            // =====================================
            // 10. VERİTABANINA KAYDET
            // =====================================

            // IsActive, HasSystemAccess, CreatedAt,
            // rol ve takım ilişkileri korunur.
            return await _userRepository
                .UpdateAsync(
                    existingUser,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Kullanıcının aktif/pasif durumunu değiştirir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return false;

            var user = await _userRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (user == null)
                return false;

            if (user.IsActive == isActive)
                return true;

            // Kullanıcı yeniden aktifleştiriliyorsa
            // bağlı olduğu birim aktif olmalıdır.
            if (isActive)
            {
                await ValidatePrimaryUnitAsync(
                    user.PrimaryUnitId,
                    cancellationToken);
            }

            return await _userRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ChangeSystemAccess

        /// <summary>
        /// Kullanıcının sisteme giriş yetkisini değiştirir.
        /// </summary>
        public async Task<bool> ChangeSystemAccessAsync(
            int id,
            bool hasSystemAccess,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return false;

            var user = await _userRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (user == null)
                return false;

            if (user.HasSystemAccess == hasSystemAccess)
                return true;

            // Sisteme erişim verilecek kullanıcı
            // gerekli giriş şartlarını sağlamalıdır.
            if (hasSystemAccess)
            {
                if (!user.IsActive)
                {
                    throw new InvalidOperationException(
                        "Pasif kullanıcıya sistem erişimi verilemez.");
                }

                ValidateLoginIdentifiers(
                    user.Username,
                    user.Email);

                if (string.IsNullOrWhiteSpace(
                    user.PasswordHash))
                {
                    throw new InvalidOperationException(
                        "Kullanıcıya sistem erişimi verilmeden " +
                        "önce güvenli parola oluşturma işlemi " +
                        "tamamlanmalıdır.");
                }

                await ValidatePrimaryUnitAsync(
                    user.PrimaryUnitId,
                    cancellationToken);
            }

            return await _userRepository
                .ChangeSystemAccessAsync(
                    id,
                    hasSystemAccess,
                    cancellationToken);
        }

        #endregion

        #region ChangePassword

        /// <summary>
        /// Kullanıcının mevcut parolasını doğrulayarak
        /// yeni parolasını BCrypt ile kaydeder.
        /// </summary>
        public async Task<bool> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                throw new ValidationException(
                    "Mevcut parola zorunludur.");
            }

            ValidateNewPassword(newPassword);

            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null)
                return false;

            if (!user.IsActive || !user.HasSystemAccess)
            {
                throw new InvalidOperationException(
                    "Kullanıcı hesabı parola değiştirme " +
                    "işlemi için uygun durumda değildir.");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new InvalidOperationException(
                    "Kullanıcıya ait geçerli bir parola bulunamadı.");
            }

            bool isCurrentPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    currentPassword,
                    user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new ValidationException(
                    "Mevcut parola hatalıdır.");
            }

            if (BCrypt.Net.BCrypt.Verify(
                newPassword,
                user.PasswordHash))
            {
                throw new ValidationException(
                    "Yeni parola mevcut parolayla aynı olamaz.");
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    newPassword,
                    workFactor: 12);

            return await _userRepository
                .UpdateAsync(
                    user,
                    cancellationToken);
        }

        #endregion

        #region ExistsEmail

        /// <summary>
        /// E-posta adresi benzersizlik kontrolü.
        /// </summary>
        public async Task<bool> ExistsEmailAsync(
            string email,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var normalizedEmail = NormalizeOptional(
                email,
                true);

            return await _userRepository
                .ExistsEmailAsync(
                    normalizedEmail!,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsUsername

        /// <summary>
        /// Kullanıcı adı benzersizlik kontrolü.
        /// </summary>
        public async Task<bool> ExistsUsernameAsync(
            string username,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var normalizedUsername = NormalizeOptional(
                username,
                true);

            return await _userRepository
                .ExistsUsernameAsync(
                    normalizedUsername!,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations kurallarını
        /// servis katmanında da uygular.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var validationContext =
                new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                validationContext,
                validateAllProperties: true);
        }

        /// <summary>
        /// Zorunlu kullanıcı alanlarını kontrol eder.
        /// </summary>
        private static void ValidateUserFields(
            string? firstName,
            string? lastName,
            int primaryUnitId)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ValidationException(
                    "Kullanıcı adı zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ValidationException(
                    "Kullanıcı soyadı zorunludur.");
            }

            ValidateId(primaryUnitId, "Ana birim");
        }

        /// <summary>
        /// Sisteme erişimi bulunan kullanıcıların
        /// en az bir giriş bilgisine sahip olmasını sağlar.
        /// </summary>
        private static void ValidateLoginIdentifiers(
            string? username,
            string? email)
        {
            if (string.IsNullOrWhiteSpace(username) &&
                string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationException(
                    "Sistem erişimi olan kullanıcı için " +
                    "kullanıcı adı veya e-posta zorunludur.");
            }
        }

        /// <summary>
        /// Yeni parolanın temel uzunluk ve boşluk
        /// kontrollerini gerçekleştirir.
        /// </summary>
        private static void ValidateNewPassword(
            string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException(
                    "Parola zorunludur.");
            }

            if (password.Length < 12)
            {
                throw new ValidationException(
                    "Parola en az 12 karakter olmalıdır.");
            }
        }

        /// <summary>
        /// Kimlik değerinin pozitif olduğunu kontrol eder.
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
        /// İsteğe bağlı metinleri temizler.
        /// Boş değerleri null olarak döndürür.
        /// </summary>
        private static string? NormalizeOptional(
            string? value,
            bool toLower = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var normalized = value.Trim();

            return toLower
                ? normalized.ToLowerInvariant()
                : normalized;
        }

        /// <summary>
        /// Kullanıcının bağlı olduğu ana birimin
        /// mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidatePrimaryUnitAsync(
            int primaryUnitId,
            CancellationToken cancellationToken)
        {
            ValidateId(primaryUnitId, "Ana birim");

            var unit = await _unitRepository
                .GetByIdAsync(
                    primaryUnitId,
                    cancellationToken);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    "Seçilen organizasyon birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimi kullanıcıya atanamaz.");
            }
        }

        /// <summary>
        /// Kullanıcı adı ve e-posta benzersizliğini kontrol eder.
        /// </summary>
        private async Task ValidateUniquenessAsync(
            string? email,
            string? username,
            int? excludedId,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                bool emailExists = await _userRepository
                    .ExistsEmailAsync(
                        email,
                        excludedId,
                        cancellationToken);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "Bu e-posta adresi başka bir " +
                        "kullanıcı tarafından kullanılmaktadır.");
                }
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                bool usernameExists = await _userRepository
                    .ExistsUsernameAsync(
                        username,
                        excludedId,
                        cancellationToken);

                if (usernameExists)
                {
                    throw new InvalidOperationException(
                        "Bu kullanıcı adı başka bir " +
                        "kullanıcı tarafından kullanılmaktadır.");
                }
            }
        }

        public async Task<int> RegisterByAdminAsync(
     RegisterUserDto dto,
     CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            // RegisterUserDto -> CreateUserDto
            var createDto = _mapper.Map<CreateUserDto>(dto);

            // Mevcut CreateAsync iş kurallarını kullan.
            int userId = await CreateAsync(
                createDto,
                cancellationToken);

            return userId;
        }

        public async Task<LoginResultUserDto?> LoginAsync(
    LoginUserDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return null;
            }

            // =====================================
            // 2. KULLANICI ADI / E-POSTA NORMALİZASYONU
            // =====================================

            string usernameOrEmail = NormalizeOptional(
                dto.UsernameOrEmail,
                true)!;

            // =====================================
            // 3. KULLANICIYI BUL
            // =====================================

            User? user;

            if (usernameOrEmail.Contains('@'))
            {
                // E-posta ile giriş
                user = await _userRepository
                    .GetByEmailAsync(
                        usernameOrEmail,
                        cancellationToken);
            }
            else
            {
                // Kullanıcı adı ile giriş
                user = await _userRepository
                    .GetByUsernameAsync(
                        usernameOrEmail,
                        cancellationToken);
            }

            // =====================================
            // 4. KULLANICI KONTROLÜ
            // =====================================

            if (user == null)
                return null;

            // =====================================
            // 5. AKTİFLİK VE SİSTEM ERİŞİMİ
            // =====================================

            if (!user.IsActive || !user.HasSystemAccess)
                return null;

            // =====================================
            // 6. PAROLA HASH KONTROLÜ
            // =====================================

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                return null;

            bool isPasswordValid;

            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return null;
            }

            if (!isPasswordValid)
                return null;

            // =====================================
            // 7. BAĞLI OLDUĞU BİRİM KONTROLÜ
            // =====================================

            await ValidatePrimaryUnitAsync(
                user.PrimaryUnitId,
                cancellationToken);

            // =====================================
            // 8. LOGIN SONUCU
            // =====================================

            return _mapper.Map<LoginResultUserDto>(user);
        }
        public async Task<UserProfileDto?> GetProfileAsync(
     int userId,
     CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var user = await _userRepository
                .GetByIdWithDetailsAsync(
                    userId,
                    cancellationToken);

            if (user == null)
                return null;

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<bool> UpdateProfileAsync(
    int userId,
    UpdateUserProfileDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. KULLANICI ID DOĞRULAMASI
            // =====================================

            ValidateId(userId, "Kullanıcı");

            // =====================================
            // 2. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 3. KULLANICIYI GETİR
            // =====================================

            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null)
                return false;

            // =====================================
            // 4. KULLANICI DURUM KONTROLÜ
            // =====================================

            if (!user.IsActive || !user.HasSystemAccess)
            {
                throw new InvalidOperationException(
                    "Kullanıcı hesabı profil güncelleme " +
                    "işlemi için uygun değildir.");
            }

            // =====================================
            // 5. ZORUNLU ALAN KONTROLLERİ
            // =====================================

            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ValidationException(
                    "Ad ve soyad bilgileri zorunludur.");
            }

            // =====================================
            // 6. PROFİL BİLGİLERİNİ GÜNCELLE
            // =====================================

            user.FirstName = dto.FirstName.Trim();

            user.LastName = dto.LastName.Trim();

            user.Phone = NormalizeOptional(dto.Phone);

            // =====================================
            // 7. DEĞİŞMEYECEK ALANLAR
            // =====================================

            // Email
            // Username
            // Title
            // PrimaryUnitId
            // PasswordHash
            // IsActive
            // HasSystemAccess
            // UserUnitRoles
            // TeamMembers
            //
            // Bu alanlara müdahale edilmez.

            // =====================================
            // 8. VERİTABANINA KAYDET
            // =====================================

            return await _userRepository
                .UpdateAsync(
                    user,
                    cancellationToken);
        }

        public async Task RequestPasswordResetAsync(
    ForgotPasswordUserDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            await _passwordResetService.RequestAsync(
                dto,
                cancellationToken);
        }

        public async Task<bool> ResetPasswordAsync(
     ResetPasswordUserDto dto,
     CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            return await _passwordResetService.ResetAsync(
                dto,
                cancellationToken);
        }
        #endregion
    }
}
