<%@ Page Language="C#" AutoEventWireup="true" CodeFile="eNotary.aspx.cs" Inherits="demos_eNotary" %>

<!DOCTYPE html>
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

    <script>
        function updateWindowSize() {
            var width = window.innerWidth ||
                        document.documentElement.clientWidth ||
                        document.body.clientWidth;
            var height = window.innerHeight ||
                            document.documentElement.clientHeight ||
                            document.body.clientHeight;
            docusignFrame.height = height - 130;
            docusignFrame.width = width;

        }

        window.onload = updateWindowSize;
        window.onresize = updateWindowSize;
    </script>
</head>
<body class="finance">

    <div class="demo">For demonstration purposes only.</div>

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

    <div id="mainForm" runat="server" class="container-fixed formz-vertical">
        <br />
        <ul class="nav nav-pills" role="tablist">
            <li><a href="Default.aspx">Templates</a></li>
            <li><a href="DynamicFields.aspx">Dynamic Fields</a></li>
            <li><a href="AnchorText.aspx">Anchor Text Fields</a></li>
            <li><a href="PDFFormFields.aspx">PDF Form Fields</a></li>
            <li><a href="EnvelopeCustom - Document Fields.aspx">Envelope & Document Fields</a></li>
            <li><a href="DOL.aspx">DOL</a></li>
            <li class="active"><a href="eNotary.aspx">eNotary</a></li>
        </ul>
        <form class="form-inline" runat="server" id="form">
            <asp:RequiredFieldValidator Display="Dynamic" ID="emailvalidator" runat="server" ControlToValidate="email" ErrorMessage="<br>* Email is a required field." ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="lnamevalidator" runat="server" ControlToValidate="lastname" ErrorMessage="<br>* Last name is a required field" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="fnameValidator" runat="server" ControlToValidate="firstname" ErrorMessage="<br>* First name is a required field" ForeColor="Red" />

            <div class="row">
                <div class="col-xs-12">
                    <h1><a id="PrefillClick" causesvalidation="false" runat="server" href="#">eNotary</a></h1>

                </div>
            </div>
            <div class="row" id="AccountInfo" visible="false" runat="server">
                <div class="col-xs-12">
                    <h2>Sender Account Information </h2>
                    <div class="form-group">
                        <label for="acctEmail">Account Email </label>
                        <input type="email" runat="server" class="form-control" id="acctEmail" placeholder="optional" title="Use this section if you want to override the default account information">
                    </div>
                    <br />
                    <div class="form-group">
                        <label for="password">Account Password</label>
                        <input type="password" runat="server" class="form-control" id="password" placeholder="optional" title="Use this section if you want to override the default account information">
                    </div>
                    <br>
                    <div class="form-group">
                        <label for="integratorKey">Integrator Key</label>
                        <input type="text" runat="server" class="form-control" id="integratorKey" placeholder="optional" title="Use this section if you want to override the default account information">
                    </div>
                    <div class="form-group">
                        <label for="accountId">Account ID</label>
                        <input type="text" runat="server" class="form-control" id="accountId" placeholder="optional" title="Use this section if you want to override the default account information">
                    </div>
                    <hr />
                </div>
            </div>
            <div class="row" id="primarySignerSection" runat="server">
                <div class="col-xs-12">
                    <h2>Signer Information</h2>
                    <br />
                    <div class="form-group">
                        <label for="firstname">First Name</label>
                        <input type="text" runat="server" class="form-control" id="firstname" placeholder="">
                    </div>
                    <div class="form-group">
                        <label for="lastname">Last Name</label>
                        <input type="text" runat="server" class="form-control" id="lastname" placeholder="">
                    </div>
                    <br>
                    <div class="form-group">
                        <label for="email">Email Address</label>
                        <input type="email" runat="server" class="form-control" id="email" placeholder="">
                    </div>
                    <hr />
                </div>
            </div>
            <div class="row" id="notarySignerSection" runat="server">
                <div class="col-xs-12">
                    <h2>Notary Information</h2>
                    <div class="form-group">
                        <label for="firstname">First Name</label>
                        <input type="text" runat="server" class="form-control" id="notaryFirstname" placeholder="">
                    </div>
                    <div class="form-group">
                        <label for="lastname">Last Name</label>
                        <input type="text" runat="server" class="form-control" id="notaryLastname" placeholder="">
                    </div>
                    <br>
                    <div class="form-group">
                        <label for="email">Email Address </label>
                        <input type="email" runat="server" class="form-control" id="notaryEmail" placeholder="">
                    </div>
                    <hr />
                </div>
            </div>
            <button type="button" visible="true" id="button" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>
        </form>
    </div>

    <iframe runat="server" id="docusignFrame" style="width: 100%; height: 768px" />

    <iframe runat="server" id="docusignFrameIE" style="width: 100%; height: 768px" />

    <!-- Google Analytics -->
    <script>
        (function (b, o, i, l, e, r) {
            b.GoogleAnalyticsObject = l; b[l] || (b[l] =
            function () { (b[l].q = b[l].q || []).push(arguments) }); b[l].l = +new Date;
            e = o.createElement(i); r = o.getElementsByTagName(i)[0];
            e.src = '//www.google-analytics.com/analytics.js';
            r.parentNode.insertBefore(e, r)
        }(window, document, 'script', 'ga'));
        ga('create', 'UA-XXXXX-X', 'auto'); ga('send', 'pageview');
    </script>

    <!-- Scripts -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
    <script src="../js/main.js"></script>

    <script type='text/javascript' id="__bs_script__">
        document.write("<script async src='//localhost:3000/browser-sync/browser-sync-client.1.9.0.js'><\/script>".replace(/HOST/g, location.hostname).replace(/PORT/g, location.port));
    </script>
</body>
</html>
