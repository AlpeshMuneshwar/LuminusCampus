using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webappi.TagHelpers
{
    [HtmlTargetElement("er-input", Attributes = "asp-for")]
    public class ErInputTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _generator;

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public string Label { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; } = "text";
        public string Placeholder { get; set; }

        public ErInputTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "er-form-group");

            // Label
            var labelText = Label ?? For.Metadata.DisplayName ?? For.Name;
            var label = new TagBuilder("label");
            label.AddCssClass("er-label");
            label.Attributes.Add("for", For.Name);
            label.InnerHtml.Append(labelText);
            
            output.Content.AppendHtml(label);

            // Wrapper
            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("er-input-wrapper");
            
            // Icon
            if (!string.IsNullOrEmpty(Icon))
            {
                var i = new TagBuilder("i");
                i.AddCssClass($"lni {Icon} er-input-icon");
                wrapper.InnerHtml.AppendHtml(i);
            }

            // Input
            var input = _generator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model,
                null,
                null);

            input.AddCssClass("er-input");
            if (!string.IsNullOrEmpty(Icon)) input.AddCssClass("has-icon");
            
            if(!string.IsNullOrEmpty(Type)) 
            {
                input.MergeAttribute("type", Type, true);
            }
            if(!string.IsNullOrEmpty(Placeholder)) 
            {
                input.MergeAttribute("placeholder", Placeholder, true);
            }
            
            wrapper.InnerHtml.AppendHtml(input);
            output.Content.AppendHtml(wrapper);

            // Validation
            var validationMsg = _generator.GenerateValidationMessage(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                message: null,
                tag: "span",
                htmlAttributes: null);
            
            validationMsg.AddCssClass("er-validation-error");
            output.Content.AppendHtml(validationMsg);
        }
    }
}
