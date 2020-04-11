using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    [RestrictChildren("breadcrumb-item")]
    public class BreadcrumbsTagHelper : TagHelper
    {
        public string DividerIcon { get; set; } = "ArrowRightAltSm";

        private readonly SvgTagHelperConfiguration _config;

        public BreadcrumbsTagHelper(SvgTagHelperConfiguration config)
        {
            _config = config;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("s-breadcrumbs");
            output.Attributes.SetAttribute("aria-label", "breadcrumb");

            var ctx = new BreadcrumbsContext();
            context.Items.Add(nameof(BreadcrumbsTagHelper), ctx);

            await output.GetChildContentAsync();

            var svgIcon = await SvgTagHelper.RenderAsync(_config, DividerIcon, "s-breadcrumbs--divider");

            for (var i = 0; i < ctx.Crumbs.Count; i++)
            {
                var crumb = ctx.Crumbs[i];

                output.Content.AppendHtml("<div class=\"s-breadcrumbs--item\">")
                    .AppendHtml(crumb);

                if (i != ctx.Crumbs.Count - 1)
                {
                    output.Content.AppendHtml(svgIcon);
                }

                output.Content.AppendHtml("</div>");
            }
        }
    }

    [HtmlTargetElement("breadcrumb-item", ParentTag = "breadcrumbs")]
    public class BreadcrumbItemTagHelper : TagHelper
    {
        public string Href { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var bcContext = (BreadcrumbsContext)context.Items[nameof(BreadcrumbsTagHelper)];

            output.TagName = "a";
            output.AddClass("s-breadcrumbs--link");
            output.Attributes.SetAttribute("href", Href);
            output.Content.AppendHtml(childContent);


            bcContext.Crumbs.Add(output.GetContent());
            output.SuppressOutput();
        }
    }

    internal class BreadcrumbsContext
    {
        public List<string> Crumbs { get; set; } = new List<string>();
    }
}
