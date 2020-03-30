using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text.RegularExpressions;

namespace Stacks.TagHelpers.Components.Controllers
{
    public enum PopoverPlacement
    {
        Auto,
        Top,
        TopStart,
        TopEnd,
        Bottom,
        BottomStart,
        BottomEnd,
        Left,
        LeftStart,
        LeftEnd,
        Right,
        RightStart,
        RightEnd
    }

    [HtmlTargetElement(Attributes = ATTRIBUTE_NAME)]
    public class SPopoverTagHelper : TagHelper
    {
        public const string ATTRIBUTE_NAME = "s-popover-toggle";

        public string PopoverId { get; set; }
        public string ToggleClass { get; set; }
        public PopoverPlacement Placement { get; set; } = PopoverPlacement.Bottom;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(ATTRIBUTE_NAME);

            output.Attributes.SetAttribute("data-controller", "s-popover");

            if (string.IsNullOrWhiteSpace(PopoverId))
            {
                throw new ArgumentNullException(nameof(PopoverId));
            }

            output.Attributes.SetAttribute("aria-controls", PopoverId);

            if (!string.IsNullOrWhiteSpace(ToggleClass))
            {
                output.Attributes.SetAttribute("data-s-popover-toggle-class", ToggleClass);
            }

            output.Attributes.SetAttribute("data-s-popover-placement", GetPopoverPlacementString());

            output.Attributes.SetAttribute("data-action", "s-popover#toggle");
        }

        private string GetPopoverPlacementString()
        {
            return Regex.Replace(Placement.ToString(), "([^A-Z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
