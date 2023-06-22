using Microsoft.AspNetCore.Components;
namespace Sfinx.App.Shared.Components.General;

public class SquareSelectionOption
{
    public SquareSelectionOption(string label, string image, object item)
    {
        Label = label;
        ImageUrl = image;
        Item = item;
    }
    public string ImageUrl { get; set; }
    public string Label { get; set; }
    public object Item { get; set; }
}