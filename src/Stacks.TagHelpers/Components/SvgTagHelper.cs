using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    [HtmlTargetElement("s-svg")]
    public class SvgTagHelper : TagHelper
    {
        private readonly SvgTagHelperConfiguration _config;

        public string Name { get; set; }

        // TODO document, this should probably be configured via DI
        public SvgTagHelper(SvgTagHelperConfiguration config) : base()
        {
            _config = config;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // completely replace this tag with the contents
            output.TagName = null;

            var path = Path.Combine(_config.SvgFolderPath, Name + ".svg");

            if (!File.Exists(path))
            {
                output.Content.SetHtmlContent("<span>Invalid svg!</span>");
                return;
            }

            // TODO there is likely a better way to do this
            // TODO check that the file exists
            // TODO cache the content so we don't have to re-read from disk every time
            var svg = File.ReadAllText(path);

            // set classes
            if (output.Attributes.ContainsName("class"))
            {
                output.Attributes.TryGetAttribute("class", out var classes);
                svg = Regex.Replace(svg, @"class=""(.+?)""", $@"class=""$1 {classes.Value}""");
            }

            output.Content.SetHtmlContent(svg);
        }

        public static async Task<string> RenderAsync(SvgTagHelperConfiguration config, string iconName, string classes)
        {
            var svg = new SvgTagHelper(config)
            {
                Name = iconName
            };

            TagHelperAttributeList attributes = null;

            if (!string.IsNullOrEmpty(classes))
            {
                attributes = new TagHelperAttributeList();
                attributes.SetAttribute("class", classes);
            }

            var content = await svg.RenderTagHelperAsync(attributes);

            return content;
        }
    }

    public class SvgTagHelperConfiguration
    {
        public string SvgFolderPath { get; set; }
    }

    public static class SvgTagHelperExtensions
    {
        public static void AddSvgTagHelper(this IServiceCollection services, SvgTagHelperConfiguration config)
        {
            services.AddSingleton(config);
        }
    }
}
