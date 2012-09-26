using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            foreach (XmlNode node in configRoot.ChildNodes)
            {
                // skip non-encryptable nodes
                if (node.Name == "configProtectedData" || node.Name == "#whitespace") continue;

                var encryptedNode = _provider.Encrypt(node);
                var attribute = config.CreateAttribute("configProtectionProvider");
                attribute.Value = "CustomProvider";
                Debug.Assert(node.Attributes != null);
                node.Attributes.Append(attribute);
                node.InnerXml = encryptedNode.OuterXml;
            }

            var encrypted = config.GetFormattedXml();
            return Json(encrypted);
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
            var encrypted = model.Encrypted; //.Trim().Replace("\r", "").Replace("\t", "    ").Replace("\n", "\n    ") + "\n";
            encrypted = string.Format(configProtectedDataFormat, model.Thumbprint, encrypted);

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
            foreach (XmlNode node in configRoot.ChildNodes)
            {
                // skip non-encryptable nodes
                if (node.Name == "configProtectedData" || node.Name == "#whitespace") continue;

                var decryptedNode = _provider.Decrypt(node);
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
        private static XmlNode FindWrapperNode(XmlNode node)
        {
            var wrapperNode = node.SelectSingleNode("* [@configProtectionProvider='CustomProvider']");
            if (wrapperNode != null) return wrapperNode;
            foreach (XmlNode childNode in node.ChildNodes)
            {
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
        public FileResult Save(ContentModel model)
        {
            var bytes = Encoding.UTF8.GetBytes(model.Content);
            return File(bytes, "application/octet-stream", string.Format("Web.{0}.config", model.Context));
        }
    }
}
