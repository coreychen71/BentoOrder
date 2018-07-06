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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        SqlConnection OpenSqlCon = new SqlConnection("server=192.168.1.39;database=EW;uid=JSIS;pwd=JSIS");
        SqlCommand SqlComm = new SqlCommand();

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string EM = "EM";
            string EA = "EA";
            OpenSqlCon.Open();
            SqlComm.CommandText = "select UserName,unitid from CURdUsers where UserId='" + txtUserName.Text + "' and " +
                "UserPassword='" + txtPassword.Text + "'";
            SqlComm.Connection = OpenSqlCon;
            SqlDataReader ReadName = SqlComm.ExecuteReader();
            if(ReadName.HasRows)
            {
                this.Hide();
                BentoOrder bo = new BentoOrder();
                ReadName.Read();
                bo.lblUserNameShow.Text = ReadName["UserName"].ToString();
                if(ReadName["unitid"].ToString().Trim()== EM | ReadName["unitid"].ToString().Trim() == EA)
                {
                    bo.btnSanitary.Enabled = true;
                    bo.btnBentoTelChange.Enabled = true;
                    bo.btnBentoTelChangeSave.Enabled = true;
                    bo.btnCancelOrderM.Enabled = true;
                    bo.btnSendM.Enabled = true;
                    bo.btnNewPeople.Enabled = true;
                    bo.btnNewPeople0.Enabled = true;
                    bo.btnNewPeople1.Enabled = true;
                    bo.btnSendMail.Enabled = true;
                    bo.txtInputName.Enabled = true;
                    bo.txtNewPeople.Enabled = true;
                    bo.st1 = "23:00";
                    bo.st2 = "23:00";
                }
                else
                {
                    //將tabBentoOrder的tpAccount Page控件關聯性移除，透過此方式達到隱藏tpAccount Page的功能
                    bo.tabBentoOrder.Controls["tpAccount"].Parent = null;
                    bo.tabBentoOrder.Controls["tpWPRManage"].Parent = null;
                    bo.btnSanitary.Enabled = false;
                    bo.btnBentoTelChange.Enabled = false;
                    bo.btnBentoTelChangeSave.Enabled = false;
                    bo.btnCancelOrderM.Enabled = false;
                    bo.btnSendM.Enabled = false;
                    bo.btnNewPeople.Enabled = false;
                    bo.btnNewPeople0.Enabled = false;
                    bo.btnNewPeople1.Enabled = false;
                    bo.btnSendMail.Enabled = false;
                    bo.txtInputName.Enabled = false;
                    bo.txtNewPeople.Enabled = false;
                }
                ReadName.Close();
                OpenSqlCon.Close();
                if(bo.ShowDialog()==DialogResult.Cancel)
                {
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("帳號或密碼輸入錯誤！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OpenSqlCon.Close();
                return;
            }
        }
    }
}
