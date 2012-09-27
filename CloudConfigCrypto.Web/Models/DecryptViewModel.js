function DecryptViewModel() {
    var self = this;
    CryptoViewModel.call(self);

    // thumbprint
    self.thumbprintRemoteValidationUrl(MvcJs.CloudConfigCrypto.ValidateDecryptionThumbprint());

    // XML input
    self.xmlInput.rules()[0].message = 'Encrypted config section(s) is required.';
    self.exampleInputXml('  <connectionStrings configProtectionProvider="CustomProvider">\n    <EncryptedData Type="http://www.w3.org/2001/04/xmlenc#Element" xmlns="http://www.w3.org/2001/04/xmlenc#">\n      <EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#aes192-cbc" />\n      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">\n        <EncryptedKey xmlns="http://www.w3.org/2001/04/xmlenc#">\n          <EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#rsa-1_5" />\n          <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">\n            <KeyName>rsaKey</KeyName>\n          </KeyInfo>\n          <CipherData>\n            <CipherValue>aA6C8LQzgGIHLkG15uBSGNpx2nmClGbzPUWbjJEUrEljKQFn/cSn1yT1RekP4y5zckltQae155zqso6bkhthuRlFkAWvKL6wd5nvIZU4/5HnyLZupqf2jzNVm+Gu2+zKs4eEBnLs9+p1eZ/zbbK9acx850cJrSTNeX5cO4bzO08TB81XCgDg4nafKqvz/CnbPz4aWT5SP/bs3V3DwOXtIKyQYTf7PkJatLQI5PkTWVt9dY9RatDyavBN0cT/4J6aIEfGxwGb9+bBVc+SrNraNQu2rj/k1tbb0aLbgKv2xyOzCXN2uwPjx06FhPX83T1ZzN1ehZaC8QtXj5PmQiWHaw==</CipherValue>\n          </CipherData>\n        </EncryptedKey>\n      </KeyInfo>\n      <CipherData>\n        <CipherValue>LyFGDOMnGRVfF8J2WC+2KSiPddT55gVVcxvkw/AJuUa2/yluDblj97M8rJai9VZ+zKxV6U6Y9eaFzrtvRyvwVb/Fvb3ku3HGjXTOqDyZckmgbiuTNAOtDoxgI7aIZWrl8oHdUVnf8zGU+yUIe5c7bCZwAqQnkg9UFDt3YX83BsdCeSfOkJdelcrViCrAdmOnpjuK4dXjebhtWbToTSurrj0pYDt0sfCPvv79IT4uQrvrt3eybwLBqJzl3HE1HBPN6PV/s4QWgQGzZcopIXy14o7BsXgGLvUFDcfYUAb+H550kFlZZFT5bzxML17yH3Mx1Y4gvsjH3jlbFYB2xm1LP9fsYcqUTXfUq1h7Qm+yKqMYY6VtXm6OKmC8zyBCWAOL+ufxajHhrKPjc1rPdQnKoFT7DlgQTwIbGs/XpuAa7V4A11dkgZjcxjR2L5LGlYOuB6J6GXvjZ6+Bk/D9EIZjUA==</CipherValue>\n      </CipherData>\n    </EncryptedData>\n  </connectionStrings>\n  <appSettings configProtectionProvider="CustomProvider">\n    <EncryptedData Type="http://www.w3.org/2001/04/xmlenc#Element" xmlns="http://www.w3.org/2001/04/xmlenc#">\n      <EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#aes192-cbc" />\n      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">\n        <EncryptedKey xmlns="http://www.w3.org/2001/04/xmlenc#">\n          <EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#rsa-1_5" />\n          <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">\n            <KeyName>rsaKey</KeyName>\n          </KeyInfo>\n          <CipherData>\n            <CipherValue>k+NzqHqxdYCd3+d+xN5HgUgchjvnCPYIewCY5Jn5ZcE2+CY9psnbS2JOSTSSwpbdK8Yj3rz6cyUwz6s3pQerzuNUyoqfpnoFRiuHh361SUeLrkuEj2PZshk8m3859+aCV4nTzgCsscjmKbFCcJ4ebp7mKvIhBMVNYCwT0gbuyw9IlmTZHxCenyqcmoDnbMlp0ugQKgFA/u9dFHtV/ehafnX9wIDSEH2LXLjK3rrlbgOQOA+8wArl2/fc/zcdwiepbhSABIJ+TGkknKmTaI1RfG77oz8kTEyHsUXHizUCZUQZYgkGNIpJsoNNh0tDUuvrOeO7051pH83XCB2doVJxQQ==</CipherValue>\n          </CipherData>\n        </EncryptedKey>\n      </KeyInfo>\n      <CipherData>\n        <CipherValue>WWD3Hhcl76U03YgjYvouM/vaS6RAiyzvq8/6PskmPdCQa4fOmv+HuDTQ6WGUWD4cD7fpz0q4idA6c6XIXB7BGlTaWpV005zPFKJMNXNwlgCSHU0DSK/CcaVeFgmHxtMtemoCFvrfs/L5MBaseL/uk9XkDO+KSyi7bQTTO1W8o2LLwReNi6xmiADtw6MgWslDR8OJojKlyv0Nt2Ez6kaEBzxaFWcakUdc0/kooybH2wFnKoj5m+hnMzUmBqLbMJ3YG8eLwOZuAaXP27yZBc6KWeUjXiPK+QIg9MAvhQxdR3E=</CipherValue>\n      </CipherData>\n    </EncryptedData>\n  </appSettings>\n  <system.net>\n    <mailSettings>\n      <smtp configProtectionProvider="CustomProvider">\n        <EncryptedData Type="http://www.w3.org/2001/04/xmlenc#Element" xmlns="http://www.w3.org/2001/04/xmlenc#">\n          <EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#aes192-cbc" />\n          <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">\n            <EncryptedKey xmlns="http://www.w3.org/2001/04/xmlenc#">\n              <EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#rsa-1_5" />\n              <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">\n                <KeyName>rsaKey</KeyName>\n              </KeyInfo>\n              <CipherData>\n                <CipherValue>Wu+0mvl+CFUi0mZ10wwTJrp9WwKcu20NY6HKL6KR5QCCBkOdDdjzmsdnTEvJtZJgzIc9VEauZRhjQkzJ1vuNBt4bUsc43UR63FrlffbmB/ijBRbbVRD/rQvLH0k+QbmkR8Xdkp2VgIOHLVH1cehf2NXCyDKhpXsflr0Usaju0r1qYkyA6leM/oi0+loLUY56GJWuiq1a2xlqLfV2a0/pKXEZrzoGuSmDmw7h+sBJ7+ZLLLV5wp2Wv1mREpwH5Nr1wfEwsDq6D++51DF8aA+kIgK/sHLaO2qizDZS7FQ8vIUpRMP1j8N6DjEAD/azh7tMrf+5Q3R3GFeV5yF/juyB9w==</CipherValue>\n              </CipherData>\n            </EncryptedKey>\n          </KeyInfo>\n          <CipherData>\n            <CipherValue>JuKHBiPgrRkekkI05uwzwLIsp9lMkCAVXRi5GlLLtU5uSfgob0neMqJ2jAFTeUYVWs3whQXuOViO+o03JwuUZiYg0WEuV+n+LG2LYakwTbsZwqF03WOlFzAVtThSJOe/EZ680AejIB5QKNrFqkuXtrAH7RC6X5oEbrzMlIUtC0qqL/i4PH+HQ+33RVyWFS5L9CNdkU5T+oSkyb4iLyf7cfASFxNaHzAgKkOAsjlgPMM=</CipherValue>\n          </CipherData>\n        </EncryptedData>\n      </smtp>\n    </mailSettings>\n  </system.net>');

    // XML output
    self.submitText('Decrypt');
    self.submittingText('Decrypting...');
    self.cryptoEndpointUrl(MvcJs.CloudConfigCrypto.Decrypt());
    self.cryptoSuccess = function (response) {
        if (response.error) {
            alert('The config section(s) could not be decrypted.\nReason: ' + response.error +
                '\n\nAre you sure this is the thumbprint of the certificate used for encryption?');
        }
        else {
            self.xmlOutput(response);
            $('#dialog').dialog({
                height: 500,
                width: 960
            });
        }
    };
};
