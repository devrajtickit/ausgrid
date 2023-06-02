using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ausgrid.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Bytescout.BarCodeReader;
using BarcodeReader;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Ausgrid.Controllers
{
    public class DashboardController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["mycon"].ConnectionString);
        /*String loginName = Session["loginName"];*//*"sislam@ausgrid.com.au";*/
        String loginName;
        static string getClref = "";
        // GET: Dashboard
        public ActionResult Index()
        {
            loginName = Session["loginName"].ToString();
            Session["loginName"] = loginName;
            loginName = Session["loginName"].ToString();
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string sqlprofile = "select * from CP_User where LoginName='" + loginName + "'";
                SqlCommand cmdprofile = new SqlCommand(sqlprofile, con);
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
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

                sdprofile.Close();
                //find clrefs
                getClref = "";
                SqlCommand userClref = new SqlCommand("select clRef from CP_User where LoginName = '" + loginName + "'", con); //here email id static pass should be change
                string[] Array_userClref = userClref.ExecuteScalar().ToString().Split(',');
                
                foreach (string s in Array_userClref)
                    getClref += "'" + s + "',";

                getClref = getClref.Substring(0, getClref.Length - 1);

                con.Close();
            }

            return View();
        }

        public ActionResult Pre_registration()
        {
            ViewBag.ShowDiv = true;
            return View();
        }

        public ActionResult Pre_registration_List()
        {
            PreRegGrid();
            return View();
        }

        public ActionResult Ad_hoc_label()
        {
            ViewBag.ShowDiv = true;
            return View();
        }
        public ActionResult Ad_hoc_label_List()
        {
            AdhocList();
            return View();
        }

        [HttpPost]
        public ActionResult BindClref()
        {
            loginName = Session["loginName"].ToString();
            string p = "";
            String query = "";
            DataSet ds;
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                //find clrefs
                //SqlCommand userClref = new SqlCommand("select clRef from CP_User where LoginName = '" + loginName + "'", con); //here email id static pass should be change
                //string[] Array_userClref = userClref.ExecuteScalar().ToString().Split(',');
                //string getClref = "";
                //foreach (string s in Array_userClref)
                //    getClref += "'" + s + "',";

                //getClref= getClref.Substring(0, getClref.Length-1);
                //if (getClref != "''")
                if (String.IsNullOrWhiteSpace(getClref) == false && getClref != "''")
                {
                    query = "select smp.CLIENT_ID, smp.CODE from CP_User cp inner join CLIENT cl on cl.ClRef in( " + getClref + " ) \n" +
                                   "inner join SAMPLEPOINT smp on smp.CLIENT_ID = cl.CLIENT_ID \n" +
                                   //"where cp.LoginName = '"+loginName+"' "; 
                                   "where cp.LoginName = '" + loginName + "' "; //here email id static pass should be change
                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                     ds= new DataSet();
                    da.Fill(ds);
                }
                else
                {
                    query = "select CLIENT_ID, CODE from SamplePoint";
                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BindTransformer()
        {
            loginName = Session["loginName"].ToString();
            String query = "";
            DataSet ds;
            string p = "";
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();

                //find clrefs
                //SqlCommand userClref = new SqlCommand("select clRef from CP_User where LoginName = '" + loginName + "'", con); //here email id static pass should be change
                //string[] Array_userClref = userClref.ExecuteScalar().ToString().Split(',');
                //string getClref = "";
                //foreach (string s in Array_userClref)
                //    getClref += "'" + s + "',";

                //getClref = getClref.Substring(0, getClref.Length - 1);

                //if ((getClref != "''")|| (strgetClref)
                if (String.IsNullOrWhiteSpace(getClref) == false && getClref != "''")
                {
                    query = "select CONCAT(smPoint.CODE+'- ',smgroup.DESCRIPTION)as Transformer, smPoint.CODE, smgroup.DESCRIPTION, client.CLIENT_ID , client.ClRef\r\n" +
                                    "from SAMPLEPOINT smPoint \r\ninner join SAMPLEGROUP smgroup on smPoint.SAMPLEGROUP_ID = smgroup.SAMPLEGROUP_ID\r\n" +
                                    "inner join CLIENT client on client.CLIENT_ID= smPoint.CLIENT_ID\r\ninner join CP_User cp on client.ClRef in( " + getClref + " )\r\n" +
                                    //"where cp.LoginName = '"+loginName+"' "; 
                                    "where cp.LoginName='" + loginName + "' \r\n"; //static mail id pass here
                                                                                   //"and client.ClRef in('1709')";   // static clref present here
                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                else
                {
                    query = "select CONCAT(smPoint.CODE+'- ',smgroup.DESCRIPTION)as Transformer \r\n" +
                            "from SAMPLEPOINT smPoint inner join SAMPLEGROUP smgroup \r\n" +
                            "on smPoint.SAMPLEGROUP_ID = smgroup.SAMPLEGROUP_ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        // For Sample Type
        [HttpPost]
        public ActionResult BindSampleType()
        {
            loginName = Session["loginName"].ToString();
            String query = "";
            DataSet ds;
            string p = "";
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();

                //find clrefs
                //SqlCommand userClref = new SqlCommand("select clRef from CP_User where LoginName = '" + loginName + "'", con); //here email id static pass should be change
                //string[] Array_userClref = userClref.ExecuteScalar().ToString().Split(',');
                //string getClref = "";
                //foreach (string s in Array_userClref)
                //    getClref += "'" + s + "',";

                //getClref = getClref.Substring(0, getClref.Length - 1);

                //if (getClref != "''")
                if (String.IsNullOrWhiteSpace(getClref) == false && getClref != "''")
                {
                    query = "SELECT SmTRef, ClRef FROM SMTYPE where CLREF In("+getClref+")"; //static mail id pass here
                                                                                   //"and client.ClRef in('1709')";   // static clref present here
                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                else
                {
                    query = "SELECT SmTRef, ClRef FROM SMTYPE";
                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BindSmPointDGA()
        {

            string p = "";
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                String query = "select ValStr from VALLIST where ValLstRef='Sampling_PT_DGA' ";
                SqlCommand cmd = new SqlCommand(query, con);
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BindSmPointOIL()
        {
            string p = "";
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                String query = "select ValStr from VALLIST where ValLstRef='Sampling_PT_OIL' ";
                SqlCommand cmd = new SqlCommand(query, con);
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                con.Close();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult BindClref2()
        {
            //it is AdHocClref lookup list value

            loginName = Session["loginName"].ToString();
            string p = "";
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                String query = "select Concat(client.ClRef, ' ', client.dept, client.Client)as AdhocClref from CLIENT client inner join CP_User cpUsr" +
                        "\r\n on Client.ClRef in(select('' + REPLACE(clref, ',', ''',''') + '') from CP_User) " +
                        "\r\n where cpUsr.LoginName= '" + loginName + "' ";
                SqlCommand cmd = new SqlCommand(query, con);
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DisplayClrefData(string clref)
        {
            string clrefVal = clref;
            string p = "";
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Sp_PreregAutofill", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@clref", clrefVal);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    p = JsonConvert.SerializeObject(ds.Tables[0]);
                }
            }
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Insertdata(string SampleType, string OrderNo, string Barcode, string ActivityNo
           , string Transformer, string Sample, string Winding, string Ambient, string TestReson
           , string Comment, string SamplePoint, string SamplePOil, string Inventory, string Serial
           , string Manufacture, string SubName, string SubPrefix, string SubNo, string Equipment
           , string Date, string SampledBy, string Clref)
        {
            loginName = Session["loginName"].ToString();
            /* DATABASE Procedure Parameters
             * 
             *      @Owner_Ident uniqueidentifier,
                    @USER_ID varchar(max),
                    @WorkOrderNo varchar(max),
                    @Op_ActivityNo varchar(max),
                    @Transformer varchar(max),
                    @Sample varchar(max),
                    @Winding varchar(max),
                    @Ambient varchar(max),
                    @SampleDGA varchar(max),
                    @SamplePOil varchar(max),
                    @Barcode varchar(max),
                    @TestReason varchar(max)
             */
            try
            {
                using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                {
                    con.Open();
                    SqlCommand getOwnerIdent = new SqlCommand("SELECT NEWID()", con);
                    var ownerIdentSQL = getOwnerIdent.ExecuteScalar().ToString();
                    Guid ownerIdent = Guid.Parse(ownerIdentSQL);

                    SqlCommand userClref = new SqlCommand("select clRef from CP_User where LoginName = '" + loginName + "'", con); //here email id static pass should be change
                    string[] Array_userClref = userClref.ExecuteScalar().ToString().Split(',');
                    string getClref = "";
                    foreach (string s in Array_userClref)
                        getClref += "" + s + ",";

                    getClref = getClref.Substring(0, getClref.Length - 1);

                    SqlCommand cmd = new SqlCommand("Sp_Insertinto_RBtbl", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Owner_Ident", ownerIdent);
                    cmd.Parameters.AddWithValue("@USER_ID", Clref);
                    cmd.Parameters.AddWithValue("@WorkOrderNo", OrderNo);
                    cmd.Parameters.AddWithValue("@Op_ActivityNo", ActivityNo);
                    //cmd.Parameters.AddWithValue("@Transformer", Transformer);
                    cmd.Parameters.AddWithValue("@Transformer", (Transformer == "0") ? " " : Transformer);
                    cmd.Parameters.AddWithValue("@Sample", Sample);
                    cmd.Parameters.AddWithValue("@Winding", Winding);
                    cmd.Parameters.AddWithValue("@Ambient", Ambient);
                    cmd.Parameters.AddWithValue("@SampleDGA", (SamplePoint == "0") ? " " : SamplePoint);
                    cmd.Parameters.AddWithValue("@SamplePOil", (SamplePOil == "0") ? " " : SamplePOil);
                    cmd.Parameters.AddWithValue("@Barcode", Barcode);
                    cmd.Parameters.AddWithValue("@TestReason", TestReson);
                    cmd.Parameters.AddWithValue("@Inventory", Inventory);
                    cmd.Parameters.AddWithValue("@Clref", Clref);
                    cmd.Parameters.AddWithValue("@getClref", getClref);
                    cmd.Parameters.AddWithValue("@Comments", Comment);
                    cmd.Parameters.AddWithValue("@SmTRef", (SampleType == "0") ? " " : SampleType);

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "<script>alert('Error while Inserting Data');</script>";

                return RedirectToAction("Pre_registration", "Dashboard");
            }
            return RedirectToAction("Pre_registration", "Dashboard");
        }


        //[HttpPost]
        //public ActionResult GeneratePDF()
        //{
        //    var report = new ActionAsPdf("Ad_hoc_label");
        //    return report;
        //}
        [HttpGet]
        public ActionResult GeneratePDF1(string textBoxValue1, string textBoxValue2, string textBoxValue3, string textBoxValue4, string textBoxValue5, string textBoxValue6
            , string textBoxValue7, string textBoxValue8, string textBoxValue9, string textBoxValue10, string textBoxValue11, string textBoxValue12, string textBoxValue13
            , string textBoxValue14, string textBoxValue15, string textBoxValue16, string textBoxValue17, string textBoxValue18, string textBoxValue19, string textBoxValue20
            , string textBoxValue21, string textBoxValue22)
        {


            // Set up a FileStream to write the PDF to a temporary file
            string tempFilePath = Path.GetTempFileName();
            try
            {
                // Create a new PDF document
                Document document = new Document();

                FileStream fileStream = new FileStream(tempFilePath, FileMode.Create);

                // Create a PdfWriter to write the document to the FileStream
                PdfWriter writer = PdfWriter.GetInstance(document, fileStream);

                //To Store PDF into Database
                MemoryStream memoryStream = new MemoryStream();
                //PDF writer to write into pdf
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);

                // Open the document
                document.Open();

                // Add a table to the document
                // Add a table to the document
                PdfPTable table = new PdfPTable(2); // Example: 2 columns

                // Add table data
                table.AddCell("Heading");
                table.AddCell("Value");
                table.AddCell("Barcode:-");
                table.AddCell(textBoxValue1);
                table.AddCell("Inventory No:-");
                table.AddCell(textBoxValue2);

                // Add a new row with a column
                PdfPCell newLineCell1 = new PdfPCell(new Phrase("Work Order No:-"));
                table.AddCell(newLineCell1);
                table.AddCell(textBoxValue3);
                table.AddCell("Serial No:-");
                table.AddCell(textBoxValue4);

                PdfPCell newLineCell2 = new PdfPCell(new Phrase("Operation Activity No:-"));
                table.AddCell(newLineCell2);
                table.AddCell(textBoxValue5);
                table.AddCell("Manufacture:-");
                table.AddCell(textBoxValue6);

                PdfPCell newLineCell3 = new PdfPCell(new Phrase("Transformer:-"));
                table.AddCell(newLineCell3);
                table.AddCell(textBoxValue7); //change

                table.AddCell("SubName:-");
                table.AddCell(textBoxValue8); //change
                table.AddCell("Sample:-");
                table.AddCell(textBoxValue9); //change
                table.AddCell("SubPrefix:- ");
                table.AddCell(textBoxValue10); //change
                table.AddCell("Winding:-");
                table.AddCell(textBoxValue11); //change
                table.AddCell("SubNo:-");
                table.AddCell(textBoxValue12); //change
                table.AddCell("Ambient:");
                table.AddCell(textBoxValue13); //change
                table.AddCell("Position No:");
                table.AddCell(textBoxValue14); //change
                table.AddCell("Test Reason:");
                table.AddCell(textBoxValue15); //change
                table.AddCell("Equipment:");
                table.AddCell(textBoxValue16); //change
                table.AddCell("Comment:");
                table.AddCell(textBoxValue17);

                table.AddCell("Date Sampled:");
                table.AddCell(textBoxValue18);
                table.AddCell("Sample Point(DGA):");
                table.AddCell(textBoxValue19);
                table.AddCell("Sampled By:");
                table.AddCell(textBoxValue20);
                table.AddCell("Sample Point(Oil Screen:");
                table.AddCell(textBoxValue21);
                table.AddCell("Clref:");
                table.AddCell(textBoxValue22); //change

                // Add more rows and cells as needed

                // Add the table to the document
                document.Add(table);

                //To Add Image
                //string imagepath = "";
                //if (!string.IsNullOrEmpty(imagePath))
                //{
                //    // Load the image from the specified path
                //    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);
                //    // Adjust the image size if needed
                //    image.ScaleToFit(200f, 200f); // Example: Adjust to fit within a 200x200 pixel box
                //                                  // Add the image to the document
                //    document.Add(image);
                //}

                document.Close();
                writer.Close();
                fileStream.Close();
                // Convert generatedPDF to byte Array
                byte[] pdfData = memoryStream.ToArray();

                memoryStream.Close();
                using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    string query = "INSERT INTO  Adhoc_PDFs (Clref, Uregref, PDF) VALUES(@Clref, @Uregref, @PDF)";
                    using (SqlCommand pdfSqlCommand = new SqlCommand(query, con))
                    {
                        pdfSqlCommand.Parameters.AddWithValue("@Clref", textBoxValue22);
                        pdfSqlCommand.Parameters.AddWithValue("@Uregref", textBoxValue1);
                        pdfSqlCommand.Parameters.AddWithValue("@PDF", pdfData);

                        pdfSqlCommand.ExecuteNonQuery();
                    }
                }
                // Close the document and FileStream
                //document.Close();
                //writer.Close();
                //fileStream.Close();

            }
            catch (Exception e3)
            {
                TempData["msg"] = "<script>alert('Error while Generating PDF');</script>";
                return RedirectToAction("Ad_hoc_label", "Dashboard");
            }
            // Return the temporary file path
            return File(tempFilePath, "application/pdf", "Table.pdf");
        }

        public ActionResult PreRegGrid()
        {
            List<Ausgrid.Models.PreList_Model> gridSamples = new List<PreList_Model>();
            try
            {
                string query = "", clref="";
                SqlCommand sqlCommand;
                SqlDataReader reader;
                string[] clrefArr;

                loginName = Session["loginName"].ToString();
                using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    SqlCommand sqlCommandCLREF = new SqlCommand("select CLref from Cp_user where LoginName='" + loginName + "'", con);
                    string clrefs = Convert.ToString(sqlCommandCLREF.ExecuteScalar());

                    clrefArr = clrefs.Split(',');
                    foreach (string st in clrefArr)
                    {
                        clref += "'" + st + "',";
                    }
                    clref = clref.Substring(0, clref.Length - 1);

                    if (String.IsNullOrWhiteSpace(clrefs) == true)
                    {
                        query = "select * from Vw_PreList";// where LoginName='" + loginName + "'";
                    }
                    else
                        query = "select * from Vw_PreList where LoginName='" + loginName + "' and ClientRef in(" + clref + ")";
                    sqlCommand = new SqlCommand(query, con);
                    reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {


                        PreList_Model modelOb = new PreList_Model();
                        modelOb.Reference = (string)reader["Reference"];

                        DateTime regDate = (DateTime)reader["RegDate"];
                        //DateTime regDate = (DateTime)reader["RegDate"];
                        DateTime dateOnly = regDate.Date;
                        modelOb.RegDate = regDate;
                        modelOb.SubsName = (string)reader["SubsName"];
                        modelOb.PositionNo = Convert.ToInt32(reader["PositionNo"]);
                        modelOb.SampledBy = Convert.ToInt32(reader["SampledBy"]);
                        modelOb.ClientRef = (string)reader["ClientRef"];
                        modelOb.Status = (string)reader["Status"];

                        gridSamples.Add(modelOb);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "<script>alert('Error while Fetch Data from Database');</script>";
                return RedirectToAction("Pre_registration_List", "Dashboard");
            }

            //return View();
            return View(gridSamples);
            //return RedirectToAction("Pre_registration_List", "Dashboard");
        }


        //For ADHOC
        public ActionResult AdhocList()
        {
            List<AdhocList_Model> adhocListGrid = new List<AdhocList_Model>();
            try
            {
                using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    string query = "select ID,Uregref,CLREF from Adhoc_PDFs";
                    SqlCommand sqlcommand = new SqlCommand(query, con);
                    SqlDataReader reader = sqlcommand.ExecuteReader();

                    while (reader.Read())
                    {
                        AdhocList_Model modelOb = new AdhocList_Model();
                        modelOb.ID = reader.GetInt32(0);
                        modelOb.UREGREF = reader["UREGREF"].ToString();
                        modelOb.CLREF = reader["CLREF"].ToString();

                        adhocListGrid.Add(modelOb);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "<script>alert('Error while Fetch Data from Database');</script>";
                return RedirectToAction("Ad_hoc_label_List", "Dashboard");
            }
            return View(adhocListGrid);
        }

        public ActionResult AdhocPrintPDF(int id)
        {
            byte[] pdfData;
            try
            {
                using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    string query = "select PDF from Adhoc_PDFs where id= @Id";
                    SqlCommand sqlCommand = new SqlCommand(query, con);
                    sqlCommand.Parameters.AddWithValue("@Id", id);
                    pdfData = (byte[])sqlCommand.ExecuteScalar();

                    // Write the PDF data to the response
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(pdfData);
                    Response.End();

                    return new EmptyResult();
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = "<script>alert('Error while RePrinting PDF');</script>";
                return RedirectToAction("Ad_hoc_label_List", "Dashboard");
                //return View();
            }
        }

        public ActionResult DeleteAdhocList(int id)
        {
            try
            {
                bool success = false;

                using (SqlConnection connection = new SqlConnection(con.ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();

                    string query = "delete  Adhoc_PDFs where id= @Id";
                    SqlCommand sqlCommand = new SqlCommand(query, con);
                    sqlCommand.Parameters.AddWithValue("@Id", id);
                    int rowAffected = sqlCommand.ExecuteNonQuery();

                    if (rowAffected > 0)
                        success = true;
                    else
                        success = false;

                }
                return Json(new { success = success });
            }
            catch (Exception e)
            {
                TempData["msg"] = "<script>alert('Error While Delete Record in Adhoc label List');</script>";
                return RedirectToAction("Ad_hoc_label_List", "Dashboard");
            }
            //return View();
        }

        [HttpPost]
        public ActionResult Validate(CP_User cp)
        {
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string uregref = cp.uRegRef;

                if (uregref == null)
                {
                    TempData["msg"] = "<script>alert('Field is blank!');</script>";
                }
                else
                {
                    var sql = "Select * from regkey where URegRef='" + cp.uRegRef + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    var checkUregref = cmd.ExecuteScalar();

                    if ((checkUregref == null))
                    {
                        TempData["uRegRef"] = uregref;
                        TempData["UserInput"] = "Suneel";
                        //TempData["msg"] = "<script>alert('Fill below field!!');</script>";
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('It is Already Exist..!!');</script>";
                    }

                }

                con.Close();
            }
            return RedirectToAction("Ad_hoc_label", "Dashboard");
        }

        [HttpPost]
        public ActionResult ValidatePre_registration(CP_User cp)
        {
            using (SqlConnection connection = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string uregref = cp.uRegRef;

                if (uregref == null)
                {
                    TempData["msg"] = "<script>alert('Field is blank!');</script>";
                }
                else
                {
                    var sql = "Select * from regkey where URegRef='" + cp.uRegRef + "'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    var checkUregref = cmd.ExecuteScalar();

                    if ((checkUregref == null))
                    {
                        TempData["uRegRef"] = uregref;
                        TempData["UserInput"] = "Suneel";
                        //TempData["msg"] = "<script>alert('Fill below field!!');</script>";
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('It is Already Exist..!!');</script>";
                    }

                }

                con.Close();
            }
            return RedirectToAction("Pre_registration", "Dashboard");
        }
        [HttpPost]
        public ActionResult UploadHtml5(string image, string type)
        {


            try
            {
                StringBuilder send = new StringBuilder();

                // Lock by send variable 
                lock (send)
                {
                    // Convert base64 string from the client side into byte array
                    byte[] bitmapArrayOfBytes = Convert.FromBase64String(image);
                    // Create Bytescout.BarCodeReader.Reader object
                    Reader reader = new Reader();
                    // Get the barcode type from user's selection in the combobox
                    reader.BarcodeTypesToFind = BarcodeReader.Barcode.GetBarcodeTypeToFindFromCombobox(type);
                    // Read barcodes from image bytes
                    reader.ReadFromMemory(bitmapArrayOfBytes);
                    // Check whether the barcode is decoded
                    if (reader.FoundBarcodes != null)
                    {
                        // Add each decoded barcode into the string 
                        foreach (FoundBarcode barcode in reader.FoundBarcodes)
                        {
                            // Add barcodes as text into the output string
                            //send.AppendLine(String.Format("{0} : {1}", barcode.Type, barcode.Value));
                            if (barcode.Value.Contains("*DEMO*"))
                            {
                                // Add barcodes as text into the output string
                                send.AppendLine(String.Format("{0}", " "));
                            }
                            else
                            {
                                //ViewBag.barcode1 = barcode.Value;
                                send.Append(String.Format("{0}", barcode.Value));
                                goto Exit;
                                //ClientScript.RegisterStartupScript(GetType(), "hwa", "stopCall();", true);
                                //ScriptManager.RegisterStartupScript(this.p,GetType(), "Javascript", "javascript:stopCall(); ", true);
                                //ScriptManager.RegisterStartupScript(this, GetType(), "text", "stopCall()", true);
                                //break;
                            }
                        }
                    }
                Exit:
                    // Return the output string as JSON
                    return Json(new { d = send.ToString() });
                }
            }
            catch (Exception ex)
            {
                // return the exception instead
                return Json(new { d = ex.Message + "\r\n" + ex.StackTrace });
            }
        }

        [HttpPost]
        public ActionResult UploadFlash(string type)
        {
            try
            {
                String send = "";
                System.Drawing.Image originalimg;
                // read barcode type set 
                MemoryStream log = new MemoryStream();
                byte[] buffer = new byte[1024];
                int c;
                // read input buffer with image and saving into the "log" memory stream
                while ((c = Request.InputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    log.Write(buffer, 0, c);
                }
                // create image object
                originalimg = System.Drawing.Image.FromStream(log);
                // resample image
                originalimg = originalimg.GetThumbnailImage(640, 400, new System.Drawing.Image.GetThumbnailImageAbort(() => { return false; }), IntPtr.Zero);

                Bitmap bp = new Bitmap(originalimg);

                bp.Save("c:\\temp\\barcode.jpg", ImageFormat.Jpeg);

                // create bytescout barcode reader object
                Reader reader = new Reader();
                // set barcode type to read
                reader.BarcodeTypesToFind = BarcodeReader.Barcode.GetBarcodeTypeToFindFromCombobox(type);
                // read barcodes from image
                reader.ReadFrom(bp);
                // if there are any result then convert them into a single stream
                if (reader.FoundBarcodes != null)
                {
                    foreach (FoundBarcode barcode in reader.FoundBarcodes)
                    {
                        // form the output string
                        send = send + (String.Format("{0} : {1}", barcode.Type, barcode.Value));
                    }
                }
                // close the memory stream
                log.Close();
                // dispose the image object
                originalimg.Dispose();
                // write output 
                return Content("<d>" + send + "</d>");
            }
            catch (Exception ex)
            {
                // write the exception if any
                return Content("<d>" + ex + "</d>");
            }
        }

        public ActionResult ScanBarcode(string barcode)
        {
            ViewBag.Barcode = barcode;
            return View();
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
            return RedirectToAction("index", "Dashboard");
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

        //Pre Reg List Popup
        //[HttpPost]
        public ActionResult GetPopupData(string reference,CP_User cpp)
        {

            cpp.clrefDropDown = "Suneel";
            //List<string> roles = new List<string>();

            //con.Open();

            //var sql = "select * from Vw_PreList_Info where reference='" + reference + "'";
            //SqlCommand cmd = new SqlCommand(sql, con);
            //SqlDataReader sdr = cmd.ExecuteReader();
            //while (sdr.Read())
            //{
            //    roles.Add(sdr["privilegeid"].ToString());


            //}
            //con.Close();

            return Json(new { success = cpp.clrefDropDown });
        }
    }
}
