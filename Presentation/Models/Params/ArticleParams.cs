using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Params {
    public class SearchArticleParams {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateUpdateArticleParams {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Headline is required!")]
        public string? Headline { get; set; }

        [Required(ErrorMessage = "Content is required!")]
        public string? Content { get; set; }

        public int? CategoryId { get; set; }
        public string? Source { get; set; }

        public List<int>? TagIds { get; set; }
    }
}
