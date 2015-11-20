namespace EW_BentoOrder
{
    partial class SelectTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectTime));
            this.lblWPSStartTime = new System.Windows.Forms.Label();
            this.lblWPSEndTime = new System.Windows.Forms.Label();
            this.btnSTSend = new System.Windows.Forms.Button();
            this.btnSTCancel = new System.Windows.Forms.Button();
            this.dtpWPSEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpWPSStart = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // lblWPSStartTime
            // 
            this.lblWPSStartTime.AutoSize = true;
            this.lblWPSStartTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblWPSStartTime.Location = new System.Drawing.Point(28, 18);
            this.lblWPSStartTime.Name = "lblWPSStartTime";
            this.lblWPSStartTime.Size = new System.Drawing.Size(89, 20);
            this.lblWPSStartTime.TabIndex = 2;
            this.lblWPSStartTime.Text = "起始時間：";
            // 
            // lblWPSEndTime
            // 
            this.lblWPSEndTime.AutoSize = true;
            this.lblWPSEndTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblWPSEndTime.Location = new System.Drawing.Point(28, 65);
            this.lblWPSEndTime.Name = "lblWPSEndTime";
            this.lblWPSEndTime.Size = new System.Drawing.Size(89, 20);
            this.lblWPSEndTime.TabIndex = 3;
            this.lblWPSEndTime.Text = "結束時間：";
            // 
            // btnSTSend
            // 
            this.btnSTSend.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSTSend.Location = new System.Drawing.Point(151, 105);
            this.btnSTSend.Name = "btnSTSend";
            this.btnSTSend.Size = new System.Drawing.Size(75, 35);
            this.btnSTSend.TabIndex = 4;
            this.btnSTSend.Text = "送出";
            this.btnSTSend.UseVisualStyleBackColor = true;
            this.btnSTSend.Click += new System.EventHandler(this.btnSTSend_Click);
            // 
            // btnSTCancel
            // 
            this.btnSTCancel.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSTCancel.Location = new System.Drawing.Point(232, 105);
            this.btnSTCancel.Name = "btnSTCancel";
            this.btnSTCancel.Size = new System.Drawing.Size(75, 35);
            this.btnSTCancel.TabIndex = 5;
            this.btnSTCancel.Text = "取消";
            this.btnSTCancel.UseVisualStyleBackColor = true;
            this.btnSTCancel.Click += new System.EventHandler(this.btnSTCancel_Click);
            // 
            // dtpWPSEnd
            // 
            this.dtpWPSEnd.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtpWPSEnd.Location = new System.Drawing.Point(107, 62);
            this.dtpWPSEnd.Name = "dtpWPSEnd";
            this.dtpWPSEnd.Size = new System.Drawing.Size(200, 25);
            this.dtpWPSEnd.TabIndex = 1;
            // 
            // dtpWPSStart
            // 
            this.dtpWPSStart.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtpWPSStart.Location = new System.Drawing.Point(107, 15);
            this.dtpWPSStart.Name = "dtpWPSStart";
            this.dtpWPSStart.Size = new System.Drawing.Size(200, 25);
            this.dtpWPSStart.TabIndex = 0;
            // 
            // SelectTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(334, 161);
            this.Controls.Add(this.btnSTCancel);
            this.Controls.Add(this.btnSTSend);
            this.Controls.Add(this.dtpWPSEnd);
            this.Controls.Add(this.lblWPSEndTime);
            this.Controls.Add(this.dtpWPSStart);
            this.Controls.Add(this.lblWPSStartTime);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "請輸入時間";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblWPSStartTime;
        private System.Windows.Forms.Label lblWPSEndTime;
        private System.Windows.Forms.Button btnSTSend;
        private System.Windows.Forms.Button btnSTCancel;
        public System.Windows.Forms.DateTimePicker dtpWPSEnd;
        public System.Windows.Forms.DateTimePicker dtpWPSStart;
    }
}