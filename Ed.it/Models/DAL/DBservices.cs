using Ed.it.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

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
        //
        // TODO: Add constructor logic here
        //
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

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

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
            string query = $@"INSERT INTO _Teacher values('{user.UserName}','{user.Password}','{user.Name}','{user.Email}','{user.TeacherType}','{user.Bdate}','{user.SchoolType}','{user.AboutMe}','{user.UrlPicture}')";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery(); // execute the command
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
    /// בודק אם משתמש קיים
    /// </summary>
    internal User GetUserDetails(string UserName, string Password)
    {
        User user = new User();
        con = Connect("DBConnectionString");                                             //שליפת זהות התוכן שהועלה עכשיו
        string query = $@"SELECT * 
                          FROM _Teacher
                          WHERE UserName='{UserName}' and Password='{Password}'";
        DataTable dataTable = new DataTable();
        da = new SqlDataAdapter(query, con);
        SqlCommandBuilder builder = new SqlCommandBuilder(da);
        DataSet ds = new DataSet();
        da.Fill(ds);
        dataTable = ds.Tables[0];
        cmd = CreateCommand(query, con);
        if (dataTable.Rows.Count != 0)//אם משתמש קיים מבצע השמה לכל השדות
        {
            user.Name = dataTable.Rows[0]["Name"].ToString();
            user.UserName = dataTable.Rows[0]["UserName"].ToString();
            user.UrlPicture= dataTable.Rows[0]["UrlPicture"].ToString();
            user.SchoolType= dataTable.Rows[0]["SchoolType"].ToString();
            user.Email = dataTable.Rows[0]["Email"].ToString();
            user.Bdate= DateTime.Parse( dataTable.Rows[0]["Bdate"].ToString());
            return user;
        }
        else
            return null;

    }

    /// <summary>
    /// העלאת תוכן ע"י משתמש
    /// </summary>
    internal int UploadContent(string userId, Content content)
    {
        try
        {
            con = Connect("DBConnectionString");
            int numEffected = 0;
            string query = $@"INSERT INTO _Teacher values('{content.ContentName}','{content.PathFile}','{content.Description}','{content.UploadedDate}')";
            cmd = CreateCommand(query, con);
            numEffected += cmd.ExecuteNonQuery(); // execute the command
            //שליפת זהות התוכן שהועלה עכשיו
            query = $@"SELECT max(ContentID) FROM _Content";
            dt = new DataTable();
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            int ContentId =Convert.ToInt32(dt.Rows[0][0].ToString());
            //הכנסת תגים עבור התוכן
            for (int i = 0; i < content.TagsContent.Count; i++)
            {
                query = $@"INSERT INTO _RelatedTo values('{content.TagsContent[i]}',{ContentId})";
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
}