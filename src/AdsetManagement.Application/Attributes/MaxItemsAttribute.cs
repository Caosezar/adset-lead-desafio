using System.ComponentModel.DataAnnotations;

namespace AdsetManagement.Application.Attributes;

public class MaxItemsAttribute : ValidationAttribute
{
    private readonly int _maxItems;

    public MaxItemsAttribute(int maxItems)
    {
        _maxItems = maxItems;
    }

    public override bool IsValid(object? value)
    {
        if (value is System.Collections.ICollection collection)
        {
            return collection.Count <= _maxItems;
        }
        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"O campo {name} pode ter no máximo {_maxItems} itens.";
    }
}