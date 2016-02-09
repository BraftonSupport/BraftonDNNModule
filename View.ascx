<%@ Control Language="C#" AutoEventWireup="true" Codebehind="View.ascx.cs" Inherits="BraftonView.Brafton_Importer_Clean.DesktopModules_Brafton_View2" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="brafton" Namespace="Brafton.Modules.Globals" Assembly="BraftonImporter7_02_02" %>
<script runat="server">



</script>
<asp:Panel ID="BraftonAdminPanel" ViewStateMode="Enabled" runat="server" Visible="false" ClientIDMode="Static">
    <div id="braftonView" >
        <asp:UpdatePanel ID="updateAPIKey" runat="server" UpdateMode="Always" EnableViewState="true" >
            <ContentTemplate>              
                <h1><asp:Image ID="Image1" runat="server" width="25px" height="25px" Style="margin-right:10px" ImageUrl="~/desktopmodules/BraftonImporter7_02_02/Images/BR.png" />Brafton Content Importer</h1>
            <asp:Button ID="Import" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="Import_Click" Text="Import Content" ViewStateMode="Enabled" Visible="true" />
            <asp:Button ID="ShowGlobals" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="show_globals" Text="Show Messages" ViewStateMode="Enabled" Visible="true" />
            <asp:Button ID="EnableAuto" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="EnableAutomaticImport" Text="Add To DNN Scheduler" ViewStateMode="Enabled" Visible="true" />
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false">
            <ProgressTemplate>
            <div id="updateBack">
            <asp:Image runat="server" width="250px" height="15px" ImageUrl="~/desktopmodules/BraftonImporter7_02_02/Images/loader-3.gif" />
            </div>
            </ProgressTemplate>
            </asp:UpdateProgress>
                <placeholder ID="UpdateAvailable" runat="server" visible="false">
                    <div style="width:50%; max-width:500px">
                        <h2 style="font-weight:900;margin-bottom:10px;display:inline"><asp:Image ID="Image2" runat="server" width="25px" height="25px" Style="margin-right:10px" ImageUrl="~/desktopmodules/BraftonImporter7_02_02/Images/warning.png" />Update Available:</h2>
                        <h4 style="display:inline"><asp:Literal ID="UpdateMessage" runat="server" /></h4>
                    </div>
                </placeholder>
                <placeholder id="errorMessages" runat="server" visible="true">
                <p class="IDs" style="margin:0px;margin-top:2px">
                <h4>Status:<asp:Image ID="StatusImage" runat="server" width="25px" height="25px" Style="margin-right:10px" ImageUrl="~/desktopmodules/BraftonImporter7_02_02/Images/pass.png" /></h4>
                <asp:Literal ID="checkedStatusLabel" runat="server" />
                <h4>Messages:<asp:Image ID="MessageImage" Visible="false" runat="server" width="25px" height="25px" Style="margin-right:10px" ImageUrl="~/desktopmodules/BraftonImporter7_02_02/Images/warning.png" /></h4>
                <asp:Literal ID="globalErrorMessage" runat="server" />
                </p>
                </placeholder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Panel>











