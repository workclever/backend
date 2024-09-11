using System.Text.Json.Serialization;

namespace WorkCleverSolution.Data;

public class BoardView : TimeAwareEntity
{
    public int BoardId { get; set; }
    public Board Board { get; set; }

    public BoardViewConfig Config { get; set; }
}

public readonly struct BoardViewConfig
{
    public string Type { get;}
    public List<int> VisibleCustomFields { get; }
    
    
    [JsonConstructor]
    public BoardViewConfig(string type, List<int> visibleCustomFields)
    {
        Type = type;
        VisibleCustomFields = visibleCustomFields;
    }
}