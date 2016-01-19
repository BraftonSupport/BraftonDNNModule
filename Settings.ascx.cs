/*
' Copyright (c) 2014  Brafton.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using Brafton.Modules.Globals;
using System.Web.UI.WebControls;

namespace Brafton.Modules.BraftonImporter7_02_02
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from BraftonImporter7_02_02SettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : BraftonImporter7_02_02ModuleSettingsBase
    {
        public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        public SqlCommand cmd = new SqlCommand();
        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            connection.Open();
            
            try
            {
                if (Page.IsPostBack == false)
                {
                    
                    MyGlobals.PopGlobals();
                    CheckSettings();

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void CheckSettings()
        {
            cmd.Connection = connection;
            cmd.CommandText = "Select Title, BlogID From Blog_Blogs";
            SqlDataReader blogOptions = cmd.ExecuteReader();
            while (blogOptions.Read())
            {
                blogIdDrpDwn.Items.Add(new ListItem(blogOptions.GetString(0), blogOptions.GetValue(1).ToString()));
            }
            blogIdDrpDwn.SelectedValue = Settings.Contains("blogIdDrpDwn") ? Settings["blogIdDrpDwn"].ToString() : "";
            blogOptions.Close();

            cmd.CommandText = "Select Username, UserID From Users";
            SqlDataReader blogAuthors = cmd.ExecuteReader();

            if (blogAuthors.HasRows)
            {
                while (blogAuthors.Read())
                {
                    blogUsersDrpDwn.Items.Add(new ListItem(blogAuthors.GetString(0), blogAuthors.GetValue(1).ToString()));
                }
            }
            blogUsersDrpDwn.SelectedValue = Settings.Contains("blogUsersDrpDwn") ? Settings["blogUsersDrpDwn"].ToString() : "1";
            blogAuthors.Close();
             
            RadioButtonList1.Text = Settings.Contains("RadioButtonList1") ? Settings["RadioButtonList1"].ToString() : "";
            BrandUrl.SelectedValue = Settings.Contains("BrandUrl") ? Settings["BrandUrl"].ToString() : "brafton.com";
            APIKey.Text = Settings.Contains("APIKey") ? Settings["APIKey"].ToString() : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
            VideoPublic.Text = Settings.Contains("VideoPublic") ? Settings["VideoPublic"].ToString() : "xxxxxxx";
            VideoPrivate.Text = Settings.Contains("VideoPrivate") ? Settings["VideoPrivate"].ToString() : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
            VideoFeedId.Text = Settings.Contains("VideoFeedId") ? Settings["VideoFeedId"].ToString() : "0";
            UpdateContent.SelectedValue = Settings.Contains("UpdateContent") ? Settings["UpdateContent"].ToString() : "0";
            DebugMode.SelectedValue = Settings.Contains("DebugMode") ? Settings["DebugMode"].ToString() : "0";
            IncludeImages.SelectedValue = Settings.Contains("IncludeImages") ? Settings["IncludeImages"].ToString() : "0";
            IncludeAtlantis.SelectedValue = Settings.Contains("IncludeAtlantis") ? Settings["IncludeAtlantis"].ToString() : "0";
            IncludejQuery.SelectedValue = Settings.Contains("IncludejQuery") ? Settings["IncludejQuery"].ToString() : "0";

            VideoPauseText.Text = Settings.Contains("VideoPauseText") ? Settings["VideoPauseText"].ToString() : "";
            VideoPauseLink.Text = Settings.Contains("VideoPauseLink") ? Settings["VideoPauseLink"].ToString() : "";
            VideoPauseAssetID.Text = Settings.Contains("VideoPauseAssetID") ? Settings["VideoPauseAssetID"].ToString() : "";

            VideoEndTitle.Text = Settings.Contains("VideoEndTitle") ? Settings["VideoEndTitle"].ToString() : "";
            VideoEndSubtitle.Text = Settings.Contains("VideoEndSubtitle") ? Settings["VideoEndSubtitle"].ToString() : "";
            VideoEndButtonText.Text = Settings.Contains("VideoEndButtonText") ? Settings["VideoEndButtonText"].ToString() : "";
            VideoEndButtonLink.Text = Settings.Contains("VideoEndButtonLink") ? Settings["VideoEndButtonLink"].ToString() : "";
            VideoEndButtonAssetID.Text = Settings.Contains("VideoEndButtonAssetID") ? Settings["VideoEndButtonAssetID"].ToString() : "";

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                var modules = new ModuleController();

                //the following are two sample Module Settings, using the text boxes that are commented out in the ASCX file.
                //module settings
                modules.UpdateModuleSetting(ModuleId, "BrandUrl", BrandUrl.Text);
                modules.UpdateModuleSetting(ModuleId, "APIKey", APIKey.Text);
                modules.UpdateModuleSetting(ModuleId, "VideoPublic", VideoPublic.Text);
                modules.UpdateModuleSetting(ModuleId, "VideoPrivate", VideoPrivate.Text);
                modules.UpdateModuleSetting(ModuleId, "VideoFeedId", VideoFeedId.Text);
                modules.UpdateModuleSetting(ModuleId, "UpdateContent", UpdateContent.Text);
                modules.UpdateModuleSetting(ModuleId, "DebugMode", DebugMode.Text);
                
                //tab module settings
                modules.UpdateTabModuleSetting(TabModuleId, "blogIdDrpDwn",  blogIdDrpDwn.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "RadioButtonList1", RadioButtonList1.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "currentPortalId", PortalSettings.PortalId.ToString());
                modules.UpdateTabModuleSetting(TabModuleId, "currentTabId", PortalSettings.ActiveTab.TabID.ToString());
                modules.UpdateTabModuleSetting(TabModuleId, "clientDomain",HttpContext.Current.Request.Url.Scheme.ToString() + "://" + HttpContext.Current.Request.Url.Host.ToString());
                modules.UpdateTabModuleSetting(TabModuleId, "blogUsersDrpDwn", blogUsersDrpDwn.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "IncludeImages", IncludeImages.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "IncludejQuery", IncludejQuery.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "IncludeAtlantis", IncludeAtlantis.Text);

                modules.UpdateTabModuleSetting(TabModuleId, "VideoPauseText", VideoPauseText.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "VideoPauseLink", VideoPauseLink.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "VideoPauseAssetID", VideoPauseAssetID.Text);

                modules.UpdateTabModuleSetting(TabModuleId, "VideoEndTitle", VideoEndTitle.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "VideoEndSubtitle", VideoEndSubtitle.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "VideoEndButtonText", VideoEndButtonText.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "VideoEndButtonLink", VideoEndButtonLink.Text);
                modules.UpdateTabModuleSetting(TabModuleId, "VideoEndButtonAssetID", VideoEndButtonAssetID.Text);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        public void VideoLinkValidator(object source, ServerValidateEventArgs args)
        {
            string url = args.Value.ToString();
            if (!string.IsNullOrEmpty(url))
            {
                args.IsValid = false;
            }
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                args.IsValid = true;
            }
            
        }
        #endregion
    }
}