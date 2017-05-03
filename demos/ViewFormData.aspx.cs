using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using ServiceReference1;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Xml.Serialization;

public partial class finance_ViewFormData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void fillEnvelopeCustomFields()
    {
        String userName = ConfigurationManager.AppSettings["API.Email"];
        String password = ConfigurationManager.AppSettings["API.Password"];
        String integratorKey = ConfigurationManager.AppSettings["API.IntegratorKey"];


        String auth = "<DocuSignCredentials><Username>" + userName
            + "</Username><Password>" + password
            + "</Password><IntegratorKey>" + integratorKey
            + "</IntegratorKey></DocuSignCredentials>";

        try
        {
            ServiceReference1.DSAPIServiceSoapClient svc = new ServiceReference1.DSAPIServiceSoapClient();
            using (OperationContextScope scope = new System.ServiceModel.OperationContextScope(svc.InnerChannel))
            {
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers.Add("X-DocuSign-Authentication", auth);
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                if (Request.QueryString.Count > 0)
                {
                    if (!Request.QueryString["envelopeID"].Equals(""))
                    {
                        // Uncomment to see what happens with request/response call
//                        EnvelopeStatus envStatus = svc.RequestStatusWithDocumentFields(Request.QueryString["envelopeID"]);
                        String envelope = Request.QueryString["envelopeID"];
                        String connectMessage = GetEnvelopeData(envelope);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(connectMessage);

                        // Create an XmlNamespaceManager to resolve the default namespace.
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                        nsmgr.AddNamespace("ds", "http://www.docusign.net/API/3.0");

                        // Get the status of the envelope
                        XmlElement root = doc.DocumentElement;

                        // Get all the form data 
                        XmlNodeList tabs = root.SelectNodes("descendant::ds:EnvelopeStatus/ds:CustomFields/ds:CustomField", nsmgr);
                        XmlNode customFieldName;
                        XmlNode customFieldValue;

                        foreach (XmlNode tab in tabs)
                        {
                            customFieldName = tab.SelectSingleNode("descendant::ds:Name", nsmgr);
                            customFieldValue = tab.SelectSingleNode("descendant::ds:Value", nsmgr);
                            Response.Write("<tr>");
                            Response.Write("<td>");
                            Response.Write(customFieldName.InnerText);
                            Response.Write("</td>");
                            Response.Write("<td>");
                            Response.Write(customFieldValue.InnerText);
                            Response.Write("</td>");
                            Response.Write("</tr>");
                        }
                    }

                    
                }
            }
        }
        catch (Exception ex)
        {
            // Log4Net Piece
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(finance_ViewFormData));
            logger.Info("\n----------------------------------------\n");
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            Response.Write(ex.Message);

        }
        finally
        {

        }

    }

    public void fillFieldData()
    {
        String userName = ConfigurationManager.AppSettings["API.Email"];
        String password = ConfigurationManager.AppSettings["API.Password"];
        String integratorKey = ConfigurationManager.AppSettings["API.IntegratorKey"];

        String auth = "<DocuSignCredentials><Username>" + userName
            + "</Username><Password>" + password
            + "</Password><IntegratorKey>" + integratorKey
            + "</IntegratorKey></DocuSignCredentials>";

        try
        {
            ServiceReference1.DSAPIServiceSoapClient svc = new ServiceReference1.DSAPIServiceSoapClient();
            using (OperationContextScope scope = new System.ServiceModel.OperationContextScope(svc.InnerChannel))
            {
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers.Add("X-DocuSign-Authentication", auth);
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                if (Request.QueryString.Count > 0)
                {
                    if (!Request.QueryString["envelopeID"].Equals(""))
                    {
                        String envelope = Request.QueryString["envelopeID"];
                        String connectMessage = GetEnvelopeData(envelope);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(connectMessage);

                        // Create an XmlNamespaceManager to resolve the default namespace.
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                        nsmgr.AddNamespace("ds", "http://www.docusign.net/API/3.0");

                        // Get the status of the envelope
                        XmlElement root = doc.DocumentElement;

                        // Get all the form data 
                        XmlNodeList documentFields = root.SelectNodes("descendant::ds:EnvelopeStatus/ds:DocumentStatuses/ds:DocumentStatus/ds:DocumentFields", nsmgr);
                        XmlNode documentFieldName;
                        XmlNode documentFieldValue;

                        foreach (XmlNode documentField in documentFields)
                        {
                            documentFieldName = documentField.SelectSingleNode("descendant::ds:Name", nsmgr);
                            documentFieldValue = documentField.SelectSingleNode("descendant::ds:Value", nsmgr);
                            Response.Write("<tr>");
                            Response.Write("<td>");
                            if (documentFieldName != null)
                                Response.Write(documentFieldName.InnerText);
                            Response.Write("</td>");
                            Response.Write("<td>");
                            if (documentFieldValue != null)
                                Response.Write(documentFieldValue.InnerText);
                            Response.Write("</td>");
                            Response.Write("</tr>");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log4Net Piece
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(finance_ViewFormData));
            logger.Info("\n----------------------------------------\n");
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            Response.Write(ex.Message);

        }
        finally
        {

        }

    }


    protected String GetEnvelopeData(String envelopeID)
    {
        SqlConnection myConnection = new SqlConnection();
        String connectMessage = "";
        String dateTime = "";

        try
        {
            myConnection.ConnectionString = ConfigurationManager.AppSettings["DBConnectionString"];

            myConnection.Open();
            SqlCommand sqlCommand = new SqlCommand();
            SqlDataReader myReader = null;
            SqlCommand myCommand = new SqlCommand("select ConnectMessage, DateTime from ConnectMsgs where envelopeID = '" + envelopeID + "'", myConnection);
            myReader = myCommand.ExecuteReader();
            if (myReader.Read())
            {
                connectMessage = myReader["ConnectMessage"].ToString();
                dateTime = myReader["DateTime"].ToString();
            }
            return connectMessage;
        }
        catch (Exception ex)
        {
            // Log4Net Piece
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(finance_ViewFormData));
            logger.Info("\n----------------------------------------\n");
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            Response.Write(ex.Message);

        }
        finally
        {
            myConnection.Close();
        }

        return null;
    }
}