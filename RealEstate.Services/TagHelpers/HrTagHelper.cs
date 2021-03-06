﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("hr", Attributes = "subject", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class HrTagHelper : TagHelper
    {
        public string Subject { get; set; }

        public string Class { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Subject))
            {
                var subject = new TagBuilder("span");
                subject.InnerHtml.Append(Subject);

                var hr = new TagBuilder("hr");

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(subject).AppendHtml(hr);

                if (!string.IsNullOrEmpty(Class))
                    foreach (var cls in Class.Split(' '))
                        output.AddClass(cls, HtmlEncoder.Default);

                output.AddClass("title-bar", HtmlEncoder.Default);
            }
            else
            {
                output.TagName = "hr";
                output.TagMode = TagMode.SelfClosing;
            }
        }
    }
}