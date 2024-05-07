using ExampleTest1.Models.DTOs;

namespace ExampleTest1.Repositories;

public interface IBooksRepository
{
    //Task<Return_Type> TaskName(Argument_Type argument_Name);

    
    Task<bool> BookExists(int id);
    
    Task<bool> GenreExists(int id);
    
    Task<BookDTO> BookGet(int id);
    
    //2 phase add
    Task<int> BookAdd(string title);
    Task BookGenreAdd(int bookId, int genreId);

}