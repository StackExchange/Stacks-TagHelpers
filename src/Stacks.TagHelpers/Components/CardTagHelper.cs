using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public class CardTagHelper : TagHelper
    {
        public string Href { get; set; }
        public bool Muted { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("s-card");

            if (!string.IsNullOrEmpty(Href))
            {
                output.TagName = "a";
                output.Attributes.SetAttribute("href", Href);
            }

            if (Muted)
            {
                output.AddClass("s-card__muted");
            }
        }
    }

    // TODO not actually part of the spec
    public class CardTitleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "h2";
            output.TagMode = TagMode.StartTagAndEndTag;

            // set the "defaults", but don't override anything that is already set
            if (!output.Attributes.ContainsName("class"))
            {
                // TODO this is not in the spec at all, but maybe it should be...
                output.Attributes.SetAttribute("class", "fs-body3 lh-sm fc-dark");
            }
        }
    }

    // TODO not actually part of the spec
    public class CardBodyTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "p";
            output.TagMode = TagMode.StartTagAndEndTag;

            // set the "defaults", but don't override anything that is already set
            if (!output.Attributes.ContainsName("class"))
            {
                // TODO this is not in the spec at all, but maybe it should be...
                output.Attributes.SetAttribute("class", "fs-body1 fc-medium");
            }
        }

    }

    // TODO not actually part of the spec
    public class CardFooterTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "p";
            output.TagMode = TagMode.StartTagAndEndTag;

            // TODO literally an empty container since there isn't anything like this in the spec
        }
    }
}
