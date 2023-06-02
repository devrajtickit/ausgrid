using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ausgrid.Models;
using System.Web.UI.WebControls;
using System.Data;

namespace Ausgrid.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
        static int attemptsNo = 0;
        static int attemptsNoWbar = 0;
        int sUserRole;
        string sLoginVerify;
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginCheck(CP_User p)
        {

            string sLogin = p.LoginName;
            string sPwd = p.password;
            Session["loginName"] = sLogin;
            try
            {
                con.Open();
            
                if (sLogin == null || sPwd == null)
                {

                    TempData["msg"] = "<script>alert('Please enter username & password..!!');</script>";

                }

                else
                {
                    string sql1 = "select LoginName from CP_User where LoginName='" + sLogin + "'";
                    SqlCommand sc = new SqlCommand(sql1, con);
                    sLogin = Convert.ToString(sc.ExecuteScalar()).Trim();

                    SqlCommand cmd;
                    string sPassword = PwdEncryption(sLogin, sPwd);
                    //Shubham
                    int id = Validate_details(sLogin, sPassword, con);
                    string check = Validate_oldPaswrd(sPassword, sLogin, con);

                    if (String.IsNullOrEmpty(check) == false)
                        TempData["msg"] = "<script>alert('" + check + "');</script>";
                    if (id == 0 && check == "")
                    {

                        //string sql = "select username,password,userRole from CP_User_Wagga where username='" + sLogin + "' and password='" + sPassword + "'";
                        cmd = new SqlCommand("Sp_CP_User_login", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@sLogin", sLogin);
                        cmd.Parameters.AddWithValue("@password", sPassword);

                        SqlDataAdapter scon = new SqlDataAdapter(cmd);
                        DataTable DT = new DataTable();
                        scon.Fill(DT);

                        string sUsername = DT.Rows[0]["LoginName"].ToString();
                        string sPass = DT.Rows[0]["password"].ToString();
                        string sUserRole = DT.Rows[0]["userRole"].ToString();


                        if (/*sLogin == "Sysadmin" &&*/ sUserRole == "0")
                        {
                            con.Close();

                            con.Open();
                            cmd = new SqlCommand("update CP_User set noOfAttempts=0 where LoginName='" + sLogin + "'", con);
                            cmd.ExecuteNonQuery();
                            return RedirectToAction("Index", "ZoomUserInterface");
                        }
                        //Response.Redirect("Login_Page.aspx");
                        else
                        {
                            con.Close();
                            con.Open();
                            cmd = new SqlCommand("update CP_User set noOfAttempts=0 where LoginName='" + sLogin + "'", con);
                            cmd.ExecuteNonQuery();
                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                    else if (id == 1)
                    {

                        TempData["msg"] = "<script>alert('Please enter correct username..!!');</script>";


                        attemptsNo = attemptsNo + 1;
                        cmd = new SqlCommand("update CP_User set noOfAttempts=" + attemptsNo + " where LoginName='" + sLogin + "'", con);
                        cmd.ExecuteNonQuery();
                        return RedirectToAction("Login", "Login");

                    }
                    else if (id == 2)
                    {
                        TempData["msg"] = "<script>alert('Please enter correct password..!!');</script>";

                        attemptsNo = attemptsNo + 1;
                        cmd = new SqlCommand("update CP_User set noOfAttempts=" + attemptsNo + " where LoginName='" + sLogin + "'", con);
                        cmd.ExecuteNonQuery();
                        return RedirectToAction("Login", "Login");

                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Invalid loginname or password..!!');</script>";

                        attemptsNo = attemptsNo + 1;
                        cmd = new SqlCommand("update CP_User set noOfAttempts=" + attemptsNo + " where LoginName='" + sLogin + "'", con);
                        cmd.ExecuteNonQuery();

                    }
            }
            con.Close();
            }
            catch (Exception e2)
            {

            }
            return RedirectToAction("Login", "Login");

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

        protected int Validate_details(string sLogin, string sPwd, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand("Sp_CP_User_Checklogin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@username", ViewState["txtEditName"].ToString());
            cmd.Parameters.AddWithValue("@sLogin", sLogin);
            cmd.Parameters.AddWithValue("@password", sPwd);
            //int returnVALUE = (int)cmd.Parameters["@sLogin"].Value;
            //cmd.ExecuteNonQuery();
            SqlParameter returnParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            int id = (int)returnParameter.Value;

            return id;
        }

        protected string Validate_oldPaswrd(string oldPaswd, string loginName, SqlConnection con)
        {
            string status = "", noOfAttempt = "", policyRecno = "", policyDisable = "", policyPreventDay = "", policyChangeDay = "";
            DateTime loginDate = DateTime.Now, lastModifydate = DateTime.Now;

            SqlCommand cmd1 = new SqlCommand("select status, noOfAttempts, loginDate, lastModifieDate from CP_User where LoginName= '" + loginName + "'", con);
            SqlDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                status = reader["status"].ToString();
                noOfAttempt = reader["noOfAttempts"].ToString();
                loginDate = Convert.ToDateTime(reader["loginDate"].ToString());
                lastModifydate = Convert.ToDateTime(reader["lastModifieDate"].ToString());
            }
            reader.Close();

            cmd1 = new SqlCommand("select recNo, DisableWrongPasswordAttempt, PreventLogonUnusedDays, ChangeWarningDay from CP_PasswordPolicy", con);
            reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                policyRecno = reader["recNo"].ToString();
                policyDisable = reader["DisableWrongPasswordAttempt"].ToString();

                policyPreventDay = reader["PreventLogonUnusedDays"].ToString();
                policyChangeDay = reader["ChangeWarningDay"].ToString();
            }

            reader.Close();

            if (String.IsNullOrEmpty(status) == false && String.IsNullOrEmpty(noOfAttempt) == false && String.IsNullOrEmpty(policyRecno) == false && String.IsNullOrEmpty(policyDisable) == false && String.IsNullOrEmpty(policyPreventDay) == false && String.IsNullOrEmpty(policyChangeDay) == false)
            {
                int inoAtempts = Int32.Parse(noOfAttempt);
                int ipolicyDisables = Int32.Parse(policyDisable);
                if (status == "2")
                    return "Your account has been suspended. Please contact Administrator.";
                else if (status == "3")
                    return "Your account has been closed. Please contact Administrator.";
                else if (status == "0")
                    return "Inactive User. Please contact Administrator.";

                if (inoAtempts >= ipolicyDisables)
                {
                    cmd1 = new SqlCommand("update CP_User set status='2' where LoginName='" + loginName + "'", con);
                    cmd1.ExecuteNonQuery();
                    return "Your account has been locked due to number of wrong password attempts exceeded.Please Contact your system admin";
                }

                double preventloginDays = (DateTime.Now - loginDate).TotalDays;
                if (preventloginDays > Convert.ToDouble(policyPreventDay) && String.IsNullOrEmpty(loginDate.ToString()) == false)
                {
                    cmd1 = new SqlCommand("update CP_User set status='2' where LoginName='" + loginName + "'", con);
                    cmd1.ExecuteNonQuery();
                    return "Your account has been locked due to inactivity. Please Contact your system admin";
                }

                double lastloginDays = (DateTime.Now - lastModifydate).TotalDays;
                if (lastloginDays >= Convert.ToDouble(policyChangeDay))
                {
                    return "Your password has expired, Please change password.";
                }

            }
            if (noOfAttempt == "")
            {
                attemptsNo = 0;
            }
            else
                attemptsNo = Int32.Parse(noOfAttempt);

            return "";
        }


    }
}