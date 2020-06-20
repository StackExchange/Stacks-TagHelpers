using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stacks.TagHelpers.Components;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Stacks.TagHelpers.Benchmarks
{
    [Config(typeof(Config))]
    public class TagHelperBenchmarks
    {
        private readonly RazorRenderer _renderer;
        public static readonly string[] AllComponents = new[] { "Avatar", "Badge", "Banner", "Breadcrumbs", "ButtonGroup", /*"Button",*/ "Card", /*"Checkbox",*/ /*"Modal",*/ /*"Popover",*/ "Svg" };

        public string[] ComponentValues { get; }
        public string[] StateValues => new[] { "With", "Without" };

        [ParamsSource(nameof(ComponentValues))]
        public string Component { get; set; }

        [ParamsSource(nameof(StateValues))]
        public string State { get; set; }

        public TagHelperBenchmarks()
        {
            _renderer = new RazorRenderer();
            // TODO I bet there's a better way to pass params to the benchmarks without resorting to static properties...
            ComponentValues = Program.ChosenComponents;
        }

        [Benchmark]
        public async Task Benchmark()
        {
            await _renderer.RenderAsync<object>("/Views/" + Component + State + ".cshtml", null);
        }
    }

    class Program
    {
        public static string[] ChosenComponents { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please select which components you'd like to run (comma separated) or type 'All' to run all components:\n" + string.Join('\n', TagHelperBenchmarks.AllComponents));
                return;
            }
            else if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                ChosenComponents = TagHelperBenchmarks.AllComponents;
            }
            else if (args.Length > 0)
            {
                // inner join all entered components and use the value from the AllComponents array for all matched entries
                ChosenComponents = args[0].Split(',')
                    .Join(TagHelperBenchmarks.AllComponents, o => o.ToLower().Trim(), i => i.ToLower(), (o, i) => i).ToArray();
            }

            if (ChosenComponents.Length == 0)
            {
                Console.WriteLine("Unable to match any entered values to components");
                return;
            }

            BenchmarkRunner.Run<TagHelperBenchmarks>(new DebugInProcessConfig());
        }

    }

    internal class Config : ManualConfig
    {
        public Config()
        {
            AddDiagnoser(MemoryDiagnoser.Default);
            //AddJob(Job.Default.WithRuntime(ClrRuntime.Net472));
            //AddJob(Job.Default.WithRuntime(CoreRuntime.Core31));
            //AddJob(new InliningDiagnoser());
        }
    }

    public class RazorRenderer
    {
        public async Task<string> RenderAsync<TModel>(string name, TModel model)
        {
            // get this assembly so we can get the compiled razor pages out of it
            var assembly = RelatedAssemblyAttribute.GetRelatedAssemblies(typeof(TagHelperBenchmarks).Assembly, false).SingleOrDefault();

            // get all the compiled razor items and filter down to just the one we want
            var allItems = new RazorCompiledItemLoader().LoadItems(assembly);
            var razorCompiledItem = allItems
                .FirstOrDefault(item => item.Identifier == name);

            if (razorCompiledItem == null)
            {
                throw new Exception("Unable to find or compile item with name: " + name);
            }

            // execute the page and get the final string
            return await GetOutput(assembly, razorCompiledItem, model);
        }

        private static async Task<string> GetOutput<TModel>(Assembly assembly, RazorCompiledItem razorCompiledItem, TModel model)
        {
            using var output = new StringWriter();

            // create services and add all of MVC because we need the ITagHelperFactory and I'm too lazy to pick it out by itself
            var services = new ServiceCollection();
            services.AddMvc();
            // add in the SvgTagHelperConfiguration so the views don't crash at runtime
            services.AddSvgTagHelper(new SvgTagHelperConfiguration
            {
                SvgFolderPath = "" //TODO
            });
            var serviceProvider = services.BuildServiceProvider();

            // get the compiled RazorPage from the assembly
            var compiledTemplate = assembly.GetType(razorCompiledItem.Type.FullName);
            var razorPage = (RazorPage)Activator.CreateInstance(compiledTemplate);

            // set the ViewData to our model
            if (razorPage is RazorPage<TModel> page)
            {
                page.ViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

            }

            // set the ViewContext + HttpContext w/ services
            razorPage.ViewContext = new ViewContext
            {
                HttpContext = new DefaultHttpContext {
                    ServiceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>()
                },
                Writer = output
            };

            // necessary fluff
            razorPage.DiagnosticSource = new DiagnosticListener("GetOutput");
            razorPage.HtmlEncoder = HtmlEncoder.Default;

            // actually execute the razor page
            await razorPage.ExecuteAsync();

            // return the compiled page as a string
            return output.ToString();
        }
    }
}
