using Ed.it.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ed.it.Controllers
{
    public class UserController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public User GetUserDetails(string UserName,string Password)
        {
            User user = new User();
            user=user.GetUserDetails(UserName,Password);
            return user;//אם מחזיר Null אז משתמש הזין פרטים לא נכוננים
        }


        /// <summary>
        /// יצירת יוזר חדש
        /// </summary>
        [HttpPost]
        [Route("api/Changes/CreateUser")]
        public void CreateUser(User NewUser)
        {
            NewUser.CreateUser();
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
