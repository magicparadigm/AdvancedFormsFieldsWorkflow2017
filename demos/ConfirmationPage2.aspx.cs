using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class demos_ConfirmationPage2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        button.InnerText = "View Status";
        button.ServerClick += new EventHandler(buttonClick);
    }

    protected void buttonClick(object sender, EventArgs e)
    {
        if (Request.QueryString.Count > 0)
            Response.Redirect("ViewConnectMessages.aspx?envelopeID=" + Request.QueryString["envelopeID"]);
        else
            Response.Redirect("ViewConnectMessages.aspx?envelopeID=" + Session["envelopeID"]);
    }
}