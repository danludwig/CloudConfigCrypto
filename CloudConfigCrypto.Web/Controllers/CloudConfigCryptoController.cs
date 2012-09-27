using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
        public ActionResult Encrypt()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Encrypt(EncryptionInput model)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("At least one validation rule is not being enforced by the client.");

            // provider node
            const string configProtectedDataFormat = "<configProtectedData><providers><add name=\"CustomProvider\" thumbprint=\"{0}\" " +
                    "type=\"Pkcs12ProtectedConfigurationProvider.Pkcs12ProtectedConfigurationProvider, PKCS12ProtectedConfigurationProvider, " +
                    "Version=1.0.0.0, Culture=neutral, PublicKeyToken=34da007ac91f901d\" /></providers></configProtectedData>{1}";

            // unencrypted sections
            var unencrypted = string.Format(configProtectedDataFormat, model.Thumbprint, model.Unencrypted);

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
            foreach (var node in configRoot.ChildNodes.Cast<XmlNode>()
                .Where(n => n.Name != "configProtectedData" && n.Name != "#whitespace"))
            {
                // look for special nested section to encrypt
                var encryptableNode = FindEncryptableNode(node) ?? node;

                if (encryptableNode.Attributes != null && encryptableNode.Attributes["configProtectionProvider"] != null)
                {
                    encryptableNode.Attributes.Remove(encryptableNode.Attributes["configProtectionProvider"]);
                }

                var encryptedNode = _provider.Encrypt(encryptableNode);
                var attribute = config.CreateAttribute("configProtectionProvider");
                attribute.Value = "CustomProvider";
                Debug.Assert(encryptableNode.Attributes != null);
                encryptableNode.Attributes.Append(attribute);
                encryptableNode.InnerXml = encryptedNode.OuterXml;
            }

            var encrypted = config.GetFormattedXml();
            return Json(encrypted);
        }

        [NonAction]
        private static XmlNode FindEncryptableNode(XmlNode node)
        {
            var wrapperNode = node.SelectSingleNode("* [@configProtectionProvider='CustomProvider']");
            if (wrapperNode != null) return wrapperNode;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (node.Name == "#whitespace") continue;
                wrapperNode = FindEncryptableNode(childNode);
                if (wrapperNode != null) return wrapperNode;
            }
            return null;
        }

        [HttpGet]
        public ActionResult Decrypt()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Decrypt(DecryptionInput model)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("At least one validation rule is not being enforced by the client.");

            // provider node
            const string configProtectedDataFormat = "<configProtectedData><providers><add name=\"CustomProvider\" thumbprint=\"{0}\" " +
                    "type=\"Pkcs12ProtectedConfigurationProvider.Pkcs12ProtectedConfigurationProvider, PKCS12ProtectedConfigurationProvider, " +
                    "Version=1.0.0.0, Culture=neutral, PublicKeyToken=34da007ac91f901d\" /></providers></configProtectedData>{1}";

            // encrypted sections
            var encrypted = string.Format(configProtectedDataFormat, model.Thumbprint, StripXdtAttributes(model.Encrypted));

            // config documents
            var config = new ConfigXmlDocument
            {
                InnerXml = "<configuration></configuration>",
                DocumentElement = { InnerXml = encrypted },
            };

            // initialize provider
            _provider.Initialize("CustomProvider", new NameValueCollection
            {
                { "thumbprint", model.Thumbprint },
            });

            // decrypt
            var configRoot = config.DocumentElement;
            Debug.Assert(configRoot != null);
            var decryptedSections = new StringBuilder();
            foreach (var node in configRoot.ChildNodes.Cast<XmlNode>()
                .Where(n => n.Name != "configProtectedData" && n.Name != "#whitespace"))
            {
                XmlNode decryptedNode;
                try
                {
                    decryptedNode = _provider.Decrypt(node);
                }
                catch (CryptographicException ex)
                {
                    return Json(new { error = ex.Message.Trim() });
                }
                if (decryptedNode.Attributes != null && decryptedNode.Attributes["configProtectionProvider"] != null)
                {
                    decryptedSections.Append(decryptedNode.InnerXml.Trim());
                }
                else
                {
                    var wrapperNode = FindWrapperNode(decryptedNode);
                    Debug.Assert(wrapperNode.ParentNode != null);
                    wrapperNode.ParentNode.InnerXml = wrapperNode.InnerXml;
                    decryptedSections.Append(decryptedNode.OuterXml);
                }
            }

            config = new ConfigXmlDocument
            {
                InnerXml = "<configuration></configuration>",
                DocumentElement = { InnerXml = decryptedSections.ToString() },
            };

            var decrypted = config.GetFormattedXml();
            return Json(decrypted);
        }

        [NonAction]
        private static string StripXdtAttributes(string xml)
        {
            xml = StripXdtAttribute(xml, "Transform");
            xml = StripXdtAttribute(xml, "Locator");
            xml = StripXdtAttribute(xml, "SupressWarnings");
            return xml;
        }

        [NonAction]
        private static string StripXdtAttribute(string xml, string attribute)
        {
            var clean = xml;
            var xdt = string.Format(" xdt:{0}=", attribute);
            while (clean.Contains(xdt))
            {
                var startIndex = clean.IndexOf(xdt, StringComparison.OrdinalIgnoreCase);
                var delimiter = clean.Substring(startIndex + xdt.Length, 1);
                var length = clean.Substring(startIndex + xdt.Length + delimiter.Length).IndexOf(delimiter, StringComparison.OrdinalIgnoreCase) + 1;
                var endIndex = startIndex + xdt.Length + length + delimiter.Length;
                var builder = new StringBuilder(clean.Substring(0, startIndex));
                builder.Append(clean.Substring(endIndex));
                clean = builder.ToString();
            }
            return clean;
        }

        [NonAction]
        private static XmlNode FindWrapperNode(XmlNode node)
        {
            var wrapperNode = node.SelectSingleNode("* [@configProtectionProvider='CustomProvider']");
            if (wrapperNode != null) return wrapperNode;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (node.Name == "#whitespace") continue;
                wrapperNode = FindWrapperNode(childNode);
                if (wrapperNode != null) return wrapperNode;
            }
            return null;
        }

        [HttpPost]
        public JsonResult ValidateThumbprint(EncryptionInput model)
        {
            var propertyName = model.PropertyName(x => x.Thumbprint);
            if (ModelState.IsValidField(propertyName)) return Json(true);
            var errorMessage = ModelState[propertyName].Errors.First().ErrorMessage;
            return Json(errorMessage);
        }

        [HttpPost]
        public JsonResult ValidatePrivateThumbprint(DecryptionInput model)
        {
            var propertyName = model.PropertyName(x => x.Thumbprint);
            if (ModelState.IsValidField(propertyName)) return Json(true);
            var errorMessage = ModelState[propertyName].Errors.First().ErrorMessage;
            return Json(errorMessage);
        }

        [HttpPost]
        public FileResult Save(ContentModel model)
        {
            var bytes = Encoding.UTF8.GetBytes(model.Content);
            return File(bytes, "application/octet-stream", string.Format("Web.{0}.config", model.Context));
        }
    }
}
