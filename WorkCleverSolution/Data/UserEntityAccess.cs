namespace WorkCleverSolution.Data;

public class UserEntityAccess : TimeAwareEntity
{
    public string EntityClass { get; set; }
    public string Permission { get; set; }

    public int UserId { get; set; }
    public int EntityId { get; set; }
}