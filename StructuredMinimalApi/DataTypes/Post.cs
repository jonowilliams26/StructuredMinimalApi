namespace StructuredMinimalApi.DataTypes;

public class Post
{
    public int Id { get; private init; }
    public required string Title { get; set; }
    public required string Text { get; set; }
}
