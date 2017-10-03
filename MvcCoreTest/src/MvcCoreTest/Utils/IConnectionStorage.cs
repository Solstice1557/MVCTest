namespace MvcCoreTest.Utils
{
    using System.Collections.Generic;

    using MvcCoreTest.Model;

    public interface IConnectionStorage
    {
        IEnumerable<ConnectionModel> GetAllConnections();

        IList<string> GetConnectionIds(string userName);

        void AddConnection(string connectionId, string userName, bool master);

        void RemoveConnection(string connectionId);
    }
}