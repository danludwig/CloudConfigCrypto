using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace CloudConfigCrypto.Web.Models
{
    public class DecryptRequestModel
    {
        public DecryptRequestModel()
        {
            Thumbprint = "";
            XmlInput = "";
        }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Thumbprint is required.")]
        [CertificateExists(StoreName = StoreName.My, StoreLocation = StoreLocation.LocalMachine, FindType = X509FindType.FindByThumbprint,
            ErrorMessage = "Your local computer certificate store does not contain a certificate with thumbprint '{1}'.")]
        [PrivateCertificateExists(StoreName = StoreName.My, StoreLocation = StoreLocation.LocalMachine, FindType = X509FindType.FindByThumbprint,
            ErrorMessage = "Your local computer certificate with thumbprint '{1}' does not have a private key.")]
        public string Thumbprint { get; set; }

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Encrypted config section(s) is required.")]
        public string XmlInput { get; set; }
    }
}
