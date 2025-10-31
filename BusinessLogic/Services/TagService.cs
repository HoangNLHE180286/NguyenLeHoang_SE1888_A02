using BusinessLogic.DTOs.Response;
using Core.Enums;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class TagService {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository) {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<TagResponse>> SearchTagAsync(string? keyword) {

            var tags = await _tagRepository.SearchTagAsync(keyword);

            return tags.Select(t => new TagResponse {
                TagId = t.TagId,
                TagName = t.TagName,
                Note = t.Note
            }).ToList();
        }
    }
}
