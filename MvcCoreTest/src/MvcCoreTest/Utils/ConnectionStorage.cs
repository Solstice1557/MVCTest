namespace MvcCoreTest.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using MvcCoreTest.Model;

    public class ConnectionStorage : IConnectionStorage
    {
        private readonly Dictionary<ConnectionModel, List<string>> connections = new Dictionary<ConnectionModel, List<string>>();

        private readonly object lockObject = new object();

        public IEnumerable<ConnectionModel> GetAllConnections()
        {
            lock (this.lockObject)
            {
                return this.connections.Where(p => p.Value.Count > 0).Select(p => p.Key).ToArray();
            }
        }

        public IList<string> GetConnectionIds(string userName)
        {
            lock (this.lockObject)
            {
                var key = new ConnectionModel(userName, false);
                if (this.connections.ContainsKey(key))
                {
                    return this.connections[key];
                }

                return new List<string>();
            }
        }

        public void AddConnection(string connectionId, string userName, bool isMaster)
        {
            lock (this.lockObject)
            {
                var model = new ConnectionModel(userName, isMaster);
                if (this.connections.ContainsKey(model))
                {
                    this.connections[model].Add(connectionId);
                }
                else
                {
                    this.connections.Add(model, new List<string> { connectionId });
                }
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (this.lockObject)
            {
                // todo remove cycle
                var keys = this.connections.Keys.ToArray();
                foreach (var key in keys)
                {
                    if (this.connections[key].Contains(connectionId))
                    {
                        this.connections[key].Remove(connectionId);
                        if (this.connections[key].Count == 0)
                        {
                            this.connections.Remove(key);
                        }

                        break;
                    }
                }
            }
        }
    }
}
