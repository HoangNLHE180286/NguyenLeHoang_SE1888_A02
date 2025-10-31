using BusinessLogic.DTOs.Requests;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Validation {
    public class CategoryValidator {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryValidator(ICategoryRepository categoryRepository) {
            _categoryRepository = categoryRepository;
        }

        public void ValidateForCreate(CreateCategoryRequest createCategoryRequest) {
            this.ValidateGeneric(createCategoryRequest);
        }

        public void ValidateForUpdate(UpdateCategoryRequest updateCategoryRequest) {
            this.ValidateGeneric(updateCategoryRequest);
        }

        // Validate generic
        private void ValidateGeneric<T>(T entity) {
            var context = new ValidationContext(entity, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(entity, context, results, true);

            foreach (var result in results) {
                foreach (var memberName in result.MemberNames) {
                    throw new ArgumentException($"{result.ErrorMessage}");
                }
            }
        }
    }
}
