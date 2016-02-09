using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;
using Brafton.Modules.Globals;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs;
using AdferoVideoDotNet.AdferoArticles;
using AdferoVideoDotNet.AdferoPhotos;

using System.IO;
namespace Brafton.Modules.VideoImporter
{
    public class BraftonVideoClass
    {
        //validation methods
        public static bool ValidateVideoPublicKey(string publicKey)
        {
            Regex reg = new Regex("[a-f0-9]{8}", RegexOptions.IgnoreCase);
            return reg.IsMatch(publicKey);
        }

        public static bool ValidateGuid(string guid)
        {
            Regex reg = new Regex("[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}", RegexOptions.IgnoreCase);
            return reg.IsMatch(guid);
        }

       
        public static string BuildEmbedCode(AdferoVideoClient VideoClient, AdferoVideoOutputsClient XC, Hashtable TabSettings, int ArticleId, string Poster)
        {
            string embedCode = "<video id='video-" + ArticleId.ToString() + "' class='ajs-default-skin atlantis-js' controls preload='auto' width='512' height='288' poster='" + Poster + "'>";
            
            foreach (AdferoVideoOutputListItem vidOut in VideoClient.VideoOutputs().ListForArticle(ArticleId, 0, 20).Items)
            {
                int vidid = vidOut.Id;
                var z = XC.Get(vidid);

                string displayPath = z.Path;
                string type = Path.GetExtension(displayPath);
                type = type.Substring(1);
                string displayHeight = z.Height.ToString();

                embedCode = embedCode + string.Format("<source src='{0}' type='video/{1}' data-resolution='{2}' />", displayPath, type, displayHeight);
                
            }
            embedCode = embedCode + "</video>";
            string cta = BuildCta(TabSettings, ArticleId);
            embedCode = "<div class='brafton-single-video'>" + embedCode + "</div>" + cta;
            return embedCode;
        }
        public static string BuildCta(Hashtable TabSettings, int ArticleId)
        {
            string fullCta = "";
            if ( TabSettings.Contains("VideoPauseText") && !string.IsNullOrWhiteSpace(TabSettings["VideoPauseText"].ToString()) )
            {
                string text = TabSettings["VideoPauseText"].ToString();
                string link = TabSettings.Contains("VideoPauseLink") ? TabSettings["VideoPauseLink"].ToString() : string.Empty;
                string asset = TabSettings.Contains("VideoPauseAssetID") && !string.IsNullOrWhiteSpace(TabSettings["VideoPauseAssetID"].ToString()) ? "assetGateway: { id: '" + TabSettings["VideoPauseAssetID"].ToString() + " }," : string.Empty;
                fullCta = ",pauseCallToAction: { " + asset + " link: '" + link + "',text: '" + text + "' }";

                string VideoEndtitle = TabSettings.Contains("VideoEndTitle") ? TabSettings["VideoEndTitle"].ToString() : "";
                string VideoEndSubtitle = TabSettings.Contains("VideoEndSubtitle") ? TabSettings["VideoEndSubtitle"].ToString() : "";
                string VideoEndButtonText = TabSettings.Contains("VideoEndButtonText") ? TabSettings["VideoEndButtonText"].ToString() : "";
                string VideoEndButtonLink = TabSettings.Contains("VideoEndButtonLink") ? TabSettings["VideoEndButtonLink"].ToString() : "";
                string VideoEndButtonAssetID = TabSettings.Contains("VideoEndButtonAssetID") ? TabSettings["VideoEndButtonAssetID"].ToString() : "";

                string endingCta = "";
                if (!string.IsNullOrWhiteSpace(VideoEndtitle) || !string.IsNullOrWhiteSpace(VideoEndSubtitle) || !string.IsNullOrWhiteSpace(VideoEndButtonText) )
                {
                    endingCta = ",endOfVideoOptions: {" + Environment.NewLine;
                    if (!string.IsNullOrWhiteSpace(VideoEndButtonAssetID))
                    {
                        endingCta = endingCta + "assetGateway: { " + Environment.NewLine + " id: '" + VideoEndButtonAssetID + "' }, " + Environment.NewLine;
                    }
                    endingCta = endingCta + "callToAction: { " + Environment.NewLine;
                    if (!string.IsNullOrWhiteSpace(VideoEndtitle))
                    {
                        endingCta = endingCta + "title: '" + VideoEndtitle + "'," + Environment.NewLine;
                    }
                    if (!string.IsNullOrWhiteSpace(VideoEndSubtitle))
                    {
                        endingCta = endingCta + "subtitle: '" + VideoEndSubtitle + "'," + Environment.NewLine;
                    }
                    if (!string.IsNullOrWhiteSpace(VideoEndButtonText))
                    {
                        endingCta = endingCta + "button: { " + Environment.NewLine + "link: '" + VideoEndButtonLink + "', text: '" + VideoEndButtonText + "' } " + Environment.NewLine;
                    }
                    //ending callToAction
                    endingCta = endingCta + " } " + Environment.NewLine;

                    //close endofvideooptions
                    endingCta = endingCta + " } ";
                }
                if (!string.IsNullOrWhiteSpace(endingCta))
                {
                    fullCta = fullCta + endingCta;
                }
            }

            string id = ArticleId.ToString();
            string pieces = id + "'" + fullCta;
            
            string cta = "<script>$(function(){ AtlantisJS.Init({videos: [" + Environment.NewLine + "{id: 'video-" + pieces + Environment.NewLine +" }" + Environment.NewLine + "] }); } );</script>";
            
            return cta;
        }

    }
}