﻿using System.Text;
using System.Web.WebPages;

namespace WiseLabs.Analytics
{
    // Adapted from RazorTemplateBase.cs[1]
    // Microsoft Public License (Ms-PL)[2]
    //
    // [1] http://razorgenerator.codeplex.com/SourceControl/changeset/view/964fcd1393be#RazorGenerator.Templating%2fRazorTemplateBase.cs
    // [2] http://razorgenerator.codeplex.com/license
    public class RazorTemplateBase
    {
        string _content;
        private readonly StringBuilder _generatingEnvironment = new StringBuilder();

        public RazorTemplateBase Layout { get; set; }

        public virtual void Execute() { }

        public void WriteLiteral(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            _generatingEnvironment.Append(textToAppend); ;
        }

        public virtual void Write(object value)
        {
            if (value == null)
                return;
            WriteLiteral(value.ToString());
        }

        public virtual object RenderBody()
        {
            return _content;
        }

        public virtual string TransformText()
        {
            Execute();

            if (Layout != null)
            {
                Layout._content = _generatingEnvironment.ToString();
                return Layout.TransformText();
            }

            return _generatingEnvironment.ToString();
        }

        public static HelperResult RenderPartial<T>() where T : RazorTemplateBase, new()
        {
            return new HelperResult(writer =>
            {
                var t = new T();
                writer.Write(t.TransformText());
            });
        }
    }
}