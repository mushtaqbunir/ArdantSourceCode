using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public interface ISystemConfiguration
    {
        Task<string> GetRemoteIpAddress();

    }
}
