namespace MvcCoreTest.Auth
{
    public class AppUser
    {
        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public bool IsMaster { get; set; }
    }
}
