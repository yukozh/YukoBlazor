namespace YukoBlazor.Shared
{
    public class Config
    {
        public Manage Manage { get; set; }
        public Profile Profile { get; set; }
    }

    public class Manage
    {
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class Profile
    {
        public string BlogTitle { get; set; }
        public string Subtitle { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string GitHub { get; set; }
    }
}
