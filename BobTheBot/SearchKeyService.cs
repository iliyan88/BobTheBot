using BobTheBot.Kernel;
using BobTheBot.RequestAndResponse;
using RJ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot
{
    public class SearchKeyService
    {
        private readonly IUnitOfWork unitOfWork;

        public SearchKeyService(
           IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Add(WordsRequest request)
        {
            var existingWordsEntity = await unitOfWork.SearchKeyRepository.GetAllWords();
            var existingWords = existingWordsEntity.Select(x => x.Word);

            foreach (var item in request.Words)
            {
                if (!existingWords.Contains(item))
                {
                    var word = new SearchKey(item);
                    unitOfWork.SearchKeyRepository.Insert(word);
                }
            }
            await unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<IEnumerable<WordsResponse>> GetAsync()
        {
            var existingWords = await unitOfWork.SearchKeyRepository.GetAllWords();
            return existingWords.Select(x => WordsResponse.Create(x));
        }

        public async Task<Result> DeleteAsync(string wordId)
        {
            var word = await unitOfWork.SearchKeyRepository.GetByIDAsync(wordId);
            if (word == null)
            {
                return Result.Fail(ErrorType.NotFound, nameof(SearchKey), wordId);
            }
            unitOfWork.SearchKeyRepository.Delete(word);
            await unitOfWork.SaveChangesAsync();
            //await categoryCache.InvalidateAsync();
            return Result.Ok();

        }
    }
}
