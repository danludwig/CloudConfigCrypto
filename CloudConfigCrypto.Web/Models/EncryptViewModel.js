function EncryptViewModel() {
    var self = this;
    CryptoViewModel.call(self);

    // thumbprint
    self.thumbprintRemoteValidationUrl(MvcJs.CloudConfigCrypto.ValidateEncryptionThumbprint());

    // XML input
    self.xmlInput.rules()[0].message = 'Encrypted config section(s) is required.';
    self.exampleInputXml('<connectionStrings>\n  <add name="MyDatabase" connectionString="Server=tcp:mydbserver.database.windows.net,1433;Database=MyDatabaseName;User ID=MyDatabaseUser@mydbserver;Password=its a secret;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=true;" providerName="System.Data.SqlClient" />\n</connectionStrings>\n<appSettings>\n  <add key=\"RecaptchaPrivateKey\" value=\"8UxmrfPQHHHHHVsEPrVwEgOJCIwQpdVWZAlw9plm\" />\n  <add key=\"GeoPlanetAppId\" value=\"XjFhslJ7dKLdfKFdK_lksdfkL9sdL9fdKfdjad-\" />\n</appSettings>\n<system.net>\n  <mailSettings>\n    <smtp configProtectionProvider="CustomProvider">\n      <network host="smtp.gmail.com" password="its a secret" port="587" userName="google.apps.user@mydomain.tld" enableSsl="true" />\n    </smtp>\n  </mailSettings>\n</system.net>');

    // XML output
    self.submitText('Encrypt');
    self.submittingText('Encrypting...');
    self.cryptoEndpointUrl(MvcJs.CloudConfigCrypto.Encrypt());
    self.cryptoSuccess = function (response) {
        self.xmlOutput(response);
        self.showXmlOutput();
    };
};
