using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace CloudConfigCrypto.Web.Models
{
    public class EncryptConfigSectionsModel
    {
        public EncryptConfigSectionsModel()
        {
            Thumbprint = "";
            Unencrypted = "";
        }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Thumbprint is required.")]
        [CertificateExists(StoreName = StoreName.My, StoreLocation = StoreLocation.LocalMachine, FindType = X509FindType.FindByThumbprint, 
            ErrorMessage = "Your local computer certificate store does not contain a certificate with the thumbprint '{1}'.")]
        public string Thumbprint { get; set; }

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Unencrypted config section(s) is required.")]
        public string Unencrypted { get; set; }
        public string Encrypted { get; set; }
    }
}
