using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using CloudConfigCrypto.Web.Models;
using System.Configuration;

namespace CloudConfigCrypto.Web.Controllers
{
    public class CloudConfigCryptoController : Controller
    {
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

            // provider
            const string configProtectedDataTemplate = @"
    <configProtectedData>
        <providers>
            <add name=""CustomProvider"" 
                thumbprint=""{0}""
                    type=""Pkcs12ProtectedConfigurationProvider.Pkcs12ProtectedConfigurationProvider, PKCS12ProtectedConfigurationProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=34da007ac91f901d"" />
        </providers>
    </configProtectedData>" + "\n    {1}";
            var unencrypted = string.Format(configProtectedDataTemplate, model.Thumbprint, model.Unencrypted.Replace("\r", "").Replace("\n", "\n    ").Replace("\t", "    ") + "\n");

            var config = new ConfigXmlDocument
            {
                InnerXml = "<configuration></configuration>"
            };
            config.CreateElement("configuration");
            config.DocumentElement.InnerXml = unencrypted;

            // encrypt
            var assembly = Assembly.Load("Pkcs12ProtectedConfigurationProvider");
            var providerType = assembly.GetTypes().First(t => typeof(ProtectedConfigurationProvider).IsAssignableFrom(t));
            var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
            provider.Initialize("CustomProvider", new NameValueCollection
            {
                { "thumbprint", model.Thumbprint },
            });

            XmlNode lastWhitespace = config.CreateTextNode("\n    ");
            foreach (XmlNode node in config.DocumentElement.ChildNodes)
            {
                if (node.Name == "#whitespace") lastWhitespace = node;
                if (node.Name == "configProtectedData" || node.Name == "#whitespace") continue;
                var encrypted = provider.Encrypt(node);
                var attribute = config.CreateAttribute("configProtectionProvider");
                attribute.Value = "CustomProvider";
                node.Attributes.Append(attribute);
                node.InnerXml = FormatEncrypted(encrypted.OuterXml, lastWhitespace.OuterXml);
            }

            model.Encrypted = config.OuterXml;

            return View(model);
        }

        private string FormatEncrypted(string outerXml, string lastWhitespace)
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
                if (topOfStack != null && !topOfStack.EndsWith(">") && fragment.Substring(1, fragment.IndexOf(">")).StartsWith(topOfStack.Substring(0, topOfStack.IndexOf(">"))))
                {
                    fragmentStack.Pop();
                }
                else if (topOfStack != null && topOfStack.StartsWith(fragment.Substring(1, fragment.IndexOf(">") - 1)))
                {
                    for (var i = 0; i < fragmentStack.Count + 1; i++)
                    {
                        formatted.Append(lastWhitespaceWithoutNewline);
                    }
                    fragmentStack.Pop();
                }
                else
                {
                    fragmentStack.Push(fragment);
                    for (var i = 0; i < fragmentStack.Count + 1; i++)
                    {
                        formatted.Append(lastWhitespaceWithoutNewline);
                    }
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
