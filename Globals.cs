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
        public static string MyGlobalError = "Don't Panic"; // can change because not const
        //public static string ArtOrBlog = "unk";
        //public static int IncludeVideo = 0;
        //public static int IncludeUpdatedFeedContent = 0;
        //public static string VideoBaseURL = "livevideo.brafton.com";
        //public static string VideoPhotoURL = "pictures.brafton.com";
        //public static string VideoPublicKey = "xxxxxx";
        //public static string VideoSecretKey = "xxxxxx";
        //public static int? VideoFeedNumber = 0;
        //public static string VideoFeedText = "xxxxxx";
        //public static string api = "xxxxxx";
        //public static string baseUrl = "xxxxxx";
        //public static string DomainName = "xxxxxx";

        //These are temp holders for values to entered into the db
        //public static string tempID = "0";
        //public static string tempTitle = "xxxxxx";
        //public static string tempExtract = "xxxxxx";
        //public static string tempcontent = "xxxxxx";
        //public static DateTime tempDate = DateTime.Today;
        //public static string tempPaths = "xxxxxx";

        //public static string imageInfo = "xxxxxx";
        //public static string imageID = "xxx";

        //public static string CompleteContent = "";
        //public static string CompleteExtract = "";
        //public static int brafID = 0;

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

                         #region Fill Globals
                         Brafton_table pk = dnnContext.Brafton_tables.FirstOrDefault(x => x.Content == "1");

                         if (pk != null)
                         {
                             //api = pk.Api != null ? pk.Api : api;

                             //baseUrl = pk.BaseUrl != null ? pk.BaseUrl : baseUrl;

                             //DomainName = pk.DomainName != null ? pk.DomainName : DomainName;

                             //VideoPublicKey = pk.VideoPublicKey != null ? pk.VideoPublicKey : VideoPublicKey;

                             //VideoSecretKey = pk.VideoSecretKey != null ? pk.VideoSecretKey : VideoSecretKey;

                             //VideoFeedNumber = pk.VideoFeedNumber != null ? pk.VideoFeedNumber : VideoFeedNumber;

                             //VideoFeedText = VideoFeedNumber.ToString();

                             //VideoBaseURL = pk.VideoBaseUrl != null ? pk.VideoBaseUrl : VideoBaseURL;

                             //VideoPhotoURL = pk.VideoPhotoURL != null ? pk.VideoPhotoURL : VideoPhotoURL;

                             //CurrentBlog = pk.BlogId != null ? pk.BlogId : CurrentBlog;

                             //currentPortal = pk.PortalId != null ? pk.PortalId : currentPortal;

                             //TabID = pk.TabId != null ? pk.TabId : TabID;

                             //Author = pk.AuthorId != null ? pk.AuthorId : Author;

                             ////ArtOrBlog = (pk.Api != null && pk.VideoSecretKey != null) ? "both" : "articles";

                             ////Decide whether photo or video
                             //if (pk.VideoSecretKey != null)
                             //{
                             //    ArtOrBlog = "video";
                             //}

                             //if (pk.Api != null)
                             //{
                             //    ArtOrBlog = "articles";
                             //}

                             //if (pk.Api != null && pk.VideoSecretKey != null)
                             //{
                             //    ArtOrBlog = "both";
                             //}

                         #endregion
                         }
                     
                     
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