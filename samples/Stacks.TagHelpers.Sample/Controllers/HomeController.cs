using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.FileProviders;
using Stacks.TagHelpers.Sample.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Sample.Controllers
{
    public class HomeController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly ICompositeViewEngine _eng;
        private readonly IViewBufferScope _vbScope;

        public HomeController(IWebHostEnvironment hostingEnvironment, ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope)
        {
            _env = hostingEnvironment;
            _eng = viewEngine;
            _vbScope = viewBufferScope;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("~/Components/{component}")]
        public async Task<IActionResult> Component(string component)
        {
            return await RenderSample(component, "Components");
        }

        [Route("~/Email/{component}")]
        public async Task<IActionResult> Email(string component)
        {
            return await RenderSample(component, "Email");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //TODO document
        private async Task<IActionResult> RenderSample(string componentName, string section)
        {
            ViewBag.Title = componentName.ToLower() + " Tag Helper";

            var path = "Views/" + section + "/" + componentName + ".cshtml";
            var file = _env.ContentRootFileProvider.GetFileInfo(path);

            if (!file.Exists)
            {
                return NotFound();
            }

            return View("Component", await GetViewModel(path, file));
        }

        //TODO document
        private async Task<IEnumerable<ComponentSampleSet>> GetViewModel(string path, IFileInfo file)
        {

            using var stream = file.CreateReadStream();
            using var fileReader = new StreamReader(stream);
            using var writer = new StringWriter();

            var viewPath = "~/" + path;

            ViewEngineResult viewResult = null;
            viewResult = _eng.GetView(viewPath, viewPath, false);

            // TODO this is a hack if I've ever seen one...
            // fill the view context with just enough to get the partial rendering
            var viewContext = new ViewContext
            {
                HttpContext = HttpContext,
                RouteData = RouteData,
                ActionDescriptor = ControllerContext.ActionDescriptor,
                FormContext = new Microsoft.AspNetCore.Mvc.ViewFeatures.FormContext()
            };

            var rawContent = fileReader.ReadToEnd();
            // oh boy I sure hope this doesn't cause any issues... maybe there's a better way?
            var renderedContent = await new PartialTagHelper(_eng, _vbScope) { ViewContext = viewContext, Name = viewPath }.RenderTagHelperAsync();

            // separate out the examples
            var rawSamples = SeparateExamples(rawContent, true);
            var renderedSamples = SeparateExamples(renderedContent, false);

            // match them up by index
            var combined = rawSamples.Join(renderedSamples,
                o => o.Title, i => i.Title,
                (raw, rendered) => new ComponentSampleSet { RawContent = raw, RenderedContent = rendered });

            return combined;
        }

        //TODO document
        private List<ComponentSample> SeparateExamples(string html, bool trimIndentation)
        {
            // oh man, I'm surely going to burn for this
            // if only TagHelpers would grant me access to their unrendered content, none of this would be necessary
            var matches = Regex.Matches(html, @"<section title=""(.+?)"">(.+?)</section>", RegexOptions.Singleline);

            // no sections, so just return the entire item as one big example
            if (matches.Count == 0)
            {
                return new List<ComponentSample> { new ComponentSample { Title = "Example", Content = html } };
            }

            return matches.Select(m => {
                // clean up the indentation and leading/trailing whitespace
                var content = m.Groups[2].Value;

                //// remove leading/trailing whitespace
                content = content.Trim();
                // remove empty lines
                content = Regex.Replace(content, @"^\s+$", "", RegexOptions.Multiline);

                if (trimIndentation)
                {
                    // drop the leading 4 spaces in each line
                    content = Regex.Replace(content, @"^\s{4}", "", RegexOptions.Multiline);
                }

                return new ComponentSample { Title = m.Groups[1].Value, Content = content };
            }).ToList();
        }
    }
}
