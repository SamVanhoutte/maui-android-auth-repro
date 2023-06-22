namespace Sfinx.App.Shared.Services.MultiLinguality;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class InheritStringLocalizationAttribute : Attribute
{
    public InheritStringLocalizationAttribute(Type inheritFrom)
    {
        InheritFrom = inheritFrom ?? throw new ArgumentNullException(nameof(inheritFrom));
    }
    public Type InheritFrom { get; }
    public int Priority { get; set; }
}