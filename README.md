# Cloud Configuration Cryptography (CloudConfigCrypto)

This is a little web project that you can use to encrypt and decrypt XML configuration files (web.config or app.config) based on the PKCS12ProtectedConfigurationProvider. You can read an [overview of this handy little tool here](http://archive.msdn.microsoft.com/pkcs12protectedconfg). There is also a [nuget package available for it here](http://nuget.org/packages/Pkcs12ProtectedConfigurationProvider). It looks like Microsoft took away the articles originally authored by Wayne Walter Berry, which is too bad - they were great articles.

## Why this project

I built this project mainly because I wanted to be able to decrypt some web.config sections that I encrypted over a year ago. I forgot some of the settings I used for the `mailSettings/smtp/network` section, and these were proving difficult to decrypt using command line tools. I ran a little spike to see if I could decrypt it using brute force, and that spike turned into this project.

## How to use it

Clone this repo, open the sln file in VS 2010 or 2012, and hit F5. You can test it out using a set of sample certificates provided in the project. Just follow the process described on the home page.

