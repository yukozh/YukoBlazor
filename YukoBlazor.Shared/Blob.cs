using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace YukoBlazor.Shared
{
    public class Blob
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string FileName { get; set; }

        [MaxLength(128)]
        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public byte[] Bytes { get; set; }
    }
}
