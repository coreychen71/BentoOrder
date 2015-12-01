using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace EW_BentoOrder
{
    public partial class BentoOrder : Form
    {
        SqlConnection OpenSqlCon = new SqlConnection("server=ERP;database=EW;uid=JSIS;pwd=JSIS");
        SqlConnection OpensqlConME = new SqlConnection("server=EWNAS;database=ME;uid=me;pwd=2dae5na");
        SqlCommand SqlComm = new SqlCommand();
        string SQLCon = "server=EWNAS;database=ME;uid=me;pwd=2dae5na";
        string SQLConERP="server=ERP;database=EW;uid=JSIS;pwd=JSIS";
        //宣告後續要將CheckBox.Item的多餘字元移除的字串
        string clear = "ACDEFGILMQSTPR0123456789";
        //宣告部門字串陣列
        string[] depart = { "總經理室", "業務部", "管理部", "財務部", "工程部", "製造研發部", "品保部", "廠長室", "壓合課",
            "生管課", "測試課", "品檢課", "乾膜課", "防焊課", "鑽孔課", "成型課" };
        //宣告部門ID字串陣列，注意：初始值的排序需要與部門字串一樣
        string[] departid = { "EG", "ES", "EM", "EA", "EE", "ER", "EQ", "MM", "EL", "EP", "ET", "EI", "FF", "LF", "DF",
            "CF" };
        //宣告每日出勤登記的時間
        string st1 = "09:00";
        string st2 = "09:00";

        public BentoOrder()
        {
            InitializeComponent();
            lblWorkPeople_Time.ForeColor = Color.Red;
        }

        /// <summary>
        /// 依假別狀態寫入SQL，要輸入時間
        /// </summary>
        /// <param name="StatusNum">假別狀態代碼</param>
        private void InsertWPSTime_Status(int StatusNum)
        {
            if (rdoClassAM.Checked == false & rdoClassPM.Checked == false)
            {
                MessageBox.Show("尚未選擇班別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else if (cboSelectWorkPeopleDepart.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else
            {
                DateTime time = new DateTime();
                time = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                if (time > t1 | time > t2)
                {
                    MessageBox.Show("已超過每日出勤登記時間！" + Environment.NewLine + "請洽詢管理部-人資專員。", "錯誤",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int Classnum = 0;
                    string Class = "";
                    if (rdoClassAM.Checked == true)
                    {
                        Classnum = 1;
                        Class = "早班";
                    }
                    else if (rdoClassPM.Checked == true)
                    {
                        Classnum = 2;
                        Class = "晚班";
                    }
                    //先撈出選定部門的部門ID
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                        cboSelectWorkPeopleDepart.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    //先檢查今日該部門人員是否已報過出勤
                    int a = chklstWorkPeopleName.CheckedItems.Count;
                    int q;
                    string name = null;
                    SqlDataReader check;
                    for (q = 0; q < a; q++)
                    {
                        OpensqlConME.Close();
                        name = chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                        SqlComm.CommandText = "select * from WorkPeople where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and Class in (1,2) and (Status is null or Status in (0,1,2,3,4,5,6,7,8))";
                        SqlComm.Connection = OpensqlConME;
                        OpensqlConME.Open();
                        check = SqlComm.ExecuteReader();
                        if (check.HasRows)
                        {
                            MessageBox.Show("該人員[" + name + "]今日已報過出勤！", "注意", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);
                            OpensqlConME.Close();
                            return;
                        }
                    }
                    OpensqlConME.Close();
                    SelectTime st = new SelectTime();
                    st.dtpWPSStart.Format = DateTimePickerFormat.Custom;
                    st.dtpWPSEnd.Format = DateTimePickerFormat.Custom;
                    st.dtpWPSStart.CustomFormat = "yyyy-MM-dd HH:mm";
                    st.dtpWPSEnd.CustomFormat = "yyyy-MM-dd HH:mm";
                    if (st.ShowDialog() == DialogResult.OK)
                    {
                        OpensqlConME.Open();
                        int A = chklstWorkPeopleName.CheckedItems.Count;
                        for (int i = 0; i < A; i++)
                        {
                            SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                                chklstWorkPeopleName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                            SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                            DataSet ReadUser = new DataSet();
                            ReadNI.Fill(ReadUser, "ReadUser");
                            SqlComm.CommandText = "insert into WorkPeople (Date,EmpId,EmpName,DepartId,Class,Status," +
                                "StartTime,EndTime,RegisterPeople,RegisterDate) values ('" +
                                DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][0].ToString().Trim() + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][1].ToString().Trim() + "','" +
                                Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "','" +
                                Classnum + "'," + StatusNum + ",'" + st.dtpWPSStart.Value.ToString("HH:mm") + "','" +
                                st.dtpWPSEnd.Value.ToString("HH:mm") + "','" + lblUserNameShow.Text.ToString() +
                                "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            SqlComm.Connection = OpensqlConME;
                            SqlComm.ExecuteNonQuery();
                        }
                        MessageBox.Show("部門：" + cboSelectWorkPeopleDepart.Text.ToString() + Environment.NewLine +
                            Class + "今日排休人數共 " + chklstWorkPeopleName.CheckedItems.Count + "位！" + Environment.NewLine +
                            "每日出勤登記已完成！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                        RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                    }
                }
            }
        }

        /// <summary>
        /// 依假別狀態寫入SQL，要輸入時間及事由
        /// </summary>
        /// <param name="StatusNum">假別狀態代碼</param>
        private void InsertWPSTimeTxt_Status(int StatusNum)
        {
            if (rdoClassAM.Checked == false & rdoClassPM.Checked == false)
            {
                MessageBox.Show("尚未選擇班別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else if (cboSelectWorkPeopleDepart.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else
            {
                DateTime time = new DateTime();
                time = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                if (time > t1 | time > t2)
                {
                    MessageBox.Show("已超過每日出勤登記時間！" + Environment.NewLine + "請洽詢管理部-人資專員。", "錯誤",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int Classnum = 0;
                    string Class = "";
                    if (rdoClassAM.Checked == true)
                    {
                        Classnum = 1;
                        Class = "早班";
                    }
                    else if (rdoClassPM.Checked == true)
                    {
                        Classnum = 2;
                        Class = "晚班";
                    }
                    //先撈出選定部門的部門ID
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                        cboSelectWorkPeopleDepart.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    //先檢查今日該部門人員是否已報過出勤
                    int a = chklstWorkPeopleName.CheckedItems.Count;
                    int q;
                    string name = null;
                    SqlDataReader check;
                    for (q = 0; q < a; q++)
                    {
                        OpensqlConME.Close();
                        name = chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                        SqlComm.CommandText = "select * from WorkPeople where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and Class in (1,2) and (Status is null or Status in (0,1,2,3,4,5,6,7,8))";
                        SqlComm.Connection = OpensqlConME;
                        OpensqlConME.Open();
                        check = SqlComm.ExecuteReader();
                        if (check.HasRows)
                        {
                            MessageBox.Show("該人員[" + name + "]今日已報過出勤！", "注意", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);
                            OpensqlConME.Close();
                            return;
                        }
                    }
                    OpensqlConME.Close();
                    SelectTimeTxt st = new SelectTimeTxt();
                    st.dtpWPSStart.Format = DateTimePickerFormat.Custom;
                    st.dtpWPSEnd.Format = DateTimePickerFormat.Custom;
                    st.dtpWPSStart.CustomFormat = "yyyy-MM-dd HH:mm";
                    st.dtpWPSEnd.CustomFormat = "yyyy-MM-dd HH:mm";
                    if (st.ShowDialog() == DialogResult.OK)
                    {
                        OpensqlConME.Open();
                        int A = chklstWorkPeopleName.CheckedItems.Count;
                        for (int i = 0; i < A; i++)
                        {
                            SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                                chklstWorkPeopleName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                            SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                            DataSet ReadUser = new DataSet();
                            ReadNI.Fill(ReadUser, "ReadUser");
                            SqlComm.CommandText = "insert into WorkPeople (Date,EmpId,EmpName,DepartId,Class,Status," +
                                "StartTime,EndTime,RegisterPeople,RegisterDate,Notation) values ('" +
                                DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][0].ToString().Trim() + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][1].ToString().Trim() + "','" +
                                Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "','" +
                                Classnum + "'," + StatusNum + ",'" + st.dtpWPSStart.Value.ToString("HH:mm") + "','" +
                                st.dtpWPSEnd.Value.ToString("HH:mm") + "','" + lblUserNameShow.Text.ToString() +
                                "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                                st.txtInputNotation.Text + "')";
                            SqlComm.Connection = OpensqlConME;
                            SqlComm.ExecuteNonQuery();
                        }
                        MessageBox.Show("部門：" + cboSelectWorkPeopleDepart.Text.ToString() + Environment.NewLine +
                            Class + "今日排休人數共 " + chklstWorkPeopleName.CheckedItems.Count + "位！" + Environment.NewLine +
                            "每日出勤登記已完成！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                        RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                    }
                }
            }
        }

        /// <summary>
        /// 依假別狀態寫入SQL，排、換休使用，單位一天
        /// </summary>
        /// <param name="StatusNum">假別狀態代碼</param>
        private void InsertWPS_Status(int StatusNum)
        {
            if (rdoClassAM.Checked == false & rdoClassPM.Checked == false)
            {
                MessageBox.Show("尚未選擇班別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else if (cboSelectWorkPeopleDepart.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else
            {
                DateTime time = new DateTime();
                time = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                if (time > t1 | time > t2)
                {
                    MessageBox.Show("已超過每日出勤登記時間！" + Environment.NewLine + "請洽詢管理部-人資專員。", "錯誤",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int Classnum = 0;
                    string Class = "";
                    if (rdoClassAM.Checked == true)
                    {
                        Classnum = 1;
                        Class = "早班";
                    }
                    else if (rdoClassPM.Checked == true)
                    {
                        Classnum = 2;
                        Class = "晚班";
                    }
                    //先撈出選定部門的部門ID
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                        cboSelectWorkPeopleDepart.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    //先檢查今日該部門人員是否已報過出勤
                    int a = chklstWorkPeopleName.CheckedItems.Count;
                    int q;
                    string name = null;
                    SqlDataReader check;
                    for (q = 0; q < a; q++)
                    {
                        OpensqlConME.Close();
                        name = chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                        SqlComm.CommandText = "select * from WorkPeople where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and Class in (1,2) and (Status is null or Status in (0,1,2,3,4,5,6,7,8))";
                        SqlComm.Connection = OpensqlConME;
                        OpensqlConME.Open();
                        check = SqlComm.ExecuteReader();
                        if (check.HasRows)
                        {
                            MessageBox.Show("該人員[" + name + "]今日已報過出勤！", "注意", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);
                            OpensqlConME.Close();
                            return;
                        }
                    }
                    OpensqlConME.Close();
                    OpensqlConME.Open();
                    int A = chklstWorkPeopleName.CheckedItems.Count;
                    for (int i = 0; i < A; i++)
                    {
                        SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                            chklstWorkPeopleName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                        SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet ReadUser = new DataSet();
                        ReadNI.Fill(ReadUser, "ReadUser");
                        SqlComm.CommandText = "insert into WorkPeople (Date,EmpId,EmpName,DepartId,Class,Status," +
                            "RegisterPeople,RegisterDate) values ('" +
                            DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][0].ToString().Trim() + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][1].ToString().Trim() + "','" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "','" +
                            Classnum + "'," + StatusNum + ",'" + lblUserNameShow.Text.ToString() +
                            "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                    }
                    MessageBox.Show("部門：" + cboSelectWorkPeopleDepart.Text.ToString() + Environment.NewLine +
                        Class + "今日排休人數共 " + chklstWorkPeopleName.CheckedItems.Count + "位！" + Environment.NewLine +
                        "每日出勤登記已完成！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    OpenSqlCon.Close();
                    OpensqlConME.Close();
                    RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                }
            }
        }

        /// <summary>
        /// 重新讀取已報出勤人員
        /// </summary>
        /// <param name="departname">ComboBox.部門</param>
        private void RenewWorkPeople(string departname)
        {
            string SQLCon = "server=ERP;database=EW;uid=JSIS;pwd=JSIS";
            string SQLComm = "select DepartId from HPSdDepartTree where DepartName='" + cboSelectWorkPeopleDepart.Text +
                "'";
            DataSet Read = new DataSet();
            using (SqlConnection sqlcon = new SqlConnection(SQLCon))
            {
                using (SqlDataAdapter Load = new SqlDataAdapter(SQLComm, sqlcon))
                {
                    Load.Fill(Read, "DepartId");
                }
            }
            SQLCon = "server=EWNAS;database=ME;uid=me;pwd=2dae5na";
            SQLComm = "select Date as '日期',EmpId as '工號',EmpName as '姓名',Class,Status,StartTime as '起始時間'," +
                "EndTime as '結束時間',Notation as '事由' from WorkPeople where DepartId='" +
                Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "' and Class in (1,2) and " +
                "(Status is null or Status in (0,1,2,3,4,5,6,7,8)) and Date between '" + DateTime.Now.
                ToString("yyyy-MM-dd") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by Date,Class asc";
            using (SqlConnection sqlcon = new SqlConnection(SQLCon))
            {
                using (SqlDataAdapter Load = new SqlDataAdapter(SQLComm, sqlcon))
                {
                    Load.Fill(Read, "WorkPeople");
                }
            }
            string classnum = "1";
            //宣告早班A和晚班P的變數，供後續加總使用
            int A = 0;
            int P = 0;
            Read.Tables["WorkPeople"].Columns.Add("班別", typeof(string));
            Read.Tables["WorkPeople"].Columns.Add("狀態", typeof(string));
            Read.Tables["WorkPeople"].Columns["班別"].SetOrdinal(5);
            Read.Tables["WorkPeople"].Columns["狀態"].SetOrdinal(6);
            for (int i = 0; i < Read.Tables["WorkPeople"].Rows.Count; i++)
            {
                if (Read.Tables["WorkPeople"].Rows[i]["Class"].ToString().Trim() == classnum)
                {
                    A++;
                    Read.Tables["WorkPeople"].Rows[i]["班別"] = "早班";
                    if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "正常出勤";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "0")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "排休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "1")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "換休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "2")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "調休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "3")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "特休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "4")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "病假";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "5")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "事假";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "6")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "曠職";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "7")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "新進人員";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "8")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "其它";
                    }
                }
                else
                {
                    P++;
                    Read.Tables["WorkPeople"].Rows[i]["班別"] = "晚班";
                    if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "正常出勤";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "0")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "排休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "1")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "換休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "2")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "調休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "3")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "特休";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "4")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "病假";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "5")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "事假";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "6")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "曠職";
                    }
                    else if (Read.Tables["WorkPeople"].Rows[i]["Status"].ToString().Trim() == "7")
                    {
                        Read.Tables["WorkPeople"].Rows[i]["狀態"] = "新進人員";
                    }
                }
            }
            //宣告早晚班加總變數
            int sumAP = A + P;
            lblWorkPeopleShowNum.Text = "已報出勤：日班" + A + "員、晚班" + P + "員、共" + sumAP + "員。";
            dgvWorkPeopleShow.DataSource = Read.Tables["WorkPeople"];
            dgvWorkPeopleShow.Columns["Class"].Visible = false;
            dgvWorkPeopleShow.Columns["Status"].Visible = false;
            dgvWorkPeopleShow.Columns["日期"].Width = 60;
            dgvWorkPeopleShow.Columns["工號"].Width = 60;
            dgvWorkPeopleShow.Columns["姓名"].Width = 60;
            dgvWorkPeopleShow.Columns["班別"].Width = 60;
            dgvWorkPeopleShow.Columns["狀態"].Width = 60;
            dgvWorkPeopleShow.Columns["起始時間"].Width = 80;
            dgvWorkPeopleShow.Columns["結束時間"].Width = 80;
            dgvWorkPeopleShow.Columns["事由"].Width = 150;
        }

        /// <summary>
        /// 統計各課出勤狀況秀在DataGridView
        /// 計算人數和出勤率
        /// </summary>
        private void WorkPeopleReferAll()
        {
            dgvWorkPeopleReferShow.Rows.Clear();
            dgvWPRshow.Rows.Clear();
            //先將depart陣列內容填入DataGridView
            DataGridViewRowCollection row = dgvWorkPeopleReferShow.Rows;
            for (int i = 0; i < depart.Count(); i++)
            {
                row.Add(new object[] { depart[i] });
            }
            DataGridViewRowCollection row1 = dgvWPRshow.Rows;
            for (int i = 0; i < depart.Count(); i++)
            {
                row1.Add(new object[] { depart[i] });
            }
            //撈出當天的出勤資料，不分早晚班
            string SQLComm = "select DepartId,Status,EmpName,Notation from WorkPeople where Class in (1,2) and " +
                "Date between '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and '" +
                DateTime.Now.ToString("yyyy-MM-dd") + "'";
            using (SqlConnection sqlcon = new SqlConnection(SQLCon))
            {
                using (SqlDataAdapter Load = new SqlDataAdapter(SQLComm, sqlcon))
                {
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "AllUser");
                    //先將Read.Tables["AllUser"]欄位1的部門編號轉換成部門名稱
                    for (int i = 0; i < Read.Tables["AllUser"].Rows.Count; i++)
                    {
                        for (int x = 0; x < departid.Count(); x++)
                        {
                            if (Read.Tables["AllUser"].Rows[i][0].ToString().Trim() == departid[x])
                            {
                                Read.Tables["Alluser"].Rows[i][0] = depart[x];
                            }
                        }
                    }
                    //依照DataGridView所建立的各部門列數與Read.Tables["AllUser"]的欄位2之出勤狀態進行各課的出勤狀態人數統計
                    for (int i = 0; i < dgvWorkPeopleReferShow.Rows.Count - 1; i++)
                    {
                        //a0=正常出勤、a1=排休、a2=換休、a3=調休、a4=特休、a5=病假、a6=事假、a7=曠職、a8=新進、a9=其它
                        int a0 = 0, a1 = 0, a2 = 0, a3 = 0, a4 = 0, a5 = 0, a6 = 0, a7 = 0, a8 = 0, a9 = 0;
                        //建立List，讓比對吻合時，將人名塞入List
                        List<string> A1 = new List<string>();
                        List<string> A2 = new List<string>();
                        List<string> A3 = new List<string>();
                        List<string> A4 = new List<string>();
                        List<string> A5 = new List<string>();
                        List<string> A6 = new List<string>();
                        List<string> A7 = new List<string>();
                        List<string> A8 = new List<string>();
                        List<string> A9 = new List<string>();
                        List<string> A9other = new List<string>();
                        //宣告假別陣列，每比對完一個部門就把該部門的各List值依序填入陣列
                        string[] all1 = new string[10];
                        string[] all2 = new string[10];
                        string[] all3 = new string[10];
                        string[] all4 = new string[10];
                        string[] all5 = new string[10];
                        string[] all6 = new string[10];
                        string[] all7 = new string[10];
                        string[] all8 = new string[10];
                        string[] all9 = new string[10];
                        string[] all9other = new string[10];
                        int x = 0;
                        for (x = 0; x < Read.Tables["AllUser"].Rows.Count; x++)
                        {
                            //讓DataGridView設好的部門別去和Read.Tables["AllUser"]進行比對
                            if (dgvWorkPeopleReferShow.Rows[i].Cells[0].Value.ToString().Trim() ==
                                Read.Tables["AllUser"].Rows[x][0].ToString().Trim())
                            {
                                //依照撈出的Status狀態，進行部門各假別的人數統計
                                if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "")
                                {
                                    a0++;
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "0")
                                {
                                    a1++;
                                    A1.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "1")
                                {
                                    a2++;
                                    A2.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "2")
                                {
                                    a3++;
                                    A3.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "3")
                                {
                                    a4++;
                                    A4.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "4")
                                {
                                    a5++;
                                    A5.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "5")
                                {
                                    a6++;
                                    A6.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "6")
                                {
                                    a7++;
                                    A7.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "7")
                                {
                                    a8++;
                                    A8.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                }
                                else if (Read.Tables["AllUser"].Rows[x][1].ToString().Trim() == "8")
                                {
                                    a9++;
                                    A9.Add(Read.Tables["AllUser"].Rows[x][2].ToString().Trim());
                                    A9other.Add(Read.Tables["AllUser"].Rows[x][3].ToString().Trim());
                                }
                            }
                            //先判斷各假別的List是否有值，若有值，才開始將List的值塞進陣列
                            if (A1.Count() > 0)
                            {
                                for (int A = 0; A < A1.Count(); A++)
                                {
                                    all1[A] = A1[A];
                                }
                            }
                            if (A2.Count() > 0)
                            {
                                for (int A = 0; A < A2.Count(); A++)
                                {
                                    all2[A] = A2[A];
                                }
                            }
                            if (A3.Count() > 0)
                            {
                                for (int A = 0; A < A3.Count(); A++)
                                {
                                    all3[A] = A3[A];
                                }
                            }
                            if (A4.Count() > 0)
                            {
                                for (int A = 0; A < A4.Count(); A++)
                                {
                                    all4[A] = A4[A];
                                }
                            }
                            if (A5.Count() > 0)
                            {
                                for (int A = 0; A < A5.Count(); A++)
                                {
                                    all5[A] = A5[A];
                                }
                            }
                            if (A6.Count() > 0)
                            {
                                for (int A = 0; A < A6.Count(); A++)
                                {
                                    all6[A] = A6[A];
                                }
                            }
                            if (A7.Count() > 0)
                            {
                                for (int A = 0; A < A7.Count(); A++)
                                {
                                    all7[A] = A7[A];
                                }
                            }
                            if (A8.Count() > 0)
                            {
                                for (int A = 0; A < A8.Count(); A++)
                                {
                                    all8[A] = A8[A];
                                }
                            }
                            if (A9.Count() > 0)
                            {
                                for (int A = 0; A < A9.Count(); A++)
                                {
                                    all9[A] = A9[A];
                                    if (A9other.Count() > 0)
                                    {
                                        all9other[A] = "(" + A9other[A] + ")";
                                    }
                                }
                            }
                        }
                        //把統計好的人數指定給DataGridView相關欄位
                        dgvWorkPeopleReferShow.Rows[i].Cells["實到"].Value = a0;
                        dgvWorkPeopleReferShow.Rows[i].Cells["排休"].Value = a1;
                        dgvWorkPeopleReferShow.Rows[i].Cells["換休"].Value = a2;
                        dgvWorkPeopleReferShow.Rows[i].Cells["調休"].Value = a3;
                        dgvWorkPeopleReferShow.Rows[i].Cells["特休"].Value = a4;
                        dgvWorkPeopleReferShow.Rows[i].Cells["病假"].Value = a5;
                        dgvWorkPeopleReferShow.Rows[i].Cells["事假"].Value = a6;
                        dgvWorkPeopleReferShow.Rows[i].Cells["曠職"].Value = a7;
                        dgvWorkPeopleReferShow.Rows[i].Cells["新進"].Value = a8;
                        dgvWorkPeopleReferShow.Rows[i].Cells["其它"].Value = a9;
                        //把各部門未出勤的人員依假別分類秀出
                        dgvWPRshow.Rows[i].Cells["排休人員"].Value = all1[0] + "  " + all1[1] + "  " + all1[2] +
                            "\r\n" + all1[3] + "  " + all1[4] + "  " + all1[5] + "\r\n" + all1[6] + "  " + all1[7] +
                            "  " + all1[8] + "  " + all1[9];
                        dgvWPRshow.Rows[i].Cells["換休人員"].Value = all2[0] + "  " + all2[1] + "  " + all2[2] +
                            "\r\n" + all2[3] + "  " + all2[4] + "  " + all2[5] + "\r\n" + all2[6] + "  " + all2[7] +
                            "  " + all2[8] + "  " + all2[9];
                        dgvWPRshow.Rows[i].Cells["調休人員"].Value = all3[0] + "  " + all3[1] + "  " + all3[2] +
                            "\r\n" + all3[3] + "  " + all3[4] + "  " + all3[5] + "\r\n" + all3[6] + "  " + all3[7] +
                            "  " + all3[8] + "  " + all3[9];
                        dgvWPRshow.Rows[i].Cells["特休人員"].Value = all4[0] + "  " + all4[1] + "  " + all4[2] +
                            "\r\n" + all4[3] + "  " + all4[4] + "  " + all4[5] + "\r\n" + all4[6] + "  " + all4[7] +
                            "  " + all4[8] + "  " + all4[9];
                        dgvWPRshow.Rows[i].Cells["病假人員"].Value = all5[0] + "  " + all5[1] + "  " + all5[2] +
                            "\r\n" + all5[3] + "  " + all5[4] + "  " + all5[5] + "\r\n" + all5[6] + "  " + all5[7] +
                            "  " + all5[8] + "  " + all5[9];
                        dgvWPRshow.Rows[i].Cells["事假人員"].Value = all6[0] + "  " + all6[1] + "  " + all6[2] +
                            "\r\n" + all6[3] + "  " + all6[4] + "  " + all6[5] + "\r\n" + all6[6] + "  " + all6[7] +
                            "  " + all6[8] + "  " + all6[9];
                        dgvWPRshow.Rows[i].Cells["曠職人員"].Value = all7[0] + "  " + all7[1] + "  " + all7[2] +
                            "\r\n" + all7[3] + "  " + all7[4] + "  " + all7[5] + "\r\n" + all7[6] + "  " + all7[7] +
                            "  " + all7[8] + "  " + all7[9];
                        dgvWPRshow.Rows[i].Cells["新進人員"].Value = all8[0] + "  " + all8[1] + "  " + all8[2] +
                            "\r\n" + all8[3] + "  " + all8[4] + "  " + all8[5] + "\r\n" + all8[6] + "  " + all8[7] +
                            "  " + all8[8] + "  " + all8[9];
                        dgvWPRshow.Rows[i].Cells["其它人員"].Value = all9[0] + all9other[0] + "  " + all9[1] +
                            all9other[1] + "  " + all9[2] + all9other[2] + "\r\n" + all9[3] +
                            all9other[3] + "  " + all9[4] + "  " + all9[5] + "\r\n" + all9[6] + "  " + all9[7] +
                            "  " + all9[8] + "  " + all9[9];
                    }
                }
            }
            //撈出各部門在職中的人數，再指定給DataGridView的應到欄位秀出
            SQLComm = "select distinct DepartId,COUNT(*) as Num from HPSdEmpInfo where EmpStatus=1 group by " +
                        "DepartId";
            using (SqlConnection sqlcon = new SqlConnection(SQLConERP))
            {
                using (SqlDataAdapter Load = new SqlDataAdapter(SQLComm, sqlcon))
                {
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "UserNum");
                    for (int i = 0; i < Read.Tables["UserNum"].Rows.Count; i++)
                    {
                        //將管理部的在職人數減去1(掛名人員)
                        if (Read.Tables["UserNum"].Rows[i][0].ToString().Trim() == "EM")
                        {
                            int x = (Convert.ToInt32(Read.Tables["UserNum"].Rows[i][1])) - 1;
                            Read.Tables["UserNum"].Rows[i][1] = x.ToString();
                        }
                        //將廠長室的在職人數減去2(掛名人員)
                        else if (Read.Tables["UserNum"].Rows[i][0].ToString().Trim() == "MM")
                        {
                            int x = (Convert.ToInt32(Read.Tables["UserNum"].Rows[i][1])) - 2;
                            Read.Tables["UserNum"].Rows[i][1] = x.ToString();
                        }
                        //先將Read.Tables的部門代碼轉成部門名稱
                        for (int x = 0; x < departid.Count(); x++)
                        {
                            if (Read.Tables["UserNum"].Rows[i][0].ToString().Trim() == departid[x])
                            {
                                Read.Tables["UserNum"].Rows[i][0] = depart[x];
                            }
                        }
                        //若Read.Tables的部門名稱等於DataGridView的部門名稱，則將在職人數帶入欄位
                        for (int s = 0; s < dgvWorkPeopleReferShow.Rows.Count - 1; s++)
                        {
                            if (dgvWorkPeopleReferShow.Rows[s].Cells[0].Value.ToString().Trim() ==
                                Read.Tables["UserNum"].Rows[i][0].ToString().Trim())
                            {
                                dgvWorkPeopleReferShow.Rows[s].Cells[1].Value = Read.Tables["UserNum"].Rows[i][1]
                                    .ToString().Trim();
                            }
                        }
                    }
                }
            }
            //將DataGridView欄位為0的值改為null
            for (int i = 0; i < dgvWorkPeopleReferShow.Rows.Count - 1; i++)
            {
                for (int x = 0; x < dgvWorkPeopleReferShow.Columns.Count; x++)
                {
                    if (dgvWorkPeopleReferShow.Rows[i].Cells[x].Value.ToString() == "0")
                    {
                        dgvWorkPeopleReferShow.Rows[i].Cells[x].Value = "";
                    }
                }
            }
            //計算實到人數與出勤率
            float alluser = float.Parse(lblAllUserShow.Text.Trim(new char[] { '員' }));
            float[] num = new float[dgvWorkPeopleReferShow.Rows.Count - 1];
            //將欄位數值帶入陣列
            for (int a = 0; a < dgvWorkPeopleReferShow.Rows.Count - 1; a++)
            {
                //判斷欄位值是否為null，若為null，則將陣列填入0，避免程式出錯
                if (dgvWorkPeopleReferShow.Rows[a].Cells[2].Value.ToString() == "")
                {
                    num[a] = 0;
                }
                else
                {
                    num[a] = float.Parse(dgvWorkPeopleReferShow.Rows[a].Cells[2].Value.ToString());
                }
            }
            //將陣列加總
            float realusersum = num.Sum();
            lblRealUserShow.Text = realusersum + "員";
            //實到人數除以應到人數
            float percent = realusersum / alluser;
            lblTodayAttendanceShow.Text = percent.ToString("#0.00%");
        }

        /// <summary>
        /// 將出勤統計資料滙出至EXCEL(Two Sheet)，並存在個人系統暫存區
        /// </summary>
        private void WPRtoExcelTemp()
        {
            //DataGridView沒有資料就不執行
            if (dgvWorkPeopleReferShow.Rows.Count <= 1 & dgvWPRshow.Rows.Count <= 1)
            {
                MessageBox.Show("沒有可滙出的資料！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else
            {
                //建一個excel物件
                Excel._Application excel = new Excel.Application();
                //建一個excel物件下的工作簿
                Excel._Workbook workbook = excel.Workbooks.Add();
                //建二個excel物件下的工作表
                Excel._Worksheet worksheet1 = excel.Worksheets.Add();
                Excel._Worksheet worksheet2 = excel.Worksheets.Add();
                //宣告表示欄位的英文和數字陣列
                string[] EnRange = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
                string[] NumRange = new string[17];
                //將數字帶入NumRange字串陣列
                int q = 0;
                for (int i = 0; i < NumRange.Count(); i++)
                {
                    q++;
                    NumRange[i] = Convert.ToString(q);
                }
                try
                {
                    string Date = DateTime.Now.ToString("yyyy-MM-dd");
                    string Temp = Path.GetTempPath();
                    //設定滙出後的存檔路徑(儲存在桌面)
                    string SaveFilePath = Temp + Date + ".xls";
                    //Set Excel Sheet Name
                    worksheet1.Name = "未出勤名單";
                    worksheet2.Name = "人數統計";

                    PGB pgb = new PGB();
                    pgb.progressBar1.Minimum = 0;
                    pgb.progressBar1.Maximum = (dgvWorkPeopleReferShow.Rows.Count - 1) + (dgvWPRshow.Rows.Count - 1);
                    pgb.progressBar1.Step = 1;
                    pgb.progressBar1.Value = 0;
                    pgb.progressBar1.ForeColor = Color.Pink;
                    pgb.progressBar1.Style = ProgressBarStyle.Continuous;
                    pgb.Show();
                    //填入dgvWPRshow欄位名稱至worksheet1
                    for (int i = 0; i < dgvWPRshow.Columns.Count; i++)
                    {
                        worksheet1.Cells[1, i + 1] = dgvWPRshow.Columns[i].HeaderText;
                    }
                    //填入dgvWPRshow資料至worksheet1
                    for (int i = 0; i < dgvWPRshow.Rows.Count - 1; i++)
                    {
                        pgb.progressBar1.Value++;
                        for (int j = 0; j < dgvWPRshow.Columns.Count; j++)
                        {
                            if (dgvWPRshow[j, i].ValueType == typeof(string))
                            {
                                worksheet1.Cells[i + 2, j + 1] = "'" + dgvWPRshow[j, i].Value.ToString();
                            }
                            else
                            {
                                worksheet1.Cells[i + 2, j + 1] = dgvWPRshow[j, i].Value.ToString();
                            }
                        }
                    }
                    //填入dgvWorkPeopleReferShow欄位名稱至worksheet2
                    for (int i = 0; i < dgvWorkPeopleReferShow.Columns.Count; i++)
                    {
                        worksheet2.Cells[1, i + 1] = dgvWorkPeopleReferShow.Columns[i].HeaderText;
                    }
                    //填入dgvWorkPeopleReferShow資料至worksheet2
                    for (int i = 0; i < dgvWorkPeopleReferShow.Rows.Count - 1; i++)
                    {
                        pgb.progressBar1.Value++;
                        for (int j = 0; j < dgvWorkPeopleReferShow.Columns.Count; j++)
                        {
                            if (dgvWorkPeopleReferShow[j, i].ValueType == typeof(string))
                            {
                                worksheet2.Cells[i + 2, j + 1] = "'" + dgvWorkPeopleReferShow[j, i].Value.ToString();
                            }
                            else
                            {
                                worksheet2.Cells[i + 2, j + 1] = dgvWorkPeopleReferShow[j, i].Value.ToString();
                            }
                        }
                    }
                    //設定滙出後，欄位寛度自動配合資料調整
                    worksheet1.Cells.EntireRow.AutoFit();
                    worksheet2.Cells.EntireRow.AutoFit();
                    //自動調整列高
                    worksheet1.Cells.EntireColumn.AutoFit();
                    worksheet2.Cells.EntireColumn.AutoFit();
                    //用字串陣列下去設定worksheet2欄位格式
                    for (int i = 0; i < EnRange.Count(); i++)
                    {
                        for (int x = 0; x < NumRange.Count(); x++)
                        {
                            //設定欄位垂直置中
                            worksheet2.get_Range(EnRange[i] + NumRange[x]).VerticalAlignment = Excel.XlHAlign.
                                xlHAlignCenter;
                            //設定欄位水平置中
                            worksheet2.get_Range(EnRange[i] + NumRange[x]).HorizontalAlignment = Excel.XlHAlign.
                                xlHAlignCenter;
                            //設定欄位框線
                            worksheet2.get_Range(EnRange[i] + NumRange[x]).Borders.LineStyle = 1;
                            //設定欄位框線顏色
                            worksheet2.get_Range(EnRange[i] + NumRange[x]).Borders.Color = Color.White;
                            //設定字型顏色為White
                            worksheet2.get_Range(EnRange[i] + NumRange[x]).Font.Color = Color.White;
                            //設定欄位背景顏色為DodgerBlue
                            worksheet2.get_Range(EnRange[i] + NumRange[x]).Interior.Color = Color.DodgerBlue;
                        }
                    }
                    //用字串陣列下去設定worksheet1欄位格式
                    for (int i = 0; i < EnRange.Count()-2; i++)
                    {
                        for (int x = 0; x < NumRange.Count(); x++)
                        {
                            if (EnRange[i] == "A")
                            {
                                worksheet1.get_Range(EnRange[i] + NumRange[x]).ColumnWidth = 10;
                                worksheet1.get_Range(EnRange[i] + NumRange[x]).HorizontalAlignment = Excel.XlHAlign.
                                xlHAlignCenter;
                            }
                            else if(EnRange[i]=="J")
                            {
                                worksheet1.get_Range(EnRange[i] + NumRange[x]).ColumnWidth = 30;
                            }
                            else
                            {
                                worksheet1.get_Range(EnRange[i] + NumRange[x]).ColumnWidth = 20;
                            }
                            worksheet1.get_Range(EnRange[i] + NumRange[x]).VerticalAlignment = Excel.XlHAlign.
                                xlHAlignCenter;
                            worksheet1.get_Range(EnRange[i] + NumRange[x]).Borders.LineStyle = 1;
                            worksheet1.get_Range(EnRange[i] + NumRange[x]).Borders.Color = Color.White;
                            worksheet1.get_Range(EnRange[i] + NumRange[x]).Font.Color = Color.White;
                            worksheet1.get_Range(EnRange[i] + NumRange[x]).Interior.Color = Color.DodgerBlue;
                        }
                    }
                    //在第一列插入新列
                    ((Excel.Range)worksheet1.Cells[1, 1]).EntireRow.Insert(null, null);
                    ((Excel.Range)worksheet2.Cells[1, 1]).EntireRow.Insert(null, null);
                    //設定worksheet1(A1)欄位格式
                    worksheet1.get_Range("A1", "J1").Merge(worksheet1.get_Range("A1", "J1").MergeCells);//合併欄位
                    worksheet1.get_Range("A1").Value = "長鴻電子-每日未出勤名單";
                    worksheet1.get_Range("A1").Font.Size = 22;
                    worksheet1.get_Range("A1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet1.get_Range("A1").Borders.LineStyle = 1;
                    worksheet1.get_Range("A1").Font.Color = Color.White;
                    worksheet1.get_Range("A1").Interior.Color = Color.DodgerBlue;
                    //設定worksheet2(A1)欄位格式
                    worksheet2.get_Range("A1", "L1").Merge(worksheet2.get_Range("A1", "L1").MergeCells);
                    worksheet2.get_Range("A1").Value = "長鴻電子-每日出勤人數統計表";
                    worksheet2.get_Range("A1").Font.Size = 22;
                    worksheet2.get_Range("A1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet2.get_Range("A1").Borders.LineStyle = 1;
                    worksheet2.get_Range("A1").Font.Color = Color.White;
                    worksheet2.get_Range("A1").Interior.Color = Color.DodgerBlue;
                    //設定worksheet2最下方二列的格式並填值進去
                    int RowNum = dgvWorkPeopleReferShow.Rows.Count + 2;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Merge(worksheet2.get_Range("C" + RowNum, "G" +
                        RowNum).MergeCells);
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Merge(worksheet2.get_Range("H" + RowNum, "L" +
                        RowNum).MergeCells);
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Borders.LineStyle = 1;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Font.Size = 16;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Borders.Color = Color.White;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Font.Color = Color.White;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Interior.Color = Color.DodgerBlue;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Borders.LineStyle = 1;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Font.Size = 16;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Borders.Color = Color.White;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Font.Color = Color.White;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Interior.Color = Color.DodgerBlue;
                    worksheet2.get_Range("C" + RowNum, "G" + RowNum).Value = lblAllUser.Text + lblAllUserShow.Text;
                    worksheet2.get_Range("H" + RowNum, "L" + RowNum).Value = lblRealUser.Text + lblRealUserShow.Text;
                    RowNum++;//+1=換下一列
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Merge(worksheet2.get_Range("C" + RowNum, "L" +
                        RowNum).MergeCells);
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Borders.LineStyle = 1;
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Font.Size = 16;
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Borders.Color = Color.White;
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Font.Color = Color.White;
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Interior.Color = Color.DodgerBlue;
                    worksheet2.get_Range("C" + RowNum, "L" + RowNum).Value = lblTodayAttendance.Text + 
                        lblTodayAttendanceShow.Text;

                    //設置禁止彈出覆蓋或儲存的彈跳視窗
                    excel.DisplayAlerts = false;
                    excel.AlertBeforeOverwriting = false;
                    //將檔案儲存到SaveFile指定的位置
                    excel.ActiveWorkbook.SaveCopyAs(SaveFilePath);
                    pgb.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    //關閉工作簿和結束Excel程式
                    excel.Workbooks.Close();
                    excel.Quit();
                    //釋放資源
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                    excel = null;
                    GC.Collect();
                }
            }
        }

        private void txtNum_keyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void BentoOrder_Load(object sender, EventArgs e)
        {
            tmrDateTime.Enabled = true;
            dtpStartDate.Value = DateTime.Now;
            dtpEndDate.Value = DateTime.Now;
            chkVegetableFood.ForeColor = Color.Red;
            lblVegetableFood.ForeColor = Color.Red;
            lblAllUserShow.Text = "";
            lblTodayAttendanceShow.Text = "";
            lblRealUserShow.Text = "";
            lblWorkPeopleShowNum.Text = "";
            rtbOrderTimeIllustrate.ForeColor = Color.Red;
            lblOrderNumShow.Text = null;
            txtCompanyName.ReadOnly = true;
            txtCompanyTel.ReadOnly = true;
            txtCompanyCellPhone.ReadOnly = true;
            btnBentoTelChangeSave.Enabled = false;
            btnSavePrice.Enabled = false;
            dgvWorkPeopleShow.ReadOnly = true;
            dgvWorkPeopleReferShow.ReadOnly = true;
            //設定從"實到"欄位開始凍結水平視窗
            dgvWorkPeopleReferShow.Columns["實到"].Frozen = true;
            dgvWPRshow.Columns["部門_1"].Frozen = true;
            dgvWPRshow.ReadOnly = true;
            //設定DataGridView自動列高
            dgvWPRshow.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            //設定DataGridView欄位可換行
            dgvWPRshow.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            SqlComm.CommandText = "select distinct HPSdEmpInfo.DepartId,HPSdDepartTree.DepartName from HPSdEmpInfo," +
                "HPSdDepartTree where HPSdEmpInfo.DepartId = HPSdDepartTree.DepartId and HPSdDepartTree.DepartId " +
                "not in ('EF')";
            SqlDataAdapter DepartId = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
            DataSet dpid = new DataSet();
            DepartId.Fill(dpid, "DepartId");
            //撈出在職中的人員，並將總數減3後，秀在Label.Text
            SqlComm.CommandText = "select EmpName from HPSdEmpInfo where EmpStatus=1";
            DepartId.SelectCommand = SqlComm;
            SqlComm.Connection = OpenSqlCon;
            DepartId.Fill(dpid, "AllUser");
            lblAllUserShow.Text = Convert.ToString(dpid.Tables["AllUser"].Rows.Count - 3) + "員";
            //Create new rows for dpid.tables
            DataRow dr = dpid.Tables["DepartId"].NewRow();
            //設定dr的資料
            dr["DepartId"] = "AA";
            dr["DepartName"] = "請選擇";
            //將dr插入到dpid.Tables["DepartId"].Rows的第一列
            dpid.Tables["DepartId"].Rows.InsertAt(dr,0);
            cboDepart.DataSource = dpid.Tables["Departid"];
            cboDepart.DisplayMember = "DepartName";
            //設定cboDpname為唯讀且不可輸入新值
            cboDepart.DropDownStyle = ComboBoxStyle.DropDownList;
            //將tpOrderRefer頁面的ComboBox讀入部門別
            //此ComboBox的DataSource要用Tables的Copy，才不會導致同資料來源的二個頁面的ComboBox連作動
            cboSelectDepartid.DataSource = dpid.Tables["DepartId"].Copy();
            cboSelectDepartid.DisplayMember = "DepartName";
            cboSelectDepartid.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSelectDepart.DataSource = dpid.Tables["DepartId"].Copy();
            cboSelectDepart.DisplayMember = "DepartName";
            cboSelectDepart.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSelectWorkPeopleDepart.DataSource = dpid.Tables["DepartId"].Copy();
            cboSelectWorkPeopleDepart.DisplayMember = "DepartName";
            cboSelectWorkPeopleDepart.DropDownStyle = ComboBoxStyle.DropDownList;
            OpenSqlCon.Close();
            SqlComm.CommandText = "select Name,Tel,CellPhone,replace( convert(varchar(20), cast "+
                "(BentoPrice as money), 1) , '.00', '') as BentoPrice from BentoCompany";
            SqlDataAdapter company = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
            company.Fill(dpid, "Company");
            txtCompanyName.Text = dpid.Tables["Company"].Rows[0][0].ToString();
            txtCompanyTel.Text = dpid.Tables["Company"].Rows[0][1].ToString();
            txtCompanyCellPhone.Text = dpid.Tables["Company"].Rows[0][2].ToString();
            txtBentoPrice.Text = dpid.Tables["Company"].Rows[0][3].ToString();
            OpensqlConME.Close();
        }

        private void tmrDateTime_Tick(object sender, EventArgs e)
        {
            lblDateTimeShow.Text = DateTime.Now.ToString();
        }

        //取消CheckListBox已勾選的項目
        private void btnCancel_Click(object sender, EventArgs e)
        {
            int A = chklstName.Items.Count;
            for (int i = 0; i < A; i++)
            {
                chklstName.SetItemChecked(i, false);
            }
        }

        //依選擇的部門代號,自動帶出部門目前仍在職的人員名字
        private void cboDepart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDepart.SelectedIndex == 0)
            {
                return;
            }
            else
            {
                //先用選到的部門中文去撈部門代號
                SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" + cboDepart.Text + "'";
                SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet Read = new DataSet();
                Load.Fill(Read, "DepartId");
                //再用部門代號撈出仍在職中的人員工號、姓名
                SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where DepartId='" +
                    Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpStatus='1'";
                SqlDataAdapter ReadEmpInfo = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet HPSdEmpInfo = new DataSet();
                ReadEmpInfo.Fill(HPSdEmpInfo, "EmpInfo");
                int A = HPSdEmpInfo.Tables["EmpInfo"].Rows.Count;
                chklstName.Items.Clear();
                for (int i = 0; i < A; i++)
                {
                    chklstName.Items.Add(HPSdEmpInfo.Tables["EmpInfo"].Rows[i][0].ToString().Trim()+
                        HPSdEmpInfo.Tables["EmpInfo"].Rows[i][1].ToString().Trim());
                }
                OpenSqlCon.Close();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == false & rdoNightOrder.Checked == false)
            {
                MessageBox.Show("尚未選擇要報餐的餐別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (cboDepart.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇要報餐的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                //判斷是否已超過規定的報餐時間
                string st1 = "09:50";
                string st2 = "14:50";
                string st3 = "21:50";
                DateTime t = new DateTime();
                t = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                DateTime t3 = Convert.ToDateTime(st3);
                if (rdoAmOrder.Checked == true & t > t1)
                {
                    MessageBox.Show("已超過中餐的報餐時間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else if (rdoPmOrder.Checked == true & t > t2)
                {
                    MessageBox.Show("已超過晚餐的報餐時間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else if (rdoNightOrder.Checked == true & t > t3)
                {
                    MessageBox.Show("已超過宵夜的報餐時間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    string num1 = null;
                    string num2 = null;
                    if (chkVegetableFood.Checked == true)
                    {
                        num2 = "1";
                    }
                    else
                    {
                        num2 = "0";
                    }
                    if (rdoAmOrder.Checked == true)
                    {
                        num1 = "1";
                    }
                    else if (rdoPmOrder.Checked == true)
                    {
                        num1 = "2";
                    }
                    else if (rdoNightOrder.Checked == true)
                    {
                        num1 = "3";
                    }
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" + cboDepart.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    //先檢查今日該部門人員是否已報過餐別
                    int a = chklstName.CheckedItems.Count;
                    int q;
                    string name = null;
                    SqlDataReader check;
                    for (q = 0; q < a; q++)
                    {
                        OpensqlConME.Close();
                        name = chklstName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                        SqlComm.CommandText = "select * from BentoOrder where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") +"' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and OrderStatus=" + num1.ToString() + " and (VegetableFood in (0,1))";
                        SqlComm.Connection = OpensqlConME;
                        OpensqlConME.Open();
                        check = SqlComm.ExecuteReader();
                        if (check.HasRows)
                        {
                            MessageBox.Show("該人員[" + name + "]今日已報過您選擇的餐別！", "注意", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);
                            OpensqlConME.Close();
                            return;
                        }

                    }
                    if (rdoAmOrder.Checked == true)
                    {
                        OpensqlConME.Close();
                        OpensqlConME.Open();
                        int A = chklstName.CheckedItems.Count;
                        for (int i = 0; i < A; i++)
                        {
                            SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                                chklstName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                            SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                            DataSet ReadUser = new DataSet();
                            ReadNI.Fill(ReadUser, "ReadUser");
                            SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                                "VegetableFood,OrderPeople,OrderDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][0].ToString() + "','" + ReadUser.Tables["ReadUser"].Rows[0][1].ToString() +
                                "','" + Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "',1," + num2.ToString() + ",'" + lblUserNameShow.Text.ToString() +
                                "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            SqlComm.Connection = OpensqlConME;
                            SqlComm.ExecuteNonQuery();
                        }
                        MessageBox.Show("部門：" + cboDepart.Text.ToString() + Environment.NewLine + "今日午餐報餐數量共 " +
                            chklstName.CheckedItems.Count + "個！" + Environment.NewLine + "已報餐完成！", "訊息",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                    }
                    else if (rdoPmOrder.Checked == true)
                    {
                        OpensqlConME.Close();
                        OpensqlConME.Open();
                        int A = chklstName.CheckedItems.Count;
                        for (int i = 0; i < A; i++)
                        {
                            SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                                chklstName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                            SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                            DataSet ReadUser = new DataSet();
                            ReadNI.Fill(ReadUser, "ReadUser");
                            SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                                "VegetableFood,OrderPeople,OrderDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][0].ToString() + "','" + ReadUser.Tables["ReadUser"].Rows[0][1].ToString() +
                                "','" + Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "',2," + num2.ToString() + ",'" + lblUserNameShow.Text.ToString() +
                                "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            SqlComm.Connection = OpensqlConME;
                            SqlComm.ExecuteNonQuery();
                        }
                        MessageBox.Show("部門：" + cboDepart.Text.ToString() + Environment.NewLine + "今日晚餐報餐數量共 " +
                            chklstName.CheckedItems.Count + "個！" + Environment.NewLine + "已報餐完成！", "訊息",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                    }
                    else if (rdoNightOrder.Checked == true)
                    {
                        OpensqlConME.Close();
                        OpensqlConME.Open();
                        int A = chklstName.CheckedItems.Count;
                        for (int i = 0; i < A; i++)
                        {
                            SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                                chklstName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                            SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                            DataSet ReadUser = new DataSet();
                            ReadNI.Fill(ReadUser, "ReadUser");
                            SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                                "VegetableFood,OrderPeople,OrderDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][0].ToString() + "','" + ReadUser.Tables["ReadUser"].Rows[0][1].ToString() +
                                "','" + Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "',3," + num2.ToString() + ",'" + lblUserNameShow.Text.ToString() +
                                "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            SqlComm.Connection = OpensqlConME;
                            SqlComm.ExecuteNonQuery();
                        }
                        MessageBox.Show("部門：" + cboDepart.Text.ToString() + Environment.NewLine + "今日宵夜報餐數量共 " +
                            chklstName.CheckedItems.Count + "個！" + Environment.NewLine + "已報餐完成！", "訊息",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                    }
                }
            }
        }

        private void btnSanitary_Click(object sender, EventArgs e)
        {
            if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == false & rdoNightOrder.Checked == false)
            {
                MessageBox.Show("尚未選擇要報餐的餐別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                string st1 = "09:50";
                string st2 = "14:50";
                string st3 = "21:50";
                DateTime t = new DateTime();
                t = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                DateTime t3 = Convert.ToDateTime(st3);
                if (rdoAmOrder.Checked == true & t > t1 == true)
                {
                    MessageBox.Show("已超過中餐的報餐時間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else if (rdoPmOrder.Checked == true & t > t2)
                {
                    MessageBox.Show("已超過晚餐的報餐時間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else if (rdoNightOrder.Checked == true & t > t3)
                {
                    MessageBox.Show("已超過宵夜的報餐時間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    string num = null;
                    if (rdoAmOrder.Checked == true & rdoPmOrder.Checked == false & rdoNightOrder.Checked == false)
                    {
                        num = "1";
                    }
                    else if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == true & rdoNightOrder.Checked == false)
                    {
                        num = "2";
                    }
                    else if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == false & rdoNightOrder.Checked == true)
                    {
                        num = "3";
                    }
                    SqlComm.CommandText = "select * from BentoOrder where (Date >= '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") +
                                    "' and Date <='" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and EmpName='聚豐' and " +
                                    "OrderStatus=" + num.ToString();
                    SqlComm.Connection = OpensqlConME;
                    OpensqlConME.Open();
                    SqlDataReader check = SqlComm.ExecuteReader();
                    if (check.HasRows)
                    {
                        string warning = null;
                        if (rdoAmOrder.Checked == true & rdoPmOrder.Checked == false & rdoNightOrder.Checked == false)
                        {
                            warning = "中餐";
                        }
                        else if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == true & rdoNightOrder.Checked == false)
                        {
                            warning = "晚餐";
                        }
                        else if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == false & rdoNightOrder.Checked == true)
                        {
                            warning = "宵夜";
                        }
                        MessageBox.Show("聚豐-清潔人員" + Environment.NewLine + "今日已報過" + warning.ToString() + "！",
                            "注意", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        OpensqlConME.Close();
                        return;
                    }
                    else
                    {
                        string num1 = null;
                        string num2 = null;
                        OpensqlConME.Close();
                        OpensqlConME.Open();
                        if (chkVegetableFood.Checked == true)
                        {
                            num2 = "1";
                        }
                        else
                        {
                            num2 = "0";
                        }
                        if (rdoAmOrder.Checked == true)
                        {
                            num1 = "1";
                        }
                        else if (rdoPmOrder.Checked == true)
                        {
                            num1 = "2";
                        }
                        else if (rdoNightOrder.Checked == true)
                        {
                            num1 = "3";
                        }
                        SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus,VegetableFood," +
                            "OrderPeople,OrderDate)" + " values ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                            "','EM999'," + "'聚豐','EM','" + num1.ToString() + "'," + num2.ToString() + ",'" +
                            lblUserNameShow.Text.ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                        MessageBox.Show("聚豐報餐完成！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpensqlConME.Close();
                        return;
                    }
                }
            }
        }

        private void btnRefer_Click(object sender, EventArgs e)
        {
            lblOrderNum.Text = "數量：";
            int num = 0;
            if (rdoReferAm.Checked == true)
            {
                num = 1;
            }
            else if (rdoReferPm.Checked == true)
            {
                num = 2;
            }
            else if (rdoReferNight.Checked == true)
            {
                num = 3;
            }
            if (rdoReferAm.Checked == false & rdoReferPm.Checked == false & rdoReferNight.Checked == false)
            {
                MessageBox.Show("尚未選擇要查詢的餐別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (cboSelectDepartid.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇要查詢的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                    cboSelectDepartid.Text + "'";
                SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet Read1 = new DataSet();
                Load.Fill(Read1, "DepartId");
                SqlComm.CommandText = "select EmpName,OrderStatus,VegetableFood,OrderPeople from BentoOrder " +
                    "where DepartId='" + Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString() +
                    "' and OrderStatus=" + num + " and " + "Date between '" +
                    DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" +
                    DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "' Order by EmpName";
                SqlDataAdapter Read = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                DataSet Data = new DataSet();
                Read.Fill(Data, "Data");
                int a = Data.Tables["Data"].Rows.Count;
                int b;
                int c = 0;
                int d;
                int dc;
                Data.Tables["Data"].Columns.Add("餐別", typeof(String));
                Data.Tables["Data"].Columns.Add("素食", typeof(String));
                Data.Tables["Data"].Columns[0].ColumnName = "姓名";
                Data.Tables["Data"].Columns[3].ColumnName = "報餐人員";
                for (b = 0; b < a; b++)
                {
                    if (Data.Tables["Data"].Rows[b][1].ToString() == "1")
                    {
                        Data.Tables["Data"].Rows[b]["餐別"] = "中餐";
                    }
                    else if (Data.Tables["Data"].Rows[b][1].ToString() == "2")
                    {
                        Data.Tables["Data"].Rows[b]["餐別"] = "晚餐";
                    }
                    else if (Data.Tables["Data"].Rows[b][1].ToString() == "3")
                    {
                        Data.Tables["Data"].Rows[b]["餐別"] = "宵夜";
                    }
                }
                for (b = 0; b < a; b++)
                {
                    if (Data.Tables["Data"].Rows[b][2].ToString() == "1")
                    {
                        Data.Tables["Data"].Rows[b]["素食"] = "Yes";
                    }
                }
                for (b = 0; b < a; b++)
                {
                    if (Data.Tables["Data"].Rows[b]["素食"].ToString() == "Yes")
                    {
                        c++;
                    }
                }
                d = Data.Tables["Data"].Rows.Count;
                dc = d - c;
                lblOrderNumShow.Text = "葷 " + dc + "個、" + "素 " + c + "個、" + " 共 " + d + "個";
                dgvBentoDataShow.DataSource = Data.Tables["Data"];
                //隱藏欄位
                dgvBentoDataShow.Columns[1].Visible = false;
                dgvBentoDataShow.Columns[2].Visible = false;
                //調整欄位位置
                dgvBentoDataShow.Columns[4].DisplayIndex = 1;
                dgvBentoDataShow.Columns[5].DisplayIndex = 2;
                //dgvBentoDataShow.Columns[0-5].
                OpensqlConME.Close();
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            string st1 = "09:50";
            string st2 = "14:50";
            string st3 = "21:50";
            DateTime t = new DateTime();
            t = DateTime.Now;
            DateTime t1 = Convert.ToDateTime(st1);
            DateTime t2 = Convert.ToDateTime(st2);
            DateTime t3 = Convert.ToDateTime(st3);
            if (rdoReferAm.Checked == true & t > t1)
            {
                MessageBox.Show("已超過中餐的報餐時間！" + Environment.NewLine + "禁止取消訂餐！", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (rdoReferPm.Checked == true & t > t2)
            {
                MessageBox.Show("已超過晚餐的報餐時間！" + Environment.NewLine + "禁止取消訂餐！", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (rdoReferNight.Checked == true & t > t3)
            {
                MessageBox.Show("已超過宵夜的報餐時間！" + Environment.NewLine + "禁止取消訂餐！", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                int num = 0;
                string order = null;
                if (rdoReferAm.Checked == true)
                {
                    num = 1;
                    order = "中餐";
                }
                else if (rdoReferPm.Checked == true)
                {
                    num = 2;
                    order = "晚餐";
                }
                else if (rdoReferNight.Checked == true)
                {
                    num = 3;
                    order = "宵夜";
                }
                if (dgvBentoDataShow.Rows.Count == 1 | dgvBentoDataShow.DataSource == null | dgvBentoDataShow.ColumnCount==1)
                {
                    MessageBox.Show("報餐查詢資料為空白！" + Environment.NewLine + "請先進行查詢！", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                else
                {
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                    cboSelectDepartid.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read1 = new DataSet();
                    Load.Fill(Read1, "DepartId");
                    SqlComm.CommandText = "update BentoOrder set OrderStatus=0,UpdateDate='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                        "',UpdatePeople='" + lblUserNameShow.Text + "' where Date between '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") +
                        "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "' and EmpName='" + dgvBentoDataShow.CurrentRow.Cells["姓名"].Value.ToString() +
                        "' and OrderStatus=" + num + " and DepartId='" +Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString()  + "'";
                    if (MessageBox.Show("您確定要取消【" + dgvBentoDataShow.CurrentRow.Cells[0].Value.ToString() + "】" + order + "訂餐？",
                        "確認訊息", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        OpensqlConME.Open();
                        SqlComm.Connection = OpensqlConME;
                        int i = SqlComm.ExecuteNonQuery();
                        if (i >= 1 & MessageBox.Show("已取消成功！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk) == DialogResult.OK)
                        {
                            OpensqlConME.Close();
                            OpensqlConME.Open();
                            SqlComm.CommandText = "select EmpName,OrderStatus,VegetableFood,OrderPeople from BentoOrder " +
                                "where DepartId='" + Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and OrderStatus=" + num + " and " +
                                "Date between '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "' Order by EmpName";
                            SqlDataAdapter Read = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                            DataSet Data = new DataSet();
                            Read.Fill(Data, "Data");
                            int a = Data.Tables["Data"].Rows.Count;
                            int b;
                            int c = 0;
                            int d;
                            int dc;
                            Data.Tables["Data"].Columns.Add("餐別", typeof(String));
                            Data.Tables["Data"].Columns.Add("素食", typeof(String));
                            Data.Tables["Data"].Columns[0].ColumnName = "姓名";
                            Data.Tables["Data"].Columns[3].ColumnName = "報餐人員";
                            for (b = 0; b < a; b++)
                            {
                                if (Data.Tables["Data"].Rows[b][1].ToString() == "1")
                                {
                                    Data.Tables["Data"].Rows[b]["餐別"] = "中餐";
                                }
                                else if (Data.Tables["Data"].Rows[b][1].ToString() == "2")
                                {
                                    Data.Tables["Data"].Rows[b]["餐別"] = "晚餐";
                                }
                                else if (Data.Tables["Data"].Rows[b][1].ToString() == "3")
                                {
                                    Data.Tables["Data"].Rows[b]["餐別"] = "宵夜";
                                }
                            }
                            for (b = 0; b < a; b++)
                            {
                                if (Data.Tables["Data"].Rows[b][2].ToString() == "1")
                                {
                                    Data.Tables["Data"].Rows[b]["素食"] = "Yes";
                                }
                            }
                            for (b = 0; b < a; b++)
                            {
                                if (Data.Tables["Data"].Rows[b]["素食"].ToString() == "Yes")
                                {
                                    c++;
                                }
                            }
                            d = Data.Tables["Data"].Rows.Count;
                            dc = d - c;
                            lblOrderNumShow.Text = "葷 " + dc + "個、" + "素 " + c + "個、" + " 共 " + d + "個";
                            dgvBentoDataShow.DataSource = Data.Tables["Data"];
                            //隱藏欄位
                            dgvBentoDataShow.Columns[1].Visible = false;
                            dgvBentoDataShow.Columns[2].Visible = false;
                            //調整欄位位置
                            dgvBentoDataShow.Columns[4].DisplayIndex = 1;
                            dgvBentoDataShow.Columns[5].DisplayIndex = 2;
                            //dgvBentoDataShow.Columns[0-5].
                            OpensqlConME.Close();
                        }
                    }
                    else
                    {
                        OpensqlConME.Close();
                        return;
                    }
                }
            }
        }

        private void btnBentoTelChange_Click(object sender, EventArgs e)
        {
            btnBentoTelChangeSave.Enabled = true;
            txtCompanyName.ReadOnly = false;
            txtCompanyTel.ReadOnly = false;
            txtCompanyCellPhone.ReadOnly = false;
        }

        private void btnBentoTelChangeSave_Click(object sender, EventArgs e)
        {
            SqlComm.CommandText = "update BentoCompany set Name='" + txtCompanyName.Text + "',Tel='" + txtCompanyTel.Text +
                "',CellPhone='" + txtCompanyCellPhone.Text + "'";
            SqlComm.Connection = OpensqlConME;
            OpensqlConME.Open();
            int i = SqlComm.ExecuteNonQuery();
            if (i == 1)
            {
                MessageBox.Show("保存成功！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                OpensqlConME.Close();
                btnBentoTelChangeSave.Enabled = false;
                txtCompanyName.ReadOnly = true;
                txtCompanyTel.ReadOnly = true;
                txtCompanyCellPhone.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("保存發生錯誤或異常！" + Environment.NewLine + "請通知MIS！", "訊息",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                OpensqlConME.Close();
                btnBentoTelChangeSave.Enabled = false;
                txtCompanyName.ReadOnly = true;
                txtCompanyTel.ReadOnly = true;
                txtCompanyCellPhone.ReadOnly = true;
                return;
            }
        }

        private void btnReferNoOrder_Click(object sender, EventArgs e)
        {
            lblOrderNumShow.Text = null;
            int num = 0;
            string order = null;
            if (rdoReferAm.Checked == false & rdoReferPm.Checked == false & rdoReferNight.Checked == false)
            {
                MessageBox.Show("尚未選擇要查詢的餐別！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (rdoReferAm.Checked == true)
                {
                    num = 1;
                    order = "中餐";
                }
                else if (rdoReferPm.Checked == true)
                {
                    num = 2;
                    order = "晚餐";
                }
                else if (rdoReferNight.Checked == true)
                {
                    num = 3;
                    order = "宵夜";
                }
                SqlComm.CommandText = "select distinct DepartId from HPSdEmpInfo";
                SqlDataAdapter read = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet Depart = new DataSet();
                read.Fill(Depart, "A");
                OpenSqlCon.Close();
                SqlComm.CommandText = "select distinct DepartId from BentoOrder where OrderStatus=" + num + " and Date between '" +
                    DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "'";
                read = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                read.Fill(Depart, "B");
                OpensqlConME.Close();
                DataTable C = new DataTable();
                C = Depart.Tables["A"].Copy();
                for (int i = 0; i < Depart.Tables["B"].Rows.Count; i++)
                {
                    for (int x = 0; x < Depart.Tables["A"].Rows.Count; x++)
                    {
                        if (Depart.Tables["B"].Rows[i][0].ToString().Trim() == Depart.Tables["A"].Rows[x][0].ToString()
                            .Trim())
                        {
                            C.Rows[x].Delete();
                        }
                    }
                }
                dgvBentoDataShow.DataSource = C;
                dgvBentoDataShow.Columns[0].HeaderText = "部門";
                int s = dgvBentoDataShow.Rows.Count - 1;
                lblOrderNum.Text = "共 " + s + "個部門，尚未報" + order + "！";
                OpensqlConME.Close();
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            int A = chklstName.Items.Count;
            for (int i = 0; i < A; i++)
            {
                chklstName.SetItemChecked(i, true);
            }
        }

        //月結統計-全部
        private void btnReferOrder_Click(object sender, EventArgs e)
        {
            if (rdOneCompany.Checked == false & rdoTwoCompany.Checked == false & rdoAllCompany.Checked == false & 
                rdoSelectDepart.Checked==false)
            {
                MessageBox.Show("尚未選擇要查詢的條件！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (rdOneCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT " +
                        "from BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" +
                        dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and DepartId not in " +
                        "('EL') and VegetableFood=0 group by Date " + "select Convert(char(10),Date,20) as Date,count" +
                        "(Date) as num1 into #BB from BentoOrder where Date between '" + dtpStartDate.Value.ToString
                        ("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus in " +
                        "(1,2,3) and DepartId not in ('EL') and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        //下述開始計算數量與金額並把結果傳回TextBox
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        //判斷負責金額的TextBox是否為null，避免後續計算金額時，發生錯誤
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        //下述先將int轉型為string，再將建立的string變數指定給TextBox並再轉成指定的貨幤格式
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[一廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if (rdoTwoCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from " +
                        "BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" +
                        dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and DepartId='EL' " +
                        "and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where " +
                        "Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.
                        ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and DepartId='EL' and VegetableFood=1 " +
                        "group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[二廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if (rdoAllCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from " +
                        "BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" +
                        dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and VegetableFood=0 " +
                        "group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where " +
                        "Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.
                        ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[全部]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    if (cboSelectDepart.SelectedIndex == 0)
                    {
                        MessageBox.Show("尚未選擇要查詢的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                            cboSelectDepart.Text + "'";
                        SqlDataAdapter Load1 = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet Read1 = new DataSet();
                        Load1.Fill(Read1, "DepartId");
                        string depart = Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString();
                        SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT " +
                            "from BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") +
                            "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and " +
                            "DepartId='" + depart + "' and VegetableFood=0 group by Date " +
                            "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder " +
                            "where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate
                            .Value.ToString("yyyy-MM-dd") + "' and OrderStatus in (1,2,3) and DepartId='" + depart +
                            "' and VegetableFood=1 group by Date " +
                            "select * from #TT full join #BB on #TT.Date=#BB.Date";
                        SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                        DataSet Read = new DataSet();
                        Load.Fill(Read, "A");
                        OpensqlConME.Close();
                        if (Read.Tables["A"].Rows.Count > 0)
                        {
                            dgvReferOrderAll.DataSource = Read.Tables["A"];
                            dgvReferOrderAll.Columns[0].HeaderText = "日期";
                            dgvReferOrderAll.Columns[1].HeaderText = "葷";
                            dgvReferOrderAll.Columns[2].HeaderText = "日期";
                            dgvReferOrderAll.Columns[3].HeaderText = "素";
                            dgvReferOrderAll.Columns[0].Width = 80;
                            dgvReferOrderAll.Columns[1].Width = 50;
                            dgvReferOrderAll.Columns[2].Width = 80;
                            dgvReferOrderAll.Columns[3].Width = 50;
                            txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                            txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                            int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                            int TotalOrder0 = 0;
                            int TotalOrder1 = 0;
                            if (txtTotalOrder0.Text != "")
                            {
                                TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                            }
                            if (txtTotalOrder1.Text != "")
                            {
                                TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                            }
                            int TotalPrice0 = TotalOrder0 * BentoPrice;
                            int TotalPrice1 = TotalOrder1 * BentoPrice;
                            int TotalPriceAll = TotalPrice0 + TotalPrice1;
                            string aa = Convert.ToString(TotalPrice0);
                            string bb = Convert.ToString(TotalPrice1);
                            string cc = Convert.ToString(TotalPriceAll);
                            txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                            txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                            txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                        }
                        else
                        {
                            MessageBox.Show("您查詢的日期區間，查無部門[" + cboSelectDepart.Text.Trim() + "]的訂餐紀錄！", "訊息", 
                                MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                        }
                    }
                }
            }
        }

        private void btnOrderStatistics_Click(object sender, EventArgs e)
        {
            if (rdoReferAm.Checked == false & rdoReferPm.Checked == false & rdoReferNight.Checked == false)
            {
                MessageBox.Show("尚未選擇要查詢的餐別！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int a = 0;
                int ew0 = 0;
                int ew1 = 0;
                int bl0 = 0;
                int bl1 = 0;
                string o = null;
                if (rdoReferAm.Checked == true)
                {
                    a = 1;
                    o = "中餐";
                }
                else if (rdoReferPm.Checked == true)
                {
                    a = 2;
                    o = "晚餐";
                }
                else if (rdoReferNight.Checked == true)
                {
                    a = 3;
                    o = "宵夜";
                }
                SqlComm.CommandText = "select * from BentoOrder where Date between '" +
                    DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") +
                    "' and OrderStatus=" + a + " and VegetableFood=0 and DepartId not in ('EL')";
                SqlDataAdapter Statistics = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                DataSet All = new DataSet();
                Statistics.Fill(All, "ew0");
                OpensqlConME.Close();
                SqlComm.CommandText = "select * from BentoOrder where Date between '" +
                    DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") +
                    "' and OrderStatus=" + a + " and VegetableFood=1 and DepartId not in ('EL')";
                Statistics = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                OpensqlConME.Close();
                Statistics.Fill(All, "ew1");
                OpensqlConME.Close();
                SqlComm.CommandText = "select * from BentoOrder where Date between '" +
                    DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") +
                    "' and OrderStatus=" + a + " and VegetableFood=0 and DepartId='EL'";
                Statistics = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                Statistics.Fill(All, "bl0");
                OpensqlConME.Close();
                SqlComm.CommandText = "select * from BentoOrder where Date between '" +
                    DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") +
                    "' and OrderStatus=" + a + " and VegetableFood=1 and DepartId='EL'";
                Statistics = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                Statistics.Fill(All, "bl1");
                ew0 = All.Tables["ew0"].Rows.Count;
                ew1 = All.Tables["ew1"].Rows.Count;
                bl0 = All.Tables["bl0"].Rows.Count;
                bl1 = All.Tables["bl1"].Rows.Count;
                MessageBox.Show("今日" + o + "的總數量如下：" + Environment.NewLine + "一廠 葷" + ew0 + "個、素" + ew1 + "個！" +
                    Environment.NewLine + "二廠 葷" + bl0 + "個、素" + bl1 + "個！", "訊息", MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
            }
        }

        private void btnChangePrice_Click(object sender, EventArgs e)
        {
            btnSavePrice.Enabled = true;
            txtBentoPrice.ReadOnly = false;
        }

        private void btnSavePrice_Click(object sender, EventArgs e)
        {
            SqlComm.CommandText = "update BentoCompany set BentoPrice='" + txtBentoPrice.Text + "'";
            SqlComm.Connection = OpensqlConME;
            OpensqlConME.Open();
            if(SqlComm.ExecuteNonQuery()==1)
            {
                MessageBox.Show("保存成功！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                OpensqlConME.Close();
                btnSavePrice.Enabled = false;
                txtBentoPrice.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("保存不成功！" + Environment.NewLine + "請聯絡MIS。", "注意", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                OpensqlConME.Close();
                btnSavePrice.Enabled = false;
                txtBentoPrice.ReadOnly = true;
            }
            
        }

        private void btnCancelOrderM_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("進行此取消動作前，請先確認是否已向團膳廠商取消數量？", "注意", MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                int num = 0;
                string order = null;
                if (rdoReferAm.Checked == true)
                {
                    num = 1;
                    order = "中餐";
                }
                else if (rdoReferPm.Checked == true)
                {
                    num = 2;
                    order = "晚餐";
                }
                else if (rdoReferNight.Checked == true)
                {
                    num = 3;
                    order = "宵夜";
                }
                if (dgvBentoDataShow.Rows.Count == 1 | dgvBentoDataShow.DataSource == null | dgvBentoDataShow.ColumnCount == 1)
                {
                    MessageBox.Show("報餐查詢資料為空白！" + Environment.NewLine + "請先進行查詢！", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                else
                {
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                    cboSelectDepartid.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read1 = new DataSet();
                    Load.Fill(Read1, "DepartId");
                    SqlComm.CommandText = "update BentoOrder set OrderStatus=0,UpdateDate='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                        "',UpdatePeople='" + lblUserNameShow.Text + "' where Date between '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") +
                        "' and '" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "' and EmpName='" + dgvBentoDataShow.CurrentRow.Cells["姓名"].Value.ToString() +
                        "' and OrderStatus=" + num + " and DepartId='" + Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "'";
                    if (MessageBox.Show("您確定要取消【" + dgvBentoDataShow.CurrentRow.Cells[0].Value.ToString() + "】" + order + "訂餐？",
                        "確認訊息", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        OpensqlConME.Open();
                        SqlComm.Connection = OpensqlConME;
                        int i = SqlComm.ExecuteNonQuery();
                        if (i >= 1 & MessageBox.Show("已取消成功！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                            == DialogResult.OK)
                        {
                            OpensqlConME.Close();
                            OpensqlConME.Open();
                            SqlComm.CommandText = "select EmpName,OrderStatus,VegetableFood,OrderPeople from BentoOrder " +
                                "where DepartId='" + Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and OrderStatus=" + num + " and " +
                                "Date between '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and '" +
                                DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "' Order by EmpName";
                            SqlDataAdapter Read = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                            DataSet Data = new DataSet();
                            Read.Fill(Data, "Data");
                            int a = Data.Tables["Data"].Rows.Count;
                            int b;
                            int c = 0;
                            int d;
                            int dc;
                            Data.Tables["Data"].Columns.Add("餐別", typeof(String));
                            Data.Tables["Data"].Columns.Add("素食", typeof(String));
                            Data.Tables["Data"].Columns[0].ColumnName = "姓名";
                            Data.Tables["Data"].Columns[3].ColumnName = "報餐人員";
                            for (b = 0; b < a; b++)
                            {
                                if (Data.Tables["Data"].Rows[b][1].ToString() == "1")
                                {
                                    Data.Tables["Data"].Rows[b]["餐別"] = "中餐";
                                }
                                else if (Data.Tables["Data"].Rows[b][1].ToString() == "2")
                                {
                                    Data.Tables["Data"].Rows[b]["餐別"] = "晚餐";
                                }
                                else if (Data.Tables["Data"].Rows[b][1].ToString() == "3")
                                {
                                    Data.Tables["Data"].Rows[b]["餐別"] = "宵夜";
                                }
                            }
                            for (b = 0; b < a; b++)
                            {
                                if (Data.Tables["Data"].Rows[b][2].ToString() == "1")
                                {
                                    Data.Tables["Data"].Rows[b]["素食"] = "Yes";
                                }
                            }
                            for (b = 0; b < a; b++)
                            {
                                if (Data.Tables["Data"].Rows[b]["素食"].ToString() == "Yes")
                                {
                                    c++;
                                }
                            }
                            d = Data.Tables["Data"].Rows.Count;
                            dc = d - c;
                            lblOrderNumShow.Text = "葷 " + dc + "個、" + "素 " + c + "個、" + " 共 " + d + "個";
                            dgvBentoDataShow.DataSource = Data.Tables["Data"];
                            //隱藏欄位
                            dgvBentoDataShow.Columns[1].Visible = false;
                            dgvBentoDataShow.Columns[2].Visible = false;
                            //調整欄位位置
                            dgvBentoDataShow.Columns[4].DisplayIndex = 1;
                            dgvBentoDataShow.Columns[5].DisplayIndex = 2;
                            //dgvBentoDataShow.Columns[0-5].
                            OpensqlConME.Close();
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void btnReferOrderAm_Click(object sender, EventArgs e)
        {
            if (rdOneCompany.Checked == false & rdoTwoCompany.Checked == false & rdoAllCompany.Checked == false & 
                rdoSelectDepart.Checked==false)
            {
                MessageBox.Show("尚未選擇要查詢的條件！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (rdOneCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from "+
                        "BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + 
                        dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus=1 and DepartId not in ('EL') and "+
                        "VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where "+
                        "Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.
                        ToString("yyyy-MM-dd") + "' and OrderStatus=1 and DepartId not in ('EL') and VegetableFood=1 "+
                        "group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        //下述開始計算數量與金額並把結果傳回TextBox
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        //判斷負責金額的TextBox是否為null，避免後續計算金額時，發生錯誤
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        //下述先將int轉型為string，再將建立的string變數指定給TextBox並再轉成指定的貨幤格式
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[一廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if (rdoTwoCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from " +
                        "BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" +
                        dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus=1 and DepartId='EL' and " +
                        "VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where " +
                        "Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.
                        ToString("yyyy-MM-dd") + "' and OrderStatus=1 and DepartId='EL' and VegetableFood=1 group by " +
                        "Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[二廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if(rdoAllCompany.Checked==true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from " +
                        "BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" +
                        dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus=1 and VegetableFood=0 group by " +
                        "Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where " +
                        "Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.
                        ToString("yyyy-MM-dd") + "' and OrderStatus=1 and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[全廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    if (cboSelectDepart.SelectedIndex == 0)
                    {
                        MessageBox.Show("尚未選擇要查詢的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                            cboSelectDepart.Text + "'";
                        SqlDataAdapter Load1 = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet Read1 = new DataSet();
                        Load1.Fill(Read1, "DepartId");
                        string depart = Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString();
                        SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT " +
                            "from BentoOrder where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") +
                            "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus=1 and DepartId='" +
                            depart + "' and VegetableFood=0 group by Date " +
                            "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder " +
                            "where Date between '" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" +
                            dtpEndDate.Value.ToString("yyyy-MM-dd") + "' and OrderStatus=1 and DepartId='" + depart +
                            "' and VegetableFood=1 group by Date " +
                            "select * from #TT full join #BB on #TT.Date=#BB.Date";
                        SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                        DataSet Read = new DataSet();
                        Load.Fill(Read, "A");
                        OpensqlConME.Close();
                        if (Read.Tables["A"].Rows.Count > 0)
                        {
                            dgvReferOrderAll.DataSource = Read.Tables["A"];
                            dgvReferOrderAll.Columns[0].HeaderText = "日期";
                            dgvReferOrderAll.Columns[1].HeaderText = "葷";
                            dgvReferOrderAll.Columns[2].HeaderText = "日期";
                            dgvReferOrderAll.Columns[3].HeaderText = "素";
                            dgvReferOrderAll.Columns[0].Width = 80;
                            dgvReferOrderAll.Columns[1].Width = 50;
                            dgvReferOrderAll.Columns[2].Width = 80;
                            dgvReferOrderAll.Columns[3].Width = 50;
                            txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                            txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                            int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                            int TotalOrder0 = 0;
                            int TotalOrder1 = 0;
                            if (txtTotalOrder0.Text != "")
                            {
                                TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                            }
                            if (txtTotalOrder1.Text != "")
                            {
                                TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                            }
                            int TotalPrice0 = TotalOrder0 * BentoPrice;
                            int TotalPrice1 = TotalOrder1 * BentoPrice;
                            int TotalPriceAll = TotalPrice0 + TotalPrice1;
                            string aa = Convert.ToString(TotalPrice0);
                            string bb = Convert.ToString(TotalPrice1);
                            string cc = Convert.ToString(TotalPriceAll);
                            txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                            txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                            txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                        }
                        else
                        {
                            MessageBox.Show("您查詢的日期區間，查無部門[" + cboSelectDepart.Text.Trim() + "]的訂餐紀錄！", "訊息",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                }
            }
        }

        private void btnReferOrderPm_Click(object sender, EventArgs e)
        {
            if (rdOneCompany.Checked == false & rdoTwoCompany.Checked == false & rdoAllCompany.Checked == false & 
                rdoSelectDepart.Checked==false)
            {
                MessageBox.Show("尚未選擇要查詢的條件！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (rdOneCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=2 and DepartId not in ('EL') and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=2 and DepartId not in ('EL') and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        //下述開始計算數量與金額並把結果傳回TextBox
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        //判斷負責金額的TextBox是否為null，避免後續計算金額時，發生錯誤
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        //下述先將int轉型為string，再將建立的string變數指定給TextBox並再轉成指定的貨幤格式
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[一廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if (rdoTwoCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=2 and DepartId='EL' and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=2 and DepartId='EL' and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[二廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if(rdoAllCompany.Checked==true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=2 and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=2 and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[全廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    if (cboSelectDepart.SelectedIndex == 0)
                    {
                        MessageBox.Show("尚未選擇要查詢的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                            cboSelectDepart.Text + "'";
                        SqlDataAdapter Load1 = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet Read1 = new DataSet();
                        Load1.Fill(Read1, "DepartId");
                        string depart = Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString();
                        SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                            dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                            "' and OrderStatus=2 and DepartId='" + depart + "' and VegetableFood=0 group by Date " +
                            "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                            dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                            "' and OrderStatus=2 and DepartId='" + depart + "' and VegetableFood=1 group by Date " +
                            "select * from #TT full join #BB on #TT.Date=#BB.Date";
                        SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                        DataSet Read = new DataSet();
                        Load.Fill(Read, "A");
                        OpensqlConME.Close();
                        if (Read.Tables["A"].Rows.Count > 0)
                        {
                            dgvReferOrderAll.DataSource = Read.Tables["A"];
                            dgvReferOrderAll.Columns[0].HeaderText = "日期";
                            dgvReferOrderAll.Columns[1].HeaderText = "葷";
                            dgvReferOrderAll.Columns[2].HeaderText = "日期";
                            dgvReferOrderAll.Columns[3].HeaderText = "素";
                            dgvReferOrderAll.Columns[0].Width = 80;
                            dgvReferOrderAll.Columns[1].Width = 50;
                            dgvReferOrderAll.Columns[2].Width = 80;
                            dgvReferOrderAll.Columns[3].Width = 50;
                            txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                            txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                            int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                            int TotalOrder0 = 0;
                            int TotalOrder1 = 0;
                            if (txtTotalOrder0.Text != "")
                            {
                                TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                            }
                            if (txtTotalOrder1.Text != "")
                            {
                                TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                            }
                            int TotalPrice0 = TotalOrder0 * BentoPrice;
                            int TotalPrice1 = TotalOrder1 * BentoPrice;
                            int TotalPriceAll = TotalPrice0 + TotalPrice1;
                            string aa = Convert.ToString(TotalPrice0);
                            string bb = Convert.ToString(TotalPrice1);
                            string cc = Convert.ToString(TotalPriceAll);
                            txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                            txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                            txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                        }
                        else
                        {
                            MessageBox.Show("您查詢的日期區間，查無部門[" + cboSelectDepart.Text.Trim() + "]的訂餐紀錄！", "訊息",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                }
            }
        }

        private void btnReferOrderNight_Click(object sender, EventArgs e)
        {
            if (rdOneCompany.Checked == false & rdoTwoCompany.Checked == false & rdoAllCompany.Checked == false & 
                rdoSelectDepart.Checked==false)
            {
                MessageBox.Show("未選擇要查詢的公司別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (rdOneCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=3 and DepartId not in ('EL') and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=3 and DepartId not in ('EL') and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        //下述開始計算數量與金額並把結果傳回TextBox
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        //判斷負責金額的TextBox是否為null，避免後續計算金額時，發生錯誤
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        //下述先將int轉型為string，再將建立的string變數指定給TextBox並再轉成指定的貨幤格式
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[一廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if (rdoTwoCompany.Checked == true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=3 and DepartId='EL' and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=3 and DepartId='EL' and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[二廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else if(rdoAllCompany.Checked==true)
                {
                    SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=3 and VegetableFood=0 group by Date " +
                        "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                        dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                        "' and OrderStatus=3 and VegetableFood=1 group by Date " +
                        "select * from #TT full join #BB on #TT.Date=#BB.Date";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "A");
                    OpensqlConME.Close();
                    if (Read.Tables["A"].Rows.Count > 0)
                    {
                        dgvReferOrderAll.DataSource = Read.Tables["A"];
                        dgvReferOrderAll.Columns[0].HeaderText = "日期";
                        dgvReferOrderAll.Columns[1].HeaderText = "葷";
                        dgvReferOrderAll.Columns[2].HeaderText = "日期";
                        dgvReferOrderAll.Columns[3].HeaderText = "素";
                        dgvReferOrderAll.Columns[0].Width = 80;
                        dgvReferOrderAll.Columns[1].Width = 50;
                        dgvReferOrderAll.Columns[2].Width = 80;
                        dgvReferOrderAll.Columns[3].Width = 50;
                        txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                        txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                        int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                        int TotalOrder0 = 0;
                        int TotalOrder1 = 0;
                        if (txtTotalOrder0.Text != "")
                        {
                            TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                        }
                        if (txtTotalOrder1.Text != "")
                        {
                            TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                        }
                        int TotalPrice0 = TotalOrder0 * BentoPrice;
                        int TotalPrice1 = TotalOrder1 * BentoPrice;
                        int TotalPriceAll = TotalPrice0 + TotalPrice1;
                        string aa = Convert.ToString(TotalPrice0);
                        string bb = Convert.ToString(TotalPrice1);
                        string cc = Convert.ToString(TotalPriceAll);
                        txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                        txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                        txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                    }
                    else
                    {
                        MessageBox.Show("您查詢的日期區間，查無[全廠]的訂餐紀錄！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    if (cboSelectDepart.SelectedIndex == 0)
                    {
                        MessageBox.Show("尚未選擇要查詢的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                            cboSelectDepart.Text + "'";
                        SqlDataAdapter Load1 = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet Read1 = new DataSet();
                        Load1.Fill(Read1, "DepartId");
                        string depart = Read1.Tables["DepartId"].Rows[0]["DepartId"].ToString();
                        SqlComm.CommandText = "select Convert(char(10),Date,20) as Date,count(Date) as num into #TT from BentoOrder where Date between '" +
                            dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                            "' and OrderStatus=3 and DepartId='" + depart + "' and VegetableFood=0 group by Date " +
                            "select Convert(char(10),Date,20) as Date,count(Date) as num1 into #BB from BentoOrder where Date between '" +
                            dtpStartDate.Value.ToString("yyyy-MM-dd") + "' and '" + dtpEndDate.Value.ToString("yyyy-MM-dd") +
                            "' and OrderStatus=3 and DepartId='" + depart + "' and VegetableFood=1 group by Date " +
                            "select * from #TT full join #BB on #TT.Date=#BB.Date";
                        SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                        DataSet Read = new DataSet();
                        Load.Fill(Read, "A");
                        OpensqlConME.Close();
                        if (Read.Tables["A"].Rows.Count > 0)
                        {
                            dgvReferOrderAll.DataSource = Read.Tables["A"];
                            dgvReferOrderAll.Columns[0].HeaderText = "日期";
                            dgvReferOrderAll.Columns[1].HeaderText = "葷";
                            dgvReferOrderAll.Columns[2].HeaderText = "日期";
                            dgvReferOrderAll.Columns[3].HeaderText = "素";
                            dgvReferOrderAll.Columns[0].Width = 80;
                            dgvReferOrderAll.Columns[1].Width = 50;
                            dgvReferOrderAll.Columns[2].Width = 80;
                            dgvReferOrderAll.Columns[3].Width = 50;
                            txtTotalOrder0.Text = Read.Tables["A"].Compute("SUM(num)", "").ToString();
                            txtTotalOrder1.Text = Read.Tables["A"].Compute("SUM(num1)", "").ToString();
                            int BentoPrice = Convert.ToInt32(txtBentoPrice.Text);
                            int TotalOrder0 = 0;
                            int TotalOrder1 = 0;
                            if (txtTotalOrder0.Text != "")
                            {
                                TotalOrder0 = Convert.ToInt32(txtTotalOrder0.Text);
                            }
                            if (txtTotalOrder1.Text != "")
                            {
                                TotalOrder1 = Convert.ToInt32(txtTotalOrder1.Text);
                            }
                            int TotalPrice0 = TotalOrder0 * BentoPrice;
                            int TotalPrice1 = TotalOrder1 * BentoPrice;
                            int TotalPriceAll = TotalPrice0 + TotalPrice1;
                            string aa = Convert.ToString(TotalPrice0);
                            string bb = Convert.ToString(TotalPrice1);
                            string cc = Convert.ToString(TotalPriceAll);
                            txtTotalPrice0.Text = decimal.Parse(aa).ToString("C0").Substring(3);
                            txtTotalPrice1.Text = decimal.Parse(bb).ToString("C0").Substring(3);
                            txtTotalPriceAll.Text = decimal.Parse(cc).ToString("C0").Substring(3);
                        }
                        else
                        {
                            MessageBox.Show("您查詢的日期區間，查無部門[" + cboSelectDepart.Text.Trim() + "]的訂餐紀錄！", "訊息",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                }
            }
        }

        private void btnSendM_Click(object sender, EventArgs e)
        {
            if (rdoAmOrder.Checked == false & rdoPmOrder.Checked == false & rdoNightOrder.Checked == false)
            {
                MessageBox.Show("尚未選擇要報餐的餐別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (cboDepart.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇要報餐的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                string num1 = null;
                string num2 = null;
                if (chkVegetableFood.Checked == true)
                {
                    num2 = "1";
                }
                else
                {
                    num2 = "0";
                }
                if (rdoAmOrder.Checked == true)
                {
                    num1 = "1";
                }
                else if (rdoPmOrder.Checked == true)
                {
                    num1 = "2";
                }
                else if (rdoNightOrder.Checked == true)
                {
                    num1 = "3";
                }
                SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" + cboDepart.Text + "'";
                SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet Read = new DataSet();
                Load.Fill(Read, "DepartId");
                //先檢查今日該部門人員是否已報過餐別
                int a = chklstName.CheckedItems.Count;
                int q;
                string name = null;
                SqlDataReader check;
                for (q = 0; q < a; q++)
                {
                    OpensqlConME.Close();
                    name = chklstName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                    SqlComm.CommandText = "select * from BentoOrder where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and OrderStatus=" + num1.ToString() + " and (VegetableFood in (0,1))";
                    SqlComm.Connection = OpensqlConME;
                    OpensqlConME.Open();
                    check = SqlComm.ExecuteReader();
                    if (check.HasRows)
                    {
                        MessageBox.Show("該人員[" + name + "]今日已報過您選擇的餐別！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        OpensqlConME.Close();
                        return;
                    }

                }
                if (rdoAmOrder.Checked == true)
                {
                    OpensqlConME.Close();
                    OpensqlConME.Open();
                    int A = chklstName.CheckedItems.Count;
                    for (int i = 0; i < A; i++)
                    {
                        SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                            chklstName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                        SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet ReadUser = new DataSet();
                        ReadNI.Fill(ReadUser, "ReadUser");
                        SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][0].ToString() + "','" + ReadUser.Tables["ReadUser"].Rows[0][1].ToString() +
                            "','" + Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "',1," + num2.ToString() + ",'" + lblUserNameShow.Text.ToString() +
                            "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                    }
                    MessageBox.Show("部門：" + cboDepart.Text.ToString() + Environment.NewLine + "今日午餐報餐數量共 " +
                        chklstName.CheckedItems.Count + "個！" + Environment.NewLine + "已報餐完成！", "訊息",
                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    OpenSqlCon.Close();
                    OpensqlConME.Close();
                }
                else if (rdoPmOrder.Checked == true)
                {
                    OpensqlConME.Close();
                    OpensqlConME.Open();
                    int A = chklstName.CheckedItems.Count;
                    for (int i = 0; i < A; i++)
                    {
                        SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                            chklstName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                        SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet ReadUser = new DataSet();
                        ReadNI.Fill(ReadUser, "ReadUser");
                        SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][0].ToString() + "','" + ReadUser.Tables["ReadUser"].Rows[0][1].ToString() +
                            "','" + Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "',2," + num2.ToString() + ",'" + lblUserNameShow.Text.ToString() +
                            "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                    }
                    MessageBox.Show("部門：" + cboDepart.Text.ToString() + Environment.NewLine + "今日晚餐報餐數量共 " +
                        chklstName.CheckedItems.Count + "個！" + Environment.NewLine + "已報餐完成！", "訊息",
                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    OpenSqlCon.Close();
                    OpensqlConME.Close();
                }
                else if (rdoNightOrder.Checked == true)
                {
                    OpensqlConME.Close();
                    OpensqlConME.Open();
                    int A = chklstName.CheckedItems.Count;
                    for (int i = 0; i < A; i++)
                    {
                        SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                            chklstName.CheckedItems[i].ToString() + "'";
                        SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet ReadUser = new DataSet();
                        ReadNI.Fill(ReadUser, "ReadUser");
                        SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][0].ToString() + "','" + ReadUser.Tables["ReadUser"].Rows[0][1].ToString() +
                            "','" + Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "',3," + num2.ToString() + ",'" + lblUserNameShow.Text.ToString() +
                            "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                    }
                    MessageBox.Show("部門：" + cboDepart.Text.ToString() + Environment.NewLine + "今日宵夜報餐數量共 " +
                        chklstName.CheckedItems.Count + "個！" + Environment.NewLine + "已報餐完成！", "訊息",
                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    OpenSqlCon.Close();
                    OpensqlConME.Close();
                }
            }
        }

        private void btnSendToExcel_Click(object sender, EventArgs e)
        {
            Excel.Application excel = null;
            try
            {
                //DataGridView沒有資料就不執行
                if (this.dgvReferOrderAll.Rows.Count <= 1)
                {
                    MessageBox.Show("沒有可滙出的資料！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                string Date = DateTime.Now.ToString("yyyy-MM-dd");
                //設定滙出後的存檔路徑(儲存在桌面)
                string SaveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) +
                    @"\BentoReport_" + Date + ".xls";
                //new 出一個Excel
                excel = new Microsoft.Office.Interop.Excel.Application();
                //看的到Excel在工作
                excel.Visible = false;
                //新增加一工作簿
                excel.Application.Workbooks.Add(true);

                PGB pgb = new PGB();
                pgb.progressBar1.Minimum = 0;
                pgb.progressBar1.Maximum = dgvReferOrderAll.Rows.Count - 1;
                pgb.progressBar1.Step = 1;
                pgb.progressBar1.Value = 0;
                pgb.progressBar1.ForeColor = Color.Pink;
                pgb.progressBar1.Style = ProgressBarStyle.Continuous;
                pgb.Show();

                //寫入欄位名稱
                for (int i = 0; i < dgvReferOrderAll.Columns.Count; i++)
                {
                    excel.Cells[1, i + 1] = dgvReferOrderAll.Columns[i].HeaderText;
                }
                //把DataGridView資料寫到Excel
                for (int i = 0; i < dgvReferOrderAll.Rows.Count - 1; i++)
                {
                    pgb.progressBar1.Value++;
                    for (int j = 0; j < dgvReferOrderAll.Columns.Count; j++)
                    {
                        if (dgvReferOrderAll[j, i].ValueType == typeof(string))
                        {
                            excel.Cells[i + 2, j + 1] = "'" + dgvReferOrderAll[j, i].Value.ToString();
                        }
                        else
                        {
                            excel.Cells[i + 2, j + 1] = dgvReferOrderAll[j, i].Value.ToString();
                        }
                    }
                    //設定欄位靠右
                    excel.get_Range("A" + (i + 2)).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.
                        xlHAlignRight;
                    excel.get_Range("C" + (i + 2)).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.
                        xlHAlignRight;
                    //設定欄位顏色
                    excel.get_Range("A" + (i + 2)).Interior.Color = Color.Pink;
                    excel.get_Range("B" + (i + 2)).Interior.Color = Color.Pink;
                    excel.get_Range("C" + (i + 2)).Interior.Color = Color.Pink;
                    excel.get_Range("D" + (i + 2)).Interior.Color = Color.Pink;
                    //設定欄位框線
                    excel.get_Range("A" + (i + 2)).Borders.LineStyle = 1;
                    excel.get_Range("B" + (i + 2)).Borders.LineStyle = 1;
                    excel.get_Range("C" + (i + 2)).Borders.LineStyle = 1;
                    excel.get_Range("D" + (i + 2)).Borders.LineStyle = 1;
                }
                /*
                先將DataGridView的Rows總數給變數aa，以利後續透過aa+1的方式來新增要加入的資料
                (將DataGridView資料轉至Excel後，在Rows下方插入TextBox.Text)
                */
                int aa = dgvReferOrderAll.Rows.Count + 1;
                excel.Cells[aa, 1] = "[葷]數量：";
                excel.Cells[aa, 2] = txtTotalOrder0.Text;
                excel.Cells[aa, 3] = "[素]數量：";
                excel.Cells[aa, 4] = txtTotalOrder1.Text;
                //設定欄位靠右
                excel.get_Range("A" + aa).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                excel.get_Range("C" + aa).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                //設定欄位顏色
                excel.get_Range("A" + aa).Interior.Color = Color.MediumOrchid;
                excel.get_Range("B" + aa).Interior.Color = Color.MediumPurple;
                excel.get_Range("C" + aa).Interior.Color = Color.MediumOrchid;
                excel.get_Range("D" + aa).Interior.Color = Color.MediumPurple;
                //設定欄位字體顏色
                excel.get_Range("A" + aa).Font.Color = Color.Snow;
                excel.get_Range("B" + aa).Font.Color = Color.Snow;
                excel.get_Range("C" + aa).Font.Color = Color.Snow;
                excel.get_Range("D" + aa).Font.Color = Color.Snow;
                //設定欄位字體為粗體
                excel.get_Range("A" + aa).Font.Bold = true;
                excel.get_Range("B" + aa).Font.Bold = true;
                excel.get_Range("C" + aa).Font.Bold = true;
                excel.get_Range("D" + aa).Font.Bold = true;
                aa = aa + 1;
                excel.Cells[aa, 1] = "[葷]金額：";
                excel.Cells[aa, 2] = txtTotalPrice0.Text;
                excel.Cells[aa, 3] = "[素]金額：";
                excel.Cells[aa, 4] = txtTotalPrice1.Text;
                excel.get_Range("A" + aa).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                excel.get_Range("C" + aa).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                excel.get_Range("A" + aa).Interior.Color = Color.MediumOrchid;
                excel.get_Range("B" + aa).Interior.Color = Color.MediumPurple;
                excel.get_Range("C" + aa).Interior.Color = Color.MediumOrchid;
                excel.get_Range("D" + aa).Interior.Color = Color.MediumPurple;
                excel.get_Range("A" + aa).Font.Color = Color.Snow;
                excel.get_Range("B" + aa).Font.Color = Color.Snow;
                excel.get_Range("C" + aa).Font.Color = Color.Snow;
                excel.get_Range("D" + aa).Font.Color = Color.Snow;
                excel.get_Range("A" + aa).Font.Bold = true;
                excel.get_Range("B" + aa).Font.Bold = true;
                excel.get_Range("C" + aa).Font.Bold = true;
                excel.get_Range("D" + aa).Font.Bold = true;
                aa = aa + 1;
                excel.Cells[aa, 3] = "總金額：";
                excel.Cells[aa, 4] = txtTotalPriceAll.Text;
                excel.get_Range("C" + aa).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                excel.get_Range("C" + aa).Interior.Color = Color.MediumOrchid;
                excel.get_Range("D" + aa).Interior.Color = Color.MediumPurple;
                excel.get_Range("C" + aa).Font.Color = Color.Snow;
                excel.get_Range("D" + aa).Font.Color = Color.Snow;
                excel.get_Range("C" + aa).Font.Bold = true;
                excel.get_Range("D" + aa).Font.Bold = true;
                //設定滙出後，欄位寛度自動配合資料調整
                excel.Cells.EntireRow.AutoFit();
                //自動調整列高
                excel.Cells.EntireColumn.AutoFit();
                //將所有欄位做垂直置中
                excel.Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                //將A1~D1的欄位做水平置中
                excel.get_Range("A1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excel.get_Range("B1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excel.get_Range("C1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excel.get_Range("D1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //設定A1~D1欄位框線
                excel.get_Range("A1").Borders.LineStyle = 1;
                excel.get_Range("B1").Borders.LineStyle = 1;
                excel.get_Range("C1").Borders.LineStyle = 1;
                excel.get_Range("D1").Borders.LineStyle = 1;
                excel.get_Range("A1").Font.Color = Color.White;
                excel.get_Range("A1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("B1").Font.Color = Color.White;
                excel.get_Range("B1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("C1").Font.Color = Color.White;
                excel.get_Range("C1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("D1").Font.Color = Color.White;
                excel.get_Range("D1").Interior.Color = Color.DodgerBlue;
                /*
                excel.get_Range("E1").Font.Color = Color.White;
                excel.get_Range("E1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("F1").Font.Color = Color.White;
                excel.get_Range("F1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("G1").Font.Color = Color.White;
                excel.get_Range("G1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("H1").Font.Color = Color.White;
                excel.get_Range("H1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("I1").Font.Color = Color.White;
                excel.get_Range("I1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("J1").Font.Color = Color.White;
                excel.get_Range("J1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("K1").Font.Color = Color.White;
                excel.get_Range("K1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("L1").Font.Color = Color.White;
                excel.get_Range("L1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("M1").Font.Color = Color.White;
                excel.get_Range("M1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("N1").Font.Color = Color.White;
                excel.get_Range("N1").Interior.Color = Color.DodgerBlue;
                excel.get_Range("O1").Font.Color = Color.White;
                excel.get_Range("O1").Interior.Color = Color.DodgerBlue;
                */

                //設置禁止彈出覆蓋或儲存的彈跳視窗
                excel.DisplayAlerts = false;
                excel.AlertBeforeOverwriting = false;
                //將檔案儲存到SaveFile指定的位置
                excel.ActiveWorkbook.SaveCopyAs(SaveFilePath);
                pgb.Close();
                MessageBox.Show("已成功滙出Excel檔！" + Environment.NewLine + "檔案儲存在您電腦的桌面，檔名：BentoReport_" +
                    Date + ".xls", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //關閉工作簿和結束Excel程式
            excel.Workbooks.Close();
            excel.Quit();
            //釋放資源
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
            excel = null;
            GC.Collect();
        }

        //每日訂餐查詢、取消-葷
        private void btnNewPeople0_Click(object sender, EventArgs e)
        {
            if (rdoReferAm.Checked == false & rdoReferPm.Checked == false & rdoReferNight.Checked == false)
            {
                MessageBox.Show("尚未選擇要報餐的餐別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if(cboSelectDepartid.SelectedIndex==0)
            {
                MessageBox.Show("尚未選擇要報餐的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                if(txtInputName.Text=="")
                {
                    MessageBox.Show("未輸入報餐人員姓名！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int num = 0;
                    string Order = "";
                    if(rdoReferAm.Checked==true)
                    {
                        num = 1;
                        Order = "中餐";
                    }
                    else if(rdoReferPm.Checked==true)
                    {
                        num = 2;
                        Order = "晚餐";
                    }
                    else if(rdoReferNight.Checked==true)
                    {
                        num = 3;
                        Order = "宵夜";
                    }
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" + 
                        cboSelectDepartid.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate)" + " values ('" + DateTime.Now.ToString
                            ("yyyy-MM-dd HH:mm:ss") + "','EW000'," + "'" + txtInputName.Text + "','" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "','" + num.ToString() +
                            "',0,'" + lblUserNameShow.Text.ToString() + "','" + DateTime.Now.ToString
                            ("yyyy-MM-dd HH:mm:ss") + "')";
                    SqlComm.Connection = OpensqlConME;
                    OpensqlConME.Open();
                    int Reply=SqlComm.ExecuteNonQuery();
                    if(Reply==1)
                    {
                        MessageBox.Show("新進人員［" + txtInputName.Text + "］" + Order + "（葷）已送出！", "訊息",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpensqlConME.Close();
                    }
                }
            }
        }

        //每日訂餐查詢、取消-素
        private void btnNewPeople1_Click(object sender, EventArgs e)
        {
            if (rdoReferAm.Checked == false & rdoReferPm.Checked == false & rdoReferNight.Checked == false)
            {
                MessageBox.Show("尚未選擇要報餐的餐別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (cboSelectDepartid.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇要報餐的部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                if (txtInputName.Text == "")
                {
                    MessageBox.Show("未輸入報餐人員姓名！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int num = 0;
                    string Order = "";
                    if (rdoReferAm.Checked == true)
                    {
                        num = 1;
                        Order = "中餐";
                    }
                    else if (rdoReferPm.Checked == true)
                    {
                        num = 2;
                        Order = "晚餐";
                    }
                    else if (rdoReferNight.Checked == true)
                    {
                        num = 3;
                        Order = "宵夜";
                    }
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" + 
                        cboSelectDepartid.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate)" + " values ('" + DateTime.Now.ToString
                            ("yyyy-MM-dd HH:mm:ss") + "','EW000'," + "'" + txtInputName.Text + "','" + 
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim()+ "','" + num.ToString() +
                            "',1,'" + lblUserNameShow.Text.ToString() + "','" + DateTime.Now.ToString
                            ("yyyy-MM-dd HH:mm:ss") + "')";
                    SqlComm.Connection = OpensqlConME;
                    OpensqlConME.Open();
                    int Reply = SqlComm.ExecuteNonQuery();
                    if (Reply == 1)
                    {
                        MessageBox.Show("新進人員［" + txtInputName.Text + "］" + Order + "（素）已送出！", "訊息",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpensqlConME.Close();
                    }
                }
            }
        }

        //每日訂餐查詢、取消-各課報餐統計
        private void btnDepartOrder_Click(object sender, EventArgs e)
        {
            if (rdoReferAm.Checked == false & rdoReferPm.Checked == false & rdoReferNight.Checked == false)
            {
                MessageBox.Show("尚未選擇要查詢的餐別！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int num = 0;
                if (rdoReferAm.Checked == true)
                {
                    num = 1;
                }
                else if (rdoReferPm.Checked == true)
                {
                    num = 2;
                }
                else if (rdoReferNight.Checked == true)
                {
                    num = 3;
                }
                //統計各部門的(葷、素)報餐數量
                SqlComm.CommandText = "select DepartId as '部門',SUM(CASE OrderStatus WHEN '" + num + 
                    "' THEN 1 ELSE 0 END) AS '葷' into #AA from BentoOrder where DepartId in "+
                    "('EG','ES','EM','EA','EE','ER','EQ','MM','EL','EP','ET','EI','FF','LF','DF','CF') "+
                    "and VegetableFood = 0 and Date between '"+DateTime.Now.ToString("yyyy-MM-dd")+"' and "+
                    "'"+DateTime.Now.ToString("yyyy-MM-dd")+"' group by DepartId " +
                    "select DepartId as '部門',SUM(CASE OrderStatus WHEN '" + num + 
                    "' THEN 1 ELSE 0 END) AS '素' into #BB from BentoOrder where DepartId in " +
                    "('EG','ES','EM','EA','EE','ER','EQ','MM','EL','EP','ET','EI','FF','LF','DF','CF') " +
                    "and VegetableFood = 1 and Date between '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and " +
                    "'" + DateTime.Now.ToString("yyyy-MM-dd") + "' group by DepartId " +
                    "select * from #AA left join #BB on #AA.部門=#BB.部門";
                SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpensqlConME);
                DataSet Read = new DataSet();
                Load.Fill(Read, "A");
                OpensqlConME.Close();
                
                //用迴圈下去跑字串比對，將符合條件的欄位部門ID值轉成中文部門別
                for (int i=0;i<Read.Tables["A"].Rows.Count;i++)
                {
                    for (int q = 0; q < departid.Count(); q++)
                    {
                        if (Read.Tables["A"].Rows[i]["部門"].ToString().Trim() == departid[q])
                        {
                            Read.Tables["A"].Rows[i]["部門"] = depart[q];
                        }
                    }
                }
                dgvBentoDataShow.Columns.Clear();
                dgvBentoDataShow.DataSource = Read.Tables["A"];
                dgvBentoDataShow.Columns[2].Visible = false;
            }
        }

        private void cboSelectWorkPeopleDepart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSelectWorkPeopleDepart.SelectedIndex == 0)
            {
                return;
            }
            else
            {
                //先用選到的部門中文去撈部門代號
                SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                    cboSelectWorkPeopleDepart.Text + "'";
                SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet Read = new DataSet();
                Load.Fill(Read, "DepartId");
                //再用部門代號撈出仍在職中的人員工號、姓名
                SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where DepartId='" +
                    Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpStatus='1'";
                SqlDataAdapter ReadEmpInfo = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                DataSet HPSdEmpInfo = new DataSet();
                ReadEmpInfo.Fill(HPSdEmpInfo, "EmpInfo");
                int A = HPSdEmpInfo.Tables["EmpInfo"].Rows.Count;
                chklstWorkPeopleName.Items.Clear();
                for (int i = 0; i < A; i++)
                {
                    chklstWorkPeopleName.Items.Add(HPSdEmpInfo.Tables["EmpInfo"].Rows[i][0].ToString().Trim() +
                        HPSdEmpInfo.Tables["EmpInfo"].Rows[i][1].ToString().Trim());
                }
                OpenSqlCon.Close();
                RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
            }
        }

        private void btnSelectWorkPeopleAll_Click(object sender, EventArgs e)
        {
            int A = chklstWorkPeopleName.Items.Count;
            for (int i = 0; i < A; i++)
            {
                chklstWorkPeopleName.SetItemChecked(i, true);
            }
        }

        private void btnCancelWorkPeopleAll_Click(object sender, EventArgs e)
        {
            int A = chklstWorkPeopleName.Items.Count;
            for (int i = 0; i < A; i++)
            {
                chklstWorkPeopleName.SetItemChecked(i, false);
            }
        }

        //每日出勤登記-正常=null
        private void btnWorkPeopleSend_Click(object sender, EventArgs e)
        {
            if(rdoClassAM.Checked==false & rdoClassPM.Checked==false)
            {
                MessageBox.Show("尚未選擇班別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else if(cboSelectWorkPeopleDepart.SelectedIndex==0)
            {
                MessageBox.Show("尚未選擇部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else
            {
                DateTime time = new DateTime();
                time = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                if (time > t1 | time > t2)
                {
                    MessageBox.Show("已超過每日出勤登記時間！" + Environment.NewLine + "請洽詢管理部-人資專員。", "錯誤",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int Classnum = 0;
                    string Class = "";
                    if(rdoClassAM.Checked==true)
                    {
                        Classnum = 1;
                        Class = "早班";
                    }
                    else if(rdoClassPM.Checked==true)
                    {
                        Classnum = 2;
                        Class = "晚班";
                    }
                    //先撈出選定部門的部門ID
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                        cboSelectWorkPeopleDepart.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    //先檢查今日該部門人員是否已報過出勤
                    int a = chklstWorkPeopleName.CheckedItems.Count;
                    int q;
                    string name = null;
                    SqlDataReader check;
                    for (q = 0; q < a; q++)
                    {
                        OpensqlConME.Close();
                        name = chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                        SqlComm.CommandText = "select * from WorkPeople where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and Class in (1,2) and (Status is null or Status in (0,1,2,3,4,5,6,7,8))";
                        SqlComm.Connection = OpensqlConME;
                        OpensqlConME.Open();
                        check = SqlComm.ExecuteReader();
                        if (check.HasRows)
                        {
                            MessageBox.Show("該人員[" + name + "]今日已報過出勤！", "注意", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);
                            OpensqlConME.Close();
                            return;
                        }
                    }
                    OpensqlConME.Close();
                    OpensqlConME.Open();
                    int A = chklstWorkPeopleName.CheckedItems.Count;
                    for (int i = 0; i < A; i++)
                    {
                        SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                            chklstWorkPeopleName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                        SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet ReadUser = new DataSet();
                        ReadNI.Fill(ReadUser, "ReadUser");
                        SqlComm.CommandText = "insert into WorkPeople (Date,EmpId,EmpName,DepartId,Class," +
                            "RegisterPeople,RegisterDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][0].ToString().Trim() + "','" +
                            ReadUser.Tables["ReadUser"].Rows[0][1].ToString().Trim() + "','" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "','" +
                            Classnum + "','" + lblUserNameShow.Text.ToString() + "','" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                    }
                    MessageBox.Show("部門：" + cboSelectWorkPeopleDepart.Text.ToString() + Environment.NewLine +
                        Class + "今日出勤人數共 " + chklstWorkPeopleName.CheckedItems.Count + "位！" + Environment.NewLine +
                        "每日出勤登記已完成！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    OpenSqlCon.Close();
                    OpensqlConME.Close();
                    RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                }
            }
        }

        //每日出勤登記-排休=0
        private void btnWPS_0_Click(object sender, EventArgs e)
        {
            InsertWPS_Status(0);
        }

        //每日出勤登記-換休=1
        private void btnWPS_1_Click(object sender, EventArgs e)
        {
            InsertWPS_Status(1);
        }

        //每日出勤登記-調休=2
        private void btnWPS_2_Click(object sender, EventArgs e)
        {
            InsertWPSTime_Status(2);
        }

        //每日出勤登記-特休=3
        private void btnWPS_3_Click(object sender, EventArgs e)
        {
            InsertWPSTime_Status(3);
        }

        //每日出勤登記-病假=4
        private void btnWPS_4_Click(object sender, EventArgs e)
        {
            InsertWPSTimeTxt_Status(4);
        }

        //每日出勤登記-事假=5
        private void btnWPS_5_Click(object sender, EventArgs e)
        {
            InsertWPSTimeTxt_Status(5);
        }

        //每日出勤登記-曠職=6
        private void btnWPS_6_Click(object sender, EventArgs e)
        {
            InsertWPSTime_Status(6);
        }

        //每日出勤登記-新進人員=7
        private void btnNewPeople_Click(object sender, EventArgs e)
        {
            if (txtNewPeople.Text == "")
            {
                MessageBox.Show("未輸入新進人員姓名！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (rdoClassAM.Checked == false & rdoClassPM.Checked == false)
                {
                    MessageBox.Show("尚未選擇班別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                else if (cboSelectWorkPeopleDepart.SelectedIndex == 0)
                {
                    MessageBox.Show("尚未選擇部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                else
                {
                    DateTime time = new DateTime();
                    time = DateTime.Now;
                    DateTime t1 = Convert.ToDateTime(st1);
                    DateTime t2 = Convert.ToDateTime(st2);
                    if (time > t1 | time > t2)
                    {
                        MessageBox.Show("已超過每日出勤登記時間！" + Environment.NewLine + "請洽詢管理部-人資專員。", "錯誤",
                            MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        int Classnum = 0;
                        string Class = "";
                        if (rdoClassAM.Checked == true)
                        {
                            Classnum = 1;
                            Class = "早班";
                        }
                        else if (rdoClassPM.Checked == true)
                        {
                            Classnum = 2;
                            Class = "晚班";
                        }
                        //先撈出選定部門的部門ID
                        SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                            cboSelectWorkPeopleDepart.Text + "'";
                        SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                        DataSet Read = new DataSet();
                        Load.Fill(Read, "DepartId");
                        //先檢查今日該部門人員是否已報過出勤
                        int a = chklstWorkPeopleName.CheckedItems.Count;
                        int q;
                        string name = null;
                        SqlDataReader check;
                        for (q = 0; q < a; q++)
                        {
                            OpensqlConME.Close();
                            name = chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                            SqlComm.CommandText = "select * from WorkPeople where (Date >= '" +
                                DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                                DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                                Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                                chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                                "' and Class in (1,2)";
                            SqlComm.Connection = OpensqlConME;
                            OpensqlConME.Open();
                            check = SqlComm.ExecuteReader();
                            if (check.HasRows)
                            {
                                MessageBox.Show("該人員[" + name + "]今日已報過出勤！", "注意", MessageBoxButtons.OK,
                                    MessageBoxIcon.Hand);
                                OpensqlConME.Close();
                                return;
                            }
                        }
                        OpensqlConME.Close();
                        OpensqlConME.Open();
                        SqlComm.CommandText = "insert into WorkPeople (Date,EmpId,EmpName,DepartId,Class," +
                            "Status,RegisterPeople,RegisterDate) values ('" + DateTime.Now.ToString("yyyy-MM-dd") +
                            "','EW000','" + txtNewPeople.Text.Trim() + "','" + Read.Tables["DepartId"].Rows[0]
                            ["DepartId"].ToString().Trim() + "','" + Classnum + "','7','" + lblUserNameShow.Text.
                            ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        SqlComm.Connection = OpensqlConME;
                        SqlComm.ExecuteNonQuery();
                        MessageBox.Show("部門：" + cboSelectWorkPeopleDepart.Text.ToString() + Environment.NewLine +
                            Class + "今日新進出勤人員： " + txtNewPeople.Text.Trim() + "，出勤登記已完成！", "訊息",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                        RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                    }
                }
            }
        }

        //每日出勤登記-其它=8
        private void btnWorkPeopleOther_Click(object sender, EventArgs e)
        {
            if (rdoClassAM.Checked == false & rdoClassPM.Checked == false)
            {
                MessageBox.Show("尚未選擇班別！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else if (cboSelectWorkPeopleDepart.SelectedIndex == 0)
            {
                MessageBox.Show("尚未選擇部門！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            else
            {
                DateTime time = new DateTime();
                time = DateTime.Now;
                DateTime t1 = Convert.ToDateTime(st1);
                DateTime t2 = Convert.ToDateTime(st2);
                if (time > t1 | time > t2)
                {
                    MessageBox.Show("已超過每日出勤登記時間！" + Environment.NewLine + "請洽詢管理部-人資專員。", "錯誤",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int Classnum = 0;
                    string Class = "";
                    if (rdoClassAM.Checked == true)
                    {
                        Classnum = 1;
                        Class = "早班";
                    }
                    else if (rdoClassPM.Checked == true)
                    {
                        Classnum = 2;
                        Class = "晚班";
                    }
                    //先撈出選定部門的部門ID
                    SqlComm.CommandText = "select DepartId from HPSdDepartTree where DepartName='" +
                        cboSelectWorkPeopleDepart.Text + "'";
                    SqlDataAdapter Load = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                    DataSet Read = new DataSet();
                    Load.Fill(Read, "DepartId");
                    //先檢查今日該部門人員是否已報過出勤
                    int a = chklstWorkPeopleName.CheckedItems.Count;
                    int q;
                    string name = null;
                    SqlDataReader check;
                    for (q = 0; q < a; q++)
                    {
                        OpensqlConME.Close();
                        name = chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray());
                        SqlComm.CommandText = "select * from WorkPeople where (Date >= '" +
                            DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "' and Date <='" +
                            DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + "') and DepartId='" +
                            Read.Tables["DepartId"].Rows[0]["DepartId"].ToString() + "' and EmpName='" +
                            chklstWorkPeopleName.CheckedItems[q].ToString().Trim().TrimStart(clear.ToArray()) +
                            "' and Class in (1,2)";
                        SqlComm.Connection = OpensqlConME;
                        OpensqlConME.Open();
                        check = SqlComm.ExecuteReader();
                        if (check.HasRows)
                        {
                            MessageBox.Show("該人員[" + name + "]今日已報過出勤！", "注意", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);
                            OpensqlConME.Close();
                            return;
                        }
                    }
                    OpensqlConME.Close();
                    SelectTimeOther sto = new SelectTimeOther();
                    sto.dtpWPSStart.Format = DateTimePickerFormat.Custom;
                    sto.dtpWPSEnd.Format = DateTimePickerFormat.Custom;
                    sto.dtpWPSStart.CustomFormat = "yyyy-MM-dd HH:mm";
                    sto.dtpWPSEnd.CustomFormat = "yyyy-MM-dd HH:mm";
                    if (sto.ShowDialog() == DialogResult.OK)
                    {
                        OpensqlConME.Open();
                        int A = chklstWorkPeopleName.CheckedItems.Count;
                        for (int i = 0; i < A; i++)
                        {
                            SqlComm.CommandText = "select EmpId,EmpName from HPSdEmpInfo where EmpName=N'" +
                                chklstWorkPeopleName.CheckedItems[i].ToString().Trim().TrimStart(clear.ToArray()) + "'";
                            SqlDataAdapter ReadNI = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
                            DataSet ReadUser = new DataSet();
                            ReadNI.Fill(ReadUser, "ReadUser");
                            SqlComm.CommandText = "insert into WorkPeople (Date,EmpId,EmpName,DepartId,Class,Status," +
                                "StartTime,EndTime,RegisterPeople,RegisterDate,Notation) values ('" +
                                DateTime.Now.ToString("yyyy-MM-dd") + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][0].ToString().Trim() + "','" +
                                ReadUser.Tables["ReadUser"].Rows[0][1].ToString().Trim() + "','" +
                                Read.Tables["DepartId"].Rows[0]["DepartId"].ToString().Trim() + "','" +
                                Classnum + "','8','" + sto.dtpWPSStart.Value.ToString("HH:mm") + "','" +
                                sto.dtpWPSEnd.Value.ToString("HH:mm") + "','" + lblUserNameShow.Text.ToString() +
                                "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                                sto.cboWorkPeopleOther.Text + "')";
                            SqlComm.Connection = OpensqlConME;
                            SqlComm.ExecuteNonQuery();
                        }
                        MessageBox.Show("部門：" + cboSelectWorkPeopleDepart.Text.ToString() + Environment.NewLine +
                            Class + "今日排休人數共 " + chklstWorkPeopleName.CheckedItems.Count + "位！" +
                            Environment.NewLine + "每日出勤登記已完成！", "訊息", MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                        OpenSqlCon.Close();
                        OpensqlConME.Close();
                        RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                    }
                }
            }
        }
        
        //每日出勤查詢-全廠統計
        private void btnWorkPeopleReferAll_Click(object sender, EventArgs e)
        {
            WorkPeopleReferAll();
        }

        //每日出勤登記-取消出勤
        private void btnCancelWorkPeople_Click(object sender, EventArgs e)
        {
            DateTime time = new DateTime();
            time = DateTime.Now;
            DateTime t1 = Convert.ToDateTime(st1);
            DateTime t2 = Convert.ToDateTime(st2);
            if (time > t1 | time > t2)
            {
                MessageBox.Show("已超過每日出勤登記時間！不得取消出勤！" + Environment.NewLine + "請洽詢管理部-人資專員。",
                    "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                if (MessageBox.Show("您確定要取消人員［"+dgvWorkPeopleShow.CurrentRow.Cells["姓名"].Value.ToString().
                    Trim()+"］今日出勤嗎？","注意",MessageBoxButtons.OK,MessageBoxIcon.Exclamation)==DialogResult.OK)
                {
                    string SQLComm = "update WorkPeople set Status='22',CancelPeople='" + lblUserNameShow.Text +
                        "',CancelDate='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where EmpId='" +
                        dgvWorkPeopleShow.CurrentRow.Cells["工號"].Value.ToString().Trim() + "' and EmpName='" +
                        dgvWorkPeopleShow.CurrentRow.Cells["姓名"].Value.ToString().Trim() + "' and Date='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "'";
                    using (SqlConnection sqlcon = new SqlConnection(SQLCon))
                    {
                        sqlcon.Open();
                        using (SqlCommand sqlcomm = new SqlCommand(SQLComm, sqlcon))
                        {
                            if(sqlcomm.ExecuteNonQuery()==1)
                            {
                                MessageBox.Show("已取消成功！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    RenewWorkPeople(cboSelectWorkPeopleDepart.Text);
                }
            }
        }

        //每日出勤查詢-寄出郵件
        private void btnSendMail_Click(object sender, EventArgs e)
        {
            WPRtoExcelTemp();
            string temp = Path.GetTempPath();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string savefilepath = temp + date + ".xls";
            //建立寄件者地址與名稱
            MailAddress ReceiverAddress = new MailAddress("sm4@ewpcb.com.tw", "每日出勤統計");
            //建立收件者地址
            MailAddress SendAddress = new MailAddress("WorkPeople@ewpcb.com.tw");
            //建立附加檔案
            Attachment attachment = new Attachment(savefilepath);
            //建立E-MAIL相關設定與訊息
            MailMessage SendMail = new MailMessage(ReceiverAddress, SendAddress);
            //Mail以HTML格式寄送
            SendMail.IsBodyHtml = true;
            //設定信件內容編碼為UTF8
            SendMail.BodyEncoding = Encoding.UTF8;
            //設定信件主旨編碼為UTF8
            SendMail.SubjectEncoding = Encoding.UTF8;
            //設定信件優先權為普通
            SendMail.Priority = MailPriority.Normal;
            SendMail.Subject = DateTime.Now.ToString("yyyy-MM-dd") + " 每日出勤回報！";//主旨
            SendMail.Body = "每日出勤請詳閱附件。" + "<br/>" + "<br/>" + "-----此封郵件由系統所寄出，請勿直接回覆！-----";//內容
            SendMail.Attachments.Add(attachment);//加上附件檔案
            //建立一個信件通訊並設定郵件主機地址與通訊埠號
            SmtpClient MySmtp = new SmtpClient("ms1.ewpcb.com.tw", 25);
            //設定寄件者的帳號與密碼
            MySmtp.Credentials = new NetworkCredential("sm4", "sm4@ew");
            try
            {
                MySmtp.Send(SendMail);
                MessageBox.Show("信件已成功寄出！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                Exception error = new Exception();
                MessageBox.Show(error.Message);
            }
            finally
            {
                MySmtp = null;
                SendMail.Dispose();
                File.Delete(savefilepath);//刪除存放在暂存區的檔案
            }
        }
    }
}
