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
        public List<Comments> CommentsList { get; set; }
        public string UploadedDate { get; set; }
        public int Likes { get; set; }
        public string ByUser { get; set; }
        public string UserPic { get; set; }
        public int PagesNumber { get; set; }
        public bool LikedByUserWhoWatch { get; set; }
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

        /// <summary>
        /// אלגוריתם חכם-הצעת תכנים למשתמש
        /// </summary>
        public List<Content> GetSuggestionsOfContents(string UserName)
        {
            List<Content> SuggestionList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
            SuggestionList = dBservices.GetSuggestionsOfContents(UserName);
            return SuggestionList;
        }

        /// <summary>
        /// הצעת תכנים לאורח-התכנים עם הכי הרבה לייקים ללא קשר לתגיות
        /// </summary>
        public List<Content> GetSuggestionsOfContentsForGuest()
        {
            List<Content> SuggestionList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
            SuggestionList = dBservices.GetSuggestionsOfContentsForGuest();
            return SuggestionList;
        }


        /// <summary>
        /// חיפוש תכנים לפי תגית-אלגוריתם חכם
        /// </summary>
        internal List<Content> Search(string tagName)
        {
            List<Content> ResultList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
            ResultList = dBservices.Search(tagName);
            return ResultList;
        }


        /// <summary>
        /// חיפוש תוכן לפי שם מצגת
        /// </summary>
        internal List<Content> SearchByName(string name)
        {
            List<Content> ResultList = new List<Content>();
            ResultList = dBservices.SearchByName(name);
            return ResultList;
        }

        /// <summary>
        /// קבלת רשימת שמות כל המצגות
        /// </summary>
        internal List<string> GetContentList()
        {
            List<string> ContentList = new List<string>();
            ContentList = dBservices.GetContents();
            return ContentList;
        }

        /// <summary>
        /// מקבל פרטי מצגת 
        /// </summary>
        public Content GetContent(string ContentID,string UserName)
        {
            Content content = new Content();
            content= dBservices.GetContent(ContentID, UserName);
            return content;
        }

        /// <summary>
        /// עדכון מספר עמודים בדטה בייס בעת העלאת מצגת
        /// </summary>
        internal Content UpdatePages(int countPages)
        {
            return dBservices.UpdatePages(countPages);
        }

        public List<Content> GetUserContents(string UserName)
        {
            List<Content> UserContent = new List<Content>();
            UserContent = dBservices.GetUserContents(UserName);
            return UserContent;
        }

        /// <summary>
        /// התראה של חג קרוב בטווח 7 ימים
        /// </summary>
        internal List<Content> GetHolidayAlert(string holiday)
        {
            List<Content> contentList = new List<Content>();
            contentList=dBservices.GetHolidayAlert(holiday);
            return contentList;
        }

        public List<Content> GetUserLikedContents(string UserName)
        {
            List<Content> UserLikedContent = new List<Content>();
            UserLikedContent = dBservices.GetUserLikedContents(UserName);
            return UserLikedContent;
        }

        //שליפת המצגות שהועלו בעשרה ימים החארונים
        public List<Content> GetLatestContent(string Days, string UserName)
        {
            return dBservices.GetLatestContent(Days, UserName);
        }
    }
}