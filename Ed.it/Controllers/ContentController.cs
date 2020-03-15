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

namespace Ed.it.Controllers
{
    public class ContentController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("api/Changes/UploadContent/{UserID}")]
        public async Task<string> UploadContent([FromBody] Content content,string UserID)
        {
            //העלאת קובץ לשרת
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_Data");//להוסיף תיקייה כזאת בהמשך
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers.ContentDisposition.FileName;
                    var localFileName = file.LocalFileName;
                    var filePath = Path.Combine(root, name);
                    File.Move(localFileName, filePath);
                    
                }
                content.UploadContent(UserID);
                return "File Uploaded";

            }
            catch (Exception ex)
            {
                throw (ex.InnerException);
            }
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