using System.Text.Json.Serialization;
using WorkCleverSolution.Data.Identity;

namespace WorkCleverSolution.Data;

public class BoardView : TimeAwareEntity
{
    public int BoardId { get; set; }
    public Board Board { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public BoardViewConfig Config { get; set; }
}

public readonly struct BoardViewConfig
{
    public string Type { get;}
    
    public string Name { get;}
    public List<int> VisibleCustomFields { get; }
    
    
    [JsonConstructor]
    public BoardViewConfig(string type, string name, List<int> visibleCustomFields)
    {
        Type = type;
        Name = name;
        VisibleCustomFields = visibleCustomFields;
    }
}