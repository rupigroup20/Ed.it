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

namespace Ed.it.Controllers
{
    public class UserController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("api/User/GetUserDetails")]
        public User GetUserDetails([FromBody]User NewUser)//
        {
            NewUser=NewUser.GetUserDetails();
            return NewUser;//אם מחזיר Null אז משתמש הזין פרטים לא נכוננים
        }


        /// <summary>
        /// יצירת יוזר חדש
        /// </summary>
        [HttpPost]
        [Route("api/User/CreateUser")]
        public bool CreateUser([FromBody]User NewUser)
        {           
            //NewUser.CreateUser();
            return true;
        }

        [HttpPost]
        [Route("api/AddPic")]
        public HttpResponseMessage UploadPic()//string UserName
        {
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
                        //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
                        string fname = httpPostedFile.FileName.Split('\\').Last();
                        var fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedFiles"), fname);
                        // Save the uploaded file  
                        httpPostedFile.SaveAs(fileSavePath);
                        imageLinks.Add("uploadedFiles/" + fname);
                    }
                }
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
    }
}
