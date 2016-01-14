<%@ Control Language="C#" AutoEventWireup="true" CodeFile="View.ascx.cs" Inherits="BraftonView.Brafton_Importer_Clean.DesktopModules_Brafton_View2" %>

<script runat="server">



</script>



<!-- <link href="<%= appPath %>/DesktopModules/Brafton/css/style.css" rel="stylesheet" type="text/css" />-->
<% if (HttpContext.Current.User.Identity.IsAuthenticated)   { %>
<div id="braftonView">
    <h1>
        Brafton DotNetNuke Module </h1>
    
    <p class="IDs">
       <%-- Current Portal ID:--%>
        <asp:Label ID="currentPortalID" runat="server" Visible="false" />
    </p>
    <p>
        ****The Brafton Module has to be installed on the same page as the DNN Blog Module
        in order to build the Permalinks****</p>

    <p class="IDs">
       <%-- Current Tab ID:--%>
        <asp:Label ID="currentTabID" runat="server" Visible="false" />

    </p>
    <asp:UpdatePanel ID="updateAPIKey" runat="server" UpdateMode="Always" EnableViewState="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="setBaseURL" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="setAPI" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="setBlogID" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Import" EventName="Click" />
        </Triggers>
        <ContentTemplate>
           <div class="firstSelectorHeader"><h2>I have:</h2></div>
            <div class="firstSelectors">
                <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                    <asp:ListItem Text="Articles" Value="articles"></asp:ListItem>
                    <asp:ListItem Text="Videos" Value="video"></asp:ListItem>
                    <asp:ListItem Text="Both" Value="both"></asp:ListItem>
                </asp:RadioButtonList></div>
            <asp:Button ID="firstSelectorSubmit" runat="server" Text="Show The Settings" OnClick="showSections" />
                           <h3>
                    Set the Blog that you want to import the articles/videos to:</h3>
                
                <p class="IDs">
                    <h2>Blog Name:</h2>
                    <br />Current Blog:
                    <asp:Literal ID="currentBlogID" runat="server" /><br />
                    <asp:DropDownList runat="server" ID="blogIdDrpDwn" ClientIDMode="Static" AutoPostBack="true"
                        ViewStateMode="Enabled" />
                    <p>
                    </p>
                    <asp:Button ID="setBlogID" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="setBlogID_Click" Text="Choose the blog" ViewStateMode="Enabled" />
                    <p class="authors">
                        <h2>Authors:</h2>
                        <br />
                        <asp:DropDownList ID="blogUsersDrpDwn" runat="server" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled" />
                        <br />
                        <asp:Button ID="setAuthorID" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="setAuthor_click" Text="Choose Article Author" ViewStateMode="Enabled" />
                        <p>
                        </p>
                        <asp:Label ID="labelError" runat="server" CssClass="error" />
                        <asp:PlaceHolder ID="ArticlePlaceHolder" runat="server" Visible="false">
                            <h2>Article Settings</h2>
                            <p>
                                Before articles can be imported from our feed all of these statements have to be <span class="boolTrue">True</span>.</p>
                            <div id="checks">
                                <p>
                                    1.) Is the DotNetNuke Blog Module Installed?
                                    <asp:Label ID="boolBlogModule" runat="server" />
                                </p>
                                <p>
                                    2.) Have you created a blog?
                                    <asp:Label ID="boolBlogCreated" runat="server" />
                                </p>
                                <p>
                                    3.) Have you Selected An Author?
                                    <asp:Label ID="boolAuthorSet" runat="server" />
                                </p>
                                <p>
                                    4.) Have you set the specific blog that you want to import the articles into?
                                    <asp:Label ID="boolCheckBlogID" runat="server" />
                                </p>
                                <p>
                                    5.) Have you set the Base Url?
                                    <asp:Label ID="boolCheckUrl" runat="server" />
                                </p>
                                <p>
                                    6.) Have you set the Brafton API Key?
                                    <asp:Label ID="boolCheckAPI" runat="server" />
                                </p>
                            </div>
                            <asp:PlaceHolder ID="setAPIPH" runat="server" Visible="false">
                                <p class="IDs">
                                    <h2>BASE URL</h2>
                                    Set The Base URL Here:
                                    <br />
                                    <%--Current Base URL:--%>Default http://api.brafton.com/ or http://api.contentlead.com/
                                    <asp:Literal ID="baseURLLabel" runat="server" />
                                    <br />
                                    <asp:TextBox ID="baseURL" runat="server" Width="350px"></asp:TextBox>
                                </p>
                                <asp:Button ID="setBaseURL" runat="server" OnClick="setBaseURL_Click" Text="Set Base URL" />
                                <p class="IDs">
                                    <h2>API Key</h2>
                                    Set The API Key Here:
                                    <br />
                                    <%--Current API:--%>
                                    <asp:Literal ID="apiURLLabel" runat="server" />
                                    <br />
                                    <asp:TextBox ID="apiURL" runat="server" Width="350px"></asp:TextBox>
                                </p>
                                <asp:Button ID="setAPI" runat="server" OnClick="setAPI_Click" Text="Set API" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="nextStep" runat="server" Visible="true"></asp:PlaceHolder>
                        </asp:PlaceHolder>
                        <%-- end of article placeholder --%>
                        <asp:PlaceHolder ID="VideoPlaceHolder" runat="server" Visible="false">
                            <h3>Video Section</h3>
                            <em>*if you do not have video included with your account please do not adjust anything below as it will have adverse effects on your blog.</em>
                            <br />
                            <br />
                            <%-- <asp:CheckBox ID="InclVideo" runat="server" AutoPostBack="true" OnCheckedChanged="showVideoSettings"  />--%>
                            <asp:PlaceHolder ID="VideoSettings" runat="server" ViewStateMode="Enabled" Visible="false">
                                <br />
                                <br />
                                <panel ID="VideoSettingsDiv" runat="server">
                                    <div class="vidInputsContainer">
                                        <asp:PlaceHolder ID="CurrentVidSetting1" runat="server" Visible="false">
                                            <br />
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="CurrentVidSetting2" runat="server" Visible="true">Please provide the following (These values can be obtained from your Account Manager):<br /></asp:PlaceHolder>
                                        <div class="vidSettingLabel">
                                            Video Base URI(Default: api.video.brafton.com):</div>
                                        <div class="vidSettingTextField">
                                            <asp:TextBox ID="VideoBaseURL" runat="server" Width="350px"></asp:TextBox>
                                        </div>
                                        <br />
                                        <div class="vidSettingLabel">
                                            Video Base URI(Default: pictures.video.brafton.com):</div>
                                        <div class="vidSettingTextField">
                                            <asp:TextBox ID="VideoPhotoURL" runat="server" Width="350px"></asp:TextBox>
                                        </div>
                                        <br />
                                        <div class="vidSettingLabel">
                                            Video Public Key:</div>
                                        <div class="vidSettingTextField">
                                            <asp:TextBox ID="VideoPublicKey" runat="server" Width="350px"></asp:TextBox>
                                        </div>
                                        <br />
                                        <div class="vidSettingLabel">
                                            Video Secret Key:</div>
                                        <div class="vidSettingTextField">
                                            <asp:TextBox ID="VideoSecretKey" runat="server" Width="350px"></asp:TextBox>
                                        </div>
                                        <br />
                                        <div class="vidSettingLabel">
                                            Video Feed Number:</div>
                                        <div class="vidSettingTextField">
                                            <asp:TextBox ID="VideoFeedNumber" runat="server" Width="350px"></asp:TextBox>
                                        </div>
                                        <br />
                                        <asp:Button ID="setVidSettings" runat="server" OnClick="checkForVideo" Text="Set Video Settings" />
                                        <asp:Button ID="updateVidSettings" runat="server" OnClick="checkForVideo" Text="Update Video Settings" Visible="false" />
                                    </div>
                                </panel>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                        <%-- End of video placeholder--%>
                        <h3>Article Feed Updates</h3>
                        Include updated feed content when syncing?
                        <asp:CheckBox ID="InclUpdatedFeedContent" runat="server" />
                        <br />
                        <em>*articles will only update at most 1 time per day regardless of the frequency of running the importer</em>
                        <br />
                        <br />
                        <br />
                        
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
                        <br />
                        <asp:Button ID="Import" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="Import_Click" Text="Import Articles" ViewStateMode="Enabled" Visible="false" />
                        <asp:Button ID="ShowGlobals" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="show_globals" Text="Show Messages" ViewStateMode="Enabled" Visible="true" />
                        <asp:Button ID="SaveSettings" runat="server" AutoPostBack="true" ClientIDMode="Static" OnClick="saveSettings" Text="Save Settings" ViewStateMode="Enabled" Visible="false" />
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false">
                            <ProgressTemplate>
                                <div id="updateBack">
                                    <h1>Loading ...</h1>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <p>
                        </p>
                        <p>
                        </p>
                    </p>
                </p>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<% } %>
<asp:Label ID="Label1" runat="server" Visible="false" OnLoad="runBraftonImporter" />











