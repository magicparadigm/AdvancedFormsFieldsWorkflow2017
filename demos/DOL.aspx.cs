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

public partial class demos_DOL : System.Web.UI.Page
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


    public class Document
    {
        public string name { get; set; }
        public string documentId { get; set; }
        public string display { get; set; }
        public Nullable<bool> includeInDownload { get; set; }
        public string signerMustAcknowledge { get; set; }
        public string fileExtension { get; set; }
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

    public class Tabs
    {
        public List<SignHereTab> signHereTabs { get; set; }
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
        public List<Signer> signers { get; set; }
    }

    public class CreateEnvelopeRequest
    {
        public string status { get; set; }
        public string emailBlurb { get; set; }
        public string emailSubject { get; set; }
        public List<Document> documents { get; set; }
        public Recipients recipients { get; set; }
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
                "<Password>" + password.Value  + "</Password>" +
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



    protected void createEnvelope()
    {

       
        // Set up the envelope
        CreateEnvelopeRequest createEnvelopeRequest = new CreateEnvelopeRequest();
        createEnvelopeRequest.emailSubject = "DOL Example";
        createEnvelopeRequest.status = "sent";
        createEnvelopeRequest.emailBlurb = "Example of how DOL functionality could work";

        // Add documents (one main document and one supplemental document to show as terms and conditions)
        createEnvelopeRequest.documents = new List<Document>();
        Document mainDocument = new Document();
        mainDocument.name = "Main Document";
        mainDocument.documentId = "1";
        mainDocument.fileExtension = "doc";
        mainDocument.includeInDownload = null;
        createEnvelopeRequest.documents.Add(mainDocument);

        Document supplementalDocument = new Document();
        supplementalDocument.name = "Terms & Conditions";
        supplementalDocument.documentId = "2";
        supplementalDocument.fileExtension = "doc";

        if (IncludeInDownloadTrue.Checked)
            supplementalDocument.includeInDownload = true;
        else if (IncludeInDownloadFalse.Checked)
            supplementalDocument.includeInDownload = false;

        // Registration type
        //if (Modal.Checked)
        //{
            supplementalDocument.display = "modal";
        //}
        //else if (Download.Checked)
        //{
        //    supplementalDocument.display = "download";    
        //}
        //else if (Inline.Checked)
        //{
        //    supplementalDocument.display = "inline";
        //}

        if (View_.Checked)
        {
            supplementalDocument.signerMustAcknowledge = "view";
        }
        else if (Accept.Checked)
        {
            supplementalDocument.signerMustAcknowledge = "accept";
        }
        else if (View_Accept.Checked)
        {
            supplementalDocument.signerMustAcknowledge = "view_accept";
        }
        else if (No_Interaction.Checked)
        {
            supplementalDocument.signerMustAcknowledge = "no_interaction";
        }

        createEnvelopeRequest.documents.Add(supplementalDocument);

        // Add recipients 
        createEnvelopeRequest.recipients = new Recipients();
        createEnvelopeRequest.recipients.signers = new List<Signer>();
        Signer signerOne = new Signer();
        signerOne.name = firstname.Value + " " + lastname.Value;
        signerOne.email = email.Value;
        signerOne.recipientId = "1";
        signerOne.roleName = "signer1";
        signerOne.routingOrder = "1";
        if (embedded.Checked)
            signerOne.clientUserId = RandomizeClientUserID();

        // Add tab for the recipient
        signerOne.tabs = new Tabs();
        signerOne.tabs.signHereTabs = new List<SignHereTab>();
        SignHereTab signHereTab = new SignHereTab();
        signHereTab.documentId = "1";
        signHereTab.pageNumber = tabPage.Value;
        signHereTab.tabId = "1";
        signHereTab.xPosition = xPosition.Value;
        signHereTab.yPosition = yPosition.Value;
        signHereTab.name = "sigTab";
        signerOne.tabs.signHereTabs.Add(signHereTab);

        createEnvelopeRequest.recipients.signers.Add(signerOne);

        string output = JsonConvert.SerializeObject(createEnvelopeRequest);

        if (accountId.Value.Length == 0)
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
        WriteStream(reqStream, "Content-Disposition: file; filename=\"Main Document\"; documentId=1\n");
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

        // write out the form bytes for the second form
        WriteStream(reqStream, "\n--" + Boundary + "\n");

        WriteStream(reqStream, "Content-Type: application/pdf\n");
        WriteStream(reqStream, "Content-Disposition: file; filename=\"Supplemental Document\"; documentId=2\n");
        WriteStream(reqStream, "\n");
        String filename2 = uploadFile2.Value;
        if (File.Exists(Server.MapPath("~/App_Data/" + filename2)))
        {
            // Read the file contents and write them to the request stream
            byte[] buf = new byte[4096];
            int len;
            // read contents of document into the request stream
            FileStream fileStream = File.OpenRead(Server.MapPath("~/App_Data/" + filename2));
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
                    if (remote.Checked)
                        Response.Redirect("ConfirmationPage.aspx");
                    else
                    {
                        // Now that we have created the envelope, get the recipient token for the first signer
                        String url = Request.Url.AbsoluteUri;
                        RecipientViewRequest recipientViewRequest = new RecipientViewRequest();
                        recipientViewRequest.authenticationMethod = "email";
                        recipientViewRequest.clientUserId = signerOne.clientUserId;
                        recipientViewRequest.email = email.Value;
                        if (!Request.Browser.IsMobileDevice)
                        {
                            recipientViewRequest.returnUrl = url.Substring(0, url.LastIndexOf("/")) + "/EmbeddedSigningComplete0.aspx?envelopeID=" + createEnvelopeResponse.envelopeId;
                        }
                        else
                        {
                            recipientViewRequest.returnUrl = url.Substring(0, url.LastIndexOf("/")) + "/ConfirmationPage.aspx?envelopeID=" + createEnvelopeResponse.envelopeId;

                        }
                        recipientViewRequest.userName = signerOne.name;

                        HttpWebRequest request2 = HttpWebRequest.Create(ConfigurationManager.AppSettings["DocuSignServer"] + "/restapi/v2/accounts/" + accountId.Value + "/envelopes/" + createEnvelopeResponse.envelopeId + "/views/recipient") as HttpWebRequest;
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
                            // If it's a non-touch aware device, show the signing session in an iFrame
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
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream(), UTF8Encoding.UTF8))
                {
                    string errorMess = reader.ReadToEnd();
                    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_DOL));
                    logger.Info("\n----------------------------------------\n");
                    logger.Error("DocuSign Error: " + errorMess);
                    logger.Error(ex.StackTrace);
                    Response.Write(ex.Message);
                }
            }
            else
            {
                log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_DOL));
                logger.Info("\n----------------------------------------\n");
                logger.Error("WebRequest Error: " + ex.Message);
                logger.Error(ex.StackTrace);
                Response.Write(ex.Message);
            }
        }

    }
   
 
}