using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stacks.TagHelpers.Sample.Models;
using System.IO;

namespace Stacks.TagHelpers.Sample.Controllers
{
    public class ComponentsController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ComponentsController(IWebHostEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment;
        }

        [Route("~/Components/{component}")]
        public IActionResult Component(string component)
        {
            ViewBag.Title = component.ToLower() + " Tag Helper";

            var camelCasedName = char.ToUpper(component[0]) + component.Substring(1);

            var file = _env.ContentRootFileProvider.GetFileInfo("Views/Components/" + camelCasedName + ".cshtml");

            if (!file.Exists)
            {
                return NotFound();
            }

            using var stream = file.CreateReadStream();
            using var fileReader = new StreamReader(stream);

            return View(new ComponentViewModel
            {
                ViewName = camelCasedName,
                ViewContent = fileReader.ReadToEnd()
            });
        }
    }
}
