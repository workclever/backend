using System.Text.Json.Serialization;

namespace WorkCleverSolution.Data;

public class CustomField : TimeAwareEntity
{
    public int ProjectId { get; set; }
    public string FieldName { get; set; }
    public string FieldType { get; set; } // text | number | single-select | multi-select
    public bool Enabled { get; set; }
    public bool ShowInTaskCard { get; set; }
    public virtual List<CustomFieldSelectOption> SelectOptions { get; set; }
}

public class CustomFieldSelectOption : TimeAwareEntity
{
    public int CustomFieldId { get; set; }
    [JsonIgnore] public CustomField CustomField { get; set; }

    public string Name { get; set; }

    // Order prop is not used yet 
    public string Order { get; set; }
    public string Color { get; set; }
}

public class TaskCustomFieldValue : TimeAwareEntity
{
    public int TaskId { get; set; }
    public int CustomFieldId { get; set; }
    public CustomField CustomField { get; set; }
    public string Value { get; set; }
}

public static class CustomFieldType
{
    public const string Text = "TEXT";
    public const string Number = "NUMBER";
    public const string SingleSelect = "SINGLESELECT";
    public const string MultiSelect = "MULTISELECT";
    public const string Date = "DATE";
    public const string Bool = "BOOL";
}