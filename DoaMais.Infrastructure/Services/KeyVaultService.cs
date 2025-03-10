using DoaMais.Application.Interface;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace DoaMais.Infrastructure.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly IVaultClient _vaultClient;
        private readonly Dictionary<string, string> _secretsCache = new();

        public KeyVaultService(IConfiguration configuration)
        {
            var vaultAddres = configuration["KeyVault:Address"];
            var vaultToken = configuration["KeyVault:Token"];

            var authMethod = new TokenAuthMethodInfo(vaultToken);
            var vaultClientSettings = new VaultClientSettings(vaultAddres, authMethod);

            _vaultClient = new VaultClient(vaultClientSettings);

            LoadAllSecrets().Wait();
        }

        private async Task LoadAllSecrets()
        {
            try
            {
                //Console.WriteLine("🔄 Carregando segredos do Vault...");

                var mountPoint = "secret"; 
                var basePath = "doamais"; 

                await LoadSecretsRecursively(basePath, mountPoint);

                //Console.WriteLine("✅ Todos os segredos foram carregados com sucesso.");
            }
            catch (VaultSharp.Core.VaultApiException ex)
            {
                //Console.WriteLine($"❌ Erro ao carregar segredos do Vault: {ex.Message}");
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
                //Console.WriteLine($"⚠ Erro ao carregar segredos em '{currentPath}': {ex.Message}");
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
