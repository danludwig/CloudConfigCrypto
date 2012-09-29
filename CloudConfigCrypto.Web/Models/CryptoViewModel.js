function CryptoViewModel() {
    var self = this;

    // thumbprint
    self.thumbprintRemoteValidationUrl = ko.observable('/');
    self.isValidatingThumbprint = ko.observable(false);
    self.thumbprint = ko.observable().extend({
        required: {
            message: 'Thumbprint is required.'
        },
        validation: {
            async: true,
            validator: function (val, vm, callback) {
                $.ajax({
                    url: self.thumbprintRemoteValidationUrl(),
                    type: 'POST',
                    data: { thumbprint: val }
                })
                .success(function (response) {
                    if (response === true) callback(response);
                    else callback({ isValid: false, message: response });
                });
            },
            message: 'Your local computer certificate store does not contain a certificate with this thumbprint.',
            params: self
        }
    });
    self.thumbprint.isValidating.subscribe(function (isValidating) {
        self.isValidatingThumbprint(isValidating);
    });
    self.thumbprintEl = undefined;
    self.sampleThumbprint = '630e33c5ead42a5564e22d920a0c5ac4b10cf052'.toUpperCase();
    self.useSampleThumbprint = function () {
        self.thumbprint(self.sampleThumbprint);
        return false;
    };
    self.clearSampleThumbprint = function () {
        self.thumbprint(undefined);
        $(self.thumbprintEl).focus();
        return false;
    };

    // XML input
    self.xmlInput = ko.observable();
    self.xmlInputEl = undefined;
    self.exampleInputXml = ko.observable('<example></example>');
    self.useExampleXmlInput = function () {
        self.xmlInput(self.exampleInputXml());
        return false;
    };
    self.clearXmlInput = function () {
        self.xmlInput(undefined);
        $(self.xmlInputEl).focus();
        return false;
    };

    // XML output
    self.xmlOutput = ko.observable();
    self.xmlOutputEl = undefined;
    self.isProcessingCrypto = ko.observable(false);
    self.selectXmlOutput = function (vm, e) {
        $(self.xmlOutputEl).select();
        if (e) e.preventDefault();
        return false;
    };
    self.submitText = ko.observable('Process');
    self.submittingText = ko.observable('Processing...');
    self.submitButtonText = ko.computed(function () {
        return self.isProcessingCrypto() ? self.submittingText() : self.submitText();
    });
    self.cryptoEndpointUrl = ko.observable('/');
    self.cryptoSuccess = function () {
        alert('Override cryptoSuccess function in viewmodel');
    };
    self.showXmlOutput = function() {
        $('#dialog').dialog({
            height: 500,
            width: 960,
            modal: true,
            show: {
                effect: 'slide',
                direction: 'down'
            },
            hide: {
                effect: 'slide',
                direction: 'down'
            }
        });
    };
    self.submit = function () {
        if (!self.isValid()) {
            self.errors.showAllMessages();
        }
        else {
            self.isProcessingCrypto(true);
            $.ajax({
                url: self.cryptoEndpointUrl(),
                type: 'POST',
                data: {
                    thumbprint: self.thumbprint(),
                    xmlInput: self.xmlInput()
                }
            })
            .success(self.cryptoSuccess)
            .error(function () {
                alert('something went wrong on the server :(');
            })
            .complete(function () {
                self.isProcessingCrypto(false);
            });
        }
    };

    // download
    self.download = function (vm, e) {
        $(e.target).closest('form').submit();
        return false;
    };
}

function CryptoViewModelValidationOptions() {
    this.insertMessages = false;
    this.decorateElement = true;
    this.errorElementClass = 'input-validation-error';
    this.errorMessageClass = 'field-validation-error';
}