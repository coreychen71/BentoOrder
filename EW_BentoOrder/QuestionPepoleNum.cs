using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace EW_BentoOrder
{
    public partial class QuestionPepoleNum : Form
    {
        SqlConnection OpenSqlCon = new SqlConnection("server=ERP;database=EW;uid=JSIS;pwd=JSIS");
        SqlCommand SqlComm = new SqlCommand();

        private void QuestionRepoleNum_Load(object sender, EventArgs e)
        {
            SqlComm.CommandText = "select distinct DepartId from HPSdEmpInfo";
            SqlDataAdapter DepartId = new SqlDataAdapter(SqlComm.CommandText, OpenSqlCon);
            DataSet dpid = new DataSet();
            DepartId.Fill(dpid, "DepartId");
            //Create new rows for dpid.tables
            DataRow dr = dpid.Tables["DepartId"].NewRow();
            //建立dr[0]的資料為請選擇
            dr[0] = "請選擇";
            //將dr插入到dpid.tables.rows的第一列
            dpid.Tables["DepartId"].Rows.InsertAt(dr, 0);
            cboSelectDepartSunday.DataSource = dpid.Tables["DepartId"];
            cboSelectDepartSunday.DisplayMember = "DepartId";
            //設定cboDpname為唯讀且不可輸入新值
            cboSelectDepartSunday.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public QuestionPepoleNum()
        {
            InitializeComponent();
        }

        private void txtNum_keyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string Order0 = txtOrder0.Text;
            string Order1 = txtOrder1.Text;
            if (Order0 == "" & Order1 == "")
            {
                MessageBox.Show("未輸入[葷]或[素]的訂餐數量！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
