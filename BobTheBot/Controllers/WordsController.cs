using BobTheBot.RequestAndResponse;
using Microsoft.AspNetCore.Mvc;
using RJ.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BobTheBot.Controllers
{
    public class WordsController : Controller
    {

        private readonly SearchKeyService searchKeyService;

        public WordsController(SearchKeyService searchKeyService)
        {
            this.searchKeyService = searchKeyService;
        }

        [HttpPost("[controller]")]
        public async Task<Result> InsertWordAsync([FromBody]WordsRequest request)
        {
            var result = await searchKeyService.Add(request);
            return result;
        }


        [HttpGet("[controller]")]
        public async Task<IEnumerable<WordsResponse>> GetWordsAsync()
        {
            var words = await searchKeyService.GetAsync();
            return words;
        }

        [HttpDelete("[controller]/{entityId:required}")]
        public async Task<Result> DeleteWordAsync(string entityId)
        {
            var result = await searchKeyService.DeleteAsync(entityId);
            return result;
        }
    }
}
