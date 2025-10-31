using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class TagRepository : ITagRepository {
        private readonly ApplicationDbContext _dbContext;

        public TagRepository(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Tag>> GetListTagByIdsAsync(IEnumerable<int> ids) {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<Tag>();

            return await _dbContext.Tags
                .Where(t => ids.Contains(t.TagId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Tag>> SearchTagAsync(string? keyword) {
            keyword = keyword?.ToLower();
            var query = _dbContext.Tags.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword)) {
                query = query.Where(a => a.TagName.ToLower().Contains(keyword));
            }

            return await query.ToListAsync();
        }
    }
}
