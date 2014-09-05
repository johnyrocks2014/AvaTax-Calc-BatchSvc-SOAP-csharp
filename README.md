AvaTaxCalc-BatchSvcSOAP-csharp
==============================
[Other Samples](http://developer.avalara.com/api-docs/api-sample-code)

Sample of BatchSvc SOAP API in c# demonstrating functionality of BatchSave and BatchFileFetch.

Note that authentication will need to be done with an AccountAdmin level username and password, and cannot be done with
an account number/license key combination.

Also, the CompanyId required is a system-assigned unique numeric identifier for your company, and is not the same as your company code. This CompanyId can be retrieved from the URL of your company profile screen on the Admin Console. For more information, please see the full documentation on BatchSvc at our developer site: http://developer.avalara.com

To run this sample (which uses .NET 2.0), you will also need a web services library available from Microsoft:
http://www.microsoft.com/en-us/download/details.aspx?id=14089
