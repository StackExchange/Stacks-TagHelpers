using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Stacks.TagHelpers.Sample.Models;
using System.Diagnostics;
using System.IO;
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
            ViewBag.Title = component.ToLower() + " Tag Helper";

            var camelCasedName = char.ToUpper(component[0]) + component.Substring(1);

            var path = "Views/Components/" + camelCasedName + ".cshtml";
            var file = _env.ContentRootFileProvider.GetFileInfo(path);

            if (!file.Exists)
            {
                return NotFound();
            }

            using var stream = file.CreateReadStream();
            using var fileReader = new StreamReader(stream);
            using var writer = new StringWriter();

            var viewPath = "~/" + path;

            ViewEngineResult viewResult = null;
            viewResult = _eng.GetView(viewPath, viewPath, false);

            // TODO this is a hack if I've ever seen one...
            // fill the view context with just enough to get the partial rendering
            var viewContext = new ViewContext {
                HttpContext = HttpContext,
                RouteData = RouteData,
                ActionDescriptor = ControllerContext.ActionDescriptor,
                FormContext = new Microsoft.AspNetCore.Mvc.ViewFeatures.FormContext()
            };

            return View(new ComponentViewModel
            {
                ViewPath = viewPath,
                ViewContent = fileReader.ReadToEnd(),
                // oh boy I sure hope this doesn't cause any issues... maybe there's a better way?
                RenderedContent = await new PartialTagHelper(_eng, _vbScope) { ViewContext = viewContext, Name = viewPath }.RenderTagHelperAsync()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
