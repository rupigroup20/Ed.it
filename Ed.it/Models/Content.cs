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
        /// חיפוש תכנים
        /// </summary>
        internal List<Content> Search(string tagName)
        {
            List<Content> ResultList = new List<Content>();
            ResultList = dBservices.Search(tagName);
            return ResultList;
        }
    }
}