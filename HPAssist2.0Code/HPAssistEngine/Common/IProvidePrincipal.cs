using System.Security.Principal;

namespace HPAssistEngine.Common
{
    public interface IProvidePrincipal
    {
        IPrincipal CreatePrincipal(string apikey, string companyCode, string username);
    }
}