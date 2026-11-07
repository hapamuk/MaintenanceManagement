using MaintenanceManagement.Application.DTOS.UserDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        #region Fields and Constructor

        private readonly IUserService _userService;
        private readonly IUnitService _unitService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IUnitService unitService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _unitService = unitService;
            _logger = logger;
        }

        #endregion

        #region Index

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var users = await _userService
                .GetAllAsync(cancellationToken);

            return View(users);
        }

        #endregion

        #region Details

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            var user = await _userService
                .GetByIdAsync(id, cancellationToken);

            if (user == null)
                return NotFound();

            return View(user);
        }

        #endregion

        #region Register

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> Register(CancellationToken cancellationToken)
        {
            await LoadPrimaryUnitsAsync(cancellationToken);

            return View(new RegisterUserDto());
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
    RegisterUserDto model,
    CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await LoadPrimaryUnitsAsync(cancellationToken);

                return View(model);
            }

            try
            {
                int userId = await _userService
                    .RegisterByAdminAsync(
                        model,
                        cancellationToken);

                TempData["Success"] =
                    "Kullanıcı başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = userId });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Kullanıcı kaydı oluşturulamadı.");

                ModelState.AddModelError(
                    string.Empty,
                    "Kullanıcı oluşturulurken hata oluştu.");
            }

            // Doğrulama hatası olduğunda
            // dropdown listesi tekrar yüklenir.
            await LoadPrimaryUnitsAsync(cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> Edit(
    int id,
    CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var user = await _userService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            // =====================================
            // 1. GÜNCELLEME MODELİNİ HAZIRLA
            // =====================================

            var model = new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                PrimaryUnitId = user.PrimaryUnitId,
                Phone = user.Phone,
                Title = user.Title,

                // Güvenlik gereği mevcut parola
                // hiçbir zaman forma doldurulmaz.
                NewPassword = null,
                ConfirmNewPassword = null
            };

            // =====================================
            // 2. ORGANİZASYON BİRİMLERİNİ GETİR
            // =====================================

            await LoadPrimaryUnitsAsync(cancellationToken);

            return View(model);
        }

       // [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,UpdateUserDto model,CancellationToken cancellationToken)
        {
            if (id <= 0 || id != model.Id)
            {
                return BadRequest();
            }

            // =====================================
            // 1. MODEL DOĞRULAMASI
            // =====================================

            if (!ModelState.IsValid)
            {
                await LoadPrimaryUnitsAsync(
                    cancellationToken);

                return View(model);
            }

            try
            {
                // =================================
                // 2. KULLANICIYI GÜNCELLE
                // =================================

                bool updated = await _userService
                    .UpdateAsync(
                        model,
                        cancellationToken);

                if (!updated)
                {
                    return NotFound();
                }

                // =================================
                // 3. BAŞARI MESAJI
                // =================================

                TempData["Success"] =
                    "Kullanıcı bilgileri başarıyla güncellendi.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.Id });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Kullanıcı güncellenemedi. Id: {Id}",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Güncelleme sırasında hata oluştu.");
            }

            // =====================================
            // 4. DROPDOWN LİSTESİNİ YENİDEN YÜKLE
            // =====================================

            await LoadPrimaryUnitsAsync(
                cancellationToken);

            // Şifre alanları hatadan sonra forma
            // yeniden basılmasın.
            model.NewPassword = null;
            model.ConfirmNewPassword = null;

            ModelState.Remove(
                nameof(UpdateUserDto.NewPassword));

            ModelState.Remove(
                nameof(UpdateUserDto.ConfirmNewPassword));

            return View(model);
        }

        #endregion

        #region Change Status

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(
            int id,
            bool isActive,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                bool result = await _userService
                    .ChangeStatusAsync(
                        id,
                        isActive,
                        cancellationToken);

                TempData[result ? "Success" : "Error"] =
                    result
                        ? "Kullanıcının durumu güncellendi."
                        : "Kullanıcı bulunamadı.";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Kullanıcı durumu değiştirilemedi.");

                TempData["Error"] =
                    "Durum değiştirilemedi.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Change System Access

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSystemAccess(
            int id,
            bool hasSystemAccess,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                bool result = await _userService
                    .ChangeSystemAccessAsync(
                        id,
                        hasSystemAccess,
                        cancellationToken);

                TempData[result ? "Success" : "Error"] =
                    result
                        ? "Sistem erişim yetkisi güncellendi."
                        : "Kullanıcı bulunamadı.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Sistem erişimi değiştirilemedi.");

                TempData["Error"] =
                    "Sistem erişimi değiştirilemedi.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Login

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction(nameof(Profile));

            ViewBag.ReturnUrl = returnUrl;

            return View(new LoginUserDto());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginUserDto model,
            string? returnUrl,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            try
            {
                var loginResult = await _userService
                    .LoginAsync(model, cancellationToken);

                if (loginResult == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Kullanıcı adı, e-posta veya şifre hatalı.");

                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                string identifier =
                    model.UsernameOrEmail.Trim();

                // Giriş sonucunda ID alanı varsaymak yerine
                // mevcut sorgu metotlarıyla kullanıcıyı bul.
                var user = identifier.Contains('@')
                    ? await _userService.GetByEmailAsync(
                        identifier, cancellationToken)
                    : await _userService.GetByUsernameAsync(
                        identifier, cancellationToken);

                if (user == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Giriş gerçekleştirilemedi.");

                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Name,
                        identifier.ToLowerInvariant())
                };

                // Admin yetkisi istemciden veya kullanıcı
                // adından türetilmez. Yetki altyapısı
                // doğrulanmış role/permission claim'lerini
                // ayrıca sağlamalıdır.

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults
                        .AuthenticationScheme);

                var principal =
                    new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults
                        .AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        AllowRefresh = true
                    });

                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Kullanıcı giriş işlemi başarısız.");

                ModelState.AddModelError(
                    string.Empty,
                    "Giriş gerçekleştirilemedi.");

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region Forgot Password

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordUserDto());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordUserDto model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService
                    .RequestPasswordResetAsync(
                        model,
                        cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Parola sıfırlama talebi işlenemedi.");
            }

            // Hesabın varlığı kullanıcıya açıklanmaz.
            TempData["Info"] =
                "Adres sistemde kayıtlıysa şifre " +
                "yenileme talimatları gönderilecektir.";

            return RedirectToAction(nameof(ForgotPassword));
        }

        #endregion

        #region Reset Password

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string? email,string? token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(
                    "Şifre sıfırlama bağlantısı geçersiz.");
            }

            var model = new ResetPasswordUserDto
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordUserDto model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool result = await _userService
                    .ResetPasswordAsync(
                        model,
                        cancellationToken);

                if (!result)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Şifre yenileme işlemi başarısız. " +
                        "Bağlantı geçersiz veya süresi dolmuş olabilir.");

                    return View(model);
                }

                TempData["Success"] =
                    "Şifreniz yenilendi. Giriş yapabilirsiniz.";

                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Şifre yenileme işlemi başarısız.");

                ModelState.AddModelError(
                    string.Empty,
                    "Şifre yenileme işlemi tamamlanamadı.");

                return View(model);
            }
        }

        #endregion

        #region Profile

        [HttpGet]
        public async Task<IActionResult> Profile(
            CancellationToken cancellationToken)
        {
            int? userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Challenge();

            var profile = await _userService
                .GetProfileAsync(
                    userId.Value,
                    cancellationToken);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        #endregion

        #region Profile Edit

        [HttpGet]
        public async Task<IActionResult> ProfileEdit(
            CancellationToken cancellationToken)
        {
            int? userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Challenge();

            var user = await _userService
                .GetByIdAsync(
                    userId.Value,
                    cancellationToken);

            if (user == null)
                return NotFound();

            var model = new UpdateUserProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(
            UpdateUserProfileDto model,
            CancellationToken cancellationToken)
        {
            int? userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Challenge();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool updated = await _userService
                    .UpdateProfileAsync(
                        userId.Value,
                        model,
                        cancellationToken);

                if (!updated)
                    return NotFound();

                TempData["Success"] =
                    "Profil bilgileriniz güncellendi.";

                return RedirectToAction(nameof(Profile));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }

            return View(model);
        }

        #endregion

        #region Change Password

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordInputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordInputModel model,
            CancellationToken cancellationToken)
        {
            int? userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Challenge();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool updated = await _userService
                    .ChangePasswordAsync(
                        userId.Value,
                        model.CurrentPassword,
                        model.NewPassword,
                        cancellationToken);

                if (!updated)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Şifre değiştirilemedi.");

                    return View(model);
                }

                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults
                        .AuthenticationScheme);

                TempData["Success"] =
                    "Şifreniz değiştirildi. Yeniden giriş yapınız.";

                return RedirectToAction(nameof(Login));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }

            return View(model);
        }

        #endregion

        #region Access Denied

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region Private Helpers

        private int? GetCurrentUserId()
        {
            string? value = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(value, out int id)
                ? id
                : null;
        }

        /// <summary>
        /// Aktif organizasyon birimlerini kayıt
        /// ekranındaki aranabilir dropdown için yükler.
        /// </summary>
        private async Task LoadPrimaryUnitsAsync(CancellationToken cancellationToken)
        {
            var units = await _unitService
                .GetActiveAsync(cancellationToken);

            ViewBag.PrimaryUnits = units
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

        #endregion
    }
}
