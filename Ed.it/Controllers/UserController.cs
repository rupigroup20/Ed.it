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
using System.Data;
using System.Data.SqlClient;
using System.Web.Http.ExceptionHandling;
using System.Globalization;
using System.Threading;
using Microsoft.Office.Interop.PowerPoint;
using System.Drawing;
using Aspose.Slides;
using System.Configuration;

namespace Ed.it.Controllers
{
    //public sealed class Presentation : IPresentation,
    //IPresentationComponent, IDisposable

    public class UserController : ApiController
    {
        bool Local = true;//עובדים על השרת או מקומי

        string UrlServer = "http://proj.ruppin.ac.il/igroup20/prod/uploadedPictures";//ניתוב שרת
        string UrlLocal = @"C:\Users\programmer\ed.it_client\public\uploadedPicturesPub\\";//ניתוב מקומי
        string UrlLocalAlmog = @"C:\Users\almog\Desktop\final project development\client\ed.it_client\public\uploadedPicturesPub\\";
        
        
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("api/User/GetUserDetails")]
        public User GetUserDetails([FromBody]User NewUser)//
        {
            try
            {
                NewUser = NewUser.GetUserDetails();
                return NewUser;//אם מחזיר Null אז משתמש הזין פרטים לא נכוננים
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
    
        }

    


        [HttpGet]
        [Route("api/User/GetTags")]
        public List<string> GetTags()
        {
            TagsUser tagsUser = new TagsUser();          
            return tagsUser.GetTagsList();

        }

        /// <summary>
        /// יצירת יוזר חדש
        /// </summary>
        [HttpPost]
        [Route("api/User/CreateUser")]
        public bool CreateUser([FromBody]User NewUser)
        {
            NewUser.CreateUser();
            return true;
        }

        [HttpPost]
        [Route("api/AddPic/{Email}")]
        public HttpResponseMessage UploadPic(string Email)
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            List<string> imageLinks = new List<string>();
            try
            {
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
                            //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
                            string fname = Email.Split('@').First() + "." + httpPostedFile.FileName.Split('\\').Last().Split('.').Last();//שם הקובץ יהיה שם משתמש
                            var fileSavePath="";
                            if (Local)
                            {
                                fileSavePath = Path.Combine(UrlLocalAlmog, fname);//אם עובדים לוקלי ישמור תמונות בתיקיית פבליק של הקליינט
                            }
                            else
                            {
                                //fileSavePath= Path.Combine(HostingEnvironment.MapPath("~/uploadedPicture"), fname);//אם עובדים על השרת שומרים תמונות בתיקייה של השרת

                                fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedPictures"), fname);
                                
                            }
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(fileSavePath);
                            //imageLinks.Add("uploadedPicture/" + fname);
                            imageLinks.Add("uploadedPictures/" + fname);
                            ////פיצול מצגת לתמונות
                            //using (Aspose.Slides.Presentation pres = new Aspose.Slides.Presentation(fileSavePath))
                            //{
                            //    foreach (ISlide sld in pres.Slides)
                            //    {
                            //        // Create a full scale image
                            //        Bitmap bmp = sld.GetThumbnail(1f, 1f);
                            //        fileSavePath= Path.Combine(HostingEnvironment.MapPath("~/uploadedPicture"), string.Format("{0}_{1}.jpg", Email.Split('@').First(), sld.SlideNumber));// $@"{Email.Split('@').First()}_{sld.SlideNumber}"
                            //        // Save the image to disk in JPEG format
                            //        bmp.Save(fileSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //    }
                            //}

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); //Will display localized message
                ExceptionLogger el = new ExceptionLogger(ex);
                System.Threading.Thread t = new System.Threading.Thread(el.DoLog);
                t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                t.Start();
            }
            return Request.CreateResponse(HttpStatusCode.Created, imageLinks);

        }

        [HttpPost]
        [Route("api/User/update/{UrlPicture}/{Email}/1")]
        public HttpResponseMessage UpdatePic(string UrlPicture, string Email)
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            List<string> imageLinks = new List<string>();

            

            try
            {
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
                            //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
               
                            try
                            {
                                if (Local)
                                {
                                    if (File.Exists(Path.Combine(UrlLocalAlmog, UrlPicture)))
                                    {
                                        // If file found, delete it    
                                        File.Delete(Path.Combine(UrlLocalAlmog, UrlPicture));
                                        Console.WriteLine("File deleted.");
                                    }
                                    else Console.WriteLine("File not found");
                                }
                                // Check if file exists with its full path    
                               else
                                {
                                    if (File.Exists(Path.Combine(UrlServer, UrlPicture)))
                                    {
                                        // If file found, delete it    
                                        File.Delete(Path.Combine(UrlServer, UrlPicture));
                                        Console.WriteLine("File deleted.");
                                    }
                                    else Console.WriteLine("File not found");
                                }
                            }
                            
                            catch (IOException ioExp)
                            {
                                Console.WriteLine(ioExp.Message);
                            }

                            string fname = Email.Split('@').First() + "." + httpPostedFile.FileName.Split('\\').Last().Split('.').Last();//שם הקובץ יהיה שם משתמש
                             var fileSavePath = "";
                            if (Local)
                            {
                                fileSavePath = Path.Combine(UrlLocalAlmog, fname);//אם עובדים לוקלי ישמור תמונות בתיקיית פבליק של הקליינט
                            }
                            else
                            {
                                //fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedPicture"), fname);//אם עובדים על השרת שומרים תמונות בתיקייה של השרת
                                fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedPictures"), fname);
                                //fileSavePath = Path.Combine(UrlServer, fname);
                            }
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(fileSavePath);
                            //imageLinks.Add("uploadedPicture/" + fname);

                            User u = new User();
                            u.UpdatePic(Email, fname);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); //Will display localized message
                ExceptionLogger el = new ExceptionLogger(ex);
                System.Threading.Thread t = new System.Threading.Thread(el.DoLog);
                t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                t.Start();
            }
            return Request.CreateResponse(HttpStatusCode.Created, imageLinks);

        }

        [HttpPut]
        [Route("api/User/UpdateUser")]
        public int Put([FromBody]User NewUser)
        {
           int numEffected = NewUser.UpdateDetails();
            return numEffected;
        }


        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }



        class ExceptionLogger
        {
            Exception _ex;

            public ExceptionLogger(Exception ex)
            {
                _ex = ex;
            }

            public void DoLog()
            {
                Console.WriteLine(_ex.ToString()); //Will display en-US message
            }
        }
    }
}
