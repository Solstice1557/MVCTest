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
            this.connectionStorage.AddConnection(this.Context.ConnectionId, userName, isMaster);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
            this.connectionStorage.RemoveConnection(this.Context.ConnectionId);
        }
    }
}
