<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminControl.ascx.cs" Inherits="Brafton.Modules.BraftonImporter7_02_02.WebUserControl1" %>

<div id="braftonView">
    <h1>Brafton DotNetNuke Module</h1>

    <asp:UpdatePanel ID="updateAPIKey" runat="server" UpdateMode="Always" EnableViewState="true">
        <ContentTemplate>              
            <placeholder id="errorMessages" runat="server" visible="true">
            <p class="IDs">
            Debuggin Message:
            <asp:Literal ID="errorCheckingLabel" runat="server" />
            <br />
            Checked Status:
            <asp:Literal ID="checkedStatusLabel" runat="server" />
            <br />
            Global Error:
            <asp:Literal ID="globalErrorMessage" runat="server" />
            <br />
            </p>
            </placeholder>
        <br />
        <asp:Button ID="Import" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="Import_Click" Text="Import Articles" ViewStateMode="Enabled" Visible="true" />
        <asp:Button ID="ShowGlobals" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="show_globals" Text="Show Messages" ViewStateMode="Enabled" Visible="true" />
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false">
        <ProgressTemplate>
        <div id="updateBack">
        <h1>Loading ...</h1>
        </div>
        </ProgressTemplate>
        </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>