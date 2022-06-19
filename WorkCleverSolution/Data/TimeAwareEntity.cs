namespace WorkCleverSolution.Data;

public class TimeAwareEntity
{
    public int Id { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DateModified { get; set; }
}