﻿using System;
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


public partial class demos_DynamicFieldsREST : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            primarySignerSection.Visible = true;
            jointSignerSection.Visible = false;

            button.Visible = true;
            uploadButton.InnerText = "Upload";
            button.InnerText = "Submit";
            docusignFrame.Visible = false;
            docusignFrameIE.Visible = false;
        }

        // Add event handlers for the navigation button on each of the wizard pages 
        PrefillClick.ServerClick += new EventHandler(prefill_Click);
        button.ServerClick += new EventHandler(button_Click);
        uploadButton.ServerClick += new EventHandler(uploadButton_Click);
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

        tabName2.Value = "DateSigned";
        tabPage2.Value = "1";
        xPosition2.Value = "175";
        yPosition2.Value = "390";
    }

    protected void button_Click(object sender, EventArgs e)
    {
        primarySignerSection.Visible = false;
        jointSignerSection.Visible = false;
        mainForm.Visible = false;
        button.Visible = false;
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

    protected String RandomizeClientUserID()
    {
        Random random = new Random();

        return (random.Next()).ToString();
    }

    public class TextTab
    {
        public string tabLabel { get; set; }
        public string value { get; set; }
    }

    public class Tabs
    {
        public List<TextTab> textTabs { get; set; }
        public List<SignHereTab> signHereTabs { get; set; }
        public List<DateSignedTab> dateSignedTabs { get; set; }
    }

    public class Signer
    {

        public string email { get; set; }
        public string name { get; set; }
        public int recipientId { get; set; }
        public string roleName { get; set; }
        public Tabs tabs { get; set; }
        public string routingOrder { get; set; }
        public string clientUserId { get; set; }


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

    }

    public class InlineTemplate
    {
        public string sequence { get; set; }
        public Recipients recipients { get; set; }
        public List<Document> documents { get; set; }
    }

    public class ServerTemplate
    {
        public string sequence { get; set; }
        public string templateId { get; set; }
    }

    public class CompositeTemplate
    {
        public string compositeTemplateId { get; set; }
        public List<InlineTemplate> inlineTemplates { get; set; }
        public List<ServerTemplate> serverTemplates { get; set; }
        public Document document { get; set; }
    }

    public class CreateEnvelopeRequest
    {
        public string status { get; set; }
        public string emailSubject { get; set; }
        public string emailBlurb { get; set; }
        public List<CompositeTemplate> compositeTemplates { get; set; }
    }
    public class CreateEnvelopeResponse
    {
        public string envelopeId { get; set; }
        public string uri { get; set; }
        public string statusDateTime { get; set; }
        public string status { get; set; }
    }

    public class SignHereTab
    {
        public string tabId { get; set; }
        public string name { get; set; }
        public string pageNumber { get; set; }
        public string documentId { get; set; }
        public string yPosition { get; set; }
        public string xPosition { get; set; }
    }

    public class DateSignedTab
    {
        public string tabId { get; set; }
        public string name { get; set; }
        public string pageNumber { get; set; }
        public string documentId { get; set; }
        public string yPosition { get; set; }
        public string xPosition { get; set; }

    }


    private static void WriteStream(Stream reqStream, string str)
    {
        byte[] reqBytes = UTF8Encoding.UTF8.GetBytes(str);
        reqStream.Write(reqBytes, 0, reqBytes.Length);
    }


    private String GetSecurityHeader()
    {
        String str = "";
        str = "<DocuSignCredentials>" + "<Username>" + ConfigurationManager.AppSettings["API.Email"] + "</Username>" +
            "<Password>" + ConfigurationManager.AppSettings["API.Password"] + "</Password>" +
            "<IntegratorKey>" + ConfigurationManager.AppSettings["API.IntegratorKey"] + "</IntegratorKey>" +
            "</DocuSignCredentials>";
        return str;
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

    protected void createEnvelope()
    {


        // Set up the envelope
        CreateEnvelopeRequest createEnvelopeRequest = new CreateEnvelopeRequest();
        createEnvelopeRequest.emailSubject = "Template Example";
        createEnvelopeRequest.status = "sent";
        createEnvelopeRequest.emailBlurb = "Example of how template functionality works";

        // Define first signer 
        Signer signer = new Signer();
        signer.email = email.Value;
        signer.name = firstname.Value + " " + lastname.Value;
        signer.recipientId = 1;
        signer.routingOrder = "1";
        signer.roleName = "Signer1";
        signer.clientUserId = RandomizeClientUserID();  // First signer is embedded 

        // Add tabs for the signer
        signer.tabs = new Tabs();
        signer.tabs.signHereTabs = new List<SignHereTab>();
        SignHereTab signHereTab = new SignHereTab();
        signHereTab.documentId = "1";
        signHereTab.pageNumber = tabPage.Value;
        signHereTab.tabId = "1";
        signHereTab.xPosition = xPosition.Value;
        signHereTab.yPosition = yPosition.Value;
        signHereTab.name = tabName.Value;
        signer.tabs.signHereTabs.Add(signHereTab);

        signer.tabs.dateSignedTabs = new List<DateSignedTab>();
        DateSignedTab dateSignedTab = new DateSignedTab();
        dateSignedTab.documentId = "1";
        dateSignedTab.pageNumber = tabPage.Value;
        dateSignedTab.tabId = "2";
        dateSignedTab.xPosition = xPosition2.Value;
        dateSignedTab.yPosition = yPosition2.Value;
        dateSignedTab.name = tabName2.Value;
        signer.tabs.dateSignedTabs.Add(dateSignedTab);

        // Define a document 
        Document document = new Document();
        document.documentId = "1";
        document.name = "Sample Form";
        document.transformPdfFields = "true";

        // Define an inline template
        InlineTemplate inline1 = new InlineTemplate();
        inline1.sequence = "2";
        inline1.recipients = new Recipients();
        inline1.recipients.signers = new List<Signer>();
        inline1.recipients.signers.Add(signer);


        // Add the inline template to a CompositeTemplate 
        CompositeTemplate compositeTemplate1 = new CompositeTemplate();
        compositeTemplate1.inlineTemplates = new List<InlineTemplate>();
        compositeTemplate1.inlineTemplates.Add(inline1);
        compositeTemplate1.document = document;

        // Add compositeTemplate to the envelope 
        createEnvelopeRequest.compositeTemplates = new List<CompositeTemplate>();
        createEnvelopeRequest.compositeTemplates.Add(compositeTemplate1);


        string output = JsonConvert.SerializeObject(createEnvelopeRequest);

        // Specify a unique boundary string that doesn't appear in the json or document bytes.
        string Boundary = "MY_BOUNDARY";

        // Set the URI
        HttpWebRequest request = HttpWebRequest.Create(ConfigurationManager.AppSettings["DocuSignServer"] + "/restapi/v2/accounts/" + ConfigurationManager.AppSettings["API.TemplatesAccountID"] + "/envelopes") as HttpWebRequest;

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
                    // Now that we have created the envelope, get the recipient token for the first signer
                    String url = Request.Url.AbsoluteUri;
                    RecipientViewRequest recipientViewRequest = new RecipientViewRequest();
                    recipientViewRequest.authenticationMethod = "email";
                    recipientViewRequest.clientUserId = signer.clientUserId;
                    recipientViewRequest.email = email.Value;
                    if (!Request.Browser.IsMobileDevice)
                    {
                        recipientViewRequest.returnUrl = url.Substring(0, url.LastIndexOf("/")) + "/EmbeddedSigningComplete0.aspx?envelopeID=" + createEnvelopeResponse.envelopeId;
                    }
                    else
                    {
                        recipientViewRequest.returnUrl = url.Substring(0, url.LastIndexOf("/")) + "/ConfirmationPage.aspx?envelopeID=" + createEnvelopeResponse.envelopeId;

                    }
                    recipientViewRequest.userName = firstname.Value + " " + lastname.Value;

                    HttpWebRequest request2 = HttpWebRequest.Create(ConfigurationManager.AppSettings["DocuSignServer"] + "/restapi/v2/accounts/" + ConfigurationManager.AppSettings["API.TemplatesAccountID"] + "/envelopes/" + createEnvelopeResponse.envelopeId + "/views/recipient") as HttpWebRequest;
                    request2.Method = "POST";

                    // Set the authenticationheader
                    request2.Headers["X-DocuSign-Authentication"] = GetSecurityHeader();

                    request2.Accept = "application/json";
                    request2.ContentType = "application/json";

                    Stream reqStream2 = request2.GetRequestStream();

                    WriteStream(reqStream2, JsonConvert.SerializeObject(recipientViewRequest));
                    HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse;

                    responseBytes = new byte[response2.ContentLength];
                    using (var reader = new System.IO.BinaryReader(response2.GetResponseStream()))
                    {
                        reader.Read(responseBytes, 0, responseBytes.Length);
                    }
                    string response2Text = Encoding.UTF8.GetString(responseBytes);

                    RecipientViewResponse recipientViewResponse = new RecipientViewResponse();
                    recipientViewResponse = JsonConvert.DeserializeObject<RecipientViewResponse>(response2Text);
                    Session.Add("envelopeID", createEnvelopeResponse.envelopeId);

                    // If it's a non-touch aware device, show the signing session in an iFrame
                    if (!Request.Browser.IsMobileDevice)
                    {
                        if (!Request.Browser.Browser.Equals("InternetExplorer") && (!Request.Browser.Browser.Equals("Safari")))
                        {
                            docusignFrame.Visible = true;
                            docusignFrame.Src = recipientViewResponse.url;
                        }
                        else // Handle IE differently since it does not allow dynamic setting of the iFrame width and height
                        {
                            docusignFrameIE.Visible = true;
                            docusignFrameIE.Src = recipientViewResponse.url;
                        }
                    }
                    // For touch aware devices, show the signing session in main browser window
                    else
                    {
                        Response.Redirect(recipientViewResponse.url);
                    }

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
                    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_DynamicFieldsREST));
                    logger.Info("\n----------------------------------------\n");
                    logger.Error("DocuSign Error: " + errorMess);
                    logger.Error(ex.StackTrace);
                    Response.Write(ex.Message);
                }
            }
            else
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_DynamicFieldsREST));
                logger.Info("\n----------------------------------------\n");
                logger.Error("WebRequest Error: " + ex.Message);
                logger.Error(ex.StackTrace);
                Response.Write(ex.Message);
            }
        }

    }
}