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
using System.Text;
using System.Xml.Linq;

public partial class finance_ViewConnectMessages : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0)
            {
                if (!Request.QueryString["envelopeID"].Equals(""))
                {
                    GetConnectMessages(Request.QueryString["envelopeID"]);
                }
            }
        }
    }

    static string PrettyXml(string xml)
    {
        var stringBuilder = new StringBuilder();

        var element = XElement.Parse(xml);

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = false;
        settings.Indent = true;
        settings.NewLineOnAttributes = true;

        using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
        {
            element.Save(xmlWriter);
        }

        return stringBuilder.ToString();
    }

    protected void Index_Changed(object sender, EventArgs e)
    {
        String connectMessage = Server.HtmlEncode(timesGenerated.SelectedValue);
        connectMessageCtl.Value = PrettyXml(Server.HtmlDecode(connectMessage));
    }

    protected void GetConnectMessages(String envelopeID)
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
            if (myReader.HasRows)
            {
                while (myReader.Read())
                {
                    connectMessage = myReader["ConnectMessage"].ToString();
                    dateTime = myReader["DateTime"].ToString();
                    timesGenerated.Items.Add(new ListItem(dateTime, connectMessage));
                }
            }

            //Response.Write(ConnectMessages.Items.Count);
            if (timesGenerated.Items.Count > 0)
            {
                connectMessageCtl.Value = PrettyXml(timesGenerated.Items[0].Value);
            }


        }
        catch (Exception ex)
        {
            // Log4Net Piece
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(finance_ViewConnectMessages));
            logger.Info("\n----------------------------------------\n");
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            Response.Write(ex.Message);

        }
        finally
        {
            myConnection.Close();
        }

    }
}