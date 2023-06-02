using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ausgrid.Models;

namespace Ausgrid.Controllers
{
    public class ZoomUserInterFaceController : Controller
    {
        static SqlConnection con;
        // string connectionString = GetConnection.GetConnectionString();
        // SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
        // GET: ZoomUserInterFace
        public ActionResult Index(CP_User cp)
        {
            con = cp.GetConnectionString();
            var loginName = Session["loginName"] as string;
            Session["loginName"] = loginName;
            con.Open();
            string sqlprofile = "select * from CP_User where LoginName='" + loginName + "'";
            SqlCommand cmdprofile = new SqlCommand(sqlprofile, con);
            SqlDataReader sdprofile = cmdprofile.ExecuteReader();
            while (sdprofile.Read())
            {
                String firstName = sdprofile["firstName"].ToString();
                String LoginName = sdprofile["LoginName"].ToString();
                String lastName = sdprofile["lastName"].ToString();
                String companyName = sdprofile["companyName"].ToString();
                String contactNo = sdprofile["contactNo"].ToString();
                ViewBag.Data1 = LoginName;
                ViewBag.Data2 = firstName;
                ViewBag.Data3 = lastName;
                ViewBag.Data4 = contactNo;
                ViewBag.Data5 = companyName;
            }

            con.Close();
            con.Open();
            string sqlpolicy = "select * from CP_PasswordPolicy";
            SqlCommand cmdpolicy = new SqlCommand(sqlpolicy, con);
            SqlDataReader sdpolicy = cmdpolicy.ExecuteReader();
            while (sdpolicy.Read())
            {
                String MinLength = sdpolicy["MinLength"].ToString();
                String MaxLength = sdpolicy["MaxLength"].ToString();
                String LetterReqLength = sdpolicy["LetterReqLength"].ToString();
                String DigitReqLength = sdpolicy["DigitReqLength"].ToString();
                String ChangeRequiredDay = sdpolicy["ChangeRequiredDay"].ToString();
                String ChangeWarningDay = sdpolicy["ChangeWarningDay"].ToString();
                String MustNotSame = sdpolicy["MustNotSame"].ToString();
                String DisableWrongPasswordAttempt = sdpolicy["DisableWrongPasswordAttempt"].ToString();
                String PreventLogonUnusedDays = sdpolicy["PreventLogonUnusedDays"].ToString();
                String MinCapitalLetters = sdpolicy["MinCapitalLetters"].ToString();
                String MinSmallLetters = sdpolicy["MinSmallLetters"].ToString();
                String MinSpecialCharacters = sdpolicy["MinSpecialCharacters"].ToString();

                ViewBag.MinLength = MinLength;
                ViewBag.MaxLength = MaxLength;
                ViewBag.LetterReqLength = LetterReqLength;
                ViewBag.DigitReqLength = DigitReqLength;
                ViewBag.ChangeRequiredDay = ChangeRequiredDay;
                ViewBag.ChangeWarningDay = ChangeWarningDay;
                ViewBag.MustNotSame = MustNotSame;
                ViewBag.DisableWrongPasswordAttempt = DisableWrongPasswordAttempt;
                ViewBag.PreventLogonUnusedDays = PreventLogonUnusedDays;
                ViewBag.MinCapitalLetters = MinCapitalLetters;
                ViewBag.MinSmallLetters = MinSmallLetters;
                ViewBag.MinSpecialCharacters = MinSpecialCharacters;
            }


            con.Close();
            /////// //for show Home Page grid///////////////
            List<CP_User> GetAll()
            {
                List<CP_User> user = new List<CP_User>();
                con.Open();
                string sql = "select * from CP_User where userRole !=0";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader sd = cmd.ExecuteReader();
                while (sd.Read())
                {
                    CP_User rec = new CP_User();
                    rec.firstName = sd["firstName"].ToString();
                    rec.LoginName = sd["LoginName"].ToString();
                    rec.lastName = sd["lastName"].ToString();
                    rec.companyName = sd["companyName"].ToString();
                    rec.userRole = Convert.ToInt32(sd["userRole"]);
                    rec.contactNo = sd["contactNo"].ToString();

                    if (sd["status"].ToString() == "1")
                    {
                        rec.status = "Active";
                    }
                    if (sd["status"].ToString() == "2")
                    {
                        rec.status = "Suspanded";
                    }
                    if (sd["status"].ToString() == "3")
                    {
                        rec.status = "Close";
                    }
                    rec.clRef = sd["clRef"].ToString();
                    rec.jobStatus = sd["jobStatus"].ToString();

                    user.Add(rec);
                }

                con.Close();
                return user;

            }
            List<CP_User> users = GetAll();

            /////// //for show security role tab grid///////////////
            List<securityRole> GetAll1()
            {
                List<securityRole> user = new List<securityRole>();
                DataTable dt = new DataTable();

                con.Open();
                string sql = "select * from CP_SecurityRole";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader sd = cmd.ExecuteReader();
                while (sd.Read())
                {
                    securityRole rec = new securityRole();
                    rec.name = sd["name"].ToString();
                    rec.roleid = sd["roleid"].ToString();
                    rec.description = sd["description"].ToString();

                    user.Add(rec);
                }

                con.Close();
                return user;

            }

            List<securityRole> users1 = GetAll1();
            var viewModel = new CombineData
            {
                list1 = users.ToList(),
                list2 = users1
            };


            return View(viewModel);


        }

        [HttpPost]
        public ActionResult Security_tab()
        {
            List<securityRole> GetAll1()
            {
                List<securityRole> user = new List<securityRole>();
                DataTable dt = new DataTable();

                con.Open();
                string sql = "select * from CP_SecurityRole";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader sd = cmd.ExecuteReader();
                while (sd.Read())
                {
                    securityRole rec = new securityRole();
                    rec.name = sd["name"].ToString();
                    rec.roleid = sd["roleid"].ToString();
                    rec.description = sd["description"].ToString();

                    user.Add(rec);
                }

                con.Close();
                return user;

            }

            List<securityRole> users1 = GetAll1();
            var viewModel = new CombineData
            {

                list2 = users1
            };

            return View("Security_tab", viewModel);
        }

        [HttpPost]
        public ActionResult Edit_dialog11(CP_User cp)
        {
            int SecurityRoleEdit = cp.SecurityRoleEdit;
            con.Close();
            con.Open();

            string sql = "update CP_User set firstname='" + cp.firstName.Trim() + "', LoginName='" + cp.LoginName.Trim() + "', lastname='" + cp.lastName.Trim() + "', companyname='" + cp.companyName + "', contactno='" + cp.contactNo + "', status=" + cp.status + ", jobstatus='" + cp.jobStatus + "', clref='" + cp.clRef.Trim() + "', noOfAttempts=0,lastModifieDate=getdate(), roleId='" + SecurityRoleEdit + "' where loginname='" + cp.LoginName.Trim() + "' ";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();
            TempData["msg"] = "<script>alert('user updated sucessful..!!');</script>";
            return RedirectToAction("index", "ZoomUserInterFace");
        }

        public JsonResult GetItems(securityRole sr)
        {
            List<securityRole> srRole = new List<securityRole>();

            SqlCommand cmd = new SqlCommand("select roleId,name from cp_securityRole ", con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                sr.roleid = rdr["roleId"].ToString();
                sr.name = rdr["name"].ToString();
                srRole.Add(sr);
            }
            con.Close();

            return Json(srRole, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UserAddform(CP_User cp)
        {
            int SecurityRole = cp.SecurityRole;
            string password = "";
            int status = 0;
            String recNo = " ";
            String passwordforHistory = " ";
            String currentDate = " ";
            con.Close();
            con.Open();

            string sql3 = "select loginName from CP_User where loginName='" + cp.LoginName + "'";
            SqlCommand sc = new SqlCommand(sql3, con);
            String checkuser = Convert.ToString(sc.ExecuteScalar()).Trim();
            con.Close();
            if (checkuser == "")
            {

                if (cp.password == cp.ConfirmPassword)
                {
                    password = PwdEncryption(cp.LoginName, cp.password);



                    if (cp.status == "Active")
                    {
                        status = 1;
                    }
                    if (cp.status == "Suspanded")
                    {
                        status = 2;
                    }
                    if (cp.status == "Close")
                    {
                        status = 3;
                    }

                    con.Close();
                    con.Open();
                    string sql = "insert into CP_User (firstname,LoginName,lastname,companyname,contactno,status,jobstatus,clref,password,noOfAttempts,loginDate,entityType,userRole,roleId,lastModifieDate) values('" + cp.firstName + "','" + cp.LoginName.Trim() + "','" + cp.lastName + "', '" + cp.companyName + "','" + cp.contactNo + "'," + status + ",'" + cp.jobStatus + "','" + cp.clRef + "','" + password + "',0,getdate(),1,1,'" + SecurityRole + "',getdate())";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();
                    string sql1 = "select * from CP_User where LoginName='" + cp.LoginName + "'";
                    SqlCommand cmd1 = new SqlCommand(sql1, con);
                    SqlDataReader sd1 = cmd1.ExecuteReader();
                    while (sd1.Read())
                    {

                        recNo = sd1["recNo"].ToString();
                        passwordforHistory = sd1["password"].ToString();
                        currentDate = Convert.ToString(sd1["lastModifieDate"]);

                    }
                    con.Close();

                    con.Open();
                    string sql2 = "insert into CP_PasswordHistory (userId,password,lastModifieDate,IsActive) values('" + recNo + "','" + password + "', Convert(datetime,'" + Convert.ToDateTime(currentDate) + "',103) ,1) ";

                    SqlCommand cmd2 = new SqlCommand(sql2, con);
                    cmd2.ExecuteNonQuery();
                    con.Close();
                    TempData["msg"] = "<script>alert('Add user sucessful..!!');</script>";
                }
                else
                {
                    TempData["msg"] = "<script>alert('password not matched..!!');</script>";
                }
            }
            else
            {
                TempData["msg"] = "<script>alert('User Already Exist..!!');</script>";
            }
            return RedirectToAction("index", "ZoomUserInterFace");
        }
        [HttpPost]
        public ActionResult changePwd(CP_User cp)
        {
            ///////////Change password for users///////////////////
            string newpwd = " ";
            if (cp.NewPassword != null)
            {
                if (cp.NewPassword == cp.ConfirmPassword)
                {
                    newpwd = PwdEncryption(cp.LoginName, cp.NewPassword);
                    var sql = "upadte CP_User set password='" + newpwd + "' where LoginName='" + (cp.LoginName).Trim() + "' ";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    TempData["msg"] = "<script>alert('Change password successfull..!!');</script>";
                }
                else
                {
                    TempData["msg"] = "<script>alert('New Password and confirm password not matched..!!');</script>";

                }
            }

            return RedirectToAction("index", "ZoomUserInterFace");
        }


        [HttpPost]
        public ActionResult changePwdAdmin(CP_User cp)
        {

            ///////////Change password for Admin///////////////////
            string loginName = Session["loginName"] as string;
            string newpwd = " ";
            if (cp.NewPassword != null)
            {
                if (cp.NewPassword == cp.ConfirmPassword)
                {
                    newpwd = PwdEncryption(loginName, cp.NewPassword);
                    var sql = "upadte CP_User set password='" + newpwd + "' where LoginName='" + (loginName).Trim() + "' ";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    TempData["msg"] = "<script>alert('Change password successfull..!!');</script>";
                }
                else
                {
                    TempData["msg"] = "<script>alert('New Password and confirm password not matched..!!');</script>";

                }

            }
            return RedirectToAction("index", "ZoomUserInterFace");
        }
        [HttpPost]
        public ActionResult pwdPolicy(PasswordPolicy pp)
        {

            int MinLength = pp.MinLength;
            int MaxLength = pp.MaxLength;
            int LetterReqLength = pp.LetterReqLength;
            int DigitReqLength = pp.DigitReqLength;
            int ChangeRequiredDay = pp.ChangeRequiredDay;
            int ChangeWarningDay = pp.ChangeWarningDay;
            int MustNotSame = pp.MustNotSame;
            int DisableWrongPasswordAttempt = pp.DisableWrongPasswordAttempt;
            int PreventLogonUnusedDays = pp.PreventLogonUnusedDays;
            int MinCapitalLetters = pp.MinCapitalLetters;
            int MinSmallLetters = pp.MinSmallLetters;
            int MinSpecialCharacters = pp.MinSpecialCharacters;

            if ((LetterReqLength + DigitReqLength + MinSpecialCharacters) > MaxLength)
            {
                TempData["msg"] = "<script>alert('Sum of Minimum Letter and Minimum Digit can't be greater then Max Password Length.');</script>";

            }

            else if ((MinCapitalLetters + MinSmallLetters) > LetterReqLength)
            {
                TempData["msg"] = "<script>alert('Sum of Minimum Capital Letter,Minimum Small Letter can't be greater then Min Letter Length.');</script>";

            }
            else if (ChangeWarningDay > ChangeRequiredDay)
            {
                TempData["msg"] = "<script>alert('Warning Day must be Less then Change Required Day.');</script>";
            }
            else if (ChangeWarningDay <= 3 || ChangeRequiredDay <= 3)
            {
                TempData["msg"] = "<script>alert('Warning day or change required day must be greater then 3.');</script>";
            }
            else
            {
                con.Open();
                var sql = "update CP_PasswordPolicy set MinLength=" + MinLength + " , MaxLength=" + MaxLength + ", LetterReqLength=" + LetterReqLength + " , DigitReqLength=" + DigitReqLength + ",ChangeRequiredDay=" + ChangeRequiredDay + ",ChangeWarningDay=" + ChangeWarningDay + ", MustNotSame=" + MustNotSame + ",DisableWrongPasswordAttempt=" + DisableWrongPasswordAttempt + ", PreventLogonUnusedDays=" + PreventLogonUnusedDays + ", MinCapitalLetters=" + MinCapitalLetters + ",  MinSmallLetters= " + MinSmallLetters + ", MinSpecialCharacters=" + MinSpecialCharacters + " ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                TempData["msg"] = "<script>alert('Password policy has been updated successfully.');</script>";
                con.Close();
            }
            return RedirectToAction("index", "ZoomUserInterFace");
        }



        [NonAction]
        public string PwdEncryption(string sLogin, string sPwd)
        {
            int PassInt;
            string FinalPwd, Appendtempstr;

            string encyPassword;
            encyPassword = "";
            FinalPwd = "";

            byte[] PwdBytes = Encoding.ASCII.GetBytes(sPwd);
            byte[] UserIDBytes = Encoding.ASCII.GetBytes(sLogin);

            if (sLogin == "") sLogin = "MULTICRAFTS";

            int length = sPwd.Length;
            for (int i = 0; i < length; i++)
            {

                if (UserIDBytes.Length <= i)
                {
                    PassInt = 32;
                }
                else
                {
                    PassInt = UserIDBytes[i];
                }

                Appendtempstr = (PwdBytes[i] + PassInt).ToString();

                if (Appendtempstr.Length < 3)
                {
                    Appendtempstr = "000" + Appendtempstr;
                    Appendtempstr = Appendtempstr.Substring(Appendtempstr.Length - 3, 3);
                }
                FinalPwd = FinalPwd + Appendtempstr;
            }

            int pwdLen = FinalPwd.Length;
            char[] pwd = FinalPwd.ToCharArray();
            string spwd, sCompletePwd;
            sCompletePwd = "";
            int pwdTotal = 0;

            for (int i = 0; i < pwdLen; i++)
            {
                spwd = pwd[i].ToString();
                pwdTotal = pwdTotal + int.Parse(spwd);
            }

            if (pwdTotal.ToString().Length < 3)
            {
                sCompletePwd = "000" + pwdTotal;
                sCompletePwd = sCompletePwd.Substring(sCompletePwd.Length - 3, 3);
            }

            encyPassword = (sCompletePwd + FinalPwd);

            return encyPassword;
        }

        [HttpPost]
        public ActionResult AddSecurityRole(CP_User cp)
        {
            int roleID = 0;
            Boolean cboxRegistration = cp.cboxRegistration;
            Boolean cboxsecurityCheck = cp.cboxsecurityCheck;
            Boolean cboxArchivedRegistrations = cp.cboxArchivedRegistrations;
            Boolean cboxSearch = cp.cboxSearch;
            Boolean cboxSearchResults = cp.cboxSearchResults;


            try
            {
                con.Close();
                con.Open();
                var RoleNameCheck = "select * from CP_SecurityRole where name='" + cp.UsernameSRole + "'";
                SqlCommand sc = new SqlCommand(RoleNameCheck, con);
                var check = sc.ExecuteScalar();
                if (check == null)
                {

                    con.Close();
                    con.Open();
                    string sql = "insert into CP_SecurityRole (name,description,systemDefined) values('" + cp.UsernameSRole + "', '" + cp.DescriptionSRole + "',1)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();
                    string sql1 = "select * from CP_SecurityRole where name='" + cp.UsernameSRole + "'";
                    SqlCommand cmd1 = new SqlCommand(sql1, con);
                    SqlDataReader sd1 = cmd1.ExecuteReader();
                    while (sd1.Read())
                    {

                        roleID = Convert.ToInt32(sd1["roleID"]);

                    }
                    con.Close();

                    if (cboxRegistration == true)
                    {
                        con.Open();
                        string sqlReg = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + roleID + "', '1')";

                        SqlCommand cmdreg = new SqlCommand(sqlReg, con);
                        cmdreg.ExecuteNonQuery();
                        con.Close();
                    }
                    if (cboxsecurityCheck == true)
                    {
                        con.Open();
                        string sqlSecurity = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + roleID + "', '2')";

                        SqlCommand cmdSecurity = new SqlCommand(sqlSecurity, con);
                        cmdSecurity.ExecuteNonQuery();
                        con.Close();

                    }
                    if (cboxArchivedRegistrations == true)
                    {
                        con.Open();
                        string sqlAchived = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + roleID + "', '3')";

                        SqlCommand cmdAchived = new SqlCommand(sqlAchived, con);
                        cmdAchived.ExecuteNonQuery();
                        con.Close();
                    }
                    if (cboxSearch == true)
                    {
                        con.Open();
                        string sqlSearch = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + roleID + "', '4')";

                        SqlCommand cmdSearch = new SqlCommand(sqlSearch, con);
                        cmdSearch.ExecuteNonQuery();
                        con.Close();

                    }
                    if (cboxSearchResults == true)
                    {
                        con.Open();
                        string sqlSearchResult = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + roleID + "', '5')";

                        SqlCommand cmdSearchResultmd = new SqlCommand(sqlSearchResult, con);
                        cmdSearchResultmd.ExecuteNonQuery();
                        con.Close();

                    }


                    TempData["msg"] = "<script>alert('New Role has been Add successful..!!');</script>";

                }
                else
                {
                    TempData["msg"] = "<script>alert('New Role ALready Exist..!!');</script>";

                }
            }
            catch (Exception e)
            {


            }

            return RedirectToAction("index", "ZoomUserInterFace");

        }


        [HttpPost]
        public ActionResult SecurityEditform(securityRole cp)
        {
            bool chkRegistration = Request.Form["ChkRegistration"] == "on";
            bool chkCurrentRegistrations = Request.Form["ChkCurrentRegistrations"] == "on";
            bool chkArchivedRegistrations = Request.Form["ChkArchivedRegistrations"] == "on";
            bool chkSearch = Request.Form["ChkSearch"] == "on";
            bool chkSearchResults = Request.Form["ChkSearchResults"] == "on";

            con.Open();
            string sqlRegDel = "delete CP_SecurityRolePrivilege where roleid='" + cp.roleid + "'";

            SqlCommand cmdregDel = new SqlCommand(sqlRegDel, con);
            cmdregDel.ExecuteNonQuery();
            con.Close();

            if (chkRegistration == true)
            {
                con.Open();
                string sqlReg = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + cp.roleid + "', '1')";

                SqlCommand cmdreg = new SqlCommand(sqlReg, con);
                cmdreg.ExecuteNonQuery();
                con.Close();
            }


            if (chkCurrentRegistrations == true)
            {
                con.Open();
                string sqlSecurity = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + cp.roleid + "', '2')";

                SqlCommand cmdSecurity = new SqlCommand(sqlSecurity, con);
                cmdSecurity.ExecuteNonQuery();
                con.Close();

            }
            if (chkArchivedRegistrations == true)
            {
                con.Open();
                string sqlAchived = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + cp.roleid + "', '3')";

                SqlCommand cmdAchived = new SqlCommand(sqlAchived, con);
                cmdAchived.ExecuteNonQuery();
                con.Close();
            }
            if (chkSearch == true)
            {
                con.Open();
                string sqlSearch = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + cp.roleid + "', '4')";

                SqlCommand cmdSearch = new SqlCommand(sqlSearch, con);
                cmdSearch.ExecuteNonQuery();
                con.Close();

            }
            if (chkSearchResults == true)
            {
                con.Open();
                string sqlSearchResult = "insert into CP_SecurityRolePrivilege (roleId,privilegeid) values('" + cp.roleid + "', '5')";

                SqlCommand cmdSearchResultmd = new SqlCommand(sqlSearchResult, con);
                cmdSearchResultmd.ExecuteNonQuery();
                con.Close();

            }


            con.Open();
            var sql = "update CP_SecurityRole set name='" + cp.name.Trim() + "' , description='" + cp.description.Trim() + "' where roleId='" + cp.roleid + "' ";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();
            TempData["msg"] = "<script>alert('Security Role update successful..!!');</script>";

            return RedirectToAction("index", "ZoomUserInterFace");

        }

        public ActionResult SecurityRoleCheckB(string roleid, CP_User cp)
        {


            Boolean cboxRegistration = false;
            Boolean cboxsecurityCheck = false;
            Boolean cboxArchivedRegistrations = false;
            Boolean cboxSearch = false;
            Boolean cboxSearchResults = false;

            List<string> roles = new List<string>();

            con.Open();

            var sql = "select * from CP_SecurityRolePrivilege where roleId='" + roleid + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                roles.Add(sdr["privilegeid"].ToString());


            }
            con.Close();

            foreach (string role in roles)
            {
                if (role == "1")
                {
                    cboxRegistration = true;

                }
                if (role == "2")
                {
                    cboxsecurityCheck = true;

                }
                if (role == "3")
                {
                    cboxArchivedRegistrations = true;

                }
                if (role == "4")
                {
                    cboxSearch = true;

                }
                if (role == "5")
                {
                    cboxSearchResults = true;

                }


            }

            return Json(new { success1 = cboxRegistration, success2 = cboxsecurityCheck, success3 = cboxArchivedRegistrations, success4 = cboxSearch, success5 = cboxSearchResults });
        }
    }
}



