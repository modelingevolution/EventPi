using System.ComponentModel.DataAnnotations;

namespace EventPi.Abstractions;

[AttributeUsage(AttributeTargets.Property |
                AttributeTargets.Field, AllowMultiple = false)]
public sealed class GuidNotEmptyAttribute : ValidationAttribute
{
    public bool AllowNull { get; set; }
    public override bool IsValid(object? value)
    {
        if (value is Guid g) return g != Guid.Empty;
        return value == null && AllowNull;
    }
}