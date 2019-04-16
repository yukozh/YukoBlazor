using System;
using System.Collections.Generic;

namespace YukoBlazor.Shared
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public Catalog Catalog { get; set; }
        public string Url { get; set; }
    }
}
