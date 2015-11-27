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
    public partial class SelectTimeOther : SelectTime
    {
        public SelectTimeOther()
        {
            InitializeComponent();
            txtInputNotation.Visible = false;
            cboWorkPeopleOther.Visible = true;
            cboWorkPeopleOther.SelectedIndex = 0;
            cboWorkPeopleOther.DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }
}
