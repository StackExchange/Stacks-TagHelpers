using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace Stacks.TagHelpers.Components.Controllers
{
    [HtmlTargetElement(Attributes = ATTRIBUTE_NAME)]
    public class SModalTagHelper : TagHelper
    {
        public const string ATTRIBUTE_NAME = "s-modal-toggle";

        public string ReturnElement { get; set; }
        public bool RemoveWhenHidden { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(ATTRIBUTE_NAME);

            output.Attributes.SetAttribute("data-controller", "s-modal");

            if (!string.IsNullOrWhiteSpace(ReturnElement))
            {
                output.Attributes.SetAttribute("data-s-modal-return-element", ReturnElement);
            }

            if (RemoveWhenHidden)
            {
                output.Attributes.SetAttribute("data-s-modal-remove-when-hidden", "true");
            }
        }
    }

    public enum ModalAction
    {
        None,
        Toggle,
        Show,
        Hide
    }

    [HtmlTargetElement(Attributes = "modal-action")]
    public class SModalActionTagHelper : TagHelper
    {
        public ModalAction ModalAction { get; set; } = ModalAction.None;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("modal-action");
            if (ModalAction != ModalAction.None)
            {
                output.Attributes.SetAttribute("data-action", "s-modal#" + ModalAction.ToString().ToLower());
            }
        }
    }

    public enum ModalTarget
    {
        None,
        Modal,
        InitialFocus
    }

    [HtmlTargetElement(Attributes = "modal-target")]
    public class SModalTargetTagHelper : TagHelper
    {
        public ModalTarget ModalTarget { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("modal-target");
            if (ModalTarget != ModalTarget.None)
            {
                var camelCase = Regex.Replace(ModalTarget.ToString(), @"^([A-Z])", (m) => m.Value.ToLower());
                output.Attributes.SetAttribute("data-target", "s-modal." + camelCase);
            }
        }
    }
}
