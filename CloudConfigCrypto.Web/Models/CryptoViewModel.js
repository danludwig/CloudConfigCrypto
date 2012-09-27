function CryptoViewModel() {
    var self = this;

    // thumbprint
    self.thumbprintRemoteValidationUrl = '/';

    self.thumbprint = ko.observable().extend({
        required: {
            message: 'Thumbprint is required.'
        },
        validation: {
            validator: function (val) {
                var isValid = false,
                    validation = this;
                $.ajax({
                    url: self.thumbprintRemoteValidationUrl,
                    type: 'POST',
                    data: { thumbprint: val },
                    async: false
                })
                .success(function (response) {
                    if (response === true) isValid = true;
                    else validation.message = response;
                });
                return isValid;
            },
            message: 'Your local computer certificate store does not contain a certificate with this thumbprint.',
            params: self
        }
    });
    self.thumbprintEl = undefined;
    self.sampleThumbprint = '630e33c5ead42a5564e22d920a0c5ac4b10cf052'.toUpperCase();
    self.useSampleThumbprint = function () {
        self.thumbprint(self.sampleThumbprint);
        return false;
    };
    self.useCustomThumbprint = function () {
        self.thumbprint(undefined);
        $(self.thumbprintEl).focus();
        return false;
    };
}