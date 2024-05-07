using System.Transactions;
using ExampleTest1.Models.DTOs;
using ExampleTest1.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExampleTest1.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _booksBooksRepository;
        public BooksController(IBooksRepository booksBooksRepository)
        {
            _booksBooksRepository = booksBooksRepository;
        }
        
        [HttpGet("{id}/genres")]
        //[Route("genres")]
        public async Task<IActionResult> GetBook(int id)
        {
            //Check for the validity
            if (!await _booksBooksRepository.BookExists(id))
                return NotFound($"Book with given ID - {id} doesn't exist");

            //Get the data
            var book = await _booksBooksRepository.BookGet(id);
            
            return Ok(book);
        }
        
        
        // Version with transaction scope
        [HttpPost]
        public async Task<IActionResult> AddBook(BookAddDTO bookWithGenres)
        {
            
            //You can check existance in loops
            foreach (var genre in bookWithGenres.Genres)
            {
                if (!await _booksBooksRepository.GenreExists(genre))
                    return NotFound($"Genre with given ID - {genre} doesn't exist");
            }

            BookDTO book;

            //Use transaction for rollback
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = await _booksBooksRepository.BookAdd(bookWithGenres.Title);

                foreach (var genre in bookWithGenres.Genres)
                {
                    await _booksBooksRepository.BookGenreAdd(id, genre);
                }
                
                book = await _booksBooksRepository.BookGet(id);
                
                scope.Complete();
            }

            return Created(Request.Path.Value ?? "api/books", book);
        }
    }
}
