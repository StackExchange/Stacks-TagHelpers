using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum PopoverPosition
    {
        Auto,
        TopCenter,
        TopLeft,
        TopRight,
        BottomCenter,
        BottomLeft,
        BottomRight,
        RightCenter,
        RightTop,
        RightBottom,
        LeftCenter,
        LeftTop,
        LeftBottom
    }

    public class PopoverTagHelper : TagHelper
    {
        public bool Tooltip { get; set; }
        public bool Visible { get; set; }
        public PopoverPosition Arrow { get; set; } = PopoverPosition.Auto;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var startingContent = await output.GetChildContentAsync();

            output.AddClass("s-popover");

            if (Tooltip)
            {
                output.AddClass("s-popover--tooltip");
            }

            if (Visible)
            {
                output.AddClass("is-visible");
            }

            output.Content.AppendHtml($@"<div class=""{GetArrowClass()}""></div>");

            output.Content.AppendHtml(startingContent.GetContent());
        }

        private string GetArrowClass()
        {
            switch (Arrow)
            {
                case PopoverPosition.TopCenter:
                    return "s-popover--arrow__tc";
                case PopoverPosition.TopLeft:
                    return "s-popover--arrow__tl";
                case PopoverPosition.TopRight:
                    return "s-popover--arrow__tr";
                case PopoverPosition.BottomCenter:
                    return "s-popover--arrow__bc";
                case PopoverPosition.BottomLeft:
                    return "s-popover--arrow__bl";
                case PopoverPosition.BottomRight:
                    return "s-popover--arrow__br";
                case PopoverPosition.RightCenter:
                    return "s-popover--arrow__rc";
                case PopoverPosition.RightTop:
                    return "s-popover--arrow__rt";
                case PopoverPosition.RightBottom:
                    return "s-popover--arrow__rb";
                case PopoverPosition.LeftCenter:
                    return "s-popover--arrow__lc";
                case PopoverPosition.LeftTop:
                    return "s-popover--arrow__lt";
                case PopoverPosition.LeftBottom:
                    return "s-popover--arrow__lb";
                case PopoverPosition.Auto:
                default:
                    return "s-popover--arrow";
            }
        }
    }
}
