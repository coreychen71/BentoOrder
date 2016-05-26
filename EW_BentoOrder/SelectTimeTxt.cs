using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EW_BentoOrder
{
    public partial class SelectTimeTxt : Form
    {
        public SelectTimeTxt()
        {
            InitializeComponent();
        }

        private void btnSTCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public virtual void btnSTSend_Click(object sender, EventArgs e)
        {
            if (txtInputNotation.Text == "")
            {
                MessageBox.Show("未輸入請假事由！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
