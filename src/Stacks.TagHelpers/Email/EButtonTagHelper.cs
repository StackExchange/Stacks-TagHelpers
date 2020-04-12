using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Email
{
    public enum EButtonStyle
    {
        Primary,
        White,
        Outlined
    }

    public enum EButtonSize
    {
        Default,
        Small
    }

    public class EButtonTagHelper : TagHelper
    {
        public EButtonStyle Style { get; set; } = EButtonStyle.Primary;
        public EButtonSize Size { get; set; } = EButtonSize.Default;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // replace the entire contents
            output.TagName = null;

            var content = await output.GetChildContentAsync();

            // Style == Primary
            var backgroundColor = "#0095ff";
            var borderColor = "#0077cc";
            var textColor = "#ffffff";
            var styleClass = "s-btn__primary";
            var boxShadow = "box-shadow: inset 0 1px 0 0 rgba(102,191,255,.75);";

            if (Style == EButtonStyle.White)
            {
                backgroundColor = "#ffffff";
                borderColor = "#ffffff";
                textColor = "#0077cc";
                styleClass = "s-btn__white";
                boxShadow = "";
            }
            else if (Style == EButtonStyle.Outlined)
            {
                backgroundColor = "#ffffff";
                borderColor = "#0077cc";
                textColor = "#0077cc";
                styleClass = "s-btn__outlined";
                boxShadow = "";
            }

            // Size == Default
            var borderRadius = "4px";
            var textHeight = "17px";
            var padding = "13px 17px";

            if (Size == EButtonSize.Small)
            {
                borderRadius = "3px";
                textHeight = "13px";
                padding = "10px";
            }

            output.Content.SetHtmlContent($@"
<td class=""s-btn {styleClass}"" style=""border-radius: {borderRadius}; background: {backgroundColor}; text-align: center;"">
    <a class=""s-btn {styleClass}"" href =""#"" style=""background: {backgroundColor}; border: 1px solid {borderColor}; {boxShadow} font-family: arial, sans-serif; font-size: {textHeight}; line-height: {textHeight}; color: {textColor}; text-align: center; text-decoration: none; padding: {padding}; display: block; border-radius: {borderRadius};"" >
        {content.GetContent()}
    </a>
</td>
");
        }
    }
}
