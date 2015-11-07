namespace EW_BentoOrder
{
    partial class QuestionPepoleNum
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestionPepoleNum));
            this.lblOrder0 = new System.Windows.Forms.Label();
            this.lblOrder1 = new System.Windows.Forms.Label();
            this.txtOrder0 = new System.Windows.Forms.TextBox();
            this.txtOrder1 = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboSelectDepartSunday = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblOrder0
            // 
            this.lblOrder0.AutoSize = true;
            this.lblOrder0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOrder0.Location = new System.Drawing.Point(303, 184);
            this.lblOrder0.Name = "lblOrder0";
            this.lblOrder0.Size = new System.Drawing.Size(24, 19);
            this.lblOrder0.TabIndex = 0;
            this.lblOrder0.Text = "葷";
            // 
            // lblOrder1
            // 
            this.lblOrder1.AutoSize = true;
            this.lblOrder1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOrder1.Location = new System.Drawing.Point(379, 184);
            this.lblOrder1.Name = "lblOrder1";
            this.lblOrder1.Size = new System.Drawing.Size(24, 19);
            this.lblOrder1.TabIndex = 1;
            this.lblOrder1.Text = "素";
            // 
            // txtOrder0
            // 
            this.txtOrder0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtOrder0.Location = new System.Drawing.Point(333, 181);
            this.txtOrder0.Name = "txtOrder0";
            this.txtOrder0.Size = new System.Drawing.Size(40, 27);
            this.txtOrder0.TabIndex = 2;
            this.txtOrder0.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNum_keyPress);
            // 
            // txtOrder1
            // 
            this.txtOrder1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtOrder1.Location = new System.Drawing.Point(409, 181);
            this.txtOrder1.Name = "txtOrder1";
            this.txtOrder1.Size = new System.Drawing.Size(40, 27);
            this.txtOrder1.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSend.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSend.Location = new System.Drawing.Point(530, 306);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(50, 27);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "送出";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.Location = new System.Drawing.Point(586, 306);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboSelectDepartSunday
            // 
            this.cboSelectDepartSunday.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboSelectDepartSunday.FormattingEnabled = true;
            this.cboSelectDepartSunday.Location = new System.Drawing.Point(242, 32);
            this.cboSelectDepartSunday.Name = "cboSelectDepartSunday";
            this.cboSelectDepartSunday.Size = new System.Drawing.Size(121, 27);
            this.cboSelectDepartSunday.TabIndex = 6;
            // 
            // QuestionPepoleNum
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 345);
            this.Controls.Add(this.cboSelectDepartSunday);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtOrder1);
            this.Controls.Add(this.txtOrder0);
            this.Controls.Add(this.lblOrder1);
            this.Controls.Add(this.lblOrder0);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuestionPepoleNum";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "請輸入各部門人員的報餐數量：";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOrder0;
        private System.Windows.Forms.Label lblOrder1;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtOrder0;
        public System.Windows.Forms.TextBox txtOrder1;
        public System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ComboBox cboSelectDepartSunday;
    }
}