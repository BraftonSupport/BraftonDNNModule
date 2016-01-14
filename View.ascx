<%@ Control Language="C#" AutoEventWireup="true" CodeFile="View.ascx.cs" Inherits="BraftonView.Brafton_Importer_Clean.DesktopModules_Brafton_View2" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="brafton" Namespace="Brafton.Modules.Globals" Assembly="BraftonImporter7_02_02" %>
<script runat="server">



</script>
<asp:Panel ID="BraftonAdminPanel" ViewStateMode="Enabled" runat="server" Visible="false" ClientIDMode="Static">
    <div id="braftonView" >
        <asp:UpdatePanel ID="updateAPIKey" runat="server" UpdateMode="Always" EnableViewState="true" >
            <ContentTemplate>              
                <h1>Brafton DotNetNuke Module</h1>
            <asp:Button ID="Import" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="Import_Click" Text="Import Content" ViewStateMode="Enabled" Visible="true" />
            <asp:Button ID="ShowGlobals" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="show_globals" Text="Show Messages" ViewStateMode="Enabled" Visible="true" />
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false">
            <ProgressTemplate>
            <div id="updateBack">
            <!--<h1>Loading ...</h1>--><asp:Image runat="server" width="250px" height="15px" ImageUrl="~/desktopmodules/BraftonImporter7_02_02/Images/loader-3.gif" />
            </div>
            </ProgressTemplate>
            </asp:UpdateProgress>
                <placeholder id="errorMessages" runat="server" visible="true">
                <p class="IDs">
                Checked Status:
                <asp:Literal ID="checkedStatusLabel" runat="server" />
                <br />
                Importer Messages:
                <asp:Literal ID="globalErrorMessage" runat="server" />
                <br />
                </p>
                </placeholder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Panel>
<asp:Label ID="Label1" runat="server" Visible="false" OnLoad="runBraftonImporter" />











