using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webappi.TagHelpers
{
    [HtmlTargetElement("er-checkbox", Attributes = "asp-for")]
    public class ErCheckboxTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _generator;

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public string Label { get; set; }

        public ErCheckboxTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "er-checkbox-group");

            // Checkbox Input
            var checkbox = _generator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                isChecked: null,
                htmlAttributes: null);

            checkbox.AddCssClass("er-checkbox");
            checkbox.Attributes.Add("id", For.Name);

            // Label
            var labelText = Label ?? For.Metadata.DisplayName ?? For.Name;
            var label = new TagBuilder("label");
            label.AddCssClass("er-checkbox-label");
            label.Attributes.Add("for", For.Name);
            label.InnerHtml.Append(labelText);

            output.Content.AppendHtml(checkbox);
            output.Content.AppendHtml(label);

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
