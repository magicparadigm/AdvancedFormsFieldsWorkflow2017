using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;
using System.IO;

//using ServiceReference1;
using System.Collections;
using Newtonsoft.Json;

using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Net.Http;

public partial class demos_CompositeTemplate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccountInfo.Visible = true;
            primarySignerSection.Visible = true;
            jointSignerSection.Visible = false;
            templates.Visible = false;
            SupplementalDocConfig.Visible = false;
            button.Visible = true;
            button2.Visible = false;
            button3.Visible = false;
            uploadfile2validator.Visible = false;
            uploadfilevalidator.Visible = false;
            tabPageValidator.Visible = false;
            tabNamevalidator.Visible = false;
            xPositionValidator.Visible = false;
            yPositionValidator.Visible = false;

            UploadButton.InnerText = "Upload";
            UploadButton2.InnerText = "Upload";
            button.InnerText = "Next";
            button2.InnerText = "Next";
            button3.InnerText = "Submit";
            docusignFrame.Visible = false;
            docusignFrameIE.Visible = false;
        }

        // Add event handlers for the navigation button on each of the wizard pages 
        PrefillClick.ServerClick += new EventHandler(prefill_Click);
        button.ServerClick += new EventHandler(button_Click);
        button2.ServerClick += new EventHandler(button2_Click);
        button3.ServerClick += new EventHandler(button3_Click);
        UploadButton.ServerClick += new EventHandler(uploadButton_Click);
        UploadButton2.ServerClick += new EventHandler(uploadButton2_Click);
    }

    protected void prefill_Click(object sender, EventArgs e)
    {
        firstname.Value = "Warren";
        lastname.Value = "Buffet";
        email.Value = "magicparadigm@live.com";
        tabName.Value = "PrimarySignerSignature";
        tabPage.Value = "1";
        xPosition.Value = "175";
        yPosition.Value = "315";
    }

    protected void button_Click(object sender, EventArgs e)
    {
        if (!email.Value.Equals("") && !firstname.Value.Equals("") && !lastname.Value.Equals(""))
        {
            AccountInfo.Visible = false;
            primarySignerSection.Visible = false;
            jointSignerSection.Visible = false;
            templates.Visible = true;
            SupplementalDocConfig.Visible = false;
            button.Visible = false;
            button2.Visible = true;
            uploadfile2validator.Visible = true;
            uploadfilevalidator.Visible = true;
            tabPageValidator.Visible = true;
            tabNamevalidator.Visible = true;
            xPositionValidator.Visible = true;
            yPositionValidator.Visible = true;
        }
    }

    protected void button2_Click(object sender, EventArgs e)
    {
        if (!uploadFile.Value.Equals("") && !uploadFile2.Value.Equals("") && !tabPage.Value.Equals("") && !tabName.Value.Equals("") && !xPosition.Value.Equals("") && !yPosition.Value.Equals(""))
        {
            AccountInfo.Visible = false;
            primarySignerSection.Visible = false;
            jointSignerSection.Visible = false;
            templates.Visible = false;
            SupplementalDocConfig.Visible = true;
            button2.Visible = false;
            button3.Visible = true;
        }
    }

    protected void button3_Click(object sender, EventArgs e)
    {
        AccountInfo.Visible = false;
        primarySignerSection.Visible = false;
        jointSignerSection.Visible = false;
        templates.Visible = false;
        SupplementalDocConfig.Visible = false;
        mainForm.Visible = false;
        button3.Visible = false;
        createEnvelope();

    }


    protected void uploadButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (FileUpload1.HasFile)
            {
                String filename = Path.GetFileName(FileUpload1.FileName);
                FileUpload1.SaveAs(Server.MapPath("~/App_Data/") + filename);
                uploadFile.Value = filename;
            }
        }
        catch (Exception ex)
        {
            uploadFile.Value = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
        }
    }

    protected void uploadButton2_Click(object sender, EventArgs e)
    {
        try
        {
            if (FileUpload2.HasFile)
            {
                String filename = Path.GetFileName(FileUpload2.FileName);
                FileUpload2.SaveAs(Server.MapPath("~/App_Data/") + filename);
                uploadFile2.Value = filename;
            }
        }
        catch (Exception ex)
        {
            uploadFile2.Value = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
        }
    }


    protected String RandomizeClientUserID()
    {
        Random random = new Random();

        return (random.Next()).ToString();
    }


  

    private static void WriteStream(Stream reqStream, string str)
    {
        byte[] reqBytes = UTF8Encoding.UTF8.GetBytes(str);
        reqStream.Write(reqBytes, 0, reqBytes.Length);
    }

    private String GetSecurityHeader()
    {
        String str = "";
        if ((acctEmail.Value.Length == 0) && (password.Value.Length == 0) && (integratorKey.Value.Length == 0))
        {
            str = "<DocuSignCredentials>" + "<Username>" + ConfigurationManager.AppSettings["API.Email"] + "</Username>" +
                "<Password>" + ConfigurationManager.AppSettings["API.Password"] + "</Password>" +
                "<IntegratorKey>" + ConfigurationManager.AppSettings["API.IntegratorKey"] + "</IntegratorKey>" +
                "</DocuSignCredentials>";
        }
        else
        {
            str = "<DocuSignCredentials>" + "<Username>" + acctEmail.Value + "</Username>" +
                "<Password>" + password.Value + "</Password>" +
                "<IntegratorKey>" + integratorKey.Value + "</IntegratorKey>" +
                "</DocuSignCredentials>";
        }
        return str;
    }


    public class ServerTemplate
    {
        public string sequence { get; set; }
        public string templateId { get; set; }
    }

    public class PhoneAuthentication
    {
        public string recipMayProvideNumber { get; set; }
        public string recordVoicePrint { get; set; }
        public List<string> senderProvidedNumbers { get; set; }
        public string validateRecipProvidedNumber { get; set; }
    }

    public class SMSAuthentication
    {
        public List<string> senderProvidedNumbers { get; set; }
    }

    public class EmailNotification
    {
        public string emailBody { get; set; }
        public string emailSubject { get; set; }
        public string supportedLanguage { get; set; }
    }

    public class RecipientAttachment
    {
        public string attachmentType { get; set; }
        public string label { get; set; }
    }

    public class SocialAuthentication
    {
        public string authentication { get; set; }
    }
    
    public class SignatureInfo
    {
        public string fontStyle { get; set; }
        public string signatureInitials { get; set; }
        public string signatureName { get; set; }
    }
    public class Signer
    {
        public string accessCode { get; set;}
        public string addAccessCodeToEmail { get; set; }
        public string clientUserId { get; set; }
        public List<string> customFields { get; set; }
        public EmailNotification emailNotification { get; set; }
        public string idCheckConfigurationName {get; set;}
        public string inheritEmailNotificationConfiguration {get;set;}
        public string note { get; set; }
        public PhoneAuthentication phoneAuthentication { get; set; }
        public SMSAuthentication smsAuthentication { get; set; }
        public List<RecipientAttachment> recipientAttachments { get; set; }
        public string recipientId { get; set; }
        public string requireIdLookup { get; set; }
        public string roleName { get; set; }
        public string routingOrder { get; set; }
        public List<SocialAuthentication> socialAuthentications { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string defaultRecipient { get; set; }
        public string signInEachLocation { get; set; }
        public SignatureInfo signatureInfo { get; set; }    
    }

    public class Recipients
    {
        public List<Signer> signers { get; set; }
    }
    
    public class Document
    {
        public string documentId { get; set; }
        public string name { get; set; }
        public string transformPdfFields { get; set; }
        public string display { get; set; }
    }

    public class Documents
    {
        public List<Document> documents { get; set; }
    }

    public class InlineTemplate
    {
        public string sequence { get; set; }
        public Recipients recipients { get; set; }
//        public List<Document> documents { get; set; }
    }



    public class CompositeTemplate
    {
        public List<ServerTemplate> serverTemplates { get; set; }
        public List<InlineTemplate> inlineTemplates { get; set; }
        public Document document { get; set; }
    }

    public class CreateEnvelopeRequest
    {
        public string status { get; set; }
        public string emailBlurb { get; set; }
        public string emailSubject { get; set; }
        public List<CompositeTemplate> compositeTemplates { get; set; }
    }

    public class CreateEnvelopeResponse
    {
        public string envelopeId { get; set; }
        public string uri { get; set; }
        public string statusDateTime { get; set; }
        public string status { get; set; }
    }
    public class RecipientViewRequest
    {
        public string authenticationMethod { get; set; }
        public string email { get; set; }
        public string returnUrl { get; set; }
        public string userName { get; set; }
        public string clientUserId { get; set; }
    }

    public class RecipientViewResponse
    {
        public string url { get; set; }
    }


//                @"{
//                ""status"" : ""Sent"",
//                ""emailBlurb"":""Test Sending template in an envelope"",
//                ""emailSubject"": ""Test Sending template in an envelope"",
//                ""compositeTemplates"":[
//                {
//                    ""serverTemplates"":[
//                      {
//                        ""sequence"":""1"",
//                        ""templateId"":""280D00CB-B0F7-479A-BBBD-A09DBB05EF6F""
//                      }
//                      ],
//                    ""inlineTemplates"": [
//                    {
//                        ""sequence"": ""1"",
//                        ""recipients"": 
//                        {
//                            ""signers"": [
//                            {
//                                ""recipientID"": ""1"",
//                                ""routingOrder"": ""1"",
//                                ""email"": ""magicparadigm@live.com"",
//                                ""name"": ""Vasudevan Sampath"",
//                                ""recipientId"": ""1"",
//                                ""roleName"": ""PrimarySigner"",
//                                ""requireIdLookup"": ""true"",
//                                ""idCheckConfigurationName"":""Phone Auth $"", 
//                                ""phoneAuthentication"":{
//                                ""recipMayProvideNumber"":""false"",
//                                ""recordVoicePrint"":""false"",
//                                ""senderProvidedNumbers"":[""8175842746""],
//                                ""validateRecipProvidedNumber"":""false""
//                                 },
//                            }]
//                        }
//                    }]
//
//                }]
//            }";
    protected void createEnvelope()
    {


        // Set up the envelope
        CreateEnvelopeRequest createEnvelopeRequest = new CreateEnvelopeRequest();
        createEnvelopeRequest.emailSubject = "Composite Template Example";
        createEnvelopeRequest.status = "sent";
        createEnvelopeRequest.emailBlurb = "Example of how Composite templates functionality works";
                
        // Define a signer 
        Signer signer = new Signer();
        signer.email = "magicparadigm@live.com";
        signer.name = "Dufus McTeague";
        signer.recipientId = "1";
        signer.routingOrder = "1";
        signer.roleName = "Signer1";
        //signer.accessCode = "1234";
        signer.requireIdLookup = "true";
//        signer.idCheckConfigurationName = "Phone Auth $";
        signer.idCheckConfigurationName = "SMS Auth $";
        //signer.phoneAuthentication = new PhoneAuthentication();
        //signer.phoneAuthentication.senderProvidedNumbers = new List<string>();
        //signer.phoneAuthentication.senderProvidedNumbers.Add("8175842746");
        //signer.phoneAuthentication.recordVoicePrint = "true";

        signer.smsAuthentication = new SMSAuthentication();
        signer.smsAuthentication.senderProvidedNumbers = new List<string>();
        signer.smsAuthentication.senderProvidedNumbers.Add("8175842746");

        // Define a documetn 
        Document document = new Document();
        document.documentId = "1";
        document.name = "Sample Form";
        document.transformPdfFields = "true";
        //document.display = "modal";

        // Define an inline template
        InlineTemplate inline1 = new InlineTemplate();
        inline1.sequence = "2";
        inline1.recipients = new Recipients();
        inline1.recipients.signers = new List<Signer>();
        inline1.recipients.signers.Add(signer);

 //       inline1.documents = new List<Document>(); 
 //       inline1.documents.Add(document);

        ServerTemplate serverTemplate1 = new ServerTemplate();
        serverTemplate1.sequence = "1";
        serverTemplate1.templateId = "3abc4466-82a9-4c65-8c55-7cf4c44dea85";

        CompositeTemplate compositeTemplate1 = new CompositeTemplate();
        compositeTemplate1.inlineTemplates = new List<InlineTemplate>();
        compositeTemplate1.inlineTemplates.Add(inline1);
        compositeTemplate1.serverTemplates = new List<ServerTemplate>();
        compositeTemplate1.serverTemplates.Add(serverTemplate1);
        compositeTemplate1.document = document;
        //compositeTemplate1.document = new Document();
        //compositeTemplate1.document.documentId = "1";
        //compositeTemplate1.document.name = "Sample Form";
        //compositeTemplate1.document.transformPdfFields = "true";
        //compositeTemplate1.document.display = "modal";

        createEnvelopeRequest.compositeTemplates = new List<CompositeTemplate>();
        createEnvelopeRequest.compositeTemplates.Add(compositeTemplate1);


        string output = JsonConvert.SerializeObject(createEnvelopeRequest);

        accountId.Value = ConfigurationManager.AppSettings["API.AccountID"];

        // Specify a unique boundary string that doesn't appear in the json or document bytes.
        string Boundary = "MY_BOUNDARY";

        // Set the URI
        HttpWebRequest request = HttpWebRequest.Create(ConfigurationManager.AppSettings["DocuSignServer"] + "/restapi/v2/accounts/" + accountId.Value + "/envelopes") as HttpWebRequest;

        // Set the method
        request.Method = "POST";

        // Set the authentication header
        request.Headers["X-DocuSign-Authentication"] = GetSecurityHeader();

        // Set the overall request content type aand boundary string
        request.ContentType = "multipart/form-data; boundary=" + Boundary;
        request.Accept = "application/json";

        // Start forming the body of the request
        Stream reqStream = request.GetRequestStream();

        // write boundary marker between parts
        WriteStream(reqStream, "\n--" + Boundary + "\n");

        // write out the json envelope definition part
        WriteStream(reqStream, "Content-Type: application/json\n");
        WriteStream(reqStream, "Content-Disposition: form-data\n");
        WriteStream(reqStream, "\n"); // requires an empty line between the header and the json body
        WriteStream(reqStream, output);

        // write out the form bytes for the first form
        WriteStream(reqStream, "\n--" + Boundary + "\n");
        WriteStream(reqStream, "Content-Type: application/pdf\n");
        WriteStream(reqStream, "Content-Disposition: file; filename=\"Sample_Form\"; documentId=1\n");
        WriteStream(reqStream, "\n");
        String filename = uploadFile.Value;
        if (File.Exists(Server.MapPath("~/App_Data/" + filename)))
        {
            // Read the file contents and write them to the request stream
            byte[] buf = new byte[4096];
            int len;
            // read contents of document into the request stream
            FileStream fileStream = File.OpenRead(Server.MapPath("~/App_Data/" + filename));
            while ((len = fileStream.Read(buf, 0, 4096)) > 0)
            {
                reqStream.Write(buf, 0, len);
            }
            fileStream.Close();
        }


        // wrte the end boundary marker - ensure that it is on its own line
        WriteStream(reqStream, "\n--" + Boundary + "--");
        WriteStream(reqStream, "\n");

        try
        {
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                byte[] responseBytes = new byte[response.ContentLength];
                using (var reader = new System.IO.BinaryReader(response.GetResponseStream()))
                {
                    reader.Read(responseBytes, 0, responseBytes.Length);
                }
                string responseText = Encoding.UTF8.GetString(responseBytes);
                CreateEnvelopeResponse createEnvelopeResponse = new CreateEnvelopeResponse();

                createEnvelopeResponse = JsonConvert.DeserializeObject<CreateEnvelopeResponse>(responseText);
                if (createEnvelopeResponse.status.Equals("sent"))
                {
                }
            }
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream(), UTF8Encoding.UTF8))
                {
                    string errorMess = reader.ReadToEnd();
                    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_CompositeTemplate));
                    logger.Info("\n----------------------------------------\n");
                    logger.Error("DocuSign Error: " + errorMess);
                    logger.Error(ex.StackTrace);
                    Response.Write(ex.Message);
                }
            }
            else
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_CompositeTemplate));
                logger.Info("\n----------------------------------------\n");
                logger.Error("WebRequest Error: " + ex.Message);
                logger.Error(ex.StackTrace);
                Response.Write(ex.Message);
            }
        }

    }

    //protected void createEnvelope()
    //{


    //    // Set up the envelope
    //    CreateEnvelopeRequest createEnvelopeRequest = new CreateEnvelopeRequest();
    //    createEnvelopeRequest.emailSubject = "DOL Example";
    //    createEnvelopeRequest.status = "sent";
    //    createEnvelopeRequest.emailBlurb = "Example of how DOL functionality could work";

    //    // Define a signer 
    //    Signer signer = new Signer();
    //    signer.email = "magicparadigm@live.com";
    //    signer.name = "Dufus McTeague";
    //    signer.recipientId = "1";
    //    signer.routingOrder = "1";
    //    signer.roleName = "Signer1";

    //    // Define a documetn 
    //    Document document = new Document();
    //    document.documentId = "1";
    //    document.name = "Sample Form";
    //    document.transformPdfFields = "true";

    //    // Define an inline template
    //    InlineTemplate inline1 = new InlineTemplate();
    //    inline1.sequence = "2";
    //    inline1.recipients = new Recipients();
    //    inline1.recipients.signers = new List<Signer>();
    //    inline1.recipients.signers.Add(signer);

    //    inline1.documents = new List<Document>();
    //    inline1.documents.Add(document);

    //    ServerTemplate serverTemplate1 = new ServerTemplate();
    //    serverTemplate1.sequence = "1";
    //    serverTemplate1.templateId = "3abc4466-82a9-4c65-8c55-7cf4c44dea85";

    //    CompositeTemplate compositeTemplate1 = new CompositeTemplate();
    //    compositeTemplate1.inlineTemplates = new List<InlineTemplate>();
    //    compositeTemplate1.inlineTemplates.Add(inline1);
    //    compositeTemplate1.serverTemplates = new List<ServerTemplate>();
    //    compositeTemplate1.serverTemplates.Add(serverTemplate1);
    //    createEnvelopeRequest.compositeTemplates = new List<CompositeTemplate>();
    //    createEnvelopeRequest.compositeTemplates.Add(compositeTemplate1);


    //    string output = JsonConvert.SerializeObject(createEnvelopeRequest);

    //    accountId.Value = ConfigurationManager.AppSettings["API.AccountID"];

    //    // Specify a unique boundary string that doesn't appear in the json or document bytes.
    //    string Boundary = "MY_BOUNDARY";

    //    // Set the URI
    //    HttpWebRequest request = HttpWebRequest.Create(ConfigurationManager.AppSettings["DocuSignServer"] + "/restapi/v2/accounts/" + accountId.Value + "/envelopes") as HttpWebRequest;

    //    // Set the method
    //    request.Method = "POST";

    //    // Set the authentication header
    //    request.Headers["X-DocuSign-Authentication"] = GetSecurityHeader();

    //    // Set the overall request content type aand boundary string
    //    request.ContentType = "multipart/form-data; boundary=" + Boundary;
    //    request.Accept = "application/json";

    //    // Start forming the body of the request
    //    Stream reqStream = request.GetRequestStream();

    //    // write boundary marker between parts
    //    WriteStream(reqStream, "\n--" + Boundary + "\n");

    //    // write out the json envelope definition part
    //    WriteStream(reqStream, "Content-Type: application/json\n");
    //    WriteStream(reqStream, "Content-Disposition: form-data\n");
    //    WriteStream(reqStream, "\n"); // requires an empty line between the header and the json body
    //    WriteStream(reqStream, output);

    //    // write out the form bytes for the first form
    //    WriteStream(reqStream, "\n--" + Boundary + "\n");
    //    WriteStream(reqStream, "Content-Type: application/pdf\n");
    //    WriteStream(reqStream, "Content-Disposition: file; filename=\"Sample_Form\"; documentId=1\n");
    //    WriteStream(reqStream, "\n");
    //    String filename = uploadFile.Value;
    //    if (File.Exists(Server.MapPath("~/App_Data/" + filename)))
    //    {
    //        // Read the file contents and write them to the request stream
    //        byte[] buf = new byte[4096];
    //        int len;
    //        // read contents of document into the request stream
    //        FileStream fileStream = File.OpenRead(Server.MapPath("~/App_Data/" + filename));
    //        while ((len = fileStream.Read(buf, 0, 4096)) > 0)
    //        {
    //            reqStream.Write(buf, 0, len);
    //        }
    //        fileStream.Close();
    //    }


    //    // wrte the end boundary marker - ensure that it is on its own line
    //    WriteStream(reqStream, "\n--" + Boundary + "--");
    //    WriteStream(reqStream, "\n");

    //    try
    //    {
    //        HttpWebResponse response = request.GetResponse() as HttpWebResponse;

    //        if (response.StatusCode == HttpStatusCode.Created)
    //        {
    //            byte[] responseBytes = new byte[response.ContentLength];
    //            using (var reader = new System.IO.BinaryReader(response.GetResponseStream()))
    //            {
    //                reader.Read(responseBytes, 0, responseBytes.Length);
    //            }
    //            string responseText = Encoding.UTF8.GetString(responseBytes);
    //            CreateEnvelopeResponse createEnvelopeResponse = new CreateEnvelopeResponse();

    //            createEnvelopeResponse = JsonConvert.DeserializeObject<CreateEnvelopeResponse>(responseText);
    //            if (createEnvelopeResponse.status.Equals("sent"))
    //            {
    //            }
    //        }
    //    }
    //    catch (WebException ex)
    //    {
    //        if (ex.Status == WebExceptionStatus.ProtocolError)
    //        {
    //            HttpWebResponse response = (HttpWebResponse)ex.Response;
    //            using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream(), UTF8Encoding.UTF8))
    //            {
    //                string errorMess = reader.ReadToEnd();
    //                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_CompositeTemplate));
    //                logger.Info("\n----------------------------------------\n");
    //                logger.Error("DocuSign Error: " + errorMess);
    //                logger.Error(ex.StackTrace);
    //                Response.Write(ex.Message);
    //            }
    //        }
    //        else
    //        {
    //            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_CompositeTemplate));
    //            logger.Info("\n----------------------------------------\n");
    //            logger.Error("WebRequest Error: " + ex.Message);
    //            logger.Error(ex.StackTrace);
    //            Response.Write(ex.Message);
    //        }
    //    }

    //}
}