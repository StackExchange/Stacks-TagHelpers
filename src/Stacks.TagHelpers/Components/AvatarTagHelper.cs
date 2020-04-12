using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum AvatarSize
    {
        Default,
        Small,
        Medium,
        Large,
        XLarge,
        XXLarge
    }

    public class AvatarTagHelper : TagHelper
    {
        public char Letter { get; set; }

        public string Image { get; set; }

        public AvatarSize Size { get; set; } = AvatarSize.Default;

        public string BadgeIcon { get; set; }

        private readonly SvgTagHelperConfiguration _config;

        public AvatarTagHelper(SvgTagHelperConfiguration config)
        {
            _config = config;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("s-avatar");

            string sizeClass;

            switch (Size)
            {
                case AvatarSize.Small:
                    sizeClass = "s-avatar__sm";
                    break;
                case AvatarSize.Medium:
                    sizeClass = "s-avatar__md";
                    break;
                case AvatarSize.Large:
                    sizeClass = "s-avatar__lg";
                    break;
                case AvatarSize.XLarge:
                    sizeClass = "s-avatar__xl";
                    break;
                case AvatarSize.XXLarge:
                    sizeClass = "s-avatar__xxl";
                    break;
                default:
                    sizeClass = null;
                    break;
            }

            if (sizeClass != null)
            {
                output.AddClass(sizeClass);
            }

            if (!string.IsNullOrWhiteSpace(Image))
            {
                // override the styles entirely to avoid potential conflicts
                output.Attributes.SetAttribute("style", $"background-image: url('{Image}')");
            }

            if (Letter != default)
            {
                output.Content.AppendHtml(@"<div class=""s-avatar--letter"">" + Letter + "</div>");
            }

            if (!string.IsNullOrWhiteSpace(BadgeIcon))
            {
                var content = await SvgTagHelper.RenderAsync(_config, BadgeIcon, "native s-avatar--badge");
                output.Content.AppendHtml(content);
            }
        }
    }
}
