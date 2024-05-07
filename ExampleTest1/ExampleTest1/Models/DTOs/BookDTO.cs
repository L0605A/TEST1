namespace ExampleTest1.Models.DTOs;

//A dto
public class BookDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = null!;

}


public class BookAddDTO
{
    public string Title { get; set; } = string.Empty;
    public List<int> Genres { get; set; } = null!;
}

