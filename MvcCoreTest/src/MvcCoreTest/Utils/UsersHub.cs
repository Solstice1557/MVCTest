namespace MvcCoreTest.Utils
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    public class UsersHub : Hub
    {
        private readonly IConnectionStorage connectionStorage;

        public UsersHub(IConnectionStorage connectionStorage)
        {
            this.connectionStorage = connectionStorage;
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();
            var userName = this.Context.User?.Identity?.Name;
            var isMaster = this.Context.User?.IsInRole("master") ?? false;
            var result = this.connectionStorage.AddConnection(this.Context.ConnectionId, userName, isMaster);

            if (result)
            {
                this.UpdateOnMaster();
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
            var result = this.connectionStorage.RemoveConnection(this.Context.ConnectionId);
            if (result)
            {
                this.UpdateOnMaster();
            }
        }

        private void UpdateOnMaster()
        {
            var masterConnectionIds =
                this.connectionStorage.GetAllConnections()
                    .Where(c => c.IsMaster)
                    .SelectMany(c => this.connectionStorage.GetConnectionIds(c.UserName))
                    .ToList();

            foreach (var connectionId in masterConnectionIds)
            {
                this.Clients.Client(connectionId).update("update");
            }
        }
    }
}
