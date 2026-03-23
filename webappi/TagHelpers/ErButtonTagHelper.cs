using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webappi.TagHelpers
{
    [HtmlTargetElement("er-button")]
    public class ErButtonTagHelper : TagHelper
    {
        public string Type { get; set; } = "button"; // submit, button, reset
        public string Variant { get; set; } = "Primary"; // Primary, Secondary, Danger
        public string Size { get; set; } = "md"; // sm, md, lg
        public string Icon { get; set; }
        public string Href { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Href))
            {
                output.TagName = "a";
                output.Attributes.Add("href", Href);
            }
            else
            {
                output.TagName = "button";
                output.Attributes.Add("type", Type);
            }

            output.Attributes.Add("class", $"er-btn er-btn-{Variant.ToLower()} er-btn-{Size}");

            var content = "";
            if (!string.IsNullOrEmpty(Icon))
            {
                content += $"<i class='lni {Icon}'></i> ";
            }

            output.PreContent.SetHtmlContent(content);
        }
    }
}
