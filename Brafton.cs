
using System;
using System.IO;
using System.Web;

using DotNetNuke.Services.Scheduling;


using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Utilities;

using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

using System.Data.SqlClient;
using System.Xml;

using DotNetNuke.Entities.Portals;
using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Common;
using System.Text.RegularExpressions;

using System.Text;
using System.Web.Hosting;

using System.Net;
using System.Drawing;
using System.Reflection;
using Brafton.Modules.Globals;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;

using AdferoVideoDotNet.AdferoArticlesVideoExtensions;
using Brafton.Modules.VideoImporter;
using AdferoVideoDotNet.AdferoArticles;
using AdferoVideoDotNet.AdferoPhotos.Photos;
using AdferoVideoDotNet.AdferoPhotos;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs;

using Brafton.BraftonError;

namespace Brafton.DotNetNuke
{

    public class BraftonSchedule : SchedulerClient
    {
        //Connection properties
        public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        public SqlConnection con2;
        public SqlCommand cmd = new SqlCommand();
        public SqlCommand cmd5;
        public int? BraftonModuleId = null;
        public Hashtable modSettings;
        public static Hashtable modTabSettings;
        public ErrorReporting BraftonErrorSystem;

        public BraftonSchedule(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            ScheduleHistoryItem = objScheduleHistoryItem;
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + ScheduleHistoryItem.ScheduleSource;
            
        }
        
        // Note we need to give a default constructor when override it
        public BraftonSchedule() : base() { }

        public override void DoWork()
        {
            try
            {
                
                if (MyGlobals.BraftonViewModuleId.Count.ToString() == "0")
                {
                    retrieveAllModules();
                }
                foreach(int NewBrafId in MyGlobals.BraftonViewModuleId){
                    BraftonModuleId = NewBrafId;

                    var modInfo = new ModuleController().GetModule((int)BraftonModuleId);
                    modSettings = modInfo.ModuleSettings;

                    var modTabInfo = new ModuleController().GetTabModule((int)modInfo.TabModuleID);
                    modTabSettings = modTabInfo.TabModuleSettings;

                    if (BraftonParameters())
                    {
                        BraftonErrorSystem = new ErrorReporting(modSettings);
                        string description = updateScriptDebug();

                        //determine what is to run
                        //string articleResult = runArticleImporter();
                        //string videoResult = runVideoImporter();
                    }
                    else
                    {
                        throw new Exception("can't run importer");
                    }
                }
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception exc)
            {
                // report a failure
                ScheduleHistoryItem.Succeeded = false;

                // log the exception into
                // the scheduler framework
                ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc.ToString());

                // call the Errored method
                Errored(ref exc);

                //log the exception into the DNN core
                Exceptions.LogException(exc);
                
                if(ErrorReporting.Loop.ToString() == "1" && ErrorReporting.CheckErrors(exc.GetType().Name)){
                    MyGlobals.LogMessage("There was a problem with the Importer.  A message has been sent to your CMS in an effort to expedite our troubleshooting efforts", 1);
                    //Make remote post to error api here
                    BraftonErrorReport(exc.ToString());
                }
                MyGlobals.LogMessage("There was an error importing your content. Please check your Event Viewer for details." + Environment.NewLine + "If the problem persists please contact techsupport@brafton.com for assistance.", 1);
            }


        }

        private bool BraftonParameters()
        {
            //this will look for the settings and ensure everything is kosher
            return true;
        }

        private void retrieveAllModules()
        {
            string target = "BraftonImporter7_02_02";
            using (DataClasses1DataContext dnncontext = new DataClasses1DataContext())
            {
                var moduleList = dnncontext.ContentItems.Where(x => x.Content.Contains(target));
                StringBuilder moduleIds = new StringBuilder();
                foreach(var modId in moduleList){
                    MyGlobals.BraftonViewModuleId.Add(modId.ModuleID);
                    moduleIds.Append(modId.ModuleID.ToString() + ": ");
                }
                MyGlobals.LogMessage("Module Ids from the query are : " + moduleIds.ToString());
            }
            
        }
        //this will call the C# class for brafton Error reporting still to write.
        private void BraftonErrorReport(string p)
        {
            BraftonErrorSystem.RemotePost(p);
        }

        

        /* Category Addition Method for Category url from articles object
         * Category addition method for use going forward.
         * */
        public List<int> addCategories(string catUrlHolder, string format = "all")
        {
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                List<string> CategoryNames = new List<string>();
                List<int> CategoryIds = new List<int>();
                XmlNode cname;

                XmlDocument tmpXMLDoc = new XmlDocument();
                tmpXMLDoc.Load(catUrlHolder);
                //get total of cats
                int length = tmpXMLDoc.GetElementsByTagName("name").Count;

                for (int i = 0; i < length; i++)
                {
                    cname = tmpXMLDoc.GetElementsByTagName("name")[i];
                    if (cname != null)
                    {
                        CategoryNames.Add(cname.InnerText);
                    }
                    else
                    {
                        CategoryNames.Add("Uncategorized");
                    }
                }
                if (CategoryNames.Count == 0)
                {
                    CategoryNames.Add("Uncategorized");
                }
                foreach (string Name in CategoryNames)
                {
                    Taxonomy_Term catcheck = dnnContext.Taxonomy_Terms.FirstOrDefault(x => x.Name == Name);
                    if (catcheck != null)
                    {
                        CategoryIds.Add(catcheck.TermID);
                    }
                    else
                    {
                        Taxonomy_Term newTerm = new Taxonomy_Term();
                        newTerm.Name = Name;
                        Taxonomy_Vocabulary BlogVocabId = dnnContext.Taxonomy_Vocabularies.FirstOrDefault(x => x.Name == "Blog Categories");
                        if (BlogVocabId != null)
                        {
                            newTerm.VocabularyID = BlogVocabId.VocabularyID;
                            dnnContext.Taxonomy_Terms.InsertOnSubmit(newTerm);
                            dnnContext.SubmitChanges();
                            CategoryIds.Add(newTerm.TermID);
                        }
                    }
                }
                return CategoryIds;
            } // end of using
        }

        /* Category Addition Method for list of Categorys from Name list of Video object
         * Category addition method for use going forward.
         * */
        public List<int> addCategories(List<string> CategoryNames)
        {
            List<int> CategoryIds = new List<int>();
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                foreach (string Name in CategoryNames)
                {
                    Taxonomy_Term catcheck = dnnContext.Taxonomy_Terms.FirstOrDefault(x => x.Name == Name);
                    if (catcheck != null)
                    {
                        CategoryIds.Add(catcheck.TermID);
                    }
                    else
                    {
                        Taxonomy_Term newTerm = new Taxonomy_Term();
                        newTerm.Name = Name;
                        Taxonomy_Vocabulary BlogVocabId = dnnContext.Taxonomy_Vocabularies.FirstOrDefault(x => x.Name == "Blog Categories");
                        if (BlogVocabId != null)
                        {
                            newTerm.VocabularyID = BlogVocabId.VocabularyID;
                            dnnContext.Taxonomy_Terms.InsertOnSubmit(newTerm);
                            dnnContext.SubmitChanges();
                            CategoryIds.Add(newTerm.TermID);
                        }
                    }
                }
            }
            return CategoryIds;
        }

        /* Category to attach Method Overload
         * Used to attached multiple categories to an article passing in a list<int>
         * */
        public void addBlogCategories(int ContentId, List<int> tempCatId)
        {
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                foreach (int CatId in tempCatId)
                {
                    ContentItems_Tag cit = dnnContext.ContentItems_Tags.FirstOrDefault(x => x.ContentItemID == ContentId && x.TermID == CatId);
                    if (cit == null)
                    {
                        ContentItems_Tag newbece = new ContentItems_Tag();
                        newbece.ContentItemID = ContentId;
                        newbece.TermID = CatId;
                        dnnContext.ContentItems_Tags.InsertOnSubmit(newbece);
                        dnnContext.SubmitChanges();
                    }
                }
            }
        }

        public string updateScriptDebug()
        {

            string typeToImport = modTabSettings["RadioButtonList1"].ToString();
            MyGlobals.debugmode = Int32.Parse(modSettings["DebugMode"].ToString()) == 1 ? true : false;
            MyGlobals.LogMessage("Importing " + typeToImport, 1);

            string appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;
            DesktopModuleInfo braftonModule = DesktopModuleController.GetDesktopModuleByFriendlyName("Brafton Content Importer");

            MyGlobals.LogMessage("The ContentItemID id is " + braftonModule.ContentItemId);
            MyGlobals.LogMessage("The ModuleID id is " + BraftonModuleId);

            string newsURL = modSettings["APIKey"].ToString();
            MyGlobals.LogMessage("The newsurl is " + newsURL);

            string baseUrl = "http://api." + modSettings["BrandUrl"].ToString() + "/";
            MyGlobals.LogMessage("The current tabId is " + modTabSettings["currentTabId"]);

            int PageTabId = Int32.Parse(modTabSettings["currentTabId"].ToString());
            int intPortalID = Int32.Parse(modTabSettings["currentPortalId"].ToString());

            MyGlobals.CurrentBlog = Int32.Parse(modTabSettings["blogIdDrpDwn"].ToString());
            MyGlobals.currentPortal = intPortalID;

            MyGlobals.LogMessage("The Clients API URL is " + baseUrl + newsURL);
            MyGlobals.LogMessage("The tab id is " + PageTabId);
            MyGlobals.LogMessage("The Portal id is " + intPortalID);

            //Blog_Entries table variables
            string artBlogID;
            string title;
            string entry;
            DateTime addedDate;
            string description = string.Empty;
            string description_debug = string.Empty;
            bool published = true;
            bool allowComments = false;
            bool displayCopyright = false;
            string photoURL;
            string byline;
            string caption;
            string imgName = string.Empty;

            //For Limit
            int l = 0;
                //Possible to delete 
            int limit = MyGlobals.Limit;
            if (typeToImport.Equals("articles", StringComparison.OrdinalIgnoreCase) || typeToImport.Equals("both", StringComparison.OrdinalIgnoreCase))
            {
                ApiContext ac = new ApiContext(newsURL, baseUrl);
            #region Article Loop
            foreach (newsItem ni in ac.News)
            {
                artBlogID = ni.id.ToString();
                MyGlobals.LogMessage("Checking for article " + artBlogID);

                title = ni.headline;
                entry = ni.text;
                description = ni.extract;
                addedDate = ni.publishDate;
                photoURL = ni.PhotosHref;
                byline = ni.byLine;
                DateTime today = DateTime.Today;

                using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
                {
                    Blog_Post be = dnnContext.Blog_Posts.FirstOrDefault(x => x.BraftonID == artBlogID);
                    
                    if ( (be != null && Int32.Parse(modSettings["UpdateContent"].ToString()) == 0) )
                    {
                        continue;
                    }

                    ContentItem ContentSpace = new ContentItem();
                    Blog_Post newBlogEntry = new Blog_Post();

                    Dictionary<string, dynamic> contentItem = new Dictionary<string,dynamic>();
                    Dictionary<string, dynamic> blogItem = new Dictionary<string, dynamic>();

                    int ContentTypeId = GetcontentTypeID();
                    int BlogId = Int32.Parse(modTabSettings["blogIdDrpDwn"].ToString());
                    contentItem.Add("ContentTypeID", ContentTypeId);
                    contentItem.Add("CreatedOnDate", addedDate);
                    contentItem.Add("TabID", -1);
                    contentItem.Add("ModuleID", GetModuleId(BlogId));
                    contentItem.Add("CreatedByUserID", Int32.Parse(modTabSettings["blogUsersDrpDwn"].ToString()));
                    contentItem.Add("LastModifiedOnDate", addedDate);
                    ContentItem AddedContentItem = be == null ? AddContentItem(contentItem) : AddContentItem(contentItem, be.ContentItemId);
                    contentItem.Clear();

                    int ContentId = AddedContentItem.ContentItemID;
                    MyGlobals.LogMessage("Content ID is " + ContentId);
                    photo.Instance photoInstance = null;
                    photo img = null;
                    imgName = null;
                    #region IMAGE HANDLER
                    try
                    {
                        img = ni.photos.First();

                        photo.Instance photoInstanceLarge = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Large).FirstOrDefault();
                        photo.Instance photoInstanceMedium = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                        photo.Instance photoInstanceSmall = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Small).FirstOrDefault();
                               

                        if (photoInstanceLarge != null)
                        {
                            photoInstance = photoInstanceLarge;
                        }
                        else if (photoInstanceMedium != null)
                        {
                            photoInstance = photoInstanceMedium;
                        }
                        else if (photoInstanceSmall != null)
                        {
                            photoInstance = photoInstanceSmall;
                        }

                    }
                    catch
                    {
                        //No Image found for article
                    }
                    //Checks to see if medium images are enabled on the feed 
                    if (photoInstance != null)
                    {
                        photoURL = photoInstance.url.ToString();
                        caption = img.caption.ToString();

                        //Checks to see if the feed has photos enabled.
                        if (!string.IsNullOrEmpty(photoURL))
                        {

                            GetImages retrieveImage2 = new GetImages(photoURL, entry, description, appPath, caption, AddedContentItem.ContentItemID);
                            retrieveImage2.DownloadImageDebug();
                            imgName = retrieveImage2._imageName;
                            if (modTabSettings["IncludeImages"].ToString() == "1")
                            {
                                //description_debug = retrieveImage2._description;
                                description = retrieveImage2._description;
                                entry = retrieveImage2._entry;
                                imgName = null;
                            }
                        }

                    }

                    #endregion IMAGE HANDLER
                            
                    contentItem.Add("Content", entry);
                    AddedContentItem = AddContentItem(contentItem, AddedContentItem.ContentItemID);

                    blogItem.Add("ContentItemId", AddedContentItem.ContentItemID);
                    blogItem.Add("BlogID", BlogId);
                    blogItem.Add("Title", title);
                    blogItem.Add("Published", published);
                    blogItem.Add("PublishedOnDate", addedDate);
                    blogItem.Add("Summary", description);
                    blogItem.Add("AllowComments", allowComments);
                    blogItem.Add("DisplayCopyright", displayCopyright);
                    blogItem.Add("BraftonID", artBlogID);
                    blogItem.Add("Image", imgName);
                    blogItem.Add("LastUpdatedOn", today);
                    Blog_Post AddedBlogItem = be == null? AddBlogItem(blogItem) : AddBlogItem(blogItem, Int32.Parse(artBlogID));
                    //Categories
                    string catUrlHolder = ni.CategoriesHref;
                    List<int> tempCatID = addCategories(catUrlHolder);
                    if (tempCatID.Count > 0)
                    {
                        addBlogCategories(ContentId, tempCatID);
                    }
                    //increment limit
                    l++;
                    ErrorReporting.Loop++;
                }
            }
            #endregion Article Loop
            }//end of articles
            
            if( (typeToImport.Equals("both", StringComparison.OrdinalIgnoreCase) || typeToImport.Equals("video", StringComparison.OrdinalIgnoreCase) ) )
            {
                MyGlobals.LogMessage("Starting Videos");
                ErrorReporting.Loop = 1;
                   ImportVideos();
             }
            string returnVal = "include video";
            return returnVal;
        }


        private int GetModuleId(int BlogId)
        {
            int ModId;
            //find the module id from the blog id.
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                Blog_Blog blog = dnnContext.Blog_Blogs.FirstOrDefault(x => x.BlogID == BlogId);
                ModId = blog.ModuleID;
            }
            return ModId;
        }

        private int GetcontentTypeID()
        {
            int ContentId;
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                string type = "DNN_Blog_Post";
                ContentType cid = dnnContext.ContentTypes.FirstOrDefault(x => x.ContentType1 == type);
                ContentId = cid.ContentTypeID;
                MyGlobals.LogMessage("Content type Id is : " + ContentId);
            }
            return ContentId;
        }

        #region Add New ContentItem
        public ContentItem AddContentItem(Dictionary<string, dynamic> Contents, int? ContentItemId = null)
        {
            using (DataClasses1DataContext dnnItem = new DataClasses1DataContext())
            {
                ContentItem NewContentItem;
                Dictionary<string, dynamic>.KeyCollection keys = Contents.Keys;
                if (ContentItemId == null)
                {
                    NewContentItem = new ContentItem();
                    foreach (string key in keys)
                    {
                        NewContentItem.GetType().GetProperty(key).SetValue(NewContentItem, Contents[key], null);

                    }
                    dnnItem.ContentItems.InsertOnSubmit(NewContentItem);                    
                }
                else
                {
                    NewContentItem = dnnItem.ContentItems.FirstOrDefault(x => x.ContentItemID == ContentItemId);
                    if (NewContentItem == null)
                    {
                        return null;
                    }
                    foreach (string key in keys)
                    {
                        NewContentItem.GetType().GetProperty(key).SetValue(NewContentItem, Contents[key], null);
                        
                    }
                }
                dnnItem.SubmitChanges();
                return NewContentItem;
            }
        }
        #endregion Add New ContentItem
        #region Add New Blog Item
        public Blog_Post AddBlogItem(Dictionary<string, dynamic> Contents, int? BraftonID = null)
        {
            using (DataClasses1DataContext dnnItem = new DataClasses1DataContext())
            {
                Blog_Post newBlogPost;
                Dictionary<string, dynamic>.KeyCollection keys = Contents.Keys;
                if (BraftonID == null)
                {
                    newBlogPost = new Blog_Post();
                    foreach (string key in keys)
                    {
                        newBlogPost.GetType().GetProperty(key).SetValue(newBlogPost, Contents[key], null);

                    }
                    dnnItem.Blog_Posts.InsertOnSubmit(newBlogPost);
                }
                else
                {
                    newBlogPost = dnnItem.Blog_Posts.FirstOrDefault(x => x.BraftonID == BraftonID.ToString());
                    if (newBlogPost == null)
                    {
                        return null;
                    }
                    foreach (string key in keys)
                    {
                        newBlogPost.GetType().GetProperty(key).SetValue(newBlogPost, Contents[key], null);

                    }
                }
                dnnItem.SubmitChanges();
                return newBlogPost;
            }
        }
        #endregion

        #region Video Import
        public void ImportVideos()
        {
            string publicKey = modSettings["VideoPublic"].ToString();
            string secretKey = modSettings["VideoPrivate"].ToString();
            MyGlobals.LogMessage("public is " + publicKey);
            MyGlobals.LogMessage("private is " + secretKey);
            int feedNumber = Int32.Parse(modSettings["VideoFeedId"].ToString());
            DateTime today = DateTime.Today;

            int PageTabId = Int32.Parse(modTabSettings["currentTabId"].ToString());

            string baseUrl = "http://livevideo.api." + modSettings["BrandUrl"].ToString() + "/v2/";
            string basePhotoUrl = "http://pictures." + modSettings["BrandUrl"].ToString() + "/v2/";
 
            AdferoVideoClient videoClient = new AdferoVideoClient(baseUrl, publicKey, secretKey);
            AdferoClient client = new AdferoClient(baseUrl, publicKey, secretKey);
            AdferoPhotoClient photoClient = new AdferoPhotoClient(basePhotoUrl);

            AdferoVideoOutputsClient xc = new AdferoVideoClient(baseUrl, publicKey, secretKey).VideoOutputs();
             
            AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient photos = client.ArticlePhotos();

            AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedsClient feeds = client.Feeds();
            AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedList feedList = feeds.ListFeeds(0, 10);
            
            AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticlesClient articles = client.Articles();
            AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleList articleList = articles.ListForFeed(feedList.Items[feedNumber].Id, "live", 0, 100);

            int articleCount = articleList.Items.Count;
            AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoriesClient categories = client.Categories();
            int limit = 0;
            foreach (AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleListItem item in articleList.Items)
            {
                if (limit >= MyGlobals.Limit)
                { 
                    return;
                }
                using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
                {
                    int brafId = item.Id;
                    Blog_Post be = dnnContext.Blog_Posts.FirstOrDefault(x => x.BraftonID == brafId.ToString());

                    if ((be != null && Int32.Parse(modSettings["UpdateContent"].ToString()) == 0))
                    {
                        continue;
                    }
                    Dictionary<string, dynamic> contentItem = new Dictionary<string, dynamic>();
                    Dictionary<string, dynamic> blogItem = new Dictionary<string, dynamic>();

                    
                    AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticle article = articles.Get(brafId);

                    string presplashLink = article.Fields.ContainsKey("preSplash") ? article.Fields["preSplash"] : "";

                    string Code = BraftonVideoClass.BuildEmbedCode(videoClient, xc, modTabSettings, brafId, presplashLink);
                    string addDate = article.Fields["date"];

                    int ContentTypeId = GetcontentTypeID();
                    int BlogId = Int32.Parse(modTabSettings["blogIdDrpDwn"].ToString());

                    contentItem.Add("ContentTypeID", ContentTypeId);
                    contentItem.Add("CreatedOnDate", DateTime.Parse(article.Fields["date"]));
                    contentItem.Add("TabID", -1);
                    contentItem.Add("ModuleID", GetModuleId(BlogId));
                    contentItem.Add("CreatedByUserID", Int32.Parse(modTabSettings["blogUsersDrpDwn"].ToString()));
                    contentItem.Add("LastModifiedOnDate", DateTime.Parse(article.Fields["date"]));

                    ContentItem AddedContentItem = be == null ? AddContentItem(contentItem) : AddContentItem(contentItem, be.ContentItemId);
                    contentItem.Clear();

                    string entry = article.Fields["content"];
                    string extract = article.Fields["extract"];
                    string description = string.Empty;
                    string appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;
                    //string description_debug;
                    string imgName = null;

                    AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotoList phot = photos.ListForArticle(brafId, 0, 20);
                    if (!string.IsNullOrEmpty(phot.Items[0].Id.ToString()))
                    {
                        int imageID = phot.Items[0].Id;
                        int photoID = photos.Get(imageID).SourcePhotoId;
                        string alttext = photos.Get(imageID).Fields["altText"];
                        string caption = photos.Get(imageID).Fields["caption"];
                        //MyGlobals.imageID = imageID.ToString();
                        string photoURL = photoClient.Photos().GetScaleLocationUrl(photoID, "y", 500).LocationUri;
                        MyGlobals.LogMessage("The image id is " + imageID + " the photoID is " + photoID + " the photo url is " + photoURL);
                        GetImages retrieveImage2 = new GetImages(photoURL, entry, extract, appPath, caption, AddedContentItem.ContentItemID);
                        retrieveImage2.DownloadImageDebug();
                        imgName = retrieveImage2._imageName;
                        if (modTabSettings["IncludeImages"].ToString() == "1")
                        {
                            //description_debug = retrieveImage2._description;
                            extract = retrieveImage2._description;
                            entry = retrieveImage2._entry;
                            imgName = null;
                        }
                        
                    }

                    contentItem.Add("Content", Code + entry);
                    //Add the content to the actual content piece
                    AddedContentItem = AddContentItem(contentItem, AddedContentItem.ContentItemID);

                    blogItem.Add("ContentItemId", AddedContentItem.ContentItemID);
                    blogItem.Add("BlogID", BlogId);
                    blogItem.Add("Title", article.Fields["title"]);
                    blogItem.Add("Published", true);
                    blogItem.Add("PublishedOnDate", DateTime.Parse(article.Fields["date"]));
                    blogItem.Add("Summary", extract);
                    blogItem.Add("AllowComments", false);
                    blogItem.Add("DisplayCopyright", false);
                    blogItem.Add("BraftonID", brafId.ToString());
                    blogItem.Add("Image", imgName);
                    blogItem.Add("LastUpdatedOn", today);
                    Blog_Post AddedBlogItem = be == null ? AddBlogItem(blogItem) : AddBlogItem(blogItem, brafId);

                    #region Categories
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //Add categories to Blog_Entry_Categories table 
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    List<string> CategoryNames = new List<string>();
                    foreach (AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoryListItem cats in client.Categories().ListForArticle(brafId, 0, 20).Items)
                    {
                        CategoryNames.Add(categories.Get(cats.Id).Name);

                    }
                    List<int> CatIdList = addCategories(CategoryNames);
                    if (CatIdList.Count > 0)
                    {
                        addBlogCategories(AddedContentItem.ContentItemID, CatIdList);
                    }
                    #endregion Categories
                }
                limit++;
                ErrorReporting.Loop++;
        }
        #endregion Video Import


    }
    }
}