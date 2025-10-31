using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response {
    public class NewsArticleResponse {
        public string NewsArticleId { get; set; }

        public string? NewsTitle { get; set; }

        public string Headline { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public Status? NewsStatus { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public CategoryResponse? Category { get; set; }

        public SystemAccountResponse? CreatedBy { get; set; }

        public SystemAccountResponse? UpdatedBy { get; set; }

        public IEnumerable<TagResponse>? Tags { get; set; }
    }

}
