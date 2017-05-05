using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace ItsyBits.Helpers {
    public class RoleAuthorizationFilter : IDashboardAuthorizationFilter {
        public bool Authorize(DashboardContext context) {
            return context.GetHttpContext().User.IsInRole("Administrator");
        }
    }
}