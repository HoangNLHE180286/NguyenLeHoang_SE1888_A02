using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Params {
    public class SearchCategoryParams {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
    }
    public class CreateUpdateCategoryParams {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string? Description { get; set; }

        public int? ParentId { get; set; }
    }
}
