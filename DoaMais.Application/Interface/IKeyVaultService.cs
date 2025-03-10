
namespace DoaMais.Application.Interface
{
    public interface IKeyVaultService
    {
        string GetSecret(string key);
    }
}
