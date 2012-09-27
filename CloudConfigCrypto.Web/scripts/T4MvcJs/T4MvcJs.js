

var MvcJs = {
	
	CloudConfigCrypto: {
		Index: function() {
			var url = "/";

			return url.replace(/([?&]+$)/g, "");
		},
		CreateCertificates: function() {
			var url = "/CloudConfigCrypto/CreateCertificates";

			return url.replace(/([?&]+$)/g, "");
		},
		ImportCertificatesLocally: function() {
			var url = "/CloudConfigCrypto/ImportCertificatesLocally";

			return url.replace(/([?&]+$)/g, "");
		},
		Encrypt: function() {
			var url = "/CloudConfigCrypto/Encrypt";

			return url.replace(/([?&]+$)/g, "");
		},
		Encrypt1: function(model) {
			var url = "/CloudConfigCrypto/Encrypt?model={model}";
			
			if (model) {
				url = url.replace("{model}", model);
			} else {
				url = url.replace("model={model}", "").replace("?&","?").replace("&&","&");
			}

			return url.replace(/([?&]+$)/g, "");
		},
		ValidateEncryptionThumbprint: function(model) {
			var url = "/CloudConfigCrypto/ValidateEncryptionThumbprint?model={model}";
			
			if (model) {
				url = url.replace("{model}", model);
			} else {
				url = url.replace("model={model}", "").replace("?&","?").replace("&&","&");
			}

			return url.replace(/([?&]+$)/g, "");
		},
		Decrypt: function() {
			var url = "/CloudConfigCrypto/Decrypt";

			return url.replace(/([?&]+$)/g, "");
		},
		Decrypt1: function(model) {
			var url = "/CloudConfigCrypto/Decrypt?model={model}";
			
			if (model) {
				url = url.replace("{model}", model);
			} else {
				url = url.replace("model={model}", "").replace("?&","?").replace("&&","&");
			}

			return url.replace(/([?&]+$)/g, "");
		},
		ValidateDecryptionThumbprint: function(model) {
			var url = "/CloudConfigCrypto/ValidateDecryptionThumbprint?model={model}";
			
			if (model) {
				url = url.replace("{model}", model);
			} else {
				url = url.replace("model={model}", "").replace("?&","?").replace("&&","&");
			}

			return url.replace(/([?&]+$)/g, "");
		},
		Save: function(model) {
			var url = "/CloudConfigCrypto/Save?model={model}";
			
			if (model) {
				url = url.replace("{model}", model);
			} else {
				url = url.replace("model={model}", "").replace("?&","?").replace("&&","&");
			}

			return url.replace(/([?&]+$)/g, "");
		}
	},
	Shared: {

	}};






