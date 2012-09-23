using System.ComponentModel.DataAnnotations;
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
        public string Thumbprint { get; set; }

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Unencrypted config section(s) is required.")]
        public string Unencrypted { get; set; }
        public string Encrypted { get; set; }
    }
}
