using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Avalara.AvaTax.Services.Proxies.BatchSvcProxy;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.Web.Services3.Addressing;
using System.IO;
using System.Web;

namespace BatchSvcTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string m_UserName = ""; //For BatchSvc, you must authenticate with username/password.
            string m_PassWord = ""; //That credential set must have Admin-level access.
            string m_Url = "https://development.avalara.net"; //Service URL
            int m_CompanyID = 145466; //This should be your CompanyID (not company code!)

            BatchSvc svc = new BatchSvc(m_Url, m_UserName, m_PassWord); //Create the service object
            AuditMessage msg = new AuditMessage();
            msg.Message = "Testing of imports/batchsvc"; //An audit message is required, but will only be visible on internal Avalara server audits.
            svc.AuditMessageValue = msg;

            loadBatch(svc, m_CompanyID);

            Console.ReadLine();
        }
        static void loadBatch(BatchSvc svc, int m_CompanyID)
        {
            Batch oBatch = new Batch();
            oBatch.CompanyId = m_CompanyID;

            oBatch.BatchTypeId = "TransactionImport"; //This is required, will tell our service what you're importing.
            BatchFile file = new BatchFile();
            oBatch.Name = "test.csv";
            file.Name = oBatch.Name; //You need to set a Batch.Name and a Batch.BatchFile.Name, but they can be the same.
            string path = "C:\\Analysis\\test\\test1.csv"; //This is your filepath for the file you want to load
            file.ContentType = "application/CSV"; //You can only load CSV, XLS, or XLSX files.
            file.Ext = ".csv";
            file.FilePath = path;

            //Here I'm just loading in the data from the csv file to the BatchFile
            try
            {
                    FileStream oStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    BinaryReader oBinreader = new BinaryReader(oStream);
                    file.Size = (Int32)oStream.Length;
                    file.Content = oBinreader.ReadBytes(file.Size);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            oBatch.Files = new BatchFile[1];
            oBatch.Files[0] = file; //It looks like you can load an array of files, but you can really only save the batch with one file in the array.
            BatchSaveResult sres = new BatchSaveResult();
            sres = svc.BatchSave(oBatch); //Calling the service with the batch

            Console.WriteLine(sres.ResultCode.ToString());


            //fetching the batch we just loaded
            FetchRequest ofetchRequest = new FetchRequest();
            ofetchRequest.Fields = "Files";
            ofetchRequest.Filters = "BatchId = " + sres.BatchId.ToString();


            BatchFetchResult result = svc.BatchFetch(ofetchRequest); //this BatchFetch is to find the BatchFileId values

            ofetchRequest.Fields = "*,Content"; //since the BatchFileFetch also uses a fetchRequest, I recycled my variable.
            ofetchRequest.Filters = "BatchFileId = " + result.Batches[0].Files[1].BatchFileId.ToString(); //I just picked one of files from the result here. 
            //There can be up to three files returned: input, result, and error.

            BatchFileFetchResult fetchresult = svc.BatchFileFetch(ofetchRequest);
            Console.WriteLine(Encoding.ASCII.GetString(fetchresult.BatchFiles[0].Content));//The file content is returned as a byte array, and I just wrote it to a console window to confirm that it looked OK.
        }


        
        class BatchSvc : BatchSvcWse
        {
            public BatchSvc(string Url, string userName, string passWord)
            {

                this.Destination = Util.GetEndpoint(Url, "Account/AccountSvc.asmx");

                // Setup WS-Security authentication
                UsernameToken userToken = new UsernameToken(userName, passWord, PasswordOption.SendPlainText);
                SoapContext requestContext = this.RequestSoapContext;
                requestContext.Security.Tokens.Add(userToken);
                requestContext.Security.Timestamp.TtlInSeconds = 300;


                Avalara.AvaTax.Services.Proxies.BatchSvcProxy.Profile profile = new Avalara.AvaTax.Services.Proxies.BatchSvcProxy.Profile();
                profile.Client = "QATestClient"; // _config.Client;
                profile.Adapter = "";
                profile.Name = "";
                profile.Machine = System.Environment.MachineName;



                this.ProfileValue = profile;
            }
        }

        class Util
        {
            public static EndpointReference GetEndpoint(string Url, string path)
            {
                EndpointReference endpoint = null;
                string url = Url;
                string viaurl = Url;

                if (url.Trim().StartsWith("https://"))
                {
                    url = url.Trim().Replace("https://", "http://");
                }

                if (!url.EndsWith("/"))
                {

                    url += "/";

                }

                if (!viaurl.EndsWith("/"))
                {

                    viaurl += "/";

                }

                endpoint = new EndpointReference(new Uri(url + path), new Uri(viaurl + path));

                return endpoint;

            }
        }
    }
}
