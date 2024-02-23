namespace StructuredMinimalApi.Types;

public class Author
{
    public int Id { get; private set; }
    public required string Name { get; set; }
    public List<Post> Posts { get; set; } = [];
}