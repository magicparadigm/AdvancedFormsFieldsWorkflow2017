using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;


using ServiceReference1;
using System.Collections;

public partial class demos_EnvelopeCustom___Document_Fields : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            primarySignerSection.Visible = true;
            jointSignerSection.Visible = false;

            button.Visible = true;
            AddEnvelopeCustomFieldButton.InnerText = "Add";
            UploadButton.InnerText = "Upload";
            button.InnerText = "Submit";
            AddDocumentFieldButton.InnerText = "Add";
            docusignFrame.Visible = false;
            docusignFrameIE.Visible = false;
        }

        // Add event handlers for the navigation button on each of the wizard pages 
        PrefillClick.ServerClick += new EventHandler(prefill_Click);
        button.ServerClick += new EventHandler(button_Click);
        AddEnvelopeCustomFieldButton.ServerClick += new EventHandler(AddEnvelopeCustomFieldButton_Click);
        UploadButton.ServerClick += new EventHandler(uploadButton_Click);
        AddDocumentFieldButton.ServerClick += new EventHandler(AddDocumentFieldButton_Click);
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

    protected void AddEnvelopeCustomFieldButton_Click(object sender, EventArgs e)
    {
        envelopeCustomFieldsList.Items.Add(new ListItem(EnvelopeCustomFieldName.Value, EnvelopeCustomFieldValue.Value));
        EnvelopeCustomFieldName.Value = "";
        EnvelopeCustomFieldValue.Value = "";
    }

    protected void AddDocumentFieldButton_Click(object sender, EventArgs e)
    {
        documentFieldsList.Items.Add(new ListItem(DocumentFieldName.Value, DocumentFieldValue.Value));
        DocumentFieldName.Value = "";
        DocumentFieldValue.Value = "";
    }

    protected String RandomizeClientUserID()
    {
        Random random = new Random();

        return (random.Next()).ToString();
    }

    protected void createEnvelope()
    {
        FileStream fs = null;

        try
        {
            String userName = ConfigurationManager.AppSettings["API.Email"];
            String password = ConfigurationManager.AppSettings["API.Password"];
            String integratorKey = ConfigurationManager.AppSettings["API.IntegratorKey"];


            String auth = "<DocuSignCredentials><Username>" + userName
                + "</Username><Password>" + password
                + "</Password><IntegratorKey>" + integratorKey
                + "</IntegratorKey></DocuSignCredentials>";
            ServiceReference1.DSAPIServiceSoapClient client = new ServiceReference1.DSAPIServiceSoapClient();

            using (OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers.Add("X-DocuSign-Authentication", auth);
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                CompositeTemplate template = new CompositeTemplate();

                // Set up the envelope
                EnvelopeInformation envInfo = new EnvelopeInformation();
                envInfo.AutoNavigation = true;
                envInfo.AccountId = ConfigurationManager.AppSettings["API.AccountId"];
                envInfo.Subject = "Envelope Custom & Document Fields Example";
                
                // Set up envelope custom fields 
                envInfo.CustomFields = new CustomField[envelopeCustomFieldsList.Items.Count];
                for (Int16 i = 0; i < envelopeCustomFieldsList.Items.Count; i++)
                {
                    envInfo.CustomFields[i] = new CustomField();
                    envInfo.CustomFields[i].Name = envelopeCustomFieldsList.Items[i].Text;
                    envInfo.CustomFields[i].Value = envelopeCustomFieldsList.Items[i].Value;
                }

                // Set up recipients 
                Recipient[] recipients;
                if (jointEmail.Value.Trim().Equals(""))
                {
                    recipients = new Recipient[1];
                }
                else
                {
                    recipients = new Recipient[2];
                }

                recipients[0] = new Recipient();
                recipients[0].ID = "1";
                recipients[0].Email = email.Value;
                recipients[0].Type = RecipientTypeCode.Signer;
                recipients[0].UserName = firstname.Value + " " + lastname.Value;
                recipients[0].CaptiveInfo = new RecipientCaptiveInfo();

                recipients[0].CaptiveInfo.ClientUserId = RandomizeClientUserID();
                recipients[0].RoutingOrder = 1;
                recipients[0].RoleName = "Signer1";

                // If there is a 2nd recipient, configure 
                if (!jointEmail.Value.Equals(""))
                {
                    recipients[1] = new Recipient();
                    recipients[1].ID = "2";
                    recipients[1].Email = jointEmail.Value;
                    recipients[1].Type = RecipientTypeCode.Signer;
                    recipients[1].UserName = jointFirstname.Value + " " + jointLastname.Value;
                    recipients[1].RoleName = "Signer2";
                    recipients[1].RoutingOrder = 1;
                }

                //Configure the inline templates 
                InlineTemplate inlineTemplate = new InlineTemplate();
                inlineTemplate.Sequence = "1";
                inlineTemplate.Envelope = new Envelope();
                inlineTemplate.Envelope.Recipients = recipients;
                inlineTemplate.Envelope.AccountId = ConfigurationManager.AppSettings["API.AccountId"];

                // Initialize tab properties 
                Tab tab = new Tab();
                tab.Type = TabTypeCode.SignHere;
                tab.XPosition = xPosition.Value;
                tab.YPosition = yPosition.Value;
                tab.TabLabel = tabName.Value;
                tab.RecipientID = "1";
                tab.DocumentID = "1";
                tab.Name = tabName.Value;
                tab.PageNumber = tabPage.Value;

                inlineTemplate.Envelope.Tabs = new Tab[] { tab };

                template.InlineTemplates = new InlineTemplate[] { inlineTemplate };


                // Configure the document
                template.Document = new Document();
                template.Document.ID = "1";
                template.Document.Name = "Sample Document";
                BinaryReader binReader = null;
                String filename = uploadFile.Value;
                if (File.Exists(Server.MapPath("~/App_Data/" + filename)))
                {
                    fs = new FileStream(Server.MapPath("~/App_Data/" + filename), FileMode.Open);
                    binReader = new BinaryReader(fs);
                }
                byte[] PDF = binReader.ReadBytes(System.Convert.ToInt32(fs.Length));
                template.Document.PDFBytes = PDF;

                template.Document.TransformPdfFields = true;
                template.Document.FileExtension = "pdf";

                // Add document fields
                template.Document.DocumentFields = new DocumentField[documentFieldsList.Items.Count];
                for (Int16 i = 0; i < documentFieldsList.Items.Count; i++)
                {
                    template.Document.DocumentFields[i] = new DocumentField();
                    template.Document.DocumentFields[i].Name = documentFieldsList.Items[i].Text;
                    template.Document.DocumentFields[i].Value = documentFieldsList.Items[i].Value;
                }

                // Add Connect configuration
                envInfo.EventNotification = new EventNotification(); ;
                envInfo.EventNotification.URL = ConfigurationManager.AppSettings["ConnectListener"];

                envInfo.EventNotification.RequireAcknowledgment = true;
                envInfo.EventNotification.RequireAcknowledgmentSpecified = true;
                envInfo.EventNotification.LoggingEnabled = true;
                envInfo.EventNotification.LoggingEnabledSpecified = true;
                envInfo.EventNotification.IncludeTimeZone = true;
                envInfo.EventNotification.IncludeTimeZoneSpecified = true;
                envInfo.EventNotification.IncludeSenderAccountAsCustomField = true;
                envInfo.EventNotification.IncludeSenderAccountAsCustomFieldSpecified = true;
                envInfo.EventNotification.IncludeEnvelopeVoidReason = true;
                envInfo.EventNotification.IncludeEnvelopeVoidReasonSpecified = true;
                envInfo.EventNotification.IncludeDocumentFields = true;
                envInfo.EventNotification.IncludeDocumentFieldsSpecified = true;
                envInfo.EventNotification.EnvelopeEvents = new EnvelopeEvent[5];
                envInfo.EventNotification.EnvelopeEvents[0] = new EnvelopeEvent();
                envInfo.EventNotification.EnvelopeEvents[0].EnvelopeEventStatusCode = EnvelopeEventStatusCode.Sent;
                envInfo.EventNotification.EnvelopeEvents[1] = new EnvelopeEvent();
                envInfo.EventNotification.EnvelopeEvents[1].EnvelopeEventStatusCode = EnvelopeEventStatusCode.Voided;
                envInfo.EventNotification.EnvelopeEvents[2] = new EnvelopeEvent();
                envInfo.EventNotification.EnvelopeEvents[2].EnvelopeEventStatusCode = EnvelopeEventStatusCode.Delivered;
                envInfo.EventNotification.EnvelopeEvents[3] = new EnvelopeEvent();
                envInfo.EventNotification.EnvelopeEvents[3].EnvelopeEventStatusCode = EnvelopeEventStatusCode.Declined;
                envInfo.EventNotification.EnvelopeEvents[4] = new EnvelopeEvent();
                envInfo.EventNotification.EnvelopeEvents[4].EnvelopeEventStatusCode = EnvelopeEventStatusCode.Completed;
                envInfo.EventNotification.RecipientEvents = new RecipientEvent[6];
                envInfo.EventNotification.RecipientEvents[0] = new RecipientEvent();
                envInfo.EventNotification.RecipientEvents[0].RecipientEventStatusCode = RecipientEventStatusCode.AuthenticationFailed;
                envInfo.EventNotification.RecipientEvents[1] = new RecipientEvent();
                envInfo.EventNotification.RecipientEvents[1].RecipientEventStatusCode = RecipientEventStatusCode.AutoResponded;
                envInfo.EventNotification.RecipientEvents[2] = new RecipientEvent();
                envInfo.EventNotification.RecipientEvents[2].RecipientEventStatusCode = RecipientEventStatusCode.Completed;
                envInfo.EventNotification.RecipientEvents[3] = new RecipientEvent();
                envInfo.EventNotification.RecipientEvents[3].RecipientEventStatusCode = RecipientEventStatusCode.Declined;
                envInfo.EventNotification.RecipientEvents[4] = new RecipientEvent();
                envInfo.EventNotification.RecipientEvents[4].RecipientEventStatusCode = RecipientEventStatusCode.Delivered;
                envInfo.EventNotification.RecipientEvents[5] = new RecipientEvent();
                envInfo.EventNotification.RecipientEvents[5].RecipientEventStatusCode = RecipientEventStatusCode.Sent;

                //Create envelope with all the composite template information 
                EnvelopeStatus status = client.CreateEnvelopeFromTemplatesAndForms(envInfo, new CompositeTemplate[] { template }, true);
                RequestRecipientTokenAuthenticationAssertion assert = new RequestRecipientTokenAuthenticationAssertion();
                assert.AssertionID = "12345";
                assert.AuthenticationInstant = DateTime.Now;
                assert.AuthenticationMethod = RequestRecipientTokenAuthenticationAssertionAuthenticationMethod.Password;
                assert.SecurityDomain = "www.magicparadigm.com";

                RequestRecipientTokenClientURLs clientURLs = new RequestRecipientTokenClientURLs();

                clientURLs.OnAccessCodeFailed = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnAccessCodeFailed";
                clientURLs.OnCancel = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnCancel";
                clientURLs.OnDecline = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnDecline";
                clientURLs.OnException = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnException";
                clientURLs.OnFaxPending = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnFaxPending";
                clientURLs.OnIdCheckFailed = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnIdCheckFailed";
                clientURLs.OnSessionTimeout = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnSessionTimeout";
                clientURLs.OnTTLExpired = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnTTLExpired";
                clientURLs.OnViewingComplete = ConfigurationManager.AppSettings["RecipientTokenClientURLsPrefix"] + "?envelopeId=" + status.EnvelopeID + "&event=OnViewingComplete";


                String url = Request.Url.AbsoluteUri;

                String recipientToken;

                clientURLs.OnSigningComplete = url.Substring(0, url.LastIndexOf("/")) + "/EmbeddedSigningComplete1.aspx?envelopeID=" + status.EnvelopeID;
                recipientToken = client.RequestRecipientToken(status.EnvelopeID, recipients[0].CaptiveInfo.ClientUserId, recipients[0].UserName, recipients[0].Email, assert, clientURLs);
                Session["envelopeID"] = status.EnvelopeID;
                if (!Request.Browser.Browser.Equals("InternetExplorer") && (!Request.Browser.Browser.Equals("Safari")))
                {
                    docusignFrame.Visible = true;
                    docusignFrame.Src = recipientToken;
                }
                else // Handle IE differently since it does not allow dynamic setting of the iFrame width and height
                {
                    docusignFrameIE.Visible = true;
                    docusignFrameIE.Src = recipientToken;
                }


            }
        }
        catch (Exception ex)
        {
            // Log4Net Piece
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(demos_EnvelopeCustom___Document_Fields));
            logger.Info("\n----------------------------------------\n");
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            Response.Write(ex.Message);

        }
        finally
        {
            if (fs != null)
                fs.Close();
        }
    }
}