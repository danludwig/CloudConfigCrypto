using System.Web.Mvc;

namespace CloudConfigCrypto.Web.Models
{
    public class ContentModel
    {
        [AllowHtml]
        public string Content { get; set; }

        public string Context { get; set; }
    }
}