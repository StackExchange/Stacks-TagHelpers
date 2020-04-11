using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stacks.TagHelpers.Components
{
    //TODO move to Button file?
    [RestrictChildren("button", "form", "a")]
    public class ButtonGroupTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("s-btn-group");
        }
    }

    // TODO is this really necessary
    [HtmlTargetElement("form", Attributes = "button-container", ParentTag = "button-group")]
    public class ButtonGroupContainer : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.AddClass("s-btn-group--container");
        }
    }
}
