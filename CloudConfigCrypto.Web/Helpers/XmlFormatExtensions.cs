using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CloudConfigCrypto.Web
{
    public static class XmlFormatExtensions
    {
         public static string GetFormattedXml(this ConfigXmlDocument config, string indent = "  ")
         {
             const string newline = "\n";
             var lastElement = "";
             var stack = new Stack<string>();
             var builder = new StringBuilder();
             var elements = config.InnerXml.Split(new[] { '<' }, StringSplitOptions.RemoveEmptyEntries)
                 .Where(n => !string.IsNullOrWhiteSpace(n))
                 .Select(n => string.Format("<{0}", n).Trim())
                 .ToArray();
             foreach (var element in elements)
             {
                 // append newline only when last line ends with >
                 if (builder.ToString().EndsWith(">"))
                     builder.Append(newline);

                 // append element to builder
                 builder.Append(element);

                 // push non-closing element onto builder
                 if (!element.StartsWith("</")) stack.Push(element);

                 // when there is only 1 element in the stack, continue
                 if (stack.Count == 1) continue;

                 // apply indentation
                 if (string.IsNullOrWhiteSpace(lastElement) || lastElement.EndsWith(">"))
                     for (var i = 0; i < stack.Count - 1; i++)
                         builder.Insert(builder.Length - element.Length, indent);

                 // self-closing elements and comments get popped off the stack
                 if (element.StartsWith("</")
                     || element.StartsWith("<!--")
                     || stack.First().EndsWith("/>"))
                     stack.Pop();
                 lastElement = element;
             }

             var result = builder.ToString();
             return result;
         }
    }
}