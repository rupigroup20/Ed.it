using Ed.it.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Linq;

/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{
    public SqlDataAdapter da;
    public DataTable dt;
    SqlConnection con;
    SqlCommand cmd;
    int counter = 0;

    public DBservices()
    {

    }




    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection Connect(String conString)
    {
        try
        {
            // read the connection string from the configuration file
            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }
        catch(Exception ex)
        {
            throw (ex);
        }
  
    }


    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 60;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

        return cmd;
    }


    /// <summary>
    /// יצירת משתמש חדש במערכת
    /// </summary>
    internal int CreateUser(User user)
    {
        try
        {           
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query = $@"INSERT INTO _User values('{user.Password}','{user.Name}','{user.Email}','{user.TeacherType}','{user.BDate}','{user.SchoolType}','{user.AboutMe.Replace("'", "''")}','{user.UrlPicture}','{user.Email.Split('@').First()}')";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery(); // execute the command

            for (int i = 0; i < user.TagsUser.Count; i++)
            {
                string query2 = $@"INSERT INTO _TagsUsedOn values({3},'{user.Email.Split('@').First()}', '{user.TagsUser[i]}')";
                cmd= CreateCommand(query2, con);
                numEffected += cmd.ExecuteNonQuery();
            }
            return numEffected;
        }
        catch (Exception ex)
        {
            return 0;
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }
    }





    /// <summary>
    ///  עדכון מספר עמודים בדטה בייס בעת העלאת מצגת
    /// </summary>
    internal Content UpdatePages(int countPages)
    {
        Content content = new Content();
        try
        {
            con = Connect("DBConnectionString");
            string query = $@"SELECT max(ContentID) FROM _Content";
            dt = new DataTable();
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            int contentID=0;
            if (dt.Rows.Count != 0)
            {
                contentID = Convert.ToInt16(dt.Rows[0][0]);
            }
            //update
            int numEffected = 0;
            query = $@"UPDATE _Content set PagesNumber={countPages} where ContentID={contentID}";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery();
            //החזרת פרטי המצגת שהועלתה עכשיו
            query = $@"SELECT PathFile,PagesNumber
                    FROM _Content
                    WHERE ContentID={contentID}";
            dt = new DataTable();
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                content.PathFile = dt.Rows[0]["PathFile"].ToString();
                content.PagesNumber = Convert.ToInt32(dt.Rows[0]["PagesNumber"]);
            }
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }
        return content;
    }


    /// <summary>
    /// בודק אם משתמש קיים
    /// </summary>
    internal User GetUserDetails(string Email, string Password)
    {
        User user = new User();
        con = Connect("DBConnectionString");                                             //שליפת זהות התוכן שהועלה עכשיו
        string query = $@"SELECT * 
                          FROM _User
                          WHERE Email='{Email}' and Password='{Password}'";
        DataTable dataTable = new DataTable();
        da = new SqlDataAdapter(query, con);
        SqlCommandBuilder builder = new SqlCommandBuilder(da);
        DataSet ds = new DataSet();
        da.Fill(ds);
        dataTable = ds.Tables[0];
        cmd = CreateCommand(query, con);
        if (dataTable.Rows.Count != 0)//אם משתמש קיים מבצע השמה לכל השדות
        {
            //user.Name = dataTable.Rows[0]["Name"].ToString();
            //user.UserName = dataTable.Rows[0]["UserName"].ToString();
            user.Name= dataTable.Rows[0]["Name"].ToString();
            user.Password = dataTable.Rows[0]["Password"].ToString();
            user.UrlPicture= dataTable.Rows[0]["UrlPicture"].ToString();
            user.SchoolType= dataTable.Rows[0]["SchoolType"].ToString();
            user.TeacherType = dataTable.Rows[0]["TeacherType"].ToString();
            user.Email = dataTable.Rows[0]["Email"].ToString();
            user.BDate=  dataTable.Rows[0]["Bdate"].ToString();
            user.AboutMe= dataTable.Rows[0]["AboutMe"].ToString();
            
            return user;
        }
        else
            return null;

    }

    /// <summary>
    /// העלאת תוכן ע"י משתמש
    /// </summary>
    internal int UploadContent(Content content)
    {
        try
        {
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query = $@"INSERT INTO _Content values('{content.ContentName.Replace("'", "''")}','{content.PathFile}','{content.ByUser}','{content.Description.Replace("'", "''")}','{content.UploadedDate}',0)";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery(); // execute the command
            //שליפת זהות התוכן שהועלה עכשיו
            query = $@"SELECT max(ContentID) FROM _Content";
            dt = new DataTable();
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            int ContentId =Convert.ToInt32(dt.Rows[0][0].ToString());
            //הכנסת תגים עבור התוכן
            for (int i = 0; i < content.TagsContent.Count; i++)
            {
                query = $@"INSERT INTO _ContentRelatedTo values('{content.TagsContent[i]}',{ContentId})";
                cmd = CreateCommand(query, con);
                numEffected += cmd.ExecuteNonQuery();
            }

            return ContentId;

           
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }
    }

    public List<string> GetTags()
    {
        List<string> Tags = new List<string>();
        SqlConnection con = null;
        string query = "";
        try
        {
            con = Connect("DBConnectionString");
            query = "select * from _TagsTest";
            cmd = CreateCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string tag;
                    tag = (string)dr["TagName"];
                    Tags.Add(tag);
                }
            }

        }

        catch (Exception ex)
        {
            // write errors to log file
            // try to handle the error
            throw ex;
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
        return Tags;// מחזיר אובייקט מסוג DBServices
    }

    public List<string> GetUsers()
    {
        List<string> Users = new List<string>();
        SqlConnection con = null;
        string query = "";
        try
        {
            con = Connect("DBConnectionString");
            query = $@"SELECT Name+'- '+UserNameByEmail as name
                       FROM _User";
            cmd = CreateCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string userName;
                    userName = (string)dr["name"];
                    Users.Add(userName);
                }
            }

        }

        catch (Exception ex)
        {
            // write errors to log file
            // try to handle the error
            throw ex;
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
        return Users;// מחזיר אובייקט מסוג DBServices
    }

    /// <summary>
    /// רשימת כל שמות המצגות
    /// </summary>
    public List<string> GetContents()
    {
        List<string> Contents = new List<string>();
        SqlConnection con = null;
        string query = "";
        try
        {
            con = Connect("DBConnectionString");
            query = $@"SELECT ContentName
                        FROM _Content";
            cmd = CreateCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string ContentName;
                    ContentName = (string)dr["ContentName"];
                    Contents.Add(ContentName);
                }
            }

        }

        catch (Exception ex)
        {
            // write errors to log file
            // try to handle the error
            throw ex;
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
        return Contents;// מחזיר אובייקט מסוג DBServices
    }

    public int UpdateDetails(User NewUser)
    {
        try
        {
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query = $@"UPDATE  _User
                            SET Password='{NewUser.Password}', Name='{NewUser.Name}', TeacherType='{NewUser.TeacherType}', BDate='{NewUser.BDate}', SchoolType='{NewUser.SchoolType}', AboutMe='{NewUser.AboutMe.Replace("'", "''")}'
                            WHERE Email='{NewUser.Email}'";
            cmd = CreateCommand(query, con);
            numEffected =cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            return 0;
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }
    }

    /// <summary>
    /// עדכון תמונת פרופיל
    /// </summary>
    public void UpdatePic(string Email, string Urlpic)
    {
        try
        {
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query = $@"UPDATE  _User
                            SET UrlPicture='{Urlpic}'
                            WHERE Email='{Email}'";
            cmd = CreateCommand(query, con);
            numEffected = cmd.ExecuteNonQuery(); // execute the command

        }
        catch (Exception ex)
        {

            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }
    }

    /// <summary>
    /// אלגוריתם חכם-הצעת תכנים למשתמש
    /// </summary>
    internal List<Content> GetSuggestionsOfContents(string userName)
    {
        Content content = new Content();
        List<Content> SuggestionList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
        List<string> TagsToSearchList = new List<string>();//רשימה של תגים שחיפשנו כדי שלא נחפש שוב את אותו תגית 
        DataTable ScoreTable = new DataTable();//טבלת עם התגים של היוזר לפי ניקוד
        DataTable ContentTagTable = new DataTable();//טבלה עבור כל התכנים של התגית המבוקשת
        DataTable TagsRealetedTable = new DataTable();//טבלה עבור התגיות שמשתייכות לתגית המבוקשת
        
        try
        {
            //שלב 1- שליפת רשימת התגים שאוהב היוזר לפי ניקוד בסדר יורד
            con = Connect("DBConnectionString");
            string query = $@"SELECT *
                              FROM _TagsUsedOn
                              WHERE UserName='{userName}'
                              ORDER BY Score desc";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ScoreTable = ds.Tables[0];

            //שלושה מחזורים באלגוריתם חכם
            string TagName="";
            if (ScoreTable.Rows.Count == 0)
                return SuggestionList;

            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(ScoreTable.Rows[i]["TagName"].ToString()))
                    TagName = ScoreTable.Rows[i]["TagName"].ToString();
                else
                    continue;

                //שלב 2
                //סופר מי 2 התווית שמתלווה הכי הרבה פעמים בתכנים בהם מופיעה גם התגית שאנחנו מחפשים
                //הראשונה כמובן תהיה התגית שאנחנו מחפשים
                query = $@"SELECT top 3 COUNT(TagName) as TagsCount,TagName
                        FROM _ContentRelatedTo
                        WHERE ContentID in(
					                        select ContentID
					                        from _ContentRelatedTo
					                        where TagName='{TagName}') 
                        GROUP by TagName
                        ORDER by TagsCount desc";
                da = new SqlDataAdapter(query, con);
                ds = new DataSet();
                da.Fill(ds);
                TagsRealetedTable = ds.Tables[0];

                //בניית השאילתה שמחזירה את כל התכנים של התגית המבוקשת בסדר יורד לפי כמות הלייקים
                //לא יוחזרו מצגות שהועלו על ידי המשתמש הנוכחי
                string TagToSearch = "";
                for (int k = 0; k < TagsRealetedTable.Rows.Count; k++)
                {
                    TagToSearch = TagsRealetedTable.Rows[k]["TagName"].ToString();
                    if(!TagsToSearchList.Contains(TagToSearch))//רק אם לא חיפשנו בעבר עבור התגית הספציפית
                    {
                        TagsToSearchList.Add(TagToSearch);//מוסיף את התגית שאנחנו הולכים להוסיף לרשימה
                                                          
                        //בניית השאילתה שמחזירה את כל התכנים של התגית המבוקשת בסדר יורד לפי כמות הלייקים                       
                        //לא יוחזרו מצגות שהועלו על ידי המשתמש הנוכחי
                        query = $@" SELECT C.ContentID,C.ContentName,C.PathFile,C.ByUser,C.Description,C.UploadDate,C.PagesNumber,R.TagName,L.Likes,U.UrlPicture
                            FROM _Content C inner join _ContentRelatedTo R on C.ContentID=R.ContentID 
                            inner join( select count(*) as Likes,ContentID
			                            from _Liked
			                            group by ContentID) as L on C.ContentID=L.ContentID inner join _User U on U.UserNameByEmail=C.ByUser
                            WHERE TagName='{TagToSearch}' and C.ByUser<>'{userName}'
                            ORDER BY Likes desc";
                        da = new SqlDataAdapter(query, con);
                        ds = new DataSet();
                        da.Fill(ds);
                        ContentTagTable = ds.Tables[0];

                        int HowManyContentsToAdd = ContentTagTable.Rows.Count;
                        if (ContentTagTable.Rows.Count >= 3)//אם יותר משלוש יקח חמישים אחוז
                            HowManyContentsToAdd = (ContentTagTable.Rows.Count) / 2;//50%

                        //הוספת התכנים לטבלת התכנים
                        for (int j = 0; j < HowManyContentsToAdd; j++)
                        {
                            //שליפת פרטים כללים על המצגת-רק להצגה 
                            if (!string.IsNullOrEmpty(ContentTagTable.Rows[j]["ContentID"].ToString()))
                            {
                                content.ContentID = Convert.ToInt32(ContentTagTable.Rows[j]["ContentID"]);
                                if (!SuggestionList.Exists(co => co.ContentID == content.ContentID))//רק אם לא מכיל כבר את התוכן
                                {
                                    content.ContentName = ContentTagTable.Rows[j]["ContentName"].ToString();
                                    content.Description = ContentTagTable.Rows[j]["Description"].ToString();
                                    content.PathFile = ContentTagTable.Rows[j]["PathFile"].ToString();
                                    content.PathFile = content.PathFile.Split('.').First() + "_1.jpg";//מציגים את התמונה הראשונה
                                    content.UserPic = ContentTagTable.Rows[j]["UrlPicture"].ToString();
                                    SuggestionList.Add(content);//מוסיף לרשימת ההמלצות
                                    content = new Content();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return SuggestionList;
    }

    /// <summary>
    /// הצעת תכנים לאורח-התכנים עם הכי הרבה לייקים ללא קשר לתגיות
    /// </summary>
    internal List<Content> GetSuggestionsOfContentsForGuest()
    {
        Content content = new Content();
        List<Content> SuggestionList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
        DataTable ContentTagTable = new DataTable();
        try
        {
            con = Connect("DBConnectionString");
            //מביא את 10 התכנים עם הכי הרבה לייקים
            string query = $@"SELECT Top 10 C.ContentID,C.ContentName,C.PathFile,CAST(Description AS NVARCHAR(200)) as Description,L.Likes,U.UrlPicture
                            FROM _Content C inner join _ContentRelatedTo R on C.ContentID=R.ContentID 
                            inner join( select count(*) as Likes,ContentID
			                            from _Liked
			                            group by ContentID) as L on C.ContentID=L.ContentID inner join _User as U on U.UserNameByEmail=C.ByUser
                            Group by C.ContentID,C.ContentName,C.PathFile,L.Likes,CAST(Description AS NVARCHAR(200)),U.UrlPicture
                            ORDER BY Likes desc";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ContentTagTable = ds.Tables[0];
           
            //הוספת התכנים לרשימה
            for (int j = 0; j < ContentTagTable.Rows.Count; j++)
            {
                //שליפת פרטים כללים על המצגת-רק להצגה 
                if (!string.IsNullOrEmpty(ContentTagTable.Rows[j]["ContentID"].ToString()))
                {
                    content.ContentID = Convert.ToInt32(ContentTagTable.Rows[j]["ContentID"]);
                    if (!SuggestionList.Exists(co => co.ContentID == content.ContentID))//רק אם לא מכיל כבר את התוכן
                    {
                        content.ContentName = ContentTagTable.Rows[j]["ContentName"].ToString();
                        content.Description = ContentTagTable.Rows[j]["Description"].ToString();
                        content.PathFile = ContentTagTable.Rows[j]["PathFile"].ToString();
                        content.PathFile = content.PathFile.Split('.').First() + "_1.jpg";//מציגים את התמונה הראשונה
                        content.UserPic = ContentTagTable.Rows[j]["UrlPicture"].ToString();
                        SuggestionList.Add(content);//מוסיף לרשימת ההמלצות
                        content = new Content();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return SuggestionList;
    }

    /// <summary>
    /// חיפוש תכנים לפי תגית
    /// </summary>
    internal List<Content> Search(string TagToSearch)
    {
        Content content = new Content();
        List<Content> ResultList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
        DataTable TagsRealetedTable = new DataTable();//טבלה עבור התגיות שמשתייכות לתגית המבוקשת
        DataTable ContentTagTable = new DataTable();//טבלה עבור כל התכנים של התגית המבוקשת

        try
        {
            //מציאת תגיות שהכי קשורות לתגית שרשמנו במנוע חיפוש
            //סופר מי 2 התווית שמתלווה הכי הרבה פעמים בתכנים בהם מופיעה גם התגית שאנחנו מחפשים
            //הראשונה כמובן תהיה התגית שאנחנו מחפשים
            con = Connect("DBConnectionString");
            string query = $@"SELECT top 3 COUNT(TagName) as TagsCount,TagName
                        FROM _ContentRelatedTo
                        WHERE ContentID in(
					                        select ContentID
					                        from _ContentRelatedTo
					                        where TagName='{TagToSearch}') 
                        GROUP by TagName
                        ORDER by TagsCount desc";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            TagsRealetedTable = ds.Tables[0];
            string TagName ="";
            for (int i = 0; i < TagsRealetedTable.Rows.Count; i++)
            {
                TagName = TagsRealetedTable.Rows[i]["TagName"].ToString();
                //בניית השאילתה שמחזירה את כל התכנים של התגית המבוקשת בסדר יורד לפי כמות הלייקים
                //לא יוחזרו מצגות שהועלו על ידי המשתמש הנוכחי
                query = $@" SELECT C.ContentID,C.ContentName,C.PathFile,C.ByUser,C.Description,C.UploadDate,C.PagesNumber,R.TagName,L.Likes,U.UrlPicture
                            FROM _Content C inner join _ContentRelatedTo R on C.ContentID=R.ContentID 
                            inner join( select count(*) as Likes,ContentID
			                            from _Liked
			                            group by ContentID) as L on C.ContentID=L.ContentID inner join _User U on U.UserNameByEmail=C.ByUser
                            WHERE TagName='{TagName}' 
                            ORDER BY Likes desc";
                da = new SqlDataAdapter(query, con);
                ds = new DataSet();
                da.Fill(ds);
                ContentTagTable = ds.Tables[0];

                //הוספת התכנים לטבלת התכנים
                for (int j = 0; j < ContentTagTable.Rows.Count; j++)
                {
                    //שליפת פרטים כללים על המצגת-רק להצגה 
                    if (!string.IsNullOrEmpty(ContentTagTable.Rows[j]["ContentID"].ToString()))
                    {
                        content.ContentID = Convert.ToInt32(ContentTagTable.Rows[j]["ContentID"]);
                        if (!ResultList.Exists(co => co.ContentID == content.ContentID))//רק אם לא מכיל כבר את התוכן
                        {
                            content.ContentName = ContentTagTable.Rows[j]["ContentName"].ToString();
                            content.Description = ContentTagTable.Rows[j]["Description"].ToString();
                            content.PathFile = ContentTagTable.Rows[j]["PathFile"].ToString();
                            content.PathFile = content.PathFile.Split('.').First() + "_1.jpg";//מציגים את התמונה הראשונה
                            content.UserPic = ContentTagTable.Rows[j]["UrlPicture"].ToString();
                            ResultList.Add(content);//מוסיף לרשימת ההמלצות
                            content = new Content();
                        }
                    }
                }
       
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return ResultList;
    }

    /// <summary>
    /// חיפוש תכנים לפי שם 
    /// </summary>
    internal List<Content> SearchByName(string name)
    {
        Content content = new Content();
        List<Content> ResultList = new List<Content>();//בניית רשימת התכנים המוצעים -מה שיוחזר בסוף
        DataTable ContentTable = new DataTable();//טבלה עבור כל התכנים של התגית המבוקשת

        try
        {
            con = Connect("DBConnectionString");
            string query = $@"SELECT C.ContentID,C.ContentName,c.PathFile,c.ByUser,C.UploadDate,C.PagesNumber,U.UrlPicture
                           FROM _Content C inner join _ContentRelatedTo R on C.ContentID=R.ContentID 
                           inner join _User U on U.UserNameByEmail=C.ByUser
						   WHERE ContentName LIKE  '{name}%'
                           GROUP BY C.ContentID,C.ContentName,c.PathFile,c.ByUser,C.UploadDate,C.PagesNumber,U.UrlPicture";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ContentTable = ds.Tables[0];
            //הוספת התכנים לטבלת התכנים
            for (int j = 0; j < ContentTable.Rows.Count; j++)
            {
                //שליפת פרטים כללים על המצגת-רק להצגה 
                if (!string.IsNullOrEmpty(ContentTable.Rows[j]["ContentID"].ToString()))
                {
                    content.ContentID = Convert.ToInt32(ContentTable.Rows[j]["ContentID"]);
                    if (!ResultList.Exists(co => co.ContentID == content.ContentID))//רק אם לא מכיל כבר את התוכן
                    {
                        content.ContentName = ContentTable.Rows[j]["ContentName"].ToString();
                        content.PathFile = ContentTable.Rows[j]["PathFile"].ToString();
                        content.PathFile = content.PathFile.Split('.').First() + "_1.jpg";//מציגים את התמונה הראשונה
                        content.UserPic = ContentTable.Rows[j]["UrlPicture"].ToString();
                        ResultList.Add(content);
                        content = new Content();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return ResultList;
    }



    internal Content GetContent(string ContentID,string UserName)
    {
        Content content = new Content();
        try
        {
            con = Connect("DBConnectionString");
            string query = $@"select c.ContentID, c.ContentName, c.PathFile, c.ByUser , c.Description, c.UploadDate, L.Likes, U.UrlPicture as UserPic,c.PagesNumber
                              from _Content C inner join (select count( ContentID) as Likes, ContentID
                                                          from _Liked
                                                          group by ContentID) as L on C.ContentID=L.ContentID 
                              inner join _User U on C.ByUser=U.UserNameByEmail
                              where C.ContentID='{ContentID}'";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count != 0)//אם משתמש קיים מבצע השמה לכל השדות
            {
                content.ContentID = Convert.ToInt32(dt.Rows[0]["ContentID"]);
                content.ContentName = dt.Rows[0]["ContentName"].ToString();
                content.PathFile = dt.Rows[0]["PathFile"].ToString();
                content.ByUser = dt.Rows[0]["ByUser"].ToString();
                content.Description = dt.Rows[0]["Description"].ToString();
                content.UploadedDate = dt.Rows[0]["UploadDate"].ToString();
                content.Likes = Convert.ToInt32(dt.Rows[0]["Likes"]);
                content.UserPic= dt.Rows[0]["UserPic"].ToString();
                content.PagesNumber= Convert.ToInt32(dt.Rows[0]["PagesNumber"]);
            }
            //קבלת התגיות המשתייכות לתוכן
            query = $@"SELECT TagName
                    FROM _ContentRelatedTo
                    WHERE ContentID={ContentID}";
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                content.TagsContent = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    content.TagsContent.Add(dt.Rows[i]["TagName"].ToString());
                }
            }

            //קבלת רשימת תגובות על המצגת
            query = $@" SELECT C.UserName,C.Comment,C.PublishDate,U.UrlPicture,U.Name
                        FROM _Comments C inner join _User U on C.UserName=U.UserNameByEmail 
                        WHERE ContentID={ContentID}
                        order by PublishDate desc ";
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                Comments comment = new Comments();
                content.CommentsList = new List<Comments>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    comment.NameWhoCommented = dt.Rows[i]["Name"].ToString();
                    comment.PublishedDate = DateTime.Parse(dt.Rows[i]["PublishDate"].ToString()).ToString("dd/MM/yyyy H:mm");
                    comment.Comment= dt.Rows[i]["Comment"].ToString();
                    comment.UrlPictureWhoCommented = dt.Rows[i]["UrlPicture"].ToString();
                    content.CommentsList.Add(comment);
                    comment = new Comments();
                }
            }

            //האם המשתמש עשה לייק על התוכן בעבר
            query = $@"SELECT *
                              FROM _Liked
                              WHERE UserName='{UserName}' and ContentID={ContentID}";
            da = new SqlDataAdapter(query, con);
            ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                content.LikedByUserWhoWatch = false;
            }
            else
            {
                content.LikedByUserWhoWatch = true;
            }

        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return content;
    }

    /// <summary>
    /// עדכון ניקוד תגיות בהתאם למקרה של המשתמש
    /// </summary>
    internal void UpdateScore(int score, string userName, int contentID)
    {
        DataTable ScoreTable = new DataTable();
        SqlDataAdapter daUpdate;
        try
        {
            con = Connect("DBConnectionString");
            string query = $@"SELECT *
                              FROM _TagsUsedOn
                              WHERE UserName='{userName}'
                              ORDER BY Score desc";
            daUpdate = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            daUpdate.Fill(ds);
            ScoreTable = ds.Tables[0];


            //קבלת התגיות המשתייכות לתוכן
            string query1 = $@"SELECT TagName
                    FROM _ContentRelatedTo
                    WHERE ContentID={contentID}";
            da = new SqlDataAdapter(query1, con);
            DataSet ds1 = new DataSet();
            da.Fill(ds1);
            dt = ds1.Tables[0];

            //רשימת התגים הקיימים בטבלת הניקוד
            var listTags = ScoreTable.Rows.OfType<DataRow>()
                   .Select(dr => dr.Field<string>("TagName")).ToList();
            //התגים של התוכן הנצפה
            var listTagsOfContent = dt.Rows.OfType<DataRow>()
                   .Select(dr => dr.Field<string>("TagName")).ToList();


            int index = -1;
            for (int i = 0; i < listTagsOfContent.Count; i++)
            {
                index = listTags.FindIndex(a => a == listTagsOfContent[i].Trim());
                int numEffected = 0;

                if (index != -1)//קיים בטבלת הניקוד של המשתמש
                {
                    query = $@"UPDATE _TagsUsedOn SET Score=Score+{score} WHERE TagName='{ScoreTable.Rows[index]["TagName"]}' and UserName='{userName}'";
                }
                else  //לא קיים בטבלת הניקוד לכן נוספת תגית חדשה
                {
                    query = $@"INSERT INTO _TagsUsedOn values({score},'{userName}','{listTagsOfContent[i]}')";
                }
                cmd = CreateCommand(query, con);
                numEffected += cmd.ExecuteNonQuery();
                index = -1;
            }
        

        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    /// <summary>
    // בודק אם משתמש צפה בתוכן כבר או לא
    /// </summary>
    internal bool CheckIfWatchedAndDownloaded(string userName,int ContentId,string Case)
    {
        DataTable WatchedTable = new DataTable();
        bool UpdateScore;
        try
        {
            con = Connect("DBConnectionString");
            string query = $@"SELECT *
                              FROM _Watched
                              WHERE UserName='{userName}' and ContentID={ContentId}";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            WatchedTable = ds.Tables[0];

            if (WatchedTable.Rows.Count==0)//משתמש לא צפה בתוכן עדיין
            {
                int numEffected = 0;
                query = $@"INSERT INTO _Watched values('{userName}',{ContentId},'false')";
                cmd = CreateCommand(query, con);
                numEffected += cmd.ExecuteNonQuery();
                UpdateScore=true;//יעדכן ניקוד
            }
            else
            {
                if (Case == "downloaded" && WatchedTable.Rows[0]["Downloaded"].ToString() == "False")//אם המקרה הוא הורדת מצגת בודק אם הורד בעבר או לא
                {
                    UpdateScore = true;
                    int numEffected = 0;
                    query = $@"UPDATE _Watched SET Downloaded='True' WHERE UserName='{userName}' and ContentID={ContentId} ";
                    cmd = CreateCommand(query, con);
                    numEffected += cmd.ExecuteNonQuery();
                }
                else //צפה כבר או הוריד כבר מצגת
                {
                    UpdateScore = false;//לא יעדכן ניקוד
                }
            }

        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
        return UpdateScore;
    }

    /// <summary>
    /// משתמש הוריד לייק מהתוכן
    /// </summary>
    internal void Like(string UserName,int ContentID,string LikeORUnlike)
    {
        try
        {
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query="";
            //אם עשה אנלייק מוחק רשומה ,אחרת מכניס
            if (LikeORUnlike == "unlike")
                query = $@"DELETE from _Liked WHERE UserName='{UserName}' and ContentID={ContentID}";
            else
                query = $@"INSERT INTO _Liked values('{UserName}',{ContentID})";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    /// <summary>
    /// הוספת תגובה למצגת
    /// </summary>
    internal List<Comments> AddComment(Comments comments)
    {
        List<Comments> commentList = new List<Comments>();
        try
        {
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query = $@"INSERT INTO _Comments values('{comments.NameWhoCommented}',{comments.ContentID},'{comments.Comment}','{comments.PublishedDate}')";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery(); // execute the command
            
            //שליפת התגובות מחדש
            query = $@" SELECT C.UserName,C.Comment,C.PublishDate,U.UrlPicture,U.Name
                        FROM _Comments C inner join _User U on C.UserName=U.UserNameByEmail 
                        WHERE ContentID={comments.ContentID}
                        order by PublishDate desc ";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count != 0)
            {
                Comments comment = new Comments();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    comment.NameWhoCommented = dt.Rows[i]["Name"].ToString();
                    comment.PublishedDate = DateTime.Parse(dt.Rows[i]["PublishDate"].ToString()).ToString("dd/MM/yyyy H:mm");
                    comment.Comment= dt.Rows[i]["Comment"].ToString();
                    comment.UrlPictureWhoCommented = dt.Rows[i]["UrlPicture"].ToString();
                    commentList.Add(comment);
                    comment = new Comments();
                }
            }

            return commentList;


        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }
    }

    /// <summary>
    ///  שליפת רשימה של כל התכנים שהעלה משתמש מסויים + כמות לייקים לתוכן
    /// </summary>
    public List<Content> GetUserContents(string UserName)
    { // לסיים
        List<Content> UserContent = new List<Content>();

        try
        {
            con = Connect("DBConnectionString");
            string query = $@"select c.ContentID, c.ContentName, c.PathFile, c.ByUser , c.Description, c.UploadDate, L.Likes, U.UrlPicture as UserPic,c.PagesNumber
                              from _Content C left join (select count( ContentID) as Likes, ContentID
                                                          from _Liked
                                                          group by ContentID) as L on C.ContentID=L.ContentID 
                              inner join _User U on C.ByUser=U.UserNameByEmail
                              where C.ByUser='{UserName}'";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows.Count != 0)//אם משתמש קיים מבצע השמה לכל השדות
                {
                    Content content = new Content();
                    content.ContentID = Convert.ToInt32(dt.Rows[i]["ContentID"]);
                    content.ContentName = dt.Rows[i]["ContentName"].ToString();
                    content.PathFile = dt.Rows[i]["PathFile"].ToString();
                    content.PathFile = content.PathFile.Split('.').First() + "_1.jpg";
                    content.ByUser = dt.Rows[i]["ByUser"].ToString();
                    content.Description = dt.Rows[i]["Description"].ToString();
                    content.UploadedDate = dt.Rows[i]["UploadDate"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[i]["Likes"].ToString()))
                    {
                        content.Likes = Convert.ToInt32(dt.Rows[i]["Likes"]);
                    }
                    else
                    {
                        content.Likes = 0;
                    }
                    
                    content.UserPic = dt.Rows[i]["UserPic"].ToString();
                    content.PagesNumber = Convert.ToInt32(dt.Rows[i]["PagesNumber"]);
                    UserContent.Add(content);
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return UserContent;
    }

    /// <summary>
    /// שליפת מצגות שמשתמש אהב
    /// </summary>
    public List<Content> GetUserLikedContents(string UserName)
    {
        List<Content> UserLikedContent = new List<Content>();

        try
        {
            con = Connect("DBConnectionString");
            string query = $@"select C.*,U.UrlPicture as UserPic
                              from _Content C inner join _User U on C.ByUser=U.UserNameByEmail 
                              where ContentID in 
                                                (select ContentID
                                                 from _Liked
                                                 where UserName='{UserName}')";
            da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows.Count != 0)//אם משתמש קיים מבצע השמה לכל השדות
                {
                    Content content = new Content();
                    content.ContentID = Convert.ToInt32(dt.Rows[i]["ContentID"]);
                    content.ContentName = dt.Rows[i]["ContentName"].ToString();
                    content.PathFile = dt.Rows[i]["PathFile"].ToString();
                    content.PathFile = content.PathFile.Split('.').First() + "_1.jpg";
                    content.ByUser = dt.Rows[i]["ByUser"].ToString();
                    content.Description = dt.Rows[i]["Description"].ToString();
                    content.UploadedDate = dt.Rows[i]["UploadDate"].ToString();
                    //content.Likes = Convert.ToInt32(dt.Rows[i]["Likes"]);
                    content.UserPic = dt.Rows[i]["UserPic"].ToString();
                    content.PagesNumber = Convert.ToInt32(dt.Rows[i]["PagesNumber"]);
                    UserLikedContent.Add(content);
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

        return UserLikedContent;
    }

    public DBservices GetTOPUserLikedContent(string UserName)
    {
        SqlConnection con = null;
        try
        {
            con = Connect("DBConnectionString");
            string query = $@"select top 8 C.ContentName, L.Likes 
                              from _Content C inner join
                                         (select count(ContentID) as Likes, ContentID
                                          from _Liked
                                          group by ContentID) L on C.ContentID=L.ContentID
                                          where C.ByUser='{UserName}'
                                          order by L.Likes desc";
            da = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
        return this;
    }
}
