using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Web.Hosting;
using System.Diagnostics;

using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Entities.Modules;

using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Brafton;
using Brafton.Modules.Globals;

using System.Net;
using Brafton.Modules.VideoImporter;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;
using System.Web.Script.Serialization;
namespace BraftonView.Brafton_Importer_Clean
{
    [SecurityCritical]
    public partial class DesktopModules_Brafton_View2 : DotNetNuke.Entities.Modules.PortalModuleBase
    {
      
        //Connection properties
        public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        public SqlConnection connection2;
        public SqlCommand cmd = new SqlCommand();
        public SqlCommand cmd2 = new SqlCommand();

        public Boolean isEditable;

        public Boolean braftonViewable()
        {
            /*
             * This is going to determine if user is logged in and if so 
             * will go and retrieve the admin user control
             * */
            isEditable = HttpContext.Current.User.Identity.IsAuthenticated;
            if (isEditable)
            {
                
                return true;
            }
            return false;
            
        }
        protected void Page_Load(object sender, EventArgs e)
        {          
            /*
             * TO DO: 
             * Need to create a check if video article somehow, and determine if we should add the atlantis scripts to the head.
             * */
            
            ModuleConfiguration.ModuleTitle = "";
            if (braftonViewable())
            {
                CheckUpdate();
                CheckStatus();
                BraftonAdminPanel.Visible = true;
            }
            HtmlLink css = new HtmlLink();
            css.Href = "http://atlantisjs.brafton.com/v1/atlantisjsv1.4.css";
            css.Attributes.Add("rel", "stylesheet");
            css.Attributes.Add("type", "text/css");
            this.Page.Header.Controls.Add(css);
            string jquery = "<script>!window.jQuery && document.write(unescape('%3Cscript src=\"//ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.js\"%3E%3C/script%3E'))</script>";
            string atlantis = "<script src='http://atlantisjs.brafton.com/v1/atlantis.min.v1.3.js' type='text/javascript'></script>";
            //do a check if jquery is turned on 
            if (Settings.Contains("IncludejQuery") && Settings["IncludeAtlantis"].ToString() == "1")
            {
                this.Page.Header.Controls.Add(new LiteralControl(jquery));
            }
            if (Settings.Contains("IncludeAtlantis") && Settings["IncludeAtlantis"].ToString() == "1")
            {
                this.Page.Header.Controls.Add(new LiteralControl(atlantis));
            }
            
            /* 
             * This will be added when using without the blog module in the version 7.1.0
            HtmlMeta meta = new HtmlMeta();
            meta.Name = "brafton";
            meta.Content = "my brafton";
            this.Page.Header.Controls.Add(meta);
             * */
            MessageImageVisible();
        }
        protected void MessageImageVisible()
        {
            if (!string.IsNullOrWhiteSpace(globalErrorMessage.Text))
            {
                MessageImage.Visible = true;
            }
            else
            {
                MessageImage.Visible = false;
            }
        }
        protected void Import_Click(object sender, EventArgs e)
        {
            Brafton.DotNetNuke.BraftonSchedule newSched = new Brafton.DotNetNuke.BraftonSchedule();
            MyGlobals.BraftonViewModuleId.Add(ModuleId);
            newSched.DoWork();          
            globalErrorMessage.Text = MyGlobals.MyGlobalError;
            MessageImageVisible();
            MyGlobals.MyGlobalError = "";
        }

        protected void show_globals(object sender, EventArgs e)
        {
            CheckStatus();
            globalErrorMessage.Text = MyGlobals.MyGlobalError;
            MessageImageVisible();
            MyGlobals.MyGlobalError = "";
        }
        protected void CheckStatus()
        {
            using (DataClasses1DataContext dnncontext = new DataClasses1DataContext())
            {
                Schedule sc = dnncontext.Schedules.FirstOrDefault(x => x.FriendlyName == "BraftonImporter");
                if (sc == null){}
                else
                {
                    if (Convert.ToBoolean(sc.Enabled))
                    {
                        EnableAuto.Text = "Disable Automatic Import";
                    }
                    else
                    {
                        EnableAuto.Text = "Enable Automatic Import";
                    }
                   
                }
            }
            string msg = "<ul>";
            bool status = true;
            if (Settings.Contains("RadioButtonList1"))
            {
                string type = Settings["RadioButtonList1"].ToString();
                if (Settings.Contains("APIKey") && (type == "articles" || type == "both") )
                {
                    string key = Settings["APIKey"].ToString();
                    Guid key_result;
                    bool valid = Guid.TryParse(key, out key_result);
                    status = status ? valid : status;
                    if (!status)
                    {
                        msg = msg + "<li>APIKey is either not set or is invalid</li>";
                    }
                }
                else if(type == "articles" || type == "both")
                {
                    msg = msg + "<li>You have not set your API Key</li>";
                    status = false;
                }
                if ((Settings.Contains("VideoPublic") || Settings.Contains("VideoPrivate")) && (type == "video" || type == "both"))
                {
                    string publicKey = Settings["VideoPublic"].ToString();
                    string privateKey = Settings["VideoPrivate"].ToString();
                    Guid private_result;
                    bool private_valid = Guid.TryParse(privateKey, out private_result);
                    status = status ? private_valid : status;
                    bool public_valid = !string.IsNullOrWhiteSpace(publicKey);
                    status = status ? public_valid : status;
                    if (!public_valid || !private_valid)
                    {
                        msg = msg + "<li>Check your Public and Private Keys to ensure they are valid</li>";
                    }
                }
                else if (type == "video" || type == "both")
                {
                    msg = msg + "<li>You have not set your Video Public and/or Private Keys</li>";
                    status = false;
                }
                if (!Settings.Contains("blogIdDrpDwn"))
                {
                    msg = msg + "<li>You have not selected a Blog to import your content into</li>";
                    status = false;
                }
                msg = msg + "</ul>";
                if (status)
                {
                    msg = string.Empty;
                }
                else
                {
                    StatusImage.ImageUrl = "~/desktopmodules/Braftonimporter7_02_02/Images/error.png";
                }
            }
            else
            {
                status = false;
                msg = "<li>No options have been saved for this module</li>";
            }
            checkedStatusLabel.Text = msg;
            Import.Enabled = status;
            EnableAuto.Enabled = status;
        }
        protected void EnableAutomaticImport(object sender, EventArgs e)
        {
            /*
             * this will add the importer to the dnn scheduler
             * */
            using (DataClasses1DataContext dnncontext = new DataClasses1DataContext())
            {
                Schedule sc = dnncontext.Schedules.FirstOrDefault(x => x.FriendlyName == "BraftonImporter");
                if (sc == null)
                {
                    
                    Schedule newSchedule = new Schedule();
                    newSchedule.TypeFullName = "Brafton.DotNetNuke.BraftonSchedule,BraftonImporter7_02_02";
                    newSchedule.TimeLapse = 1;
                    newSchedule.TimeLapseMeasurement = "h";
                    newSchedule.RetryTimeLapse = 15;
                    newSchedule.RetryTimeLapseMeasurement = "m";
                    newSchedule.RetainHistoryNum = 0;
                    newSchedule.CatchUpEnabled = false;
                    newSchedule.Enabled = true;
                    newSchedule.FriendlyName = "BraftonImporter";
                    newSchedule.CreatedOnDate = DateTime.Now;
                    newSchedule.ScheduleStartDate = DateTime.Now;
                    newSchedule.AttachToEvent = "";
                    newSchedule.ObjectDependencies = "";
                    dnncontext.Schedules.InsertOnSubmit(newSchedule);
                    dnncontext.SubmitChanges();
                    MyGlobals.LogMessage("Created scheduler Entry for Brafton Importer", 1);
                    EnableAuto.Text = "Turn Off Automatic Import";
                }
                else
                {
                    if (Convert.ToBoolean(sc.Enabled))
                    {
                        sc.Enabled = false;
                        MyGlobals.LogMessage("The Schedule has been disabled", 1);
                        EnableAuto.Text = "Turn On Automatic Import";
                    }
                    else
                    {
                        sc.Enabled = true;
                        MyGlobals.LogMessage("The Schedule has been enabled", 1);
                        EnableAuto.Text = "Turn Off Automatic Import";
                    }
                    dnncontext.SubmitChanges();
                }
                globalErrorMessage.Text = MyGlobals.MyGlobalError;
                MessageImageVisible();
                MyGlobals.MyGlobalError = "";
            }
        }
        public void runBraftonImporter(object sender, EventArgs e)
        {
            DateTime nx = new DateTime(1970, 1, 1);
            TimeSpan ts = DateTime.UtcNow - nx;
            string timestamp = ((int)ts.TotalSeconds).ToString();

            int LastImport = Settings.Contains("LastImport") ? Int32.Parse(Settings["LastImport"].ToString()) : 0;
            int diff = Int32.Parse(timestamp) - LastImport;
            if (diff > 10800)
            {
                Brafton.DotNetNuke.BraftonSchedule newSched = new Brafton.DotNetNuke.BraftonSchedule();
                MyGlobals.BraftonViewModuleId.Add(ModuleId);
                var modules = new ModuleController();
                modules.UpdateModuleSetting(ModuleId, "LastImport", timestamp);
                newSched.DoWork();
                
            }

        }
        protected void CheckUpdate()
        {
            string msg = string.Empty;
            DesktopModuleInfo braftonModule = DesktopModuleController.GetDesktopModuleByFriendlyName("Brafton Content Importer");
            string SystemVersion = braftonModule.Version.ToString();
            Version currentVersion = new Version(SystemVersion);

            string url = "http://development.updater.brafton.com/u/dotnetnuke/update";
            HttpWebRequest report = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)report.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            var json = new JavaScriptSerializer();
            //Dictionary<string, string> updaterResponse = new Dictionary<string, string>();
            var jsonData = json.Deserialize<Dictionary<string, string>>(result.ToString());
            
            string updatedVersion = jsonData["new_version"].ToString();
            
            Version remoteVerion = new Version(updatedVersion);
            int compare_result = currentVersion.CompareTo(remoteVerion);
            if (compare_result < 0)
            {
                UpdateAvailable.Visible = true;
                msg = "There is an update available for your Importer.<br/>  You are currently running V" + SystemVersion + ", However V" + updatedVersion + " is available.<br/>You can Download the updated version <a href='" + jsonData["download_link"].ToString() + "'>HERE</a>";
            }
            UpdateMessage.Text = msg;
            
        }
    }
}
