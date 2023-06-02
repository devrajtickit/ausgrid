using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ausgrid.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace Ausgrid.Models
{
    public class CP_User
    {
        public long recNo { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        //[DisplayName("Login Name")]
        public string LoginName { get; set; }
        public string LoginName1 { get; set; }
        //[Required(ErrorMessage = "Password is required.")]
        //[DisplayName("Password")]
        //  [DataType(DataType.Password)]
        public string password { get; set; }
        //[Required]
        //[Compare("Password", ErrorMessage = "Passwords do not match.")]
        //  [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string NewPassword { get; set; }

        public string contactNo { get; set; }
        public int entityType { get; set; }
        public int userRole { get; set; }

        public long roleId { get; set; }

        public string status { get; set; }

        public string clRef { get; set; }
        public bool clRefFlag { get; set; }
        public string companyName { get; set; }
        public string jobStatus { get; set; }
        public Nullable<System.DateTime> loginDate { get; set; }
        public Nullable<int> noOfAttempts { get; set; }
        public Nullable<System.DateTime> lastModifieDate { get; set; }
        public Nullable<System.DateTime> passwordInactiveDate { get; set; }
        public string registrationClientRef { get; set; }

        public string username { get; set; }

        public string sLogin { get; set; }
        public string uRegRef { get; set; }
        public int SecurityRole { get; set; }
        public int SecurityRoleEdit { get; set; }
        public string UsernameSRole { get; set; }

        public string clrefDropDown { get; set; }

        public string DescriptionSRole { get; set; }

        /// <summary>
        /// for UserSecurityRole checkBox
        /// </summary>
        public Boolean cboxRegistration { get; set; }
        public Boolean cboxsecurityCheck { get; set; }
        public Boolean cboxArchivedRegistrations { get; set; }
        public Boolean cboxSearch { get; set; }
        public Boolean cboxSearchResults { get; set; }


        public SqlConnection GetConnectionString()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
            //String connectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            return con;
        }

    }


}
