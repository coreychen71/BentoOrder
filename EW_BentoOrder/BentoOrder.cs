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

namespace EW_BentoOrder
{
    public partial class BentoOrder : Form
    {
        public BentoOrder()
        {
            InitializeComponent();
        }

        SqlConnection OpenSqlCon = new SqlConnection("server=ERP;database=EW;uid=JSIS;pwd=JSIS");
        SqlConnection OpensqlConME = new SqlConnection("server=EWNAS;database=ME;uid=me;pwd=2dae5na");
        SqlCommand SqlComm = new SqlCommand();
        //後續要將CheckBox.Item的多餘字元移除
        string clear = "ACDEFGILMQSTPR0123456789";

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
            rtbOrderTimeIllustrate.ForeColor = Color.Red;
            lblOrderNumShow.Text = null;
            txtCompanyName.ReadOnly = true;
            txtCompanyTel.ReadOnly = true;
            txtCompanyCellPhone.ReadOnly = true;
            btnBentoTelChangeSave.Enabled = false;
            btnSavePrice.Enabled = false;
            SqlComm.CommandText = "select distinct HPSdEmpInfo.DepartId,HPSdDepartTree.DepartName from HPSdEmpInfo," +
                "HPSdDepartTree where HPSdEmpInfo.DepartId = HPSdDepartTree.DepartId and HPSdDepartTree.DepartId " +
                "not in ('EF')";
            SqlDataAdapter DepartId = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
            DataSet dpid = new DataSet();
            DepartId.Fill(dpid, "DepartId");
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
                        if (Depart.Tables["B"].Rows[i][0].ToString() == Depart.Tables["A"].Rows[x][0].ToString())
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
                            chklstName.CheckedItems[i].ToString() + "'";
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
            Microsoft.Office.Interop.Excel.Application excel = null;
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
                //寫入欄位名稱
                for (int i = 0; i < dgvReferOrderAll.Columns.Count; i++)
                {
                    excel.Cells[1, i + 1] = dgvReferOrderAll.Columns[i].HeaderText;
                }

                PGB pgb = new PGB();
                pgb.progressBar1.Minimum = 1;
                pgb.progressBar1.Maximum = dgvReferOrderAll.Rows.Count;
                pgb.progressBar1.Step = 1;
                pgb.Show();
                pgb.progressBar1.PerformStep();
                
                //把DataGridView資料寫到Excel
                for (int i = 0; i < dgvReferOrderAll.Rows.Count - 1; i++)
                {
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
                    SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate)" + " values ('" + DateTime.Now.ToString
                            ("yyyy-MM-dd HH:mm:ss") + "','EW000'," + "'" + txtInputName.Text + "','" + cboSelectDepartid
                            .Text + "','" + num.ToString() + "',0,'" + lblUserNameShow.Text.ToString() + "','" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
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
                    SqlComm.CommandText = "insert into BentoOrder (Date,EmpId,EmpName,DepartId,OrderStatus," +
                            "VegetableFood,OrderPeople,OrderDate)" + " values ('" + DateTime.Now.ToString
                            ("yyyy-MM-dd HH:mm:ss") + "','EW000'," + "'" + txtInputName.Text + "','" + cboSelectDepartid
                            .Text + "','" + num.ToString() + "',1,'" + lblUserNameShow.Text.ToString() + "','" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
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
                //宣告部門字串陣列
                string[] depart = {"總經理室","業務部","管理部","財務部","工程部","製造研發部","品保部","廠長室","壓合課",
                    "生管課","測試課","品檢課","乾膜課","防焊課","鑽孔課","成型課" };
                //宣告部門ID字串陣列，注意：初始值的排序需要與部門字串一樣
                string[] departid = {"EG","ES","EM","EA","EE","ER","EQ","MM","EL","EP","ET","EI","FF","LF","DF",
                    "CF" };
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
    }
}
