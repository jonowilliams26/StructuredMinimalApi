namespace Chirper.Data.Types;

public interface IEntity
{
    int Id { get; }
}

public interface IOwnedEntity : IEntity
{
    int UserId { get; }
}