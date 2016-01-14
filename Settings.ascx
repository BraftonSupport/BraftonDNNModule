<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Brafton.Modules.BraftonImporter7_02_02.Settings" %>


<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->
 

<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Basic Settings</a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <p>We strongly recommend you read the documentation on this importer to ensure you have set the options that will best suit your installation of DotNetNuke.</p>
        </div>
        <div class="dnnFormItem">
            
            <dnn:label ID="currentPortlID" runat="server" visible="false"/>
            <dnn:label ID="currentTabID" runat="server" visible="false" />
            <dnn:label ID="clientDomain" runat="server" visible="false" />
            <dnn:label ID="ImportType" runat="server" />
            <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                <asp:ListItem Text="Articles" Value="articles"></asp:ListItem>
                <asp:ListItem Text="Videos" Value="video"></asp:ListItem>
                <asp:ListItem Text="Both" Value="both"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="BlogName" runat="server" />
            <asp:DropDownList runat="server" ID="blogIdDrpDwn" ClientIDMode="Static" AutoPostBack="true" ViewStateMode="Enabled" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="Brand" runat="server" />
            <asp:DropDownList ID="BrandUrl" runat="server">
                <asp:ListItem Text="Brafton" value="brafton.com"></asp:ListItem>
                <asp:ListItem Text="ContentLEAD" Value="contentlead.com" ></asp:ListItem>
                <asp:ListItem Text="Castleford" Value="castleford.com.au"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="API" runat="server" />
            <asp:TextBox ID="APIKey" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="Author" runat="server" />
            <asp:DropDownList ID="blogUsersDrpDwn" runat="server" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="AddImages" runat="server" />
            <asp:DropDownList ID="IncludeImages" runat="server">
                <asp:ListItem Text="Let Blog Module Handle Images" Value="0"></asp:ListItem>
                <asp:ListItem Text="Insert Into Content and Summary " Value="1"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="Update" runat="server" />
            <asp:DropDownList ID="UpdateContent" runat="server">
                <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                <asp:ListItem Text="YES" Value="1"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="Debug" runat="server" />
            <asp:DropdownList ID="DebugMode" runat="server">
                <asp:ListItem Text="OFF" Value="0"></asp:ListItem>
                <asp:ListItem Text="ON" Value="1"></asp:ListItem>
            </asp:DropdownList>
        </div>
    </fieldset>
<!--
    <h2 id="H2" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Article Settings</a></h2>
    <fieldset>
        
    </fieldset>
    -->
    <h2 id="H1" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Video Settings</a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="PublicKey" runat="server" />
            <asp:TextBox ID="VideoPublic" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="PrivateKey" runat="server" />
            <asp:TextBox ID="VideoPrivate" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="FeedID" runat="server" />
            <asp:TextBox ID="VideoFeedId" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="PauseText" runat="server" />
            <asp:TextBox ID="VideoPauseText" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="jQuery" runat="server" />
            <asp:DropDownList ID="IncludejQuery" runat="server">
                <asp:ListItem Text="Add jQuery to <head>" Value="1"></asp:ListItem>
                <asp:ListItem Text="I dont need jQuery" Value="0"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="atlantis" runat="server" />
            <asp:DropDownList ID="IncludeAtlantis" runat="server">
                <asp:ListItem Text="Add AtlantisJS to <head>" Value="1"></asp:ListItem>
                <asp:ListItem Text="Omit AtlantisJS script" Value="0"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="PauseLink" runat="server" />
            <asp:TextBox ID="VideoPauseLink" runat="server" />
            <asp:CustomValidator ID="VideoLinkValidate" runat="server" OnServerValidate="VideoLinkValidator" 
                ControlToValidate="VideopauseLink" ErrorMessage="Must Be a valid Url">
            </asp:CustomValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="PauseAssetID" runat="server" />
            <asp:TextBox ID="VideoPauseAssetID" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="EndTitle" runat="server" />
            <asp:TextBox ID="VideoEndTitle" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="EndSubtitle" runat="server" />
            <asp:TextBox ID="VideoEndSubtitle" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="EndButtonText" runat="server" />
            <asp:TextBox ID="VideoEndButtonText" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="EndButtonLink" runat="server" />
            <asp:TextBox ID="VideoEndButtonLink" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="EndButtonAssetID" runat="server" />
            <asp:TextBox ID="VideoEndButtonAssetID" runat="server" />
        </div>
    </fieldset>
    <script>
        
    </script>

