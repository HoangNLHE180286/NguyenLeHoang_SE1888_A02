using Core.Enums;

namespace Presentation.Models.Data {
    public class CategoryData {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Status? Status { get; set; }
        public int? ArticleQuantity { get; set; }
        public CategoryData? Parent { get; set; }
    }
}
