namespace StructuredMinimalApi.Types;

public class Post
{
    public int Id { get; private init; }
    public required string Title { get; set; }
    public required string Text { get; set; }
    public required Author Author { get; init; }
}