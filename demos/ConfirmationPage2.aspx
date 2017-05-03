<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfirmationPage2.aspx.cs" Inherits="demos_ConfirmationPage2" %>

<html class="no-js" lang="">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Advanced Forms, Fields and Workflow</title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="apple-touch-icon">

    <!-- Styles and Fonts -->
    <link rel="stylesheet" href="../style/screen.css">
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,600,700,300' rel='stylesheet' type='text/css'>
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" rel='stylesheet' type='text/css'>
</head>

<body class="finance">
    <header>
        <div class="container-fixed">

            <nav class="navbar">
                <div class="navbar-mini">
                    <ul>
                        <li><a href="<%=ConfigurationManager.AppSettings["sourcecode"] %>">Source Code</a></li>
                        <li><a href="https://www.docusign.com/developer-center">DocuSign DevCenter</a></li>
                        <li><a href="https://www.docusign.com/p/APIGuide/Content/Sending%20Group/Rules%20for%20CompositeTemplate%20Usage.htm">Field Transforms</a></li>
                    </ul>
                </div>
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#collaps0r">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand" href="default.aspx">Advanced Forms, Fields and Workflow <span>Momentum</span></a>
                </div>
            </nav>

        </div>
    </header>
    <div class="container-fixed formz-vertical">
        <form class="form-inline" runat="server" id="form">
            <div class="row">
                <div class="col-lg-1">
                    <img class="center-block" src="../images/homebkgrd-financial.jpg" style="height: 573px; width: 1023px; margin-top: 70px" />
                </div>
            </div>
            <button type="Submit" visible="true" id="button" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>

        </form>
    </div>

</body>
</html>
