using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;


namespace Brafton.Modules.Globals
{
    

    public static class MyGlobals
    {
        //public const string Prefix = "ID_"; // cannot change
        public static string MyGlobalError = string.Empty; // can change because not const

        //Variables used 
        public static int? CurrentBlog = 0;
        public static int? currentPortal = 0;
        public static int Limit = 30;
        public static int? TabID = 0;
        public static int? Author = 1;
        public static List<int> BraftonViewModuleId = new List<int>();
        public static Boolean debugmode = false;

        public static void AdminControls(object sender, EventArgs e)
        {

        }
            public static void PopGlobals()

                 {
                     using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
                     {
                     
                     }

                 }
            public static void LogMessage(string msg, int severity = 0)
            {
                if (severity == 1 || debugmode)
                {
                    MyGlobalError = MyGlobalError + msg + "<br/>";
                }
            }

    }

    public class XmlBase
    {




        public void WriteToXml()
        {

        }

        public void ReadToXml()
        {

        }

    }




}