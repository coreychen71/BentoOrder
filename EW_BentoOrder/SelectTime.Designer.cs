﻿namespace EW_BentoOrder
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
            this.SuspendLayout();
            // 
            // txtInputNotation
            // 
            this.txtInputNotation.Visible = false;
            // 
            // lblNotation
            // 
            this.lblNotation.Visible = false;
            // 
            // btnSTSend
            // 
            this.btnSTSend.Click += new System.EventHandler(this.btnSTSend_Click);
            // 
            // SelectTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 211);
            this.Name = "SelectTime";
            this.Text = "請選擇時間";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}