namespace YukoBlazor.Server.Models
{
    public class Config
    {
        public Manage Manage { get; set; }
    }

    public class Manage
    {
        public string User { get; set; }
        public string Password { get; set; }
    }
}
