using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webappi.TagHelpers
{
    [HtmlTargetElement("er-select", Attributes = "asp-for")]
    public class ErSelectTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _generator;

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public string Label { get; set; }
        public string Icon { get; set; }
        public string Placeholder { get; set; }

        [HtmlAttributeName("asp-items")]
        public IEnumerable<SelectListItem> Items { get; set; }

        public ErSelectTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "er-form-group");

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

            // Select
            var select = _generator.GenerateSelect(
                ViewContext,
                For.ModelExplorer,
                Placeholder, // optionLabel
                For.Name, // expression
                Items, // selectList
                allowMultiple: false, // Required parameter in .NET 10
                htmlAttributes: null);

            select.AddCssClass("er-input");
            if (!string.IsNullOrEmpty(Icon)) select.AddCssClass("has-icon");
            
            // Get child content (options)
            var childContent = await output.GetChildContentAsync();
            if (!childContent.IsEmptyOrWhiteSpace)
            {
                select.InnerHtml.AppendHtml(childContent);
            }

            wrapper.InnerHtml.AppendHtml(select);
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
