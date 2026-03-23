using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webappi.TagHelpers
{
    [HtmlTargetElement("er-card")]
    public class ErCardTagHelper : TagHelper
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Icon { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "er-card");

            var content = "";

            // Header Section
            if (!string.IsNullOrEmpty(Title))
            {
                content += $"<div class='er-card-header'>";
                
                if (!string.IsNullOrEmpty(Icon))
                {
                    content += $"<div class='er-card-icon'><i class='lni {Icon}'></i></div>";
                }

                content += "<div class='er-card-title-group'>";
                content += $"<h3 class='er-card-title'>{Title}</h3>";
                if (!string.IsNullOrEmpty(Subtitle))
                {
                    content += $"<p class='er-card-subtitle'>{Subtitle}</p>";
                }
                content += "</div>"; // End Group
                
                // Allow injection of actions in header via a named slot or similar later, 
                // for now simple title.
                content += "</div>"; // End Header
            }

            // Body Section (Existing Content)
            output.PreContent.SetHtmlContent(content + "<div class='er-card-body'>");
            output.PostContent.SetHtmlContent("</div>");
        }
    }
}
