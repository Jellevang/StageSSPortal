using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Domain.Gebruikers;

namespace BL
{
    public class SignInManager : SignInManager<Gebruiker, string>
    {
        public SignInManager(GebruikerManager gebruikerManager, IAuthenticationManager authenticationManager)
            : base(gebruikerManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Gebruiker user)
        {
            return user.GenerateUserIdentityAsync((GebruikerManager)UserManager);
        }
        public static SignInManager Create(GebruikerManager manager, IOwinContext context)
        {
            return new SignInManager(manager, context.Authentication);
        }
    }
}
