using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum BannerType
    {
        Default,
        Info,
        Success,
        Warning,
        Danger
    }

    public class BannerTagHelper : TagHelper
    {
        public BannerType Type { get; set; }
        public bool Important { get; set; }
        public bool Shown { get; set; }
        public bool Pinned { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "aside";
            output.TagMode = TagMode.StartTagAndEndTag;

            var startingContent = await output.GetChildContentAsync();

            output.AddClass("s-banner");

            string bannerTypeClass = null;

            switch (Type)
            {
                case BannerType.Info:
                    bannerTypeClass = "s-banner__info";
                    break;
                case BannerType.Success:
                    bannerTypeClass = "s-banner__success";
                    break;
                case BannerType.Warning:
                    bannerTypeClass = "s-banner__warning";
                    break;
                case BannerType.Danger:
                    bannerTypeClass = "s-banner__danger";
                    break;
            }

            if (bannerTypeClass != null)
            {
                output.AddClass(bannerTypeClass);
            }

            if (Important)
            {
                output.AddClass("s-banner__important");
            }

            if (Pinned)
            {
                output.AddClass("is-pinned");
            }

            // set the a11y attributes
            output.Attributes.SetAttribute("role", "alert");
            output.Attributes.SetAttribute("aria-hidden", Shown ? "false" : "true");

            output.Content.AppendHtml(startingContent.GetContent());
        }
    }
}
