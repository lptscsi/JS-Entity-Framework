using System.Security.Claims;

namespace RIAPP.DataService.Core.Security
{
    /// <summary>
    /// Провайдер текущего пользователя работающего со службой
    /// </summary>
    public interface IUserProvider
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        ClaimsPrincipal User { get; }
    }
}
