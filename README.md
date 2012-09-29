# Cloud Configuration Cryptography (CloudConfigCrypto)

This is a little web project that you can use to encrypt and decrypt XML configuration files (web.config or app.config) based on the PKCS12ProtectedConfigurationProvider. You can read an [overview of this handy little tool here](http://archive.msdn.microsoft.com/pkcs12protectedconfg). There is also a [nuget package available for it here](http://nuget.org/packages/Pkcs12ProtectedConfigurationProvider). It looks like Microsoft linked away from the articles originally authored by Wayne Walter Berry, which is too bad - they were great articles. If you'd like to read them, here they are:

 - [Securing Your Connection String in Windows Azure: Part 1](http://blogs.msdn.com/b/sqlazure/archive/2010/09/07/10058942.aspx)
 - [Securing Your Connection String in Windows Azure: Part 2](http://blogs.msdn.com/b/sqlazure/archive/2010/09/08/10059359.aspx)
 - [Securing Your Connection String in Windows Azure: Part 3](http://blogs.msdn.com/b/sqlazure/archive/2010/09/09/10059889.aspx)
 - [Securing Your Connection String in Windows Azure: Part 4](http://blogs.msdn.com/b/sqlazure/archive/2010/09/10/10060395.aspx)

Contrary to the article titles, you can use this tool to encrypt **many** config sections -- not just the `connectionStrings`. I have also used it to successfully encrypt `appSettings`, `dataCacheClients`, and the `system.net/mailSettings/smtp` sections when deploying to Azure.

## Why this project

I built this project mainly because I wanted to be able to decrypt some web.config sections that I encrypted over a year ago. I forgot some of the settings I used for the `dataCacheClients` and `system.net/mailSettings/smtp/network` section, and these were proving difficult to decrypt using command line tools. I ran a little spike to see if I could decrypt them by brute force, and that spike turned into this project.

## How to use it

Clone this repo, open the sln file in VS 2010 or 2012, and hit F5. You can test it out using a set of sample certificates provided in the project. Just follow the process described on the home page.

