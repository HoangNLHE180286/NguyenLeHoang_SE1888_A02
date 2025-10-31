using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Response {
    public class CategoryResponse {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        public Status? Status { get; set; }
        public CategoryResponse? ParentCategory { get; set; }

        public int ArticleCount { get; set; } = 0;
    }
}
