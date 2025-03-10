using DoaMais.Application.Interface;
using Microsoft.Extensions.Configuration;
using Serilog;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace DoaMais.Infrastructure.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly IVaultClient _vaultClient;
        private readonly Dictionary<string, string> _secretsCache = new();
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public KeyVaultService(IConfiguration configuration)
        {
            _logger = Log.ForContext<KeyVaultService>();

            _configuration = configuration;

            var vaultAddres = _configuration["KeyVault:Address"] ?? throw new ArgumentNullException("Address for KeyVault not found.");
            var vaultToken = _configuration["KeyVault:Token"] ?? throw new ArgumentNullException("Token for KeyVault not found.");

            var authMethod = new TokenAuthMethodInfo(vaultToken);
            var vaultClientSettings = new VaultClientSettings(vaultAddres, authMethod);

            _vaultClient = new VaultClient(vaultClientSettings);

            LoadAllSecrets().Wait();
        }

        private async Task LoadAllSecrets()
        {
            try
            {
                _logger.Information("[KeyVaultService] - Carregando segredos do Vault...");

                var mountPoint = _configuration["KeyVault:MountPoint"] ?? throw new ArgumentNullException("MountPoint for KeyVault not found."); 
                var basePath = _configuration["KeyVault:BasePath"] ?? throw new ArgumentNullException("BasePath for KeyVault not found.");

                await LoadSecretsRecursively(basePath, mountPoint);

                _logger.Information("[KeyVaultService] - Todos os segredos foram carregados com sucesso.");
            }
            catch (VaultSharp.Core.VaultApiException ex)
            {
                _logger.Error($"[KeyVaultService] - Erro ao carregar segredos do Vault: {ex.Message}");

                _secretsCache.Clear();
            }
        }

        private async Task LoadSecretsRecursively(string currentPath, string mountPoint)
        {
            try
            {
                var paths = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretPathsAsync(currentPath, mountPoint: mountPoint);

                foreach (var path in paths.Data.Keys)
                {
                    var fullPath = $"{currentPath}/{path}".TrimEnd('/'); 

                    try
                    {
                        var secretData = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(fullPath, mountPoint: mountPoint);

                        foreach (var key in secretData.Data.Data.Keys)
                        {
                            var secretValue = secretData.Data.Data[key]?.ToString();
                            if (!string.IsNullOrEmpty(secretValue))
                            {
                                var secretKey = $"{fullPath}:{key}"; 
                                _secretsCache[key] = secretValue;
                            }
                        }
                    }
                    catch (VaultSharp.Core.VaultApiException)
                    {
                        await LoadSecretsRecursively(fullPath, mountPoint);
                    }
                }
            }
            catch (VaultSharp.Core.VaultApiException ex)
            {
                _logger.Error($"[KeyVaultService] - Erro ao carregar segredos em '{currentPath}': {ex.Message}");
            }
        }

        public string GetSecret(string key)
        {
            return _secretsCache.TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"Chave '{key}' não encontrada no Vault.");
        }
    }
}
