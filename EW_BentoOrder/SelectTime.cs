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
    public partial class SelectTime : SelectTimeTxt
    {
        public SelectTime()
        {
            InitializeComponent();
        }

        public override void btnSTSend_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
