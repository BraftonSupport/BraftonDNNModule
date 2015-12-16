
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
        public BraftonSchedule(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            ScheduleHistoryItem = objScheduleHistoryItem;

        }

        // Note we need to give a default constructor when override it
        public BraftonSchedule() : base() { }

        #region Debugging


   
        public override void DoWork()
        {
            try
            {
                // start the process
                //string ups = updateScript();
                //updateScript();
                string description = updateScriptDebug();
                //return description; //left for quick error reporting
                // then report success to the scheduler framework
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
                //string ErrorMessage = "EXCEPTION";
                //return ErrorMessage;
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Exception error";

                //Make remote post to error api here
                BraftonErrorReport(exc.ToString());
            }


        }
        //this will call the C# class for brafton Error reporting still to write.
        private void BraftonErrorReport(string p)
        {
            
        }
        #endregion

        #region GetBraftonSettings
        public string getNewsURL()
        {

            cmd.CommandText = "SELECT Api FROM Brafton WHERE content='1'";
            string feedURL = cmd.ExecuteScalar().ToString();
            return feedURL;
        }

        public string getBaseURL()
        {
            cmd.CommandText = "SELECT BaseUrl FROM Brafton WHERE content='1'";
            string baseURL = cmd.ExecuteScalar().ToString();
            return baseURL;
        }

        int getBlogID()
        {
            cmd.CommandText = "Select BlogId from Brafton Where Content = '1'";
            int blogID = (int)cmd.ExecuteScalar();
            return blogID;
        }
        public int getUpdatedContent()
        {
            int? UpdatedContent = 0;
            //cmd5 = new SqlCommand();

            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {
                Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.BraftonTable uf = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                //Insert into category
                if (uf != null)

                {
                    if (uf.IncUpdatedFeedContentValue != null)
                    {
                        //int? v1 =  uf.IncUpdatedFeedContentValue;
                        //UpdatedContent = v1 ?? default(int);
                        UpdatedContent = uf.IncUpdatedFeedContentValue;
                        

                    }
                    else
                    {
                        UpdatedContent = 0;
                        uf.IncUpdatedFeedContentValue = 0;

                        dnnContext.SubmitChanges();
                        
                    }
                   
                }
                
            }
            int? v1 = UpdatedContent;
            int v2 = v1 ?? default(int);
            return v2;
        }

        public string getUpdatedContent2()
        {
            var xx = "Select IncUpdatedFeedContentValue from Brafton Where Content = '1'";


            return xx;
        }

        int getPortalID()
        {
            cmd.CommandText = "Select PortalId from Brafton Where Content = '1'";
            int intPortalID = (int)cmd.ExecuteScalar();
            return intPortalID;
        }

        int getTabID()
        {
            cmd.CommandText = "Select TabId from Brafton Where Content = '1'";
            int PageTabID = (int)cmd.ExecuteScalar();
            return PageTabID;
        }



        int getLimit()
        {
            cmd.CommandText = "Select Limit from Brafton Where Content = '1'";
            if (!DBNull.Value.Equals(cmd.ExecuteScalar()))
            {
                return (int)cmd.ExecuteScalar();
            }
            else
            {

                return 20;
            }
        }

        string getDomainName()
        {
            cmd.CommandText = "Select DomainName from Brafton Where Content = '1'";
            string domainName = (string)cmd.ExecuteScalar();
            connection.Close();
            cmd.Dispose();
            return domainName;
        }

        #endregion

        #region SupportFuncs
        public string strip(string alias)
        {
            // invalid chars, make into spaces
            alias = Regex.Replace(alias, @"[^a-zA-Z0-9\s-]", "");
            // convert multiple spaces/hyphens into one space       
            alias = Regex.Replace(alias, @"[\s-]+", " ").Trim();
            // hyphens
            alias = Regex.Replace(alias, @"\s", "-");

            return alias;
        }

        //////////////Add Categories///////////////////////

        //end add BlogCategories

        public int addCategories(string catUrlHolder)
        {
            //using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                string category;
                int tempCatID;

                XmlDocument tmpXMLDoc = new XmlDocument();
                tmpXMLDoc.Load(catUrlHolder);
                //get total of cats
                int length = tmpXMLDoc.GetElementsByTagName("name").Count;
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "total cats is  : " + length + "<br/>";
                XmlNode name = tmpXMLDoc.GetElementsByTagName("name")[0];
                //XmlNode name = tmpXMLDoc.GetElementsByTagName("name")[0];

                if (name != null)
                {
                    category = name.InnerText;

                }
                else
                {
                    category = "Uncategorized";
                }

                //Blog_Category bc = dnnContext.Blog_Categories.FirstOrDefault(x => x.Category == category);
                Taxonomy_Term bc = dnnContext.Taxonomy_Terms.FirstOrDefault(x => x.Name == category);
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "First Category is  : " + category + "<br/>";
                if (bc != null)
                {
                    //get the catID
                    tempCatID = bc.TermID;
                    
                }
                else
                {
                    //create the slug

                    //string tempCatSlug = strip(category) + ".aspx";

                    //Insert cat
                   
                    Taxonomy_Term newbc = new Taxonomy_Term();

                    newbc.Name = category;
                    //newbc.ParentTermID = 0;
                    //newbc.Slug = tempCatSlug;
                    newbc.VocabularyID = 3;

                    dnnContext.Taxonomy_Terms.InsertOnSubmit(newbc);
                    dnnContext.SubmitChanges();
                    //set the catID

                    tempCatID = newbc.TermID;
                   //need to add the termid now to the blog_terms table termid = termid and view order is?

                }
                return tempCatID;
            } // end of using
        }
        //end add categories

        public void addBlogCategories(int ContentId, int tempCatId)
        {
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                //for update get the content_tag entry with contentId = ContentId and reset items
                ContentItems_Tag newbece = new ContentItems_Tag();
                newbece.ContentItemID = ContentId;
                newbece.TermID = tempCatId;
                dnnContext.ContentItems_Tags.InsertOnSubmit(newbece);
                dnnContext.SubmitChanges();

            }
        }
        #endregion

 

        #region Debugging update script
        public string updateScriptDebug()
        {
            if (MyGlobals.ArtOrBlog == "articles" || MyGlobals.ArtOrBlog == "both")
            {

            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Start of articles<br>";
            connection.Open();
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Open Connection<br>";
            cmd.Connection = connection;
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "We have connection<br>";

            //Get current directory for style sheets and images
            //was getting errors from the following so simplified it to AppDomainAppPath.ToString();
            string appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;
            //string appPath = HttpRuntime.AppDomainAppVirtualPath.ToString();//AppDomainAppPath

            //Base api URL
            string newsURL = getNewsURL();
            string baseUrl = getBaseURL();
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "The News Url is " + newsURL + " and the base url is " + baseUrl + "<br/>";
            ApiContext ac = new ApiContext(newsURL, baseUrl);
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "we retrieved the feed<br>";
            //Since this column is an identity column in the table this value does not actually get inserted.
            //int entryID = 0;

            //Used to compare the current BraftonID from the xml feed to the Brafton IDs in the Blog_Entries table
            //int compareIDs;

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
            int PageTabId = getTabID();
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "The tab id is " + PageTabId + "<br/>";
            ////////////////////////////

            //Blog_Categories table variables
            //string slug;
            //string category;
            //int parentID = 0;
            int intPortalID = getPortalID();
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "The Portal id is " + intPortalID + "<br/>";
            MyGlobals.currentPortal = intPortalID;
            ////////////////////////////

            //This is for storing all of the category xml urls during the iteration
            //ArrayList xmlArtCatURLs = new ArrayList();

            //Blog_Entry_Categories table Arrays, these are populated while populating
            //The Blog_Entry DataTable and Blog_Categories DataTable
            //ArrayList entryIDArray = new ArrayList();
            //ArrayList categoryArray = new ArrayList();
            /////////////////////////////////////////////////////////////////////////

            //DataTable articleTable = DataTables.GetTable("Blog_Entries");

            //For Limit
            int l = 0;

            //Set the limit of the amount of articles that can be imported at a time
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Setting the limit";
            int limit = getLimit();
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "The limit is " + limit + "<br/>";


            #region Article Loop DEBUG
            //Fill Blog_Entries DataTable
            foreach (newsItem ni in ac.News)
            {
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Starting Loop<br>";
                if (l < limit)
                {
                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "In the indi loop<br/>";
                    artBlogID = ni.id.ToString();
                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "checking article : " + artBlogID + "<br/>";
                    title = ni.headline;
                    entry = ni.text;
                    description = ni.extract;
                    addedDate = ni.publishDate;
                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "article date is  : " + addedDate + "<br/>";
                    photoURL = ni.PhotosHref;
                    byline = ni.byLine;
                    //imgName = string.Empty;
                    DateTime today = DateTime.Today;
                    //for operations after update or insert
                    int entryId;
                    string tempSlug;



                    using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
                    {
                        Blog_Post be = dnnContext.Blog_Posts.FirstOrDefault(x => x.BraftonID == artBlogID);

                        if (be != null)
                        {
                         #region Update Article
                         
                            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Start of update<br>";
                          
                            //If update feed content is on we do some stuff
                            int updateCheck = MyGlobals.IncludeUpdatedFeedContent;
                            int lastDayUpd = 0;
                            int todayDay = 0;

                            lastDayUpd = be.LastUpdatedOn.Value.DayOfYear;
                            todayDay = DateTime.Today.DayOfYear;
                            //TODO set this check back in place FJD
                            

                              //if the article has not been updated today
                            //if (lastDayUpd != todayDay)
                            if(true)
                             {
                                //if they have updated feed content checked
                                if (updateCheck == 1)
                                {
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //Update the article
                                 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                 //Lets get the entryID that matches the BraftonID
                                 //But if there isn't one it means this is a new article and we should skip the update

                            cmd.CommandText = "IF (SELECT ContentItemId FROM Blog_Posts WHERE BraftonID='" + artBlogID + "') IS NULL BEGIN SELECT 0 END ELSE (SELECT ContentItemId FROM Blog_Posts WHERE BraftonID='" + artBlogID + "')";

                            int ContentItemId = (int)cmd.ExecuteScalar();
                            ContentItem ce = dnnContext.ContentItems.FirstOrDefault(x => x.ContentItemID == ContentItemId);
                            if (ContentItemId > 0)
                            {
                                //DateTime today = DateTime.Today;
                                //string displayDate = today.ToString("dd/MM/yyyy");

                                #region IMAGE HANDLER

                                //photo img = ni.photos.First();
                                //photo.Instance photoInstance = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                                //string photoTest = photoInstance.type.ToString();

                                photo img = ni.photos.First();
                                photo.Instance photoInstanceLarge = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Large).FirstOrDefault();
                                photo.Instance photoInstanceMedium = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                                photo.Instance photoInstanceSmall = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Small).FirstOrDefault();
                                photo.Instance photoInstance;

                                if (photoInstanceLarge != null)
                                {
                                    photoInstance = photoInstanceLarge;
                                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Large update<br>";
                                }
                                else if (photoInstanceMedium != null)
                                {
                                    photoInstance = photoInstanceMedium;
                                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Medium update<br>";
                                }
                                else if (photoInstanceSmall != null)
                                {
                                    photoInstance = photoInstanceSmall;
                                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Small update<br>";
                                }
                                else
                                {
                                    photoInstance = null;
                                }
                                //Otherwise leave it as null and move on


                                //Checks to see if large images are enabled on the feed 
                                if (photoInstance != null)
                                {



                                    photoURL = photoInstance.url.ToString();
                                    caption = img.caption.ToString();


                                    //Checks to see if the feed has photos enabled.
                                    if (!string.IsNullOrEmpty(photoURL))
                                    {
                                        GetImages retrieveImage2 = new GetImages(photoURL, entry, description, appPath, caption, ContentItemId);
                                        retrieveImage2.DownloadImageDebug();

                                        //The images is placed into the description and the entry here


                                        entry = retrieveImage2._entry;
                                        //description = retrieveImage2._description;

                                        //string testUrl = HttpContext.Current.Request.Url.Host;

                                    }
                                }



                                #endregion IMAGE HANDLER

                                //Checks to see if the feed has the byline enabled.
                                if (!string.IsNullOrEmpty(byline))
                                {
                                    entry = entry.Insert(entry.Length, "<br /><br /><span class='byline'> By " + byline + "</span>");
                                }


                                #region Update Article


                                be.Title = title;
                                //be.Entry = entry;
                                ce.Content = description;
                                //be.Copyright = null;
                                be.LastUpdatedOn = today;
                                be.AllowComments = false;
                                //dnnContext.
                                dnnContext.SubmitChanges();
                                #endregion

                                //entryId = be.EntryID;
                                tempSlug = title;

                                //Update Permalinks
                                //setPermalinks(PageTabId, entryId, tempSlug, artBlogID);

                                // string catUrlHolder = ni.CategoriesHref;

                                //int tempCatID = addCategories(catUrlHolder);

                                //addBlogCategories(tempCatID, entryId);

                                string catUrlHolder = ni.CategoriesHref;
                                //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Category is  : " + newBlogEntry.BlogID + "<br/>";
                                int tempCatID = addCategories(catUrlHolder);

                                addBlogCategories(ContentItemId, tempCatID);
                            } //end if contentitemid = 0
                                         
                                 }//end of update check = 1

                             }//end of check for update today 
                             
                         #endregion Update Article
                        }
                    
                        else
                        
                        {
                            ContentItem ContentSpace = new ContentItem();
                            int ContentTypeId = GetcontentTypeID();
                            ContentSpace.ContentTypeID = ContentTypeId;
                            ContentSpace.CreatedOnDate = addedDate;
                            ContentSpace.TabID = -1;
                            int BlogId = getBlogID();
                            MyGlobals.CurrentBlog = BlogId;
                            ContentSpace.ModuleID = GetModuleId(BlogId);
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Module Id is  : " + ContentSpace.ModuleID + "<br/>";
                            //Get the ModuleId for item with tabId = TabId and ContentType = ContentId;
                            //newContentEntry.ContentKey = null;
                            //newContentEntry.Indexed = false;
                            ContentSpace.CreatedByUserID = 1;
                            //newContentEntry.LastModifiedByUserID = 1;
                            ContentSpace.LastModifiedOnDate = addedDate;
                            //newContentEntry.StateID = null;
                            dnnContext.ContentItems.InsertOnSubmit(ContentSpace);
                            dnnContext.SubmitChanges();
                            int ContentId = ContentSpace.ContentItemID;
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Content Id is  : " + ContentId + "<br/>";
                           #region New Article
                           photo img = ni.photos.First();
                            photo.Instance photoInstanceLarge = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Large).FirstOrDefault();
                            photo.Instance photoInstanceMedium = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                            photo.Instance photoInstanceSmall = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Small).FirstOrDefault();
                            photo.Instance photoInstance;

                            if (photoInstanceLarge != null)
                            {
                                photoInstance = photoInstanceLarge;
                               MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Large<br>";
                            }
                            else if (photoInstanceMedium != null)
                            {
                                photoInstance = photoInstanceMedium;
                                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Medium<br>";
                            }
                            else if (photoInstanceSmall != null)
                            {
                                photoInstance = photoInstanceSmall;
                                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Small<br>";
                            }
                            //Otherwise leave it as null and move on
                            else
                            {
                                photoInstance = null;
                            }
                           


                           #region IMAGE HANDLER
                            //Checks to see if medium images are enabled on the feed 
                            if (photoInstance != null)
                            {
                                photoURL = photoInstance.url.ToString();
                                caption = img.caption.ToString();

                                //Checks to see if the feed has photos enabled.
                                if (!string.IsNullOrEmpty(photoURL))
                                {
                                    
                                    GetImages retrieveImage2 = new GetImages(photoURL, entry, description, appPath, caption, ContentId);
                                    retrieveImage2.DownloadImageDebug();

                                    //The images is placed into the description and the entry here

                                    description_debug = retrieveImage2._description;

                                    //description = retrieveImage2._description;

                                    entry = retrieveImage2._entry;
                                    imgName = retrieveImage2._imageName;
                                    MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Image name is " + imgName + "<br/>";
                                    
                                }
                            }
                           #endregion IMAGE HANDLER
                            //Checks to see if the feed has the byline enabled.
                            if (!string.IsNullOrEmpty(byline))
                            {
                                entry = entry.Insert(entry.Length, "<br /><br /><span class='byline'> By " + byline + "</span>");
                            }
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "about to add items : <br/>";
                            //ContentItem newContentEntry = new ContentItem();
                            Blog_Post newBlogEntry = new Blog_Post();

                            ContentSpace.Content = entry;
                            //newContentEntry.ContentItemID = ContentId;
                            /*
                            int ContentTypeId = GetcontentTypeID();
                            newContentEntry.ContentTypeID = ContentTypeId;
                            newContentEntry.CreatedOnDate = addedDate;
                            newContentEntry.TabID = -1;
                            int BlogId = getBlogID();
                            newContentEntry.ModuleID = GetModuleId(BlogId);
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Module Id is  : " + newContentEntry.ModuleID + "<br/>";
                            //Get the ModuleId for item with tabId = TabId and ContentType = ContentId;
                            //newContentEntry.ContentKey = null;
                            //newContentEntry.Indexed = false;
                            newContentEntry.CreatedByUserID = 1;
                            //newContentEntry.LastModifiedByUserID = 1;
                            newContentEntry.LastModifiedOnDate = addedDate;
                            //newContentEntry.StateID = null;
                             * */
                            //dnnContext.ContentItems.InsertOnSubmit(newContentEntry);
                            
                            dnnContext.SubmitChanges();
                            //int ContentId = newContentEntry.ContentItemID;
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Content Id is  : " + ContentId + "<br/>";
                            newBlogEntry.ContentItemId = ContentId;
                            newBlogEntry.BlogID = BlogId;
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Blog Id is  : " + newBlogEntry.BlogID + "<br/>";
                            newBlogEntry.Title = title;
                            newBlogEntry.Published = published;
                            newBlogEntry.PublishedOnDate = addedDate;
                            newBlogEntry.Summary = description;
                            newBlogEntry.AllowComments = allowComments;
                            newBlogEntry.DisplayCopyright = displayCopyright;
                            newBlogEntry.BraftonID = artBlogID;
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Image name before insert is  : " + imgName + "<br/>";
                            newBlogEntry.Image = imgName;
                            newBlogEntry.LastUpdatedOn = today;


                            dnnContext.Blog_Posts.InsertOnSubmit(newBlogEntry);
                            dnnContext.SubmitChanges();

                            //Categories
                           string catUrlHolder = ni.CategoriesHref;
                           MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Category is  : " + newBlogEntry.BlogID + "<br/>";
                           int tempCatID = addCategories(catUrlHolder);

                           addBlogCategories(ContentId, tempCatID);


                           //increment limit
                           l++;
                            /////////////////////////////////////////////////////
                             #endregion New Article
                        }
                       
                    }



                }
            }
           
            #endregion Article Loop DEBUG

            cmd.Dispose();

            connection.Close();
                

            return description_debug;
        }//end of articles
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Video Include Global =" + MyGlobals.IncludeVideo;
            //If the feed includes videos
            if (MyGlobals.IncludeVideo == 1)
            {
                //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Start of videos<br>";
                    ImportVideos();
             }
           // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "ARTORBLOG" + MyGlobals.ArtOrBlog;
            string returnVal = "include video";
            return returnVal;
            
            
        }


        private int GetModuleId(int BlogId)
        {
            int ModId = -1;
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
            int ContentId = 1;
            using (DataClasses1DataContext dnnContext = new DataClasses1DataContext())
            {
                string type = "DNN_Blog_Post";
                ContentType cid = dnnContext.ContentTypes.FirstOrDefault(x => x.ContentType1 == type);

                ContentId = cid.ContentTypeID;
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Content type id is  : " + ContentId + "<br/>";
            }
            return ContentId;
        }
        #endregion

        #region Video Import

        public void ImportVideos()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Import articles with video enabled
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            string publicKey = MyGlobals.VideoPublicKey;
            string secretKey = MyGlobals.VideoSecretKey;
            int feedNumber;

            int tempCatID;
            int tempEntryID;
            string tempSlug;
            int PageTabId = getTabID();


                    

            //Validation below. I added these validation methods beneath the ImportVideos() method

             if (!int.TryParse(MyGlobals.VideoFeedText, out feedNumber))
            {
               
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Invalid video feed number. Stopping";
                return;
            }

            if (!BraftonVideoClass.ValidateVideoPublicKey(publicKey))
            {
               
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Invalid video public key. Stopping.";
                return;
            }

            if (!BraftonVideoClass.ValidateGuid(secretKey))
            {
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Invalid video public key. Stopping.";
                return;
            }

           

            //This is establishing the URLs for the video api,creating a new videoClient object, and then using the client libraries to get the video articles from the feed - Ly
            //string baseUrl = "http://api.video.brafton.com";
            //string basePhotoUrl = "http://pictures.directnews.co.uk/v2/"; 

            string baseUrl = "http://" + MyGlobals.VideoBaseURL +"/v2/";
            string basePhotoUrl = "http://" + MyGlobals.VideoPhotoURL + "/v2/";

            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********Global base URL- brafton.cs***********************<br>";
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + baseUrl;
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";


            AdferoVideoClient videoClient = new AdferoVideoClient(baseUrl, publicKey, secretKey);
            AdferoClient client = new AdferoClient(baseUrl, publicKey, secretKey);
            AdferoPhotoClient photoClient = new AdferoPhotoClient(basePhotoUrl);

            AdferoVideoOutputsClient xc = new AdferoVideoClient(baseUrl, publicKey, secretKey).VideoOutputs();
             
            AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient photos = client.ArticlePhotos();
            string scaleAxis = AdferoVideoDotNet.AdferoPhotos.Photos.AdferoScaleAxis.X;

            AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedsClient feeds = client.Feeds();
            AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedList feedList = feeds.ListFeeds(0, 10);

            AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticlesClient articles = client.Articles();
            AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleList articleList = articles.ListForFeed(feedList.Items[feedNumber].Id, "live", 0, 100);

            int articleCount = articleList.Items.Count;
            AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoriesClient categories = client.Categories();

            foreach (AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleListItem item in articleList.Items)
            {
                int brafId = item.Id;
                AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticle article = articles.Get(brafId);
                MyGlobals.brafID = brafId;

                string brafIDForInsert = brafId.ToString();


                string presplashLink;

                if (article.Fields.ContainsKey("preSplash"))
                {
                    presplashLink = article.Fields["preSplash"];
                    
                }
                else
                {
                    presplashLink = "";
                }
                

                #region Build Embed
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Build the embed to be added to the content section of the blog entry
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                string display = "<video id='video-"+brafIDForInsert+"' class='ajs-default-skin atlantis-js' controls preload='auto' width='512' height='288' poster='"+presplashLink+"'>";

                // For each Video Output 
                
                foreach (AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs.AdferoVideoOutputListItem vidOut in videoClient.VideoOutputs().ListForArticle(brafId, 0, 20).Items)
                {
                    
                    int vidid = vidOut.Id;

                    string displayType = "";

                    var z = xc.Get(vidid);

                    displayType = z.Path.Substring(z.Path.Length - 3);



                    if (displayType == "flv")
                    {
                        displayType = "flash";
                    }

                    if (displayType == "ebm")
                    {

                        displayType = "webm";
                    }

                    string displayPath = z.Path;

                    string displayHeight = z.Height.ToString();

                    display = display + "<source src='" + displayPath + "' type='video/" + displayType + "' data-resolution='" + displayHeight + "' />";

                }


                // Add the closing tag and the atlantis script
                display = display + "</video><script type='text/javascript'>var atlantisVideo = AtlantisJS.Init({videos: [{id: 'video-" + brafIDForInsert + "'}]});</script>";

                #endregion Build embed



                #region set globals
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Set Global Variables in Globals.cs
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //This area adds all the fields to the Global variables class so that they are accessible from the BraftonVideoClass
                //I know there is a better way to do this but I did it this was for simplicity and in case I needed them somewhere else
                string addDate = article.Fields["date"];

                MyGlobals.tempID = brafIDForInsert;

                if (article.Fields.ContainsKey("title"))
                {
                    MyGlobals.tempTitle = article.Fields["title"];

                }
                else
                {
                    MyGlobals.tempTitle = "";
                }
                

                if (article.Fields.ContainsKey("extract"))
                {
                    MyGlobals.tempExtract = article.Fields["extract"];

                }
                else
                {
                    MyGlobals.tempExtract = "";
                }

                if (article.Fields.ContainsKey("content"))
                {
                    MyGlobals.tempcontent = article.Fields["content"];

                }
                else
                {
                    MyGlobals.tempcontent = "";
                }
                
                MyGlobals.tempDate = DateTime.Parse(addDate);
                MyGlobals.tempPaths = display;
                #endregion


                tempEntryID = BraftonVideoClass.InsertVideoPost();
                tempSlug = article.Fields["title"];
               

                #region Update Permalink
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Update permalink 
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                
                string permalink;
                string slug = strip(tempSlug);
                    //Create Permalink
                    
                   
                using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                {
                    Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Entry pk = dnnContext.Blog_Entries.FirstOrDefault(x => x.BraftonID == brafIDForInsert);

                                    //Update the permalink
                                    if (pk != null)
                                    {

                                        permalink = "/blog/EntryId/" + pk.EntryID + "/" + slug;

                                        pk.PermaLink = permalink;
                                        dnnContext.SubmitChanges();
                                      
                                    }

                }

                #endregion Update Permalink

                #region Categories
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Add categories to Blog_Entry_Categories table 
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                foreach (AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoryListItem cats in client.Categories().ListForArticle(brafId,0,20).Items)
                {
                    int categoryId;
                    int catTest = cats.Id;
                    string catName = categories.Get(catTest).Name;
                    if (catName == null)
                    {
                        catName = "Uncategorized";
                    }
                    int pID = categories.Get(catTest).ParentId;
                    string catslug = strip(catName) + ".aspx";
                   
                    



                    using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                    {
                        Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Category ca = dnnContext.Blog_Categories.FirstOrDefault(x => x.Category == catName);

                        //Insert into category
                        if (ca != null)
                        {

                            categoryId = ca.CatID;
                        }

                        else
                        {
                          
                            Blog_Category newBlogCat = new Blog_Category();
                            newBlogCat.Category = catName;
                            newBlogCat.Slug = catslug;
                            newBlogCat.ParentID = pID;
                            newBlogCat.PortalID = 0;

                            dnnContext.Blog_Categories.InsertOnSubmit(newBlogCat);
                            dnnContext.SubmitChanges();

                            categoryId = newBlogCat.CatID;
                                                        
                        }

                        Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Entry_Category bec = dnnContext.Blog_Entry_Categories.FirstOrDefault(x => x.EntryID == tempEntryID && x.CatID == categoryId);

                        if (bec == null)
                        {
                            Blog_Entry_Category newBlogEntryCat = new Blog_Entry_Category();
                            newBlogEntryCat.EntryID = tempEntryID;
                            newBlogEntryCat.CatID = categoryId;

                            dnnContext.Blog_Entry_Categories.InsertOnSubmit(newBlogEntryCat);
                            dnnContext.SubmitChanges();
                        }

                    }

                }

                #endregion Categories


        }


        #endregion Video Import


    }
    }
}





