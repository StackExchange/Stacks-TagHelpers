using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum ButtonWeight
    {
        Clear,
        Outlined,
        Filled
    }

    public enum ButtonState
    {
        Secondary,
        Primary,
        Danger,
        Muted
    }

    public enum ButtonSize
    {
        Default,
        XSmall,
        Small,
        // Default lies here size-wise
        Medium,
        Large,
        XLarge
    }

    public enum ButtonBranding
    {
        None,
        Facebook,
        Google,
        Github
    }

    [HtmlTargetElement(Attributes = "s-btn")]
    public class ButtonTagHelper : TagHelper
    {
        public ButtonWeight Weight { get; set; } = ButtonWeight.Clear;
        public ButtonState State { get; set; } = ButtonState.Secondary;
        public ButtonSize Size { get; set; } = ButtonSize.Default;
        public ButtonBranding Branding { get; set; } = ButtonBranding.None;
        public bool Loading { get; set; }
        public bool Dropdown { get; set; }
        public bool Unset { get; set; }
        public bool Link { get; set; }
        public bool Selected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (State == ButtonState.Primary && Weight != ButtonWeight.Clear)
            {
                // TODO throw necessary?
                throw new System.Exception("Cannot have a Type set when State is set to Primary.");
            }

            if (Branding != ButtonBranding.None && (Weight != ButtonWeight.Clear || State != ButtonState.Secondary))
            {
                // TODO throw necessary?
                throw new System.Exception("Cannot have Weight or State set when Branding is set.");
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.TryGetAttribute("s-btn", out var btnAttr);
            output.Attributes.Remove(btnAttr);

            output.AddClass("s-btn");

            if (Weight != ButtonWeight.Clear)
            {
                output.AddClass("s-btn__" + Weight.ToString().ToLower());
            }

            if (State != ButtonState.Secondary)
            {
                output.AddClass("s-btn__" + State.ToString().ToLower());
            }

            if (Branding != ButtonBranding.None)
            {
                output.AddClass("s-btn__" + Branding.ToString().ToLower());
            }

            var sizeClass = "";
            switch (Size)
            {
                case ButtonSize.XSmall:
                    sizeClass = "s-btn__xs";
                    break;
                case ButtonSize.Small:
                    sizeClass = "s-btn__sm";
                    break;
                case ButtonSize.Medium:
                    sizeClass = "s-btn__md";
                    break;
                case ButtonSize.Large:
                    sizeClass = "s-btn__lg";
                    break;
                case ButtonSize.XLarge:
                    sizeClass = "s-btn__xl";
                    break;
                case ButtonSize.Default:
                default:
                    break;
            }

            output.AddClass(sizeClass);

            if (Loading)
            {
                output.AddClass("is-loading");
            }

            if (Selected)
            {
                output.AddClass("is-selected");
            }

            if (Dropdown)
            {
                output.AddClass("s-btn__dropdown");
            }

            if (Unset)
            {
                output.AddClass("s-btn__unset");
            }

            if (Link)
            {
                output.AddClass("s-btn__link");
            }
        }
    }

    [HtmlTargetElement("button-badge")]
    public class ButtonBadgeTagHelper : TagHelper
    {
        public bool Number { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.AddClass("s-btn--badge");

            var content = await output.GetChildContentAsync();

            if (Number)
            {
                output.Content
                    .AppendHtml("<span class=\"s-btn--number\">")
                    .AppendHtml(content)
                    .AppendHtml("</span>");
            }
            else
            {
                output.Content.AppendHtml(content);
            }
        }
    }

    //TODO is this really even necessary?
    [HtmlTargetElement(Attributes = "s-btn-icon", ParentTag = "s-svg")]
    public class ButtonIconTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.AddClass("s-btn--icon");
        }
    }
}
