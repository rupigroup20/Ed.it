using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ed.it.Models
{
    public class Content
    {
        public string ContentName { get; set; }
        public string PathFile { get; set; }
        public string Description { get; set; }
        public List<string> TagsContent { get; set; }//תגים שהתוכן מתויג בו
        public DateTime UploadedDate { get; set; }
        public int Likes { get; set; }
        DBservices dBservices = new DBservices();

        public Content()
        {

        }

        /// <summary>
        /// העלאת תוכן ע"י משתמש
        /// </summary>
        public void UploadContent(string UserId)
        {
            dBservices.UploadContent(UserId, this);
        }

    }
}