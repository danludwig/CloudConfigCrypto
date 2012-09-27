using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace CloudConfigCrypto.Web.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CertificateExistsAttribute : ValidationAttribute
    {
        private object Value { get; set; }

        public StoreName StoreName { get; set; }
        public StoreLocation StoreLocation { get; set; }
        public X509FindType FindType { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Value);
        }

        protected X509Certificate2 GetCertificate(object value)
        {
            var store = new X509Store(StoreName, StoreLocation);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(FindType, value, false);
            store.Close();
            if (certificates.Count < 1) return null;
            X509Certificate2 defaultCertificate = null;
            foreach (var certificate in certificates)
            {
                if (defaultCertificate == null) defaultCertificate = certificate;
                if (certificate.HasPrivateKey) return certificate;
            }
            return defaultCertificate;
        }

        public override bool IsValid(object value)
        {
            Value = value;
            if (string.IsNullOrWhiteSpace(value as string)) return true;
            var certificate = GetCertificate(value);
            if (certificate == null) return false;

            // store find will match the beginning part of the thumbprint
            switch (FindType)
            {
                case X509FindType.FindByThumbprint:
                    if (!string.Equals(certificate.Thumbprint, value.ToString(), StringComparison.OrdinalIgnoreCase))
                        return false;
                    break;
            }

            return true;
        }
    }
}