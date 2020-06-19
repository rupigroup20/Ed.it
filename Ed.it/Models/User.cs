using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


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
            if (UrlPicture != "")//יש תמונת פרופיל
            {
                UrlPicture = Email.Split('@').First() + "." + UrlPicture.Split('\\').Last().Split('.').Last();
            }
            dBservices.CreateUser(this);
        }

        /// <summary>
        /// קבלת פרטי משתמש וולידצית משתמש
        /// </summary>
        internal User GetUserDetails()
        {
            User user = new User();
            user = dBservices.GetUserDetails(Email, Password);
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

        /// <summary>
        /// ///שליפת שמונת המצגות הכי אהבות של משתמש מסויים
        /// </summary>
        public DBservices GetTOPUserLikedContent(string UserName)
        {
            return dBservices.GetTOPUserLikedContent(UserName);
        }

        /// <summary>
        /// רשימת כל המשתמשים במערכת
        /// </summary>
        internal List<string> GetUserList()
        {
            List<string> UserList = new List<string>();
            UserList = dBservices.GetUsersForSearch();
            return UserList;
        }

   
            /// <summary>
            /// עדכון ניקוד תגיות בהתאם למקרה של המשתמש
            /// </summary>
            internal void UpdateScore(string Case, string UserName, int ContentID)
            {
                int Score = 0;
                bool Update = true;

                switch (Case)
                {
                    case "watch":
                        Score = 1;
                        Update = dBservices.CheckIfWatchedAndDownloaded(UserName, ContentID, Case);//בודק אם המשתמש צפה כבר בתוכן בעבר או לא
                        break;

                    case "downloaded":
                        Score = 3;
                        Update = dBservices.CheckIfWatchedAndDownloaded(UserName, ContentID, Case);//בודק אם הוריד מצגת בעבר
                        break;

                    case "like":
                        Score = 2;
                        dBservices.Like(UserName, ContentID, Case);
                        break;

                    case "unlike":
                        Score = -2;
                        dBservices.Like(UserName, ContentID, Case);
                        break;


                }

                if (Update)
                    dBservices.UpdateScore(Score, UserName, ContentID);
            }
        public User GetUserProfile(string UserName)
        {
            User user = new User();
            user = dBservices.GetUserProfile(UserName);
            return user;
        }

        //שליפת כל היוזרים 
        public List<User> GetUsers()
        {
          return dBservices.GetUsers();

        }

    }
}
