using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ed.it.Models
{
    public class Comments
    {
        public int CommentID { get; set; }
        public string NameWhoCommented { get; set; }
        public string UrlPictureWhoCommented { get; set; }
        public string Comment { get; set; }
        public string PublishedDate { get; set; }

        public Comments()
        {

        }
    }
}