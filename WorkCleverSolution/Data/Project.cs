namespace WorkCleverSolution.Data;

public class Project : TimeAwareEntity
{
    public string Name { get; set; }
    public string Slug { get; set; }

    public int OwnerUserId { get; set; }
}