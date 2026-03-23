using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webappi.TagHelpers
{
    [HtmlTargetElement("er-badge")]
    public class ErBadgeTagHelper : TagHelper
    {
        public string Variant { get; set; } = "Primary"; // Primary, Secondary, Success, Danger, Warning, Info
        public string Size { get; set; } = "md"; // sm, md, lg

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Attributes.Add("class", $"er-badge er-badge-{Variant.ToLower()} er-badge-{Size}");
        }
    }
}
