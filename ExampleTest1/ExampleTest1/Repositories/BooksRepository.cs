using ExampleTest1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace ExampleTest1.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;
    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //Sample Task Implementation
    
    //Does Book exist
    public async Task<bool> BookExists(int id)
    {
	    //Query
	    var query = "SELECT 1 FROM BOOKS WHERE PK = @ID";

	    //Establish Connection via connection String
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
	    //Create command
	    await using SqlCommand command = new SqlCommand();

	    //Put the command on connection
	    command.Connection = connection;
	    //Put the query into the command
	    command.CommandText = query;
	    //Add with parameter value (To avoid SQL Injection)
	    command.Parameters.AddWithValue("@ID", id);

	    //Open the connection
	    await connection.OpenAsync();

	    //Get first object from the execution
	    var res = await command.ExecuteScalarAsync();

	    return res is not null;
    }
    
    //Does Genre Exist
    public async Task<bool> GenreExists(int id)
    {
	    //Query
	    var query = "SELECT 1 FROM GENRES WHERE PK = @ID";

	    //Establish Connection via connection String
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
	    //Create command
	    await using SqlCommand command = new SqlCommand();

	    //Put the command on connection
	    command.Connection = connection;
	    //Put the query into the command
	    command.CommandText = query;
	    //Add with parameter value (To avoid SQL Injection)
	    command.Parameters.AddWithValue("@ID", id);

	    //Open the connection
	    await connection.OpenAsync();

	    //Get first object from the execution
	    var res = await command.ExecuteScalarAsync();

	    return res is not null;
    }

    //Get Book
    public async Task<BookDTO> BookGet(int id)
       {
	       
	       //Get query
	    var query = @"SELECT 
							Books.PK AS BookID,
							Books.Title AS BookTitle,
							Books_genres.FK_Genre AS BookGenreID,
							Genres.name AS BookGenre
						FROM Books
						JOIN Books_Genres ON Books.PK = Books_Genres.FK_BOOK
						JOIN Genres ON  Books_Genres.FK_GENRE = Genres.PK
						WHERE Books.PK = @PK";
	    
	    //Establish Connection via connection String
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
	    //Create command
	    await using SqlCommand command = new SqlCommand();

	    //Put the command on connection
	    command.Connection = connection;
	    //Put the query into the command
	    command.CommandText = query;
	    //Add with parameter value (To avoid SQL Injection)
	    command.Parameters.AddWithValue("@PK", id);

	    //Open the connection
	    await connection.OpenAsync();

	    //Execute an async reader
	    var reader = await command.ExecuteReaderAsync();

	    //Get value from the read data
	    var bookIdOrdinal = reader.GetOrdinal("BookID");
	    var bookTitleOrdinal = reader.GetOrdinal("BookTitle");
	    var bookGenreIdOrdinal = reader.GetOrdinal("BookGenreID");
	    var bookGenreNameOrdinal = reader.GetOrdinal("BookGenre");

	    //Make the DTO for return
	    BookDTO bookDto = null;

	    //While there is more data to read
	    while (await reader.ReadAsync())
	    {
		    //Add genre to genre list
		    if (bookDto is not null)
		    {
			    bookDto.Genres.Add(reader.GetString(bookGenreNameOrdinal));
		    }
		    else
		    {
			    //Make the DTO from read Data
			    bookDto = new BookDTO()
			    {
				    Id = reader.GetInt32(bookIdOrdinal),
				    Title = reader.GetString(bookTitleOrdinal),
				    Genres = new List<string>()
				    {
					    reader.GetString(bookGenreNameOrdinal)
				    }
			    };
		    }
	    }

	    //If no data found, throw excetion
	    if (bookDto is null) throw new Exception();
        
	    //Otherwise, return the DTO
        return bookDto;
    }

    
    //Add Sample
    public async Task<int> BookAdd(string title)
    {
	    var insert = @"INSERT INTO BOOKS VALUES(@title);
					   SELECT @@IDENTITY AS ID;";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@title", title);

	    await connection.OpenAsync();
	    
	    var id = await command.ExecuteScalarAsync();

	    if (id is null) throw new Exception();
	    
	    return Convert.ToInt32(id);
    }
    

    //Chase that add with another
    public async Task BookGenreAdd(int bookId, int genreId)
    {
	    var query = $"INSERT INTO Books_Genres VALUES(@BookID, @GenreId)";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@BookID", bookId);
	    command.Parameters.AddWithValue("@GenreId", genreId);

	    await connection.OpenAsync();

	    await command.ExecuteNonQueryAsync();
    }
}