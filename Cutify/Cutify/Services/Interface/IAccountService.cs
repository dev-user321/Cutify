using Cutify.Models;
using Cutify.ViewModels;

namespace Cutify.Services.Interface
{
    public interface IAccountService
    {
        Task<(bool Succeeded, string ErrorMessage)> LoginAsync(LoginVM loginVM, HttpContext httpContext);
        Task<(bool Succeeded, string ErrorMessage)> RegisterAsync(RegisterVM registerVM, HttpContext httpContext);
        Task<(bool Succeeded, string RedirectAction)> VerifyEmailAsync(string[] codes, HttpContext httpContext);
        Task<(bool Succeeded, string ErrorMessage)> ConfirmEmailAsync(ConfirmEmailVM confirmEmailVM, HttpContext httpContext);
        Task<(bool Succeeded, string ErrorMessage)> ResetPasswordAsync(ResetPasswordVM resetPasswordVM, HttpContext httpContext);
        Task ResendVerificationCodeAsync(HttpContext httpContext);
        Task LogoutAsync(HttpContext httpContext);
        Task<IEnumerable<Reservation>> GetMyReservationsAsync(string userId, DateTime selectedDate);

    }

}
