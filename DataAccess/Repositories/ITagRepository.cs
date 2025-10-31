using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public interface ITagRepository {
        Task<IEnumerable<Tag>> GetListTagByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Tag>> SearchTagAsync(string? keyword);
    }
}
