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

public partial class demos_eNotary : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            primarySignerSection.Visible = true;
            notarySignerSection.Visible = true;
            button.Visible = true;
            button.InnerText = "Submit";
            docusignFrame.Visible = false;
            docusignFrameIE.Visible = false;
        }

        // Add event handlers for the navigation button on each of the wizard pages 
        PrefillClick.ServerClick += new EventHandler(prefill_Click);
        button.ServerClick += new EventHandler(button_Click);
    }

    protected void prefill_Click(object sender, EventArgs e)
    {
        firstname.Value = "Warren";
        lastname.Value = "Buffet";
        email.Value = "magicparadigm@live.com";
        notaryEmail.Value = "notary17@mailinator.com";
        notaryFirstname.Value = "Sally";
        notaryLastname.Value = "Notary";
    }

    protected void button_Click(object sender, EventArgs e)
    {
        if (!email.Value.Equals("") && !firstname.Value.Equals("") && !lastname.Value.Equals(""))
        {
            AccountInfo.Visible = false;
            primarySignerSection.Visible = false;
            notarySignerSection.Visible = false;
            button.Visible = false;
            mainForm.Visible = false;
            createEnvelope();
        }
    }


    protected void button3_Click(object sender, EventArgs e)
    {
        AccountInfo.Visible = false;
        primarySignerSection.Visible = false;
        notarySignerSection.Visible = false;

    }



    protected String RandomizeClientUserID()
    {
        Random random = new Random();

        return (random.Next()).ToString();
    }

    

    public class NotaryHost
    {
        public int recipientId { get; set; }
        public string email { get; set; }
        public string name { get; set; }
    }

    public class InPersonSigner
    {
        public Tabs tabs { get; set; }
        public string recipientId { get; set; }
        public string routingOrder { get; set; }
        public string hostEmail { get; set; }
        public string hostName { get; set; }
        public string inPersonSigningType { get; set; }
        public NotaryHost notaryHost { get; set; }
    }

    public class InlineTemplate
    {
        public string sequence { get; set; }
        public Recipients recipients { get; set; }
    }

    public class DocumentField
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Document
    {
        public string documentId { get; set; }
        public string name { get; set; }
        public List<DocumentField> documentFields { get; set; }
        public string transformPdfFields { get; set; }
        public string documentBase64 { get; set; }
    }

    public class CompositeTemplate
    {
        public List<ServerTemplate> serverTemplates { get; set; }
        public List<InlineTemplate> inlineTemplates { get; set; }
        public Document document { get; set; }
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

    public class FullNameTab
    {
        public string tabId { get; set; }
        public string name { get; set; }
        public string pageNumber { get; set; }
        public string documentId { get; set; }
        public string yPosition { get; set; }
        public string xPosition { get; set; }

    }

    public class Tabs
    {
        public List<SignHereTab> signHereTabs { get; set; }
        public List<DateSignedTab> dateSignedTabs { get; set; }
        public List<FullNameTab> fullNameTabs { get; set; }
    }

    public class Signer
    {
        public Tabs tabs { get; set; }
        public string routingOrder { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string recipientId { get; set; }
        public string roleName { get; set; }
        public string clientUserId { get; set; }
    }

    public class Recipients
    {
        public Tabs tabs { get; set; }

        public List<InPersonSigner> inPersonSigners { get; set; }

        public List<Signer> signers { get; set; }
    }

    public class CreateEnvelopeRequest
    {
            public string authoritativeCopy { get; set; }
            public string status { get; set; }
            public string emailSubject { get; set; }
            public string emailBlurb { get; set; }
            public List<CompositeTemplate> compositeTemplates { get; set; }
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
            str = "<DocuSignCredentials>" + "<Username>" + ConfigurationManager.AppSettings["API.NotaryEmail"] + "</Username>" +
                "<Password>" + ConfigurationManager.AppSettings["API.NotaryPassword"] + "</Password>" +
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

    public class ServerTemplate
    {
        public string sequence { get; set; }
        public string templateId { get; set; }
    }



    protected void createEnvelope()
    {


        // Set up the envelope
        CreateEnvelopeRequest createEnvelopeRequest = new CreateEnvelopeRequest();
        createEnvelopeRequest.emailSubject = "eNotary Example";
        createEnvelopeRequest.status = "sent";

        //Define inperson signers 
        InPersonSigner inPersonSigner = new InPersonSigner();
        //       inPersonSigner.hostEmail = "docusign_notary1+demo@outlook.com";
        inPersonSigner.hostEmail = email.Value;
        inPersonSigner.hostName = firstname.Value + " " + lastname.Value;
        inPersonSigner.recipientId = "3";
        inPersonSigner.routingOrder = "3";
        inPersonSigner.inPersonSigningType = "notary";

        NotaryHost notaryHost = new NotaryHost();
        notaryHost.recipientId = 4;
        notaryHost.email = notaryEmail.Value;
        notaryHost.name = notaryFirstname.Value + " " + notaryLastname.Value;

        // Add tab for the recipient
        inPersonSigner.tabs = new Tabs();
        inPersonSigner.tabs.signHereTabs = new List<SignHereTab>();
        SignHereTab signHereTab = new SignHereTab();
        signHereTab.documentId = "1";
        signHereTab.pageNumber = "1";
        signHereTab.tabId = "1";
        signHereTab.xPosition = "110";
        signHereTab.yPosition = "210";
        signHereTab.name = "sigTab";
        inPersonSigner.tabs.signHereTabs.Add(signHereTab);

        inPersonSigner.tabs.dateSignedTabs = new List<DateSignedTab>();
        DateSignedTab dateSignedTab = new DateSignedTab();
        dateSignedTab.documentId = "1";
        dateSignedTab.pageNumber = "1";
        dateSignedTab.tabId = "2";
        dateSignedTab.xPosition = "425";
        dateSignedTab.yPosition = "260";
        dateSignedTab.name = "dateSignedTab";
        inPersonSigner.tabs.dateSignedTabs.Add(dateSignedTab);

        inPersonSigner.tabs.fullNameTabs = new List<FullNameTab>();
        FullNameTab fullNameTab = new FullNameTab();
        fullNameTab.documentId = "1";
        fullNameTab.pageNumber = "1";
        fullNameTab.tabId = "3";
        fullNameTab.xPosition = "90";
        fullNameTab.yPosition = "150";
        fullNameTab.name = "fullNameTab";
        inPersonSigner.tabs.fullNameTabs.Add(fullNameTab);

        inPersonSigner.notaryHost = notaryHost;


        // Define a document 
        Document document = new Document();
        document.documentId = "1";
        document.name = "Sample Form";
        document.transformPdfFields = "true";
        //document.display = "modal";

        // Define an inline template
        InlineTemplate inline1 = new InlineTemplate();
        inline1.sequence = "2";
        inline1.recipients = new Recipients();
        //        inline1.recipients.signers = new List<Signer>();
        //        inline1.recipients.signers.Add(signer);
        inline1.recipients.inPersonSigners = new List<InPersonSigner>();
        inline1.recipients.inPersonSigners.Add(inPersonSigner);
        
        //       inline1.documents = new List<Document>(); 
        //       inline1.documents.Add(document);

        ServerTemplate serverTemplate1 = new ServerTemplate();
        serverTemplate1.sequence = "1";
        serverTemplate1.templateId = "d7a2c6d9-3b20-4086-a408-ada85efc00fd";

        CompositeTemplate compositeTemplate1 = new CompositeTemplate();
        compositeTemplate1.inlineTemplates = new List<InlineTemplate>();
        compositeTemplate1.inlineTemplates.Add(inline1);
        //        compositeTemplate1.serverTemplates = new List<ServerTemplate>();
        //        compositeTemplate1.serverTemplates.Add(serverTemplate1);
        compositeTemplate1.document = document;
        compositeTemplate1.document = new Document();
        compositeTemplate1.document.documentId = "1";
        compositeTemplate1.document.name = "Affidavit";
        compositeTemplate1.document.transformPdfFields = "true";
        //compositeTemplate1.document.display = "modal";

        createEnvelopeRequest.compositeTemplates = new List<CompositeTemplate>();
        createEnvelopeRequest.compositeTemplates.Add(compositeTemplate1);


        string output = JsonConvert.SerializeObject(createEnvelopeRequest);

        accountId.Value = ConfigurationManager.AppSettings["API.NotaryAccountID"];

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
        String filename = "Docusign_Affidavit.pdf";
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
                    Response.Redirect("ConfirmationPage.aspx");
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
                    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_eNotary));
                    logger.Info("\n----------------------------------------\n");
                    logger.Error("DocuSign Error: " + errorMess);
                    logger.Error(ex.StackTrace);
                    Response.Write(ex.Message);
                }
            }
            else
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_eNotary));
                logger.Info("\n----------------------------------------\n");
                logger.Error("WebRequest Error: " + ex.Message);
                logger.Error(ex.StackTrace);
                Response.Write(ex.Message);
            }
        }

    }

}