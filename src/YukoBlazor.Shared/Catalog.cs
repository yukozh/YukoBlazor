using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace YukoBlazor.Shared
{
    public class Catalog
    {
        [MaxLength(20)]
        public string Id { get; set; }

        public string Display { get; set; }

        public int Priority { get; set; }

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
