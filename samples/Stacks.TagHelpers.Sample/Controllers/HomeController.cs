using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stacks.TagHelpers.Sample.Models;
using System.Diagnostics;
using System.IO;

namespace Stacks.TagHelpers.Sample.Controllers
{
    public class HomeController : Controller
    {

        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("~/Components/{component}")]
        public IActionResult Component(string component)
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

            return View(new ComponentViewModel
            {
                ViewPath = "~/" + path,
                ViewContent = fileReader.ReadToEnd()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
