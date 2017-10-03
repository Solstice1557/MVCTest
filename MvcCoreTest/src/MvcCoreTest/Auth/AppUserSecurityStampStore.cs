namespace MvcCoreTest.Auth
{
    using System.Collections.Generic;
    
    public class AppUserSecurityStampStore : IAppUserSecurityStampStore
    {
        private readonly Dictionary<string, string> secuirityStamps = new Dictionary<string, string>();

        public void SetSecurityStamp(string userName, string stamp)
        {
            if (this.secuirityStamps.ContainsKey(userName))
            {
                this.secuirityStamps[userName] = stamp;
            }
            else
            {
                this.secuirityStamps.Add(userName, stamp);
            }
        }

        public string GetSecurityStamp(string userName)
        {
            var stamp = this.secuirityStamps.ContainsKey(userName)
                            ? this.secuirityStamps[userName]
                            : string.Empty;
            return stamp;
        }
    }
}
