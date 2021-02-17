using System.Net.Http;
using System.Threading.Tasks;

namespace WebAPI
{
    public interface ISimulateHttpRequestTimeout
    {
        Task<HttpResponseMessage> Get();
    }
}
