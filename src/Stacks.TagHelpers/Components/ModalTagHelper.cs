using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum ModalType
    {
        Default,
        Danger
    }

    [RestrictChildren("modal-body", "modal-footer")]
    public class ModalTagHelper : TagHelper
    {
        public ModalType Type { get; set; }
        public bool Full { get; set; }
        public bool Dismissable { get; set; } = true;
        public string Title { get; set; }
        public bool Shown { get; set; }

        private readonly SvgTagHelperConfiguration _config;

        public ModalTagHelper(SvgTagHelperConfiguration config)
        {
            _config = config;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "aside";
            output.TagMode = TagMode.StartTagAndEndTag;
            var uniqueId = "id_" + Guid.NewGuid().ToString("N");

            var ctx = new ModalContext { UniqueId = uniqueId };
            context.Items.Add(nameof(ModalTagHelper), ctx);

            await output.GetChildContentAsync();

            var clearSvg = await SvgTagHelper.RenderAsync(_config, "ClearSm", null);

            output.AddClass("s-modal");

            // set a11y attributes
            output.Attributes.SetAttribute("tabindex", -1);
            output.Attributes.SetAttribute("role", "dialog");
            output.Attributes.SetAttribute("aria-labelledby", uniqueId + "title");
            output.Attributes.SetAttribute("aria-describedby", uniqueId + "description");

            // set the visibility
            output.Attributes.SetAttribute("aria-hidden", Shown ? "false" : "true");

            output.Content.AppendHtml($@"
<div class=""s-modal--dialog"" role=""document"">
    <h1 class=""s-modal--header"" id=""{uniqueId}-title"">{Title}</h1>
    {ctx.Body}
    {ctx.Footer}
    <a href=""#"" class=""s-modal--close s-btn s-btn__muted"" aria-label=""Close"" data-action=""s-modal#hide"">{clearSvg}</a>
</div>
");
        }
    }

    [HtmlTargetElement("modal-body", ParentTag = "modal")]
    [HtmlTargetElement("modal-footer", ParentTag = "modal")]
    public class ModalChildTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var modalContext = (ModalContext)context.Items[nameof(ModalTagHelper)];

            output.Content.AppendHtml(childContent);

            if (context.TagName == "modal-body")
            {
                output.TagName = "p";
                output.AddClass("s-modal--body");
                output.Attributes.SetAttribute("id", modalContext.UniqueId + "-description");
                modalContext.Body = output.GetContent();
            }
            else
            {
                output.TagName = "div";
                output.AddClass("s-modal--footer");
                modalContext.Footer = output.GetContent();
            }

            output.SuppressOutput();
        }

    }

    public class ModalContext
    {
        public string UniqueId { get; set; }

        public string Body { get; set; }
        public string Footer { get; set; }
    }
}
