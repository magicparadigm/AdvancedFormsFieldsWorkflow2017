<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DOL.aspx.cs" Inherits="demos_DOL" %>

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
            <li class="active"><a href="DOL.aspx">DOL</a></li>
            <li><a href="eNotary.aspx">eNotary</a></li>
        </ul>
        <form class="form-inline" runat="server" id="form">
            <asp:RequiredFieldValidator Display="Dynamic" ID="emailvalidator" runat="server" ControlToValidate="email" ErrorMessage="<br>* Email is a required field." ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="lnamevalidator" runat="server" ControlToValidate="lastname" ErrorMessage="<br>* Last name is a required field" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="fnameValidator" runat="server" ControlToValidate="firstname" ErrorMessage="<br>* First name is a required field" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="uploadfilevalidator" runat="server" ControlToValidate="uploadfile" ErrorMessage="<br>* Upload of a main  document is required" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="uploadfile2validator" runat="server" ControlToValidate="uploadfile2" ErrorMessage="<br>* Upload of a supplemental  document is required" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="tabNamevalidator" runat="server" ControlToValidate="tabName" ErrorMessage="<br>* Signature field name is a required field" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="tabPageValidator" runat="server" ControlToValidate="tabPage" ErrorMessage="<br>* Signature field page is a required field" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="xPositionValidator" runat="server" ControlToValidate="xPosition" ErrorMessage="<br>* Signature field xPosition is required" ForeColor="Red" />
            <asp:RequiredFieldValidator Display="Dynamic" ID="yPositionValidator" runat="server" ControlToValidate="yPosition" ErrorMessage="<br>* Signature field yPosition is required" ForeColor="Red" />
            <div class="row">
                <div class="col-xs-12">
                    <h1><a id="PrefillClick" causesvalidation="false" runat="server" href="#">Dept of Labor Fiduciary Rule (DOL)</a></h1>

                </div>
            </div>
            <div class="row" id="AccountInfo" runat="server">
                <div class="col-xs-12">
                    <h2>Account Information </h2>
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
                    <div class="form-group">
                        <div class="radio">
                            <input runat="server" type="radio" class="form-control" checked name="SignerType" value="embedded" id="embedded">
                            Embedded Signer
                        </div>
                        <div class="radio">
                            <input runat="server" type="radio" class="form-control" name="SignerType" value="remote" id="remote">
                            Remote Signer 
                        </div>
                    </div>
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
            <div class="row" id="jointSignerSection" runat="server">
                <div class="col-xs-12">
                    <h2>Joint Account Holder</h2>
                    <div class="form-group">
                        <label for="firstname">First Name</label>
                        <input type="text" runat="server" class="form-control" id="jointFirstname" placeholder="">
                    </div>
                    <div class="form-group">
                        <label for="lastname">Last Name</label>
                        <input type="text" runat="server" class="form-control" id="jointLastname" placeholder="">
                    </div>
                    <br>
                    <div class="form-group">
                        <label for="email">Email Address </label>
                        <input type="email" runat="server" class="form-control" id="jointEmail" placeholder="">
                    </div>
                    <hr />
                </div>
            </div>
            <div class="row" id="templates" runat="server">
                <div class="col-xs-12">
                    <h2>Select Main Document</h2>
                    <div class="form-group">
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </div>
                    <div class="form-group">
                        <button type="button" visible="true" id="UploadButton" causesvalidation="false" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>
                    </div>
                    <div class="form-group">
                        <label for="uploadFile">Uploaded File </label>
                        <input type="text" runat="server" class="form-control" id="uploadFile" placeholder="" readonly="readonly" style="width: 500px">
                    </div>
                    <br />
                    <div class="col-xs-12">
                        <h2>Signature Field</h2>
                        <div class="form-group">
                            <label for="tabName">Name</label>
                            <input type="text" runat="server" class="form-control" id="tabName" placeholder="">
                        </div>
                        <div class="form-group">
                            <label for="page">Page</label>
                            <input type="text" runat="server" class="form-control" id="tabPage" placeholder="">
                        </div>
                        <br>
                        <div class="form-group">
                            <label for="tabName">X Position</label>
                            <input type="text" runat="server" class="form-control" id="xPosition" placeholder="">
                        </div>
                        <div class="form-group">
                            <label for="page">Y Position</label>
                            <input type="text" runat="server" class="form-control" id="yPosition" placeholder="">
                        </div>
                        <hr />
                    </div>
                </div>
                <br />
                <div class="col-xs-12">
                    <h2>Select Supplemental Document</h2>
                    <div class="form-group">
                        <asp:FileUpload ID="FileUpload2" runat="server" />
                    </div>
                    <div class="form-group">
                        <button type="button" visible="true" id="UploadButton2" causesvalidation="false" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>
                    </div>
                    <div class="form-group">
                        <label for="uploadFile">Uploaded File </label>
                        <input type="text" runat="server" class="form-control" id="uploadFile2" placeholder="" readonly="readonly" style="width: 500px">
                    </div>

                    <hr />
                </div>
            </div>

            <div class="col-xs-12" id="SupplementalDocConfig" runat="server">
<%--                <div class="col-xs-12">
                    <h2>How should the supplemental information be displayed?</h2>
                    <div class="form-group">
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="Display" value="Modal" id="Modal" checked>
                            Modal - Show the supplemental information in a modal window
                        </div>
                        <div class="radio">
                            <input type="radio"  runat="server" class="form-control" name="Display" value="Download" id="Download">
                            New Browser Window - Show the supplemental information in a new browser window
                        </div>
                        <div class="radio">
                            <input type="radio"  runat="server" class="form-control" name="Display" value="Inline" id="Inline">
                            Inline - Show the supplemental information in regular signing window
                        </div>
                    </div>
                    <hr />
                </div>--%>
                <div class="col-xs-12">
                    <h2>How should the signer acknowledge the supplemental information?</h2>
                    <div class="form-group">
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="SignerAcknowledgement" value="View_" id="View_" checked>
                            View - The signer must view the supplemental information
                        </div>
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="SignerAcknowledgement" value="Accept" id="Accept">
                            Accept - The signer must accept the supplemental information but is not required to view it
                        </div>
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="SignerAcknowledgement" value="View_Accept" id="View_Accept">
                            View & Accept - The signer must view and accept the supplemental information
                        </div>
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="SignerAcknowledgement" value="No_Interaction" id="No_Interaction">
                            No Interaction - The signer is not required to view or accept the supplemental information
                        </div>
                    </div>
                    <hr />
                </div>
                <div class="col-xs-12">
                    <h2>Should the supplemental information be included in any combined document download?</h2>
                    <div class="form-group">
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="IncludeInDownload" value="IncludeInDownloadTrue" id="IncludeInDownloadTrue" checked>
                            Yes - If the documents are downloaded as a combined document, the supplemental information is included 
                        </div>
                        <div class="radio">
                            <input type="radio" runat="server" class="form-control" name="IncludeInDownload" value="IncludeInDownloadFalse" id="IncludeInDownloadFalse">
                            No - If the documents are downloaded as a combined document, the supplemental information is not included 
                            <br />
                            <br />
                        </div>
                    </div>
                </div>
            </div>

            <button type="button" visible="true" id="button" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>
            <button type="button" visible="true" id="button2" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>
            <button type="button" visible="true" id="button3" runat="server" class="btn" style="color: #fff; padding: 10px 80px; font-size: 14px; margin: 40px auto; display: block;"></button>
        </form>
    </div>

    <iframe runat="server" id="docusignFrame"   style ="width: 100%; height: 768px" />

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
