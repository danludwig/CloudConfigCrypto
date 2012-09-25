using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using CloudConfigCrypto.Web.Models;

namespace CloudConfigCrypto.Web.Controllers
{
    public class CloudConfigCryptoController : Controller
    {
        private readonly ProtectedConfigurationProvider _provider;

        public CloudConfigCryptoController(ProtectedConfigurationProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult CreateCertificates()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ImportCertificatesLocally()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EncryptConfigSections()
        {
            return View(new EncryptConfigSectionsModel());
        }

        [HttpPost]
        public ActionResult EncryptConfigSections(EncryptConfigSectionsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // provider node
            const string configProtectedDataFormat = @"
    <configProtectedData>
        <providers>
            <add name=""CustomProvider"" 
                thumbprint=""{0}""
                    type=""Pkcs12ProtectedConfigurationProvider.Pkcs12ProtectedConfigurationProvider, PKCS12ProtectedConfigurationProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=34da007ac91f901d"" />
        </providers>
    </configProtectedData>" + "\n    {1}";

            // unencrypted sections
            var unencrypted = model.Unencrypted.Trim().Replace("\r", "").Replace("\t", "    ").Replace("\n", "\n    ") + "\n";
            unencrypted = string.Format(configProtectedDataFormat, model.Thumbprint, unencrypted);

            // config document
            var config = new ConfigXmlDocument
            {
                InnerXml = "<configuration></configuration>",
                DocumentElement = { InnerXml = unencrypted },
            };

            // initialize provider
            _provider.Initialize("CustomProvider", new NameValueCollection
            {
                { "thumbprint", model.Thumbprint },
            });

            // encrypt
            var configRoot = config.DocumentElement;
            Debug.Assert(configRoot != null);
            XmlNode lastWhitespace = config.CreateTextNode("\n    ");
            foreach (XmlNode node in configRoot.ChildNodes)
            {
                // skip non-encryptable nodes
                if (node.Name == "#whitespace") lastWhitespace = node;
                if (node.Name == "configProtectedData" || node.Name == "#whitespace") continue;

                var encrypted = _provider.Encrypt(node);
                var attribute = config.CreateAttribute("configProtectionProvider");
                attribute.Value = "CustomProvider";
                Debug.Assert(node.Attributes != null);
                node.Attributes.Append(attribute);
                node.InnerXml = FormatXml(encrypted.OuterXml, lastWhitespace.OuterXml);
            }

            model.Encrypted = config.OuterXml;

            return View(model);
        }

        private string FormatXml(string outerXml, string lastWhitespace)
        {
            var formatted = new StringBuilder();
            var lastWhitespaceWithoutNewline = lastWhitespace.Replace("\n", "");
            var fragments = outerXml.Split(new[] { '<' }, StringSplitOptions.RemoveEmptyEntries);
            var fragmentStack = new Stack<string>();
            foreach (var fragment in fragments)
            {
                if (formatted.Length == 0 || formatted.ToString().EndsWith(">"))
                    formatted.Append("\n");
                var topOfStack = fragmentStack.FirstOrDefault();
                if (topOfStack != null
                    && !topOfStack.EndsWith(">")
                    && fragment.Substring(1, fragment.IndexOf(">", StringComparison.Ordinal))
                        .StartsWith(topOfStack.Substring(0, topOfStack.IndexOf(">", StringComparison.Ordinal))))
                {
                    fragmentStack.Pop();
                }
                else if (topOfStack != null
                    && topOfStack.StartsWith(fragment.Substring(1, fragment.IndexOf(">", StringComparison.Ordinal) - 1)))
                {
                    for (var i = 0; i < fragmentStack.Count + 1; i++)
                        formatted.Append(lastWhitespaceWithoutNewline);
                    fragmentStack.Pop();
                }
                else
                {
                    fragmentStack.Push(fragment);
                    for (var i = 0; i < fragmentStack.Count + 1; i++)
                        formatted.Append(lastWhitespaceWithoutNewline);
                }
                formatted.Append("<");
                formatted.Append(fragment);
                if (fragment.EndsWith("/>")) fragmentStack.Pop();
            }

            formatted.Append(lastWhitespace);

            var returnValue = formatted.ToString();
            return returnValue;
        }
    }
}
