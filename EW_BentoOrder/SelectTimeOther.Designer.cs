namespace EW_BentoOrder
{
    partial class SelectTimeOther
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboWorkPeopleOther = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtInputNotation
            // 
            this.txtInputNotation.Location = new System.Drawing.Point(12, 170);
            this.txtInputNotation.Size = new System.Drawing.Size(52, 29);
            // 
            // lblNotation
            // 
            this.lblNotation.Location = new System.Drawing.Point(28, 110);
            this.lblNotation.Size = new System.Drawing.Size(89, 20);
            this.lblNotation.Text = "其它假別：";
            this.lblNotation.Visible = true;
            // 
            // cboWorkPeopleOther
            // 
            this.cboWorkPeopleOther.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboWorkPeopleOther.FormattingEnabled = true;
            this.cboWorkPeopleOther.Items.AddRange(new object[] {
            "請選擇",
            "待補",
            "公假",
            "婚假",
            "喪假",
            "產假",
            "颱風假"});
            this.cboWorkPeopleOther.Location = new System.Drawing.Point(107, 107);
            this.cboWorkPeopleOther.Name = "cboWorkPeopleOther";
            this.cboWorkPeopleOther.Size = new System.Drawing.Size(200, 28);
            this.cboWorkPeopleOther.TabIndex = 8;
            this.cboWorkPeopleOther.Visible = false;
            // 
            // SelectTimeOther
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 211);
            this.Controls.Add(this.cboWorkPeopleOther);
            this.Name = "SelectTimeOther";
            this.Text = "請選擇時間和假別";
            this.Controls.SetChildIndex(this.txtInputNotation, 0);
            this.Controls.SetChildIndex(this.dtpWPSStart, 0);
            this.Controls.SetChildIndex(this.dtpWPSEnd, 0);
            this.Controls.SetChildIndex(this.btnSTSend, 0);
            this.Controls.SetChildIndex(this.btnSTCancel, 0);
            this.Controls.SetChildIndex(this.lblNotation, 0);
            this.Controls.SetChildIndex(this.cboWorkPeopleOther, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox cboWorkPeopleOther;
    }
}