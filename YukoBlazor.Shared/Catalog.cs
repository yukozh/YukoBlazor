using System.ComponentModel.DataAnnotations;

namespace YukoBlazor.Shared
{
    public class Catalog
    {
        [MaxLength(20)]
        public string Id { get; set; }

        public string Display { get; set; }

        public int Priority { get; set; }
    }
}
