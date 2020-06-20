using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Components
{
    [HtmlTargetElement("s-svg")]
    public class SvgTagHelper : TagHelper
    {
        private const string INVALID_PATH_CONTENT = "<span>Invalid svg name/path</span>";
        private const string INVALID_CONFIG_CONTENT = "<span>Invalid SvgTagHelperConfiguration.SvgFolderPath</span>";

        private readonly SvgTagHelperConfiguration _config;
        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        public string Name { get; set; }

        // TODO document, this should probably be configured via DI
        public SvgTagHelper(SvgTagHelperConfiguration config) : base()
        {
            _config = config;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }

            // cache the entire svg, which is generated based on Name + added classes
            var hasClassAttribute = output.Attributes.TryGetAttribute("class", out var classes);
            var cacheKey = Name + (hasClassAttribute ? "_" + classes.Value : "");

            // completely replace this tag with the contents
            output.TagName = null;

            // if the item is in the cache, set and return early
            if (_cache.ContainsKey(cacheKey))
            {
                output.Content.SetHtmlContent(_cache[cacheKey]);
                return;
            }

            // if the config is invalid, return early
            // TODO throwing might be better?
            if (string.IsNullOrEmpty(_config?.SvgFolderPath))
            {
                _cache[cacheKey] = INVALID_CONFIG_CONTENT;
                output.Content.SetHtmlContent(INVALID_CONFIG_CONTENT);
                return;
            }

            var path = Path.Combine(_config.SvgFolderPath, Name + ".svg");

            // if the icon can't be loaded from disk, return early
            // TODO throwing might be better?
            if (!File.Exists(path))
            {
                _cache[cacheKey] = INVALID_PATH_CONTENT;
                output.Content.SetHtmlContent(INVALID_PATH_CONTENT);
                return;
            }

            // TODO there is likely a better way to do this
            var svg = File.ReadAllText(path);

            // set classes onto the svg string
            // TODO support svgs that don't have a class or the class attribute exists, but is not on the top level element
            if (hasClassAttribute)
            {
                svg = Regex.Replace(svg, @"class=""(.+?)""", $@"class=""$1 {classes.Value}""");
            }

            // set the cache entry
            _cache[cacheKey] = svg;

            output.Content.SetHtmlContent(svg);
        }

        /// <summary>
        /// Renders an icon's svg string
        /// </summary>
        /// <param name="config">The SvgTagHelperConfiguration to base our SvgTagHelper on</param>
        /// <param name="iconName">The name of the icon to render</param>
        /// <param name="classes">The classes to add to the icon's class attribute</param>
        /// <returns></returns>
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
