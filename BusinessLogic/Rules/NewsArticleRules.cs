using BusinessLogic.DTOs.Requests;
using Core.Enums;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Rules {
    public class NewsArticleRules {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticleRules(INewsArticleRepository newsArticleRepository) {
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task CheckForCreate(CreateNewsArticleRequest createNewsArticleRequest) {
            await this.CheckNewsArticleAsync(createNewsArticleRequest.NewsTitle, CheckMode.MustNotExist);
        }

        public async Task CheckForUpdate(UpdateNewsArticleRequest updateNewsArticleRequest) {
            var current = await _newsArticleRepository.GetNewsArticleByIdAsync(updateNewsArticleRequest.NewsArticleId);
            var existing = await _newsArticleRepository.GetNewsArticleByTitleAsync(updateNewsArticleRequest.NewsTitle);

            if(updateNewsArticleRequest.NewsTitle != current.NewsTitle && existing != null) {
                throw new InvalidOperationException("Article has already existed!");
            }
        }

        private async Task CheckNewsArticleAsync(string newsTitle, CheckMode mode) {
            var article = await _newsArticleRepository.GetNewsArticleByTitleAsync(newsTitle);   
            switch (mode) {
                case CheckMode.MustExist:
                if (article == null)
                    throw new InvalidOperationException("Article doesn't exist!");
                break;

                case CheckMode.MustNotExist:
                if (article != null)
                    throw new InvalidOperationException("Article has already existed!");
                break;
            }
        }
    }
}
