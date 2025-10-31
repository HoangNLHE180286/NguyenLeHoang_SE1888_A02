using Core.Enums;

namespace Presentation.Models.Data {
    public class ArticleData {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Headline { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? Modifier { get; set; }
        public List<TagData>? Tags { get; set; } = new List<TagData>();
    }
}
