using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace Stacks.TagHelpers
{
    public static class TagHelperExtensions
    {
        private static readonly TagHelperAttributeList _emptyAttributes = new TagHelperAttributeList();
        private static readonly IDictionary<object, object> _emptyItems = ImmutableDictionary<object, object>.Empty;

        /// <summary>
        /// TODO document
        /// </summary>
        /// <param name="tagHelper"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public async static Task<string> RenderTagHelperAsync(this TagHelper tagHelper, TagHelperAttributeList attributes = null)
        {
            attributes = attributes ?? _emptyAttributes;

            var output = new TagHelperOutput("div", attributes, (_, __) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
            var context = new TagHelperContext(attributes, _emptyItems, Guid.NewGuid().ToString());

            await tagHelper.ProcessAsync(context, output);

            return output.GetContent();
        }

        /// <summary>
        /// TODO document
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public static string GetContent(this TagHelperOutput output)
        {
            using (var writer = new StringWriter())
            {
                output.WriteTo(writer, NullHtmlEncoder.Default);
                return writer.ToString();
            }
        }

        /// <summary>
        /// TODO document
        /// </summary>
        /// <param name="output"></param>
        /// <param name="classValue"></param>
        public static void AddClass(this TagHelperOutput output, string classValue)
        {
            output.Attributes.AppendToAttribute("class", classValue);
        }

        /// <summary>
        /// TODO document
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public static void AppendToAttribute(this TagHelperAttributeList attributes, string attribute, string value)
        {
            var hasAttribute = attributes.TryGetAttribute(attribute, out var classAttribute);

            var existingValue = "";

            if (hasAttribute)
            {
                existingValue = classAttribute.Value.ToString() + " ";
            }

            existingValue += value;

            attributes.SetAttribute(attribute, existingValue);
        }
    }
}
