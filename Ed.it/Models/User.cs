using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ed.it.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string SchoolType { get; set; }
        public string TeacherType { get; set; }
        public DateTime Bdate { get; set; }
        public string AboutMe { get; set; }
        public string UrlPicture { get; set; }

  

        public List<Content> ContentsUser { get; set; }//תכנים שהעלה היוזר
        public List<string> TagsUser { get; set; }//תגים שהיוזר סימן כאהובים
        public List<string> Subjects { get; set; }//מקצועות שהמורה מלמד(לא לשימוש,לקריאה בלבד
        public List<TagsUser> TagsScore { get; set; }//רשימת התגים של היוזר עם הניקוד שלו
        DBservices dBservices = new DBservices();

        public User(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }



        /// <summary>
        /// יצירת משתמש חדש
        /// </summary>
        internal void CreateUser()
        {
            dBservices.CreateUser(this);
        }

        /// <summary>
        /// קבלת פרטי משתמש וולידצית משתמש
        /// </summary>
        internal User GetUserDetails(string UserName, string Password)
        {
            User user = new User();
            user= dBservices.GetUserDetails(UserName,Password);
            return user;        
         
        }
    }
}