using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace CloudConfigCrypto.Web.Models
{
    public class CertificateExistsAttribute : ValidationAttribute
    {
        private object _value;

        public StoreName StoreName { get; set; }
        public StoreLocation StoreLocation { get; set; }
        public X509FindType FindType { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _value);
        }

        public override bool IsValid(object value)
        {
            _value = value;
            var valueAsString = value as string;

            if (string.IsNullOrWhiteSpace(valueAsString)) return true;


            var store = new X509Store(StoreName, StoreLocation);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(FindType, value, false);

            if (certificates.Count < 1)
                return false;

            switch (FindType)
            {
                case X509FindType.FindByThumbprint:
                    if (!string.Equals(certificates[0].Thumbprint, value.ToString(), StringComparison.OrdinalIgnoreCase))
                        return false;
                    break;

                default:
                    throw new NotImplementedException(string.Format(
                        "The X509FindType '{0}' is not yet implemented for the '{1}'.",
                        FindType, GetType().Name));
            }

            store.Close();
            return true;
        }
    }
}