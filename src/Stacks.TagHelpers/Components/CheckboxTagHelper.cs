using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    public enum CheckboxState
    {
        None,
        Warning,
        Error,
        Success
    }

    [HtmlTargetElement("input", Attributes = "s-checkbox")]
    public class CheckboxTagHelper : TagHelper
    {
        /// <summary>
        /// Set this as the final helper to run for input so we get asp-* helpers rendered
        /// </summary>
        public override int Order => int.MaxValue;

        public string Label { get; set; }
        public string Description { get; set; }
        public CheckboxState State { get; set; } = CheckboxState.None;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // completely replace this tag with our output
            output.TagName = null;

            if (string.IsNullOrEmpty(Label))
            {
                // TODO is the throw necessary?
                throw new System.Exception("Cannot create a checkbox with an empty label");
            }

            if (context.AllAttributes.TryGetAttribute("id", out var inputAttr) && string.IsNullOrEmpty(inputAttr.Value.ToString()))
            {
                // TODO is the throw necessary?
                throw new System.Exception("Cannot create a checkbox with an empty id.");
            }

            // check if this is inside a checkbox group
            var isInGroup = context.Items.TryGetValue(nameof(CheckboxGroupTagHelper), out var groupContext);

            var inputId = inputAttr?.Value.ToString() ?? context.UniqueId;
            var disabledClass = context.AllAttributes.TryGetAttribute("disabled", out _) ? "is-disabled" : "";
            var checkboxState = State != CheckboxState.None ? "has-" + State.ToString().ToLower() : "";
            var descriptionClass = State == CheckboxState.None ? "s-description" : "s-input-message";

            // we'll render into a StringBuilder since we don't know if this is going to output or being rendered by the parent
            var builder = new StringBuilder();

            if (!isInGroup)
            {
                builder.Append("<fieldset>");
            }
            else
            {
                builder.Append("<div class=\"grid--cell\">");
            }

            // build the output
            builder.Append($@"
<div class=""grid gs8 {disabledClass} {checkboxState}"">
    <div class=""grid--cell"">
        {CreateInput(context, inputId)}
    </div>
    <label class=""grid--cell s-label fw-normal"" for=""{inputId}"">
        {Label}
        {(!string.IsNullOrEmpty(Description) ? "<p class=\"" + descriptionClass + "\">" + Description + "</p>" : "")}
    </label>
</div>
");

            // TODO ugh why does this have to be so complicated!
            if (!isInGroup)
            {
                builder.Append("</fieldset>");
                output.Content.SetHtmlContent(builder.ToString());
            }
            else
            {
                builder.Append("</div>");
                (groupContext as CheckboxGroupContext).Checkboxes.Add(builder.ToString());
                output.SuppressOutput();
            }
        }

        /// <summary>
        /// Creates the input tag based on our original context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputId"></param>
        /// <returns></returns>
        private string CreateInput(TagHelperContext context, string inputId)
        {
            var builder = new TagBuilder("input");
            builder.MergeAttributes(context.AllAttributes.ToDictionary(a => a.Name, a => a.Value));
            builder.TagRenderMode = TagRenderMode.SelfClosing;

            // add the necessary classes to the existing classes if they exist
            var classes = "s-checkbox";
            if (builder.Attributes.TryGetValue("class", out var existingClasses))
            {
                classes = classes + " " + existingClasses;
            }

            // ensure the required attributes are set correctly
            builder.MergeAttribute("type", "checkbox");
            builder.MergeAttribute("id", inputId);
            builder.MergeAttribute("class", classes);

            // remove the taghelper attributes
            builder.Attributes.Remove("s-checkbox");
            builder.Attributes.Remove("label");
            builder.Attributes.Remove("description");

            using (var writer = new StringWriter())
            {
                builder.WriteTo(writer, NullHtmlEncoder.Default);

                return writer.ToString();
            }
        }
    }

    [RestrictChildren("input", "checkbox-group-legend")]
    public class CheckboxGroupTagHelper : TagHelper
    {
        public bool Horizontal { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "fieldset";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("grid gs8 gsy fd-column");

            // flag to children that they are inside a group
            var ctx = new CheckboxGroupContext();
            context.Items.Add(nameof(CheckboxGroupTagHelper), ctx);

            // evaluate our child checkboxes
            var childContent = await output.GetChildContentAsync();

            // append any items that happen to have rendered
            output.Content.AppendHtml(childContent.GetContent());

            if (Horizontal)
            {
                output.Content.AppendHtml("<div class=\"grid--cell\"><div class=\"grid gs16\">");
            }

            // render the individual checkboxes
            foreach (var check in ctx.Checkboxes)
            {
                output.Content.AppendHtml(check);
            }

            if (Horizontal)
            {
                output.Content.AppendHtml("</div></div>");
            }
        }
    }

    [HtmlTargetElement(ParentTag = "checkbox-group")]
    public class CheckboxGroupLegendTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "legend";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("grid--cell s-label");
        }
    }

    internal class CheckboxGroupContext
    {
        public List<string> Checkboxes { get; set; } = new List<string>();
    }
}
