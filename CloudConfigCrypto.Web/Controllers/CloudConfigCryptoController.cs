using System;
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
        public ActionResult Encrypt()
        {
            return View();
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

        [HttpPost]
        public FileResult Save(ContentModel model)
        {
            var bytes = Encoding.UTF8.GetBytes(model.Content);
            return File(bytes, "application/octet-stream", "Web.Encrypted.config");
        }
    }
}
