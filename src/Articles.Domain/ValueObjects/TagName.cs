namespace Articles.Domain.ValueObjects;

public record TagName
{
    public string Value { get; }
    
    private TagName(string value)
    {
        Value = value;
    }
    
    public static TagName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Tag name cannot be empty", nameof(value));
        }
        
        value = value.Trim().ToLowerInvariant();
        
        return new TagName(value);
    }
    
    public static implicit operator string(TagName tagName) => tagName.Value;
} 