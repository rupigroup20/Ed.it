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
        public int ContentID { get; set; }
        DBservices dBservices = new DBservices();

        public Comments()
        {

        }

        public List<Comments> AddComment()
        {
            PublishedDate = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            return dBservices.AddComment(this);
        }

        //שליפת כל התגובות של מצגת מסויימת עבור אדמין
        public List<Comments> GetCommentsA(string ContentId)
        {
            return dBservices.GetCommentsA(ContentId);
        }

        //מחיקת תגובה ע"י אדמין
        public int DeleteComment(int CommentID)
        {
           return dBservices.DeleteComment(CommentID);    
        }
    }
}