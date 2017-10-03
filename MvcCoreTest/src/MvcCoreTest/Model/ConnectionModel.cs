namespace MvcCoreTest.Model
{
    using System;

    public class ConnectionModel : IEquatable<ConnectionModel>
    {
        public ConnectionModel(string userName, bool isMaster)
        {
            this.UserName = userName;
            this.IsMaster = isMaster;
        }

        public string UserName { get; }

        public bool IsMaster { get; }

        public bool Equals(ConnectionModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.UserName, other.UserName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ConnectionModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.UserName?.ToLower().GetHashCode() ?? 0;
            }
        }

        public static bool operator ==(ConnectionModel left, ConnectionModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConnectionModel left, ConnectionModel right)
        {
            return !Equals(left, right);
        }
    }
}
