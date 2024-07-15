namespace Chirper.Data.Types;

public interface IEntity
{
    int Id { get; }
    Guid ReferenceId { get; }
}

public interface IOwnedEntity
{
    int UserId { get; }
}