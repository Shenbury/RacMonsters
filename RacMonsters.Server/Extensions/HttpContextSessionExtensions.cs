using Microsoft.AspNetCore.Http;
using RacMonsters.Server.Models;
using RacMonsters.Server.Services.Sessions;
using System.Threading.Tasks;

namespace RacMonsters.Server.Extensions
{
    public static class HttpContextSessionExtensions
    {
        private const string SessionKey = "Session";

        public static Session? GetSession(this HttpContext httpContext)
        {
            if (httpContext == null) return null;
            if (httpContext.Items.TryGetValue(SessionKey, out var obj) && obj is Session s)
            {
                return s;
            }
            return null;
        }

        public static void SetSession(this HttpContext httpContext, Session session)
        {
            if (httpContext == null) return;
            httpContext.Items[SessionKey] = session;
        }

        public static async Task PersistSessionAsync(this HttpContext httpContext, ISessionService sessionService)
        {
            if (httpContext == null || sessionService == null) return;
            var s = httpContext.GetSession();
            if (s != null)
            {
                await sessionService.UpdateSession(s);
            }
        }
    }
}
