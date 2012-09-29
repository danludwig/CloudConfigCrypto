using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CloudConfigCrypto.Web.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrivateCertificateExistsAttribute : CertificateExistsAttribute
    {
        public override bool IsValid(object value)
        {
            if (!base.IsValid(value)) return true;
            var certificate = GetCertificate(value);
            if (!certificate.HasPrivateKey) return false;
            try
            {
                var test = certificate.PrivateKey;
                Debug.Assert(test != null);
            }
            catch (CryptographicException)
            {
                return false;
            }
            return true;
        }
    }
}