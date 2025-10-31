using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response {
    public class TagResponse {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
        public IEnumerable<NewsArticleResponse>? NewsArticles { get; set; }
    }
}
