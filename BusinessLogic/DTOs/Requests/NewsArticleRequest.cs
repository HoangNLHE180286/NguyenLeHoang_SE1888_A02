using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Requests {
    public class SearchNewsArticleRequest {
        public string? Keyword { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public Status? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class CreateNewsArticleRequest {
        [Required(ErrorMessage = "News title is required!")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required!")]
        public string? Headline { get; set; }

        [Required(ErrorMessage = "News content is required!")]
        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "There is no author to create news!")]
        public int CreatedById { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<int>? TagIds { get; set; }
    }

    public class UpdateNewsArticleRequest {
        public string NewsArticleId { get; set; }

        [Required(ErrorMessage = "News title is required!")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required!")]
        public string? Headline { get; set; }

        [Required(ErrorMessage = "News content is required!")]
        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public Status? Status { get; set; }

        public int CategoryId { get; set; }

        [Required(ErrorMessage = "There is no modifier to update news!")]
        public int UpdatedById { get; set; }

        public IEnumerable<int>? TagIds { get; set; }
    }
}
