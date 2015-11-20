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
    public partial class SelectTime : Form
    {
        public SelectTime()
        {
            InitializeComponent();
        }

        private void btnSTCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSTSend_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
