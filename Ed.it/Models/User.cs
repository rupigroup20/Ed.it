using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Ed.it.Models
{
    public class User
    {
        //public string UserID { get; set; }
        //public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string SchoolType { get; set; }
        public string TeacherType { get; set; }
        public string BDate { get; set; }
        public string AboutMe { get; set; }
        public string UrlPicture { get; set; }
        public HttpContext FormDataPic { get; set; }


        public List<Content> ContentsUser { get; set; }//תכנים שהעלה היוזר
        public List<string> TagsUser { get; set; }//תגים שהיוזר סימן כאהובים
        public List<string> Subjects { get; set; }//מקצועות שהמורה מלמד(לא לשימוש,לקריאה בלבד
        public List<TagsUser> TagsScore { get; set; }//רשימת התגים של היוזר עם הניקוד שלו
        DBservices dBservices = new DBservices();

        public User()
        {
      
        }



        /// <summary>
        /// יצירת משתמש חדש
        /// </summary>
        internal void CreateUser()
        {
            if(UrlPicture!="")//יש תמונת פרופיל
            {
                UrlPicture= Email.Split('@').First() + "." + UrlPicture.Split('\\').Last().Split('.').Last();
            }
            dBservices.CreateUser(this);
        }

        /// <summary>
        /// קבלת פרטי משתמש וולידצית משתמש
        /// </summary>
        internal User GetUserDetails()
        {
            User user = new User();
            user= dBservices.GetUserDetails(Email,Password);
            return user;        
         
        }

        public int UpdateDetails()
        {
           int numEffected = dBservices.UpdateDetails(this);
            return numEffected;
        }

        public void UpdatePic(string Email, string Urlpic)
        {
            dBservices.UpdatePic(Email, Urlpic);
        }
    }
}