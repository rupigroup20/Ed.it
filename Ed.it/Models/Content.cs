using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Ed.it.Models
{
    public class Content
    {
        public int ContentID { get; set; }
        public string ContentName { get; set; }
        public string PathFile { get; set; }
        public string Description { get; set; }
        public List<string> TagsContent { get; set; }//תגים שהתוכן מתויג בו
        public string UploadedDate { get; set; }
        public int Likes { get; set; }
        public string ByUser { get; set; }
        DBservices dBservices = new DBservices();

        public Content()
        {

        }

        /// <summary>
        /// העלאת תוכן ע"י משתמש
        /// </summary>
        public int UploadContent()
        {
            ContentID = dBservices.UploadContent(this);
            return ContentID;
        }

        public DataTable GetSuggestionsOfContents(string UserName)
        {

            return null;
        }

    }
}