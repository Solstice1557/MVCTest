namespace MvcCoreTest.Auth
{
    public interface IAppUserSecurityStampStore
    {
        void SetSecurityStamp(string userName, string stamp);

        string GetSecurityStamp(string userName);
    }
}
