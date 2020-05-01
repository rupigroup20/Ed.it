using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ed.it.Models;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Web.Hosting;
using System.Drawing;
using Aspose.Slides;

namespace Ed.it.Controllers
{
    public class ContentController : ApiController
    {
        bool Local = true;//עובדים על השרת או מקומי

        string UrlServer = "http://proj.ruppin.ac.il/igroup20/prod/uploadedContents/";//ניתוב שרת
        string UrlLocal = @"C:\Users\programmer\ed.it_client\public\uploadedFilesPub\\";//ניתוב מקומי
        string UrlLocalAlmog = @"C:\Users\almog\Desktop\final project development\client\ed.it_client\public\uploadedFilesPub\\";
        

        /// <summary>
        /// הצעת תכנים עבור משתמש רשום
        /// </summary>
        [HttpGet]
        [Route("api/Content/SuggestContent/{UserName}")]
        public List<Content> GetSuggestContentForUser(string UserName)
        {
            try
            {
                Content content = new Content();
                return content.GetSuggestionsOfContents(UserName);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// הצעת תכנים עבור אורח-דף בית עבור משתמש שלא התחבר
        /// </summary>
        [HttpGet]
        [Route("api/Content/SuggestContentForGuest")]
        public List<Content> GetSuggestContentForGuest()
        {
            try
            {
                Content content = new Content();
                return content.GetSuggestionsOfContentsForGuest();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// שליפת רשימת כל שמות המצגות
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Content/GetContents")]
        public List<string> GetContents()
        {
            try
            {
                Content content = new Content();
                return content.GetContentList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// חיפוש תכנים לפי תגית
        /// </summary>
        [HttpGet]
        [Route("api/Content/Search/Tags/{TagName}")]
        public List<Content> Search(string TagName)
        {
            try
            {
                Content content = new Content();
                return content.Search(TagName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// חיפוש תכנים לפי שמות מצגות
        /// </summary>
        [HttpGet]
        [Route("api/Content/Search/Contents/{Name}")]
        public List<Content> SearchByName(string Name)
        {
            try
            {
                Content content = new Content();
                return content.SearchByName(Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// שמירת תוכן בעת העלאה
        /// </summary>
        [HttpPost]
        [Route("api/Content/AddContent")]
        public HttpResponseMessage AddContent(Content content)
        {
            HttpResponseMessage response;
            try
            {
                content.PathFile = $@"{content.ContentName}-{content.ByUser}.{content.PathFile.Split('\\').Last().Split('.').Last()}";//מחליף את שם הקובץ ל //holocaust-shiftan92.pptx
                content.UploadContent();
                response = Request.CreateResponse(HttpStatusCode.Created);
                return response;
            }
            catch(Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Error-"+ex.Message);
                return response;
            }
        }

        /// <summary>
        /// שמירת קובץ התוכן פלוס תמונות של המצגת
        /// </summary>
        [HttpPost]
        [Route("api/Content/UploadContent/{ByUser}/{ContentName}")]    ///
        public HttpResponseMessage UploadContent(string ByUser, string ContentName)
        {
            Content content = new Content();
            List<string> imageLinks = new List<string>();
            var httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count > 0)
            {
                //Loop through uploaded files  
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];

                    // this is an example of how you can extract addional values from the Ajax call
                    string name = httpContext.Request.Form["user"];

                    if (httpPostedFile != null)
                    {
                        // Construct file save path  
                        string fname = $@"{ContentName}-{ByUser}.{httpPostedFile.FileName.Split('\\').Last().Split('.').Last()}";//holocaust-shiftan92.pptx
                        var fileSavePath = "";
                        if (Local)
                        {
                            fileSavePath = Path.Combine(UrlLocal, fname);//אם עובדים לוקלי ישמור תמונות בתיקיית פבליק של הקליינט
                        }
                        else
                        {
                            fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedContents"), fname);//אם עובדים על השרת שומרים תמונות בתיקייה של השרת
                        }
                        // Save the uploaded file  
                        httpPostedFile.SaveAs(fileSavePath);

                        //פיצול מצגת לתמונות
                        using (Aspose.Slides.Presentation pres = new Aspose.Slides.Presentation(fileSavePath))
                        {
                            int countPages = 0;
                            foreach (ISlide sld in pres.Slides)
                            {
                                if (Local)
                                {
                                    
                                    // Create a full scale image
                                    Bitmap bmp = sld.GetThumbnail(1f, 1f);

                                    fileSavePath = Path.Combine(UrlLocal, string.Format("{0}-{1}_{2}.jpg", ContentName, ByUser, sld.SlideNumber));// $@"{Email.Split('@').First()}_{sld.SlideNumber}"
                                                                                                                                                       // Save the image to disk in JPEG format
                                    bmp.Save(fileSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    // Create a full scale image
                                    Bitmap bmp = sld.GetThumbnail(1f, 1f);
                                    //fileSavePath = Path.Combine(UrlServer, string.Format("{0}-{1}_{2}.jpg", ContentName, ByUser, sld.SlideNumber));// $@"{Email.Split('@').First()}_{sld.SlideNumber}"
                                    fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedContents"), string.Format("{0}-{1}_{2}.jpg", ContentName, ByUser, sld.SlideNumber));                                                                                                                 // Save the image to disk in JPEG format
                                    bmp.Save(fileSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                countPages++;
                            }
                            //עדכון מספר עמודים של התוכן בדטה בייס
                            content=content.UpdatePages(countPages);              
                        }

                    }
                }
                // Return status code  
                return Request.CreateResponse(HttpStatusCode.Created, content);

            }
                return Request.CreateResponse(HttpStatusCode.Created, "שגיאה בהעלאת קובץ");
        }

        [HttpGet]
        [Route("api/Content/GetContent/{ContentID}/{UserName}")]
        public Content GetContent(string ContentID,string UserName)
        {
            Content content = new Content();
            return content.GetContent(ContentID, UserName);
        }

        [HttpGet]
        [Route("api/Content/GetUserContents/{UserName}")]
        public  List<Content> GetUserContents(string UserName)
        {
            List<Content> UserContent = new List<Content>();
            Content content = new Content();
            UserContent = content.GetUserContents(UserName);
            return UserContent;
        }

        [HttpGet]
        [Route("api/Content/GetUserLikedContents/{UserName}")]
        public List<Content> GetUserLikedContents(string UserName)
        {
            List<Content> UserLikedContent = new List<Content>();
            Content content = new Content();
            UserLikedContent = content.GetUserLikedContents(UserName);
            return UserLikedContent;
        }


        /// <summary>
        /// משתמש העלה תגובה יחזיר לאחר מכן רשימה מעודכנת של התגובות
        /// </summary>
        [HttpPost]
        [Route("api/Content/AddComment")]
        public List<Models.Comments> AddComment(Models.Comments comment)
        {
            List<Models.Comments> commentList = new List<Models.Comments>();
            commentList = comment.AddComment();
            return commentList;
        }
            // DELETE api/values/5
            public void Delete(int id)
        {
        }
    }
}