namespace MvcCoreTest.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR.Infrastructure;

    using MvcCoreTest.Auth;
    using MvcCoreTest.Model;
    using MvcCoreTest.Utils;

    [Route("api/[controller]/[action]/{id?}")]
    [Authorize(Roles = "master")]
    public class ManagerController : Controller
    {
        private readonly IConnectionStorage connectionStorage;
        private readonly UserManager<AppUser> userManager;
        private readonly IConnectionManager connectionManager;

        public ManagerController(
            IConnectionStorage connectionStorage, 
            UserManager<AppUser> userManager, 
            IConnectionManager connectionManager)
        {
            this.connectionStorage = connectionStorage;
            this.userManager = userManager;
            this.connectionManager = connectionManager;
        }

        [HttpPost]
        public IEnumerable<ConnectionModel> GetClients()
        {
            return this.connectionStorage.GetAllConnections();
        }

        [HttpPost]
        public async Task<bool> KickClient(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            var user = await this.userManager.FindByNameAsync(id);
            if (user == null)
            {
                return false;
            }

            await this.userManager.UpdateSecurityStampAsync(user);

            var connectionIds = this.connectionStorage.GetConnectionIds(user.UserName);
            var hub = this.connectionManager.GetHubContext<UsersHub>();
            foreach (var connectionId in connectionIds)
            {
                hub.Clients.Client(connectionId).reset("asdasd");
            }

            return true;
        }
    }
}
