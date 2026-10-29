using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Data
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        #region CreateDbContext

        public ApplicationDbContext CreateDbContext(
            string[] args)
        {
            // =====================================
            // 1. APPSETTINGS DOSYASINI BUL
            // =====================================

            string appSettingsPath =
                FindAppSettingsPath();

            // =====================================
            // 2. SQL CONNECTION STRING OKU
            // =====================================

            string connectionString =
                ReadConnectionString(appSettingsPath);

            // =====================================
            // 3. DBCONTEXT OPTIONS OLUŞTUR
            // =====================================

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            // =====================================
            // 4. SQL SERVER AYARLARI
            // =====================================

            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(
                        typeof(ApplicationDbContext)
                            .Assembly
                            .GetName()
                            .Name);
                });

            // =====================================
            // 5. DBCONTEXT OLUŞTUR
            // =====================================

            return new ApplicationDbContext(
                optionsBuilder.Options);
        }

        #endregion

        #region FindAppSettingsPath

        /// <summary>
        /// Çalışma dizininden veya uygulama
        /// dizininden başlayarak WebUI projesinin
        /// appsettings.json dosyasını bulur.
        ///
        /// Infrastructure ve WebUI projelerinin
        /// aynı Solution altında olduğu
        /// mevcut yapı için hazırlanmıştır.
        /// </summary>
        private static string FindAppSettingsPath()
        {
            // =====================================
            // 1. BAŞLANGIÇ DİZİNLERİ
            // =====================================

            string[] startDirectories =
            {
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory
            };

            // =====================================
            // 2. HER BAŞLANGIÇ DİZİNİNİ KONTROL ET
            // =====================================

            foreach (string startDirectory in
                startDirectories.Distinct(
                    StringComparer.OrdinalIgnoreCase))
            {
                var directory =
                    new DirectoryInfo(startDirectory);

                // =================================
                // 3. ÜST KLASÖRLERİ TARA
                // =================================

                while (directory != null)
                {
                    // =================================
                    // 3.1 DOĞRUDAN WEBUI İÇİNDEYSE
                    // =================================

                    if (string.Equals(
                        directory.Name,
                        "MaintenanceManagement.WebUI",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string directPath = Path.Combine(
                            directory.FullName,
                            "appsettings.json");

                        if (File.Exists(directPath))
                        {
                            return directPath;
                        }
                    }

                    // =================================
                    // 3.2 WEBUI ALT KLASÖRÜNÜ ARA
                    // =================================

                    string webUiPath = Path.Combine(
                        directory.FullName,
                        "MaintenanceManagement.WebUI",
                        "appsettings.json");

                    if (File.Exists(webUiPath))
                    {
                        return webUiPath;
                    }

                    // =================================
                    // 3.3 ÜST DİZİNE GEÇ
                    // =================================

                    directory = directory.Parent;
                }
            }

            // =====================================
            // 4. DOSYA BULUNAMADI
            // =====================================

            throw new FileNotFoundException(
                "MaintenanceManagement.WebUI " +
                "projesine ait appsettings.json " +
                "dosyası bulunamadı. " +
                "WebUI ve Infrastructure proje " +
                "klasörlerinin konumlarını kontrol edin.");
        }

        #endregion

        #region ReadConnectionString

        /// <summary>
        /// appsettings.json dosyasından
        /// ConnectionStrings:SqlConnection
        /// değerini okur.
        ///
        /// Environment variable varsa
        /// öncelikle onu kullanır.
        /// </summary>
        private static string ReadConnectionString(
            string appSettingsPath)
        {
            // =====================================
            // 1. ENVIRONMENT VARIABLE KONTROLÜ
            // =====================================

            string? environmentConnection =
                Environment.GetEnvironmentVariable(
                    "ConnectionStrings__SqlConnection");

            if (!string.IsNullOrWhiteSpace(
                environmentConnection))
            {
                return environmentConnection;
            }

            // =====================================
            // 2. JSON DOSYASINI OKU
            // =====================================

            string json = File.ReadAllText(
                appSettingsPath);

            using JsonDocument document =
                JsonDocument.Parse(json);

            // =====================================
            // 3. CONNECTIONSTRINGS BÖLÜMÜ
            // =====================================

            if (!document.RootElement.TryGetProperty(
                "ConnectionStrings",
                out JsonElement connectionStrings))
            {
                throw new InvalidOperationException(
                    "appsettings.json içerisinde " +
                    "ConnectionStrings bölümü bulunamadı.");
            }

            // =====================================
            // 4. SQLCONNECTION DEĞERİ
            // =====================================

            if (!connectionStrings.TryGetProperty(
                "SqlConnection",
                out JsonElement sqlConnection))
            {
                throw new InvalidOperationException(
                    "appsettings.json içerisinde " +
                    "SqlConnection bağlantısı bulunamadı.");
            }

            string? connectionString =
                sqlConnection.GetString();

            // =====================================
            // 5. BOŞ BAĞLANTI KONTROLÜ
            // =====================================

            if (string.IsNullOrWhiteSpace(
                connectionString))
            {
                throw new InvalidOperationException(
                    "SqlConnection bağlantı bilgisi " +
                    "boş olamaz.");
            }

            // =====================================
            // 6. BAĞLANTI BİLGİSİNİ DÖNDÜR
            // =====================================

            return connectionString;
        }

        #endregion
    }
}
