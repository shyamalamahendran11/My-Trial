using System.Security.Principal;

namespace HPAssistEngine.Common
{
    public class PrincipalProvider : IProvidePrincipal
    {
        public IPrincipal CreatePrincipal(string apiKey, string companyCode, string username)
        {
            string roleName = HPDAL.CommonDAL.AuthenticateUser(apiKey, companyCode, username);
            if (string.IsNullOrEmpty(roleName))
                return null;
            var identity = new GenericIdentity(username);
            IPrincipal principal = new GenericPrincipal(identity, new[] { roleName });
            return principal;
        }
    }
}