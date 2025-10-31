using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Requests {
    public class CreateCategoryRequest {
        [Required(ErrorMessage = "Name is required!")]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string? CategoryDescription { get; set; }

        public int? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryRequest {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string? CategoryDescription { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
