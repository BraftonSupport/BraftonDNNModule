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

        //Application path variable
        //public string appPath;
        public Boolean isEditable;

        //Local Variables
        //public int checkBlogModule;
        //public int checkFriendURLS;
        //public int checkAuthor;
        //public int checkBlogCreated;
        //public int checkNewsAPI;
        //public int checkBaseUrl;
        //public int checkLimit;
        //public int checkBlogID;
        //public int checkVidID;
        //public Dictionary<string, int> checkAll = new Dictionary<string, int>();

        ////Global Variables
        //public int IncludeUpdatedFeedContent;
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
            //runBraftonImporter();
            /*
             * TO DO: 
             * Need to create a check if video article somehow, and determine if we should add the atlantis scripts to the head.
             * */
            ModuleConfiguration.ModuleTitle = "";
            if (braftonViewable())
            {
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
            CheckStatus();
            /* 
             * This will be added when using without the blog module in the version 7.1.0
            HtmlMeta meta = new HtmlMeta();
            meta.Name = "brafton";
            meta.Content = "my brafton";
            this.Page.Header.Controls.Add(meta);
             * */

        }
        protected void Import_Click(object sender, EventArgs e)
        {
            Brafton.DotNetNuke.BraftonSchedule newSched = new Brafton.DotNetNuke.BraftonSchedule();
            MyGlobals.BraftonViewModuleId.Add(ModuleId);
            newSched.DoWork();          
            globalErrorMessage.Text = MyGlobals.MyGlobalError;
            MyGlobals.MyGlobalError = "";
        }

        protected void show_globals(object sender, EventArgs e)
        {
            CheckStatus();
            globalErrorMessage.Text = MyGlobals.MyGlobalError;
            MyGlobals.MyGlobalError = "";
        }
        protected void CheckStatus()
        {
            /* This will
             * this will check if all relevant data has been set
             * check for valid api key and domain set,
             * check for if video is enabled and public private and feedid are all set
             * */
            checkedStatusLabel.Text = "All options have been set properly";
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
                    MyGlobals.LogMessage("Creating schedule Entry", 1);
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
                }
                else
                {
                    if (Convert.ToBoolean(sc.Enabled))
                    {
                        sc.Enabled = false;
                        MyGlobals.LogMessage("The Schedule has been disabled", 1);
                    }
                    else
                    {
                        sc.Enabled = true;
                        MyGlobals.LogMessage("The Schedule has been enabled", 1);
                    }
                    dnncontext.SubmitChanges();
                }
                globalErrorMessage.Text = MyGlobals.MyGlobalError;
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
    }
}
