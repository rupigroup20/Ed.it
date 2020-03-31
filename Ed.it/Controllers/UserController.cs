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


namespace Ed.it.Controllers
{
    public class UserController : ApiController
    {
        string UrlServer = "http://proj.ruppin.ac.il/igroup20/prod/uploadFiles/";//ניתוב שרת
        string UrlLocal = @"C:\Users\programmer\Ed.it\Ed.it\uploadedFiles\\";//ניתוב מקומי
        bool Local = true;//עובדים על השרת או מקומי
        
        
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
                if (Local)//ניתוב תמונה
                {
                    NewUser.UrlPicture = UrlLocal + NewUser.UrlPicture;
                }
                else
                {
                    NewUser.UrlPicture = UrlServer + NewUser.UrlPicture;
                }
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
                            var fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedFiles"), fname);
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(fileSavePath);
                            imageLinks.Add("uploadedFiles/" + fname);

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
