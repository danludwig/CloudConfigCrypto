using System;

namespace CloudConfigCrypto.Web.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrivateCertificateExistsAttribute : CertificateExistsAttribute
    {
        public override bool IsValid(object value)
        {
            if (!base.IsValid(value)) return true;
            var certificate = GetCertificate(value);
            return certificate.HasPrivateKey;
        }
    }
}