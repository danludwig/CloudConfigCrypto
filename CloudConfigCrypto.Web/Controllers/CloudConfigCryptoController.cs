using System;
using System.Collections.Generic;
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
        // the Pkcs12ProtectedConfigurationProvider dependency is injected using its base class.
        private readonly ProtectedConfigurationProvider _provider;

        public CloudConfigCryptoController(ProtectedConfigurationProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult CreateCertificates()
        {
            return View();
        }

        [HttpGet]
        public ViewResult ImportCertificatesLocally()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Encrypt()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Encrypt(EncryptionInput model)
        {
            ThrowWhenModelStateIsInvalid();

            var config = CreateConfigXmlDocument(model.Thumbprint, model.Unencrypted);

            InitializeProvider(model.Thumbprint);

            // encrypt
            foreach (var node in GetEligibleCryptoNodes(config))
            {
                // look for special nested section to encrypt
                var encryptableNode = FindNestedCryptoNode(node) ?? node;

                // strip off the configProtectionProvider attribute if it exists (do not want to encrypt it)
                if (encryptableNode.Attributes != null && encryptableNode.Attributes["configProtectionProvider"] != null)
                    encryptableNode.Attributes.Remove(encryptableNode.Attributes["configProtectionProvider"]);

                // encrypt the node
                var encryptedNode = _provider.Encrypt(encryptableNode);

                // create a new configProtectionProvider attribute
                var attribute = config.CreateAttribute("configProtectionProvider");
                attribute.Value = "CustomProvider";
                Debug.Assert(encryptableNode.Attributes != null);

                // append the attribute to the node that was encrypted (not the encryption result)
                encryptableNode.Attributes.Append(attribute);

                // nest the encryption result into the node that was encrypted
                encryptableNode.InnerXml = encryptedNode.OuterXml;
            }

            // format and return the encrypted xml
            var encrypted = config.GetFormattedXml();
            return Json(encrypted);
        }

        [HttpPost]
        public JsonResult ValidateEncryptionThumbprint(EncryptionInput model)
        {
            var propertyName = model.PropertyName(x => x.Thumbprint);
            if (ModelState.IsValidField(propertyName)) return Json(true);
            var errorMessage = ModelState[propertyName].Errors.First().ErrorMessage;
            return Json(errorMessage);
        }

        [HttpGet]
        public ViewResult Decrypt()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Decrypt(DecryptionInput model)
        {
            ThrowWhenModelStateIsInvalid();

            var config = CreateConfigXmlDocument(model.Thumbprint, model.Encrypted);

            InitializeProvider(model.Thumbprint);

            // decrypt
            var decryptedSections = new StringBuilder();
            foreach (var node in GetEligibleCryptoNodes(config))
            {
                XmlNode decryptedNode;
                try
                {
                    // sometimes the content may not be decryptable using the provided thumbprint
                    decryptedNode = _provider.Decrypt(node);
                }
                catch (CryptographicException ex)
                {
                    // when decryption fails with this thumbprint, display message to the user
                    return Json(new { error = ex.Message.Trim() });
                }

                // when the decrypted node already has a configProtectionProvider attribute, push it into the builder
                if (decryptedNode.Attributes != null && decryptedNode.Attributes["configProtectionProvider"] != null)
                {
                    // the decrypted node wraps the decrypted xml, so only push its inner xml
                    decryptedSections.Append(decryptedNode.InnerXml.Trim());
                }

                // otherwise, find the decryption target
                else
                {
                    var cryptoNode = FindNestedCryptoNode(decryptedNode);
                    Debug.Assert(cryptoNode.ParentNode != null);

                    // get rid of the crypto node when decrypting
                    cryptoNode.ParentNode.InnerXml = cryptoNode.InnerXml;
                    decryptedSections.Append(decryptedNode.OuterXml);
                }
            }

            // create a brand new config document after decryption is complete
            config = new ConfigXmlDocument
            {
                InnerXml = "<configuration></configuration>",
                DocumentElement = { InnerXml = decryptedSections.ToString() },
            };

            // format and return the decrypted xml
            var decrypted = config.GetFormattedXml();
            return Json(decrypted);
        }

        [HttpPost]
        public JsonResult ValidateDecryptionThumbprint(DecryptionInput model)
        {
            var propertyName = model.PropertyName(x => x.Thumbprint);
            if (ModelState.IsValidField(propertyName)) return Json(true);
            var errorMessage = ModelState[propertyName].Errors.First().ErrorMessage;
            return Json(errorMessage);
        }

        [HttpPost]
        public FileResult Save(ContentModel model)
        {
            // download the cryption content to a file
            var bytes = Encoding.UTF8.GetBytes(model.Content);
            return File(bytes, "application/octet-stream", string.Format("Web.{0}.config", model.Context));
        }

        [NonAction]
        private void ThrowWhenModelStateIsInvalid()
        {
            // all validation should happen at the client. When this exception is thrown,
            // more work needs to be done to validate everything client-side (ko.validation).
            if (!ModelState.IsValid)
                throw new InvalidOperationException("At least one validation rule is not being enforced by the client.");
        }

        [NonAction]
        private static ConfigXmlDocument CreateConfigXmlDocument(string thumbprint, string userXml)
        {
            // the configProtectedData section is used by .NET to decrypt configuration files.
            const string configProtectedDataFormat = "<configProtectedData><providers><add name=\"CustomProvider\" thumbprint=\"{0}\" " +
                    "type=\"Pkcs12ProtectedConfigurationProvider.Pkcs12ProtectedConfigurationProvider, PKCS12ProtectedConfigurationProvider, " +
                    "Version=1.0.0.0, Culture=neutral, PublicKeyToken=34da007ac91f901d\" /></providers></configProtectedData>{1}";

            // automatically remove config transform attributes that may be pasted in with the user XML.
            var cleanUserXml = StripXdtAttributes(userXml);

            // prepend the cleaned user XML with a formatted configProtectedData node.
            var configInnerXml = string.Format(configProtectedDataFormat, thumbprint, cleanUserXml);

            // create and return a new configuration xml document
            var config = new ConfigXmlDocument
            {
                InnerXml = "<configuration></configuration>",
                DocumentElement = { InnerXml = configInnerXml },
            };
            return config;
        }

        [NonAction]
        private static string StripXdtAttributes(string xml)
        {
            // strip out config transform xdt: attributes
            xml = StripXdtAttribute(xml, "Transform");
            xml = StripXdtAttribute(xml, "Locator");
            xml = StripXdtAttribute(xml, "SupressWarnings");
            return xml;
        }

        [NonAction]
        private static string StripXdtAttribute(string xml, string attribute)
        {
            // TODO: would it be more reliable to push the XML into an XmlDocument
            // and use it to strip out these attributes, rather than doing string surgery?
            var clean = xml;
            var xdt = string.Format(" xdt:{0}=", attribute);
            while (clean.Contains(xdt))
            {
                // find out where the attribute starts
                var startIndex = clean.IndexOf(xdt, StringComparison.OrdinalIgnoreCase);

                // delimiter may be either a single or double quote
                var delimiter = clean.Substring(startIndex + xdt.Length, 1);

                // the attribute value length is variable, compute it
                var length = clean.Substring(startIndex + xdt.Length + delimiter.Length)
                    .IndexOf(delimiter, StringComparison.OrdinalIgnoreCase) + 1;

                // the attribute ends at start index, plus attribute & value lengths, plus final delimiter
                var endIndex = startIndex + xdt.Length + length + delimiter.Length;

                // use a string builder to amputate the xdt: config transform attribute from the XML
                var builder = new StringBuilder(clean.Substring(0, startIndex));
                builder.Append(clean.Substring(endIndex));
                clean = builder.ToString();
            }
            return clean;
        }

        [NonAction]
        private void InitializeProvider(string thumbprint)
        {
            // the provider needs to be initialized with the certificate thumbprint
            _provider.Initialize("CustomProvider", new NameValueCollection
            {
                { "thumbprint", thumbprint },
            });
        }

        [NonAction]
        private static IEnumerable<XmlNode> GetEligibleCryptoNodes(ConfigXmlDocument config)
        {
            // ignore configProtectedData and whitespace nodes when crypting
            var configRoot = config.DocumentElement;
            Debug.Assert(configRoot != null);
            return configRoot.ChildNodes.Cast<XmlNode>()
                .Where(n => n.Name != "configProtectedData" && n.Name != "#whitespace")
                .ToArray();
        }

        [NonAction]
        private static XmlNode FindNestedCryptoNode(XmlNode node)
        {
            // search recursively for an explicit configProtectionProvider cryption target
            var cryptoNode = node.SelectSingleNode("* [@configProtectionProvider='CustomProvider']");
            if (cryptoNode != null) return cryptoNode;

            foreach (XmlNode childNode in node.ChildNodes
                .Cast<XmlNode>().Where(n => n.Name != "#whitespace"))
                return FindNestedCryptoNode(childNode);

            return null;
        }
    }
}
