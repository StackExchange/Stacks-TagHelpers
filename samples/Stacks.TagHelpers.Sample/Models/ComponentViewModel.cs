namespace Stacks.TagHelpers.Sample.Models
{
    public class ComponentViewModel
    {
        public string ViewPath { get; set; }
        public string ViewContent { get; set; }
        public string RenderedContent { get; set; }
    }

    public class ComponentSampleSet
    {
        public ComponentSample RawContent { get; set; }
        public ComponentSample RenderedContent { get; set; }
    }

    public class ComponentSample
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
