using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum BadgeSize
    {
        Default,
        Mini
    }

    public enum BadgeType
    {
        Default,
        Bronze,
        Silver,
        Gold,
        Bounty,
        Votes,
        Answered,
        Rep,
        RepDown,
        Important
    }

    public class BadgeTagHelper : TagHelper
    {
        public string Image { get; set; }
        public BadgeType Type { get; set; } = BadgeType.Default;
        public BadgeSize Size { get; set; } = BadgeSize.Default;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var startingContent = await output.GetChildContentAsync();

            output.AddClass("s-badge");

            string badgeTypeClass = null;

            switch (Type)
            {
                case BadgeType.Bronze:
                    badgeTypeClass = "s-badge__bronze";
                    break;
                case BadgeType.Silver:
                    badgeTypeClass = "s-badge__silver";
                    break;
                case BadgeType.Gold:
                    badgeTypeClass = "s-badge__gold";
                    break;
                case BadgeType.Bounty:
                    badgeTypeClass = "s-badge__bounty";
                    break;
                case BadgeType.Votes:
                    badgeTypeClass = "s-badge__votes";
                    break;
                case BadgeType.Answered:
                    badgeTypeClass = "s-badge__answered";
                    break;
                case BadgeType.Rep:
                    badgeTypeClass = "s-badge__rep";
                    break;
                case BadgeType.RepDown:
                    badgeTypeClass = "s-badge__rep-down";
                    break;
                case BadgeType.Important:
                    badgeTypeClass = "s-badge__important";
                    break;
            }

            if (badgeTypeClass != null)
            {
                output.AddClass(badgeTypeClass);
            }

            string badgeSizeClass = null;

            switch (Size)
            {
                case BadgeSize.Mini:
                    badgeSizeClass = "s-badge__mini";
                    break;
            }

            if (badgeSizeClass != null)
            {
                output.AddClass(badgeSizeClass);
            }

            if (!string.IsNullOrWhiteSpace(Image))
            {
                output.Content.AppendHtml($@"<img class=""s-badge--image"" src=""{Image}"" aria-hidden=""true"" />");
            }

            if (!startingContent.IsEmptyOrWhiteSpace)
            {
                var contentStr = startingContent.GetContent();
                output.Content.AppendHtml(contentStr);
            }
        }
    }
}
