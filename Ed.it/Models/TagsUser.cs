using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ed.it.Models
{
    public class TagsUser
    {
        public string TagName { get; set; }
        public int Score { get; set; }
        DBservices dBservices = new DBservices();

        public TagsUser()
        {

        }

        public List<string> GetTagsList()
        {
            List<string> TagsList = new List<string>();
            TagsList=dBservices.GetTags();
            return TagsList;
        }
    }
}