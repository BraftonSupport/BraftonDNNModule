using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web.Hosting;
using ImportImages;
using System.IO;
using System.Drawing;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using Brafton.Modules.Globals;

using DotNetNuke.Entities;

class GetImages
{

    public string _description;
    public string _entry;
    public string _photoURL;
    public string _appPath;
    public string _caption;
    public string _descript;
    public int _contentId;
    public string _imageName;
    Boolean _isDescription = false;

    public GetImages(string photoURL, string entry, string description, string appPath, string caption, int contentId)
    {
        _description = description;
        _entry = entry;
        _photoURL = photoURL;
        _appPath = appPath;
        _caption = caption;
        _contentId = contentId;
    }

    public void DownloadImageDebug()
    {

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        SqlCommand cmd = new SqlCommand();
        connection.Open();
        cmd.Connection = connection;
        cmd.CommandText = "Select URL FROM WebServers";
        SqlDataReader urls = cmd.ExecuteReader();
        int count = 0;
        string RootURl = string.Empty;
        while (urls.Read())
        {
            RootURl = "http://" + urls.GetString(0);
            ++count;
        }
        if (count != 1)
        {
            RootURl = "";
        }
        //string newRoot = Brafton.DotNetNuke.BraftonSchedule.modTabSettings["clientDomain"].ToString();
        string imageName = "";

        // To grab the first instance of the <p> and insert the image in there
        if (!String.IsNullOrEmpty(_description))
        {
            _isDescription = true;
            int tmpDesPos = _description.IndexOf("<p>");
        }
        int tmpEntPos = _entry.IndexOf("<p>");
        
        // Download the image from the feed
        int lastPlace = _photoURL.IndexOf("_", 0);
        if (lastPlace.ToString() == "-1")
        {
            lastPlace = _photoURL.LastIndexOf(".");
        }
        int firstPlace = _photoURL.LastIndexOf("/");
        imageName = _photoURL.Substring(firstPlace, lastPlace - firstPlace);
        _imageName = imageName.Substring(1);

        ImageDownload imgObject = new ImageDownload();
        imgObject.saveImage(_photoURL, imageName, _contentId);

        string path = imgObject.urlDirectory;
        string ImageUrl = "/" + path + "/" + imageName + ".jpg";
        //Keep this before the image download in case the posts were deleted for reimport
        _entry = _entry.Insert(tmpEntPos + "<p>".Length, "<div class='newsThumbSingle'><img alt=\"" + _caption + "\" class='br-thumbnail-image-article' style='width:200px;height:200px;' src='"+ RootURl  + ImageUrl + "' /><div class='caption'>" + _caption + "</div></div>");
        _entry = _entry.Insert(0, "<style type='text/css'> .newsThumbSingle {background:#ddd;margin:5px;padding:5px;position:relative;float:right;-moz-border-radius: 3px;border-radius: 3px;z-index:1;text-align:center;max-width:200px;} .caption{font-size:10px;line-height:normal;}</style>");
        if (_isDescription)
        {
            _description = _description.Insert(0, "<img class='br-thumbnail-image' style='padding:10px;float:left;width:33%;height:auto;max-width:250px;' src='" + RootURl +ImageUrl + "' />");
        }
    }


    public bool isDescription { get; set; }
}
