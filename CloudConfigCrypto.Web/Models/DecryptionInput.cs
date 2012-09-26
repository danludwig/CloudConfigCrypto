using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace CloudConfigCrypto.Web.Models
{
    public class DecryptionInput
    {
        public DecryptionInput()
        {
            Thumbprint = "";
            Encrypted = "";
        }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Thumbprint is required.")]
        [CertificateExists(StoreName = StoreName.My, StoreLocation = StoreLocation.LocalMachine, FindType = X509FindType.FindByThumbprint,
            ErrorMessage = "Your local computer certificate store does not contain a certificate with thumbprint '{1}'.")]
        public string Thumbprint { get; set; }

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Encrypted config section(s) is required.")]
        public string Encrypted { get; set; }
    }
}
