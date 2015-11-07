namespace EW_BentoOrder
{
    partial class BentoOrder
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BentoOrder));
            this.tabBentoOrder = new System.Windows.Forms.TabControl();
            this.tpEveryDayOrder = new System.Windows.Forms.TabPage();
            this.btnSendSunday = new System.Windows.Forms.Button();
            this.btnSendM = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.txtCompanyCellPhone = new System.Windows.Forms.TextBox();
            this.txtCompanyTel = new System.Windows.Forms.TextBox();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.btnBentoTelChangeSave = new System.Windows.Forms.Button();
            this.btnBentoTelChange = new System.Windows.Forms.Button();
            this.lblCompanyTel = new System.Windows.Forms.Label();
            this.rtbOrderTimeIllustrate = new System.Windows.Forms.RichTextBox();
            this.btnSanitary = new System.Windows.Forms.Button();
            this.chkVegetableFood = new System.Windows.Forms.CheckBox();
            this.lblVegetableFood = new System.Windows.Forms.Label();
            this.rdoNightOrder = new System.Windows.Forms.RadioButton();
            this.chklstName = new System.Windows.Forms.CheckedListBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboDepart = new System.Windows.Forms.ComboBox();
            this.lblSelectDeport = new System.Windows.Forms.Label();
            this.lblOrderPeople = new System.Windows.Forms.Label();
            this.lblSelectOrder = new System.Windows.Forms.Label();
            this.rdoPmOrder = new System.Windows.Forms.RadioButton();
            this.rdoAmOrder = new System.Windows.Forms.RadioButton();
            this.tpOrderRefer = new System.Windows.Forms.TabPage();
            this.btnNewPeople1 = new System.Windows.Forms.Button();
            this.lblInputName = new System.Windows.Forms.Label();
            this.txtInputName = new System.Windows.Forms.TextBox();
            this.btnNewPeople0 = new System.Windows.Forms.Button();
            this.btnCancelOrderM = new System.Windows.Forms.Button();
            this.btnOrderStatistics = new System.Windows.Forms.Button();
            this.btnReferNoOrder = new System.Windows.Forms.Button();
            this.btnCancelOrder = new System.Windows.Forms.Button();
            this.lblOrderNumShow = new System.Windows.Forms.Label();
            this.lblOrderNum = new System.Windows.Forms.Label();
            this.dgvBentoDataShow = new System.Windows.Forms.DataGridView();
            this.btnRefer = new System.Windows.Forms.Button();
            this.rdoReferNight = new System.Windows.Forms.RadioButton();
            this.rdoReferPm = new System.Windows.Forms.RadioButton();
            this.rdoReferAm = new System.Windows.Forms.RadioButton();
            this.lblReferOrder = new System.Windows.Forms.Label();
            this.cboSelectDepartid = new System.Windows.Forms.ComboBox();
            this.lblSelectDepartid = new System.Windows.Forms.Label();
            this.tpAccount = new System.Windows.Forms.TabPage();
            this.btnSendToExcel = new System.Windows.Forms.Button();
            this.rdoSelectDepart = new System.Windows.Forms.RadioButton();
            this.cboSelectDepart = new System.Windows.Forms.ComboBox();
            this.btnReferOrderNight = new System.Windows.Forms.Button();
            this.btnReferOrderPm = new System.Windows.Forms.Button();
            this.btnReferOrderAm = new System.Windows.Forms.Button();
            this.btnSavePrice = new System.Windows.Forms.Button();
            this.btnChangePrice = new System.Windows.Forms.Button();
            this.txtTotalPriceAll = new System.Windows.Forms.TextBox();
            this.lblTotalPriceAll = new System.Windows.Forms.Label();
            this.txtTotalPrice1 = new System.Windows.Forms.TextBox();
            this.txtTotalPrice0 = new System.Windows.Forms.TextBox();
            this.lblTotalPrice1 = new System.Windows.Forms.Label();
            this.lblTotalPrice0 = new System.Windows.Forms.Label();
            this.txtTotalOrder1 = new System.Windows.Forms.TextBox();
            this.txtTotalOrder0 = new System.Windows.Forms.TextBox();
            this.lblTotalOrder1 = new System.Windows.Forms.Label();
            this.lblTotalOrder0 = new System.Windows.Forms.Label();
            this.txtBentoPrice = new System.Windows.Forms.TextBox();
            this.lblBentoPrice = new System.Windows.Forms.Label();
            this.dgvReferOrderAll = new System.Windows.Forms.DataGridView();
            this.rdoAllCompany = new System.Windows.Forms.RadioButton();
            this.rdoTwoCompany = new System.Windows.Forms.RadioButton();
            this.rdOneCompany = new System.Windows.Forms.RadioButton();
            this.btnReferOrderAll = new System.Windows.Forms.Button();
            this.lblSelectReferCompany = new System.Windows.Forms.Label();
            this.lblSelectReferDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.lblDateTimeShow = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserNameShow = new System.Windows.Forms.Label();
            this.tmrDateTime = new System.Windows.Forms.Timer(this.components);
            this.btnDepartOrder = new System.Windows.Forms.Button();
            this.tabBentoOrder.SuspendLayout();
            this.tpEveryDayOrder.SuspendLayout();
            this.tpOrderRefer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBentoDataShow)).BeginInit();
            this.tpAccount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReferOrderAll)).BeginInit();
            this.SuspendLayout();
            // 
            // tabBentoOrder
            // 
            this.tabBentoOrder.Controls.Add(this.tpEveryDayOrder);
            this.tabBentoOrder.Controls.Add(this.tpOrderRefer);
            this.tabBentoOrder.Controls.Add(this.tpAccount);
            this.tabBentoOrder.Location = new System.Drawing.Point(12, 28);
            this.tabBentoOrder.Name = "tabBentoOrder";
            this.tabBentoOrder.SelectedIndex = 0;
            this.tabBentoOrder.Size = new System.Drawing.Size(800, 561);
            this.tabBentoOrder.TabIndex = 0;
            // 
            // tpEveryDayOrder
            // 
            this.tpEveryDayOrder.Controls.Add(this.btnSendSunday);
            this.tpEveryDayOrder.Controls.Add(this.btnSendM);
            this.tpEveryDayOrder.Controls.Add(this.btnSelectAll);
            this.tpEveryDayOrder.Controls.Add(this.txtCompanyCellPhone);
            this.tpEveryDayOrder.Controls.Add(this.txtCompanyTel);
            this.tpEveryDayOrder.Controls.Add(this.txtCompanyName);
            this.tpEveryDayOrder.Controls.Add(this.btnBentoTelChangeSave);
            this.tpEveryDayOrder.Controls.Add(this.btnBentoTelChange);
            this.tpEveryDayOrder.Controls.Add(this.lblCompanyTel);
            this.tpEveryDayOrder.Controls.Add(this.rtbOrderTimeIllustrate);
            this.tpEveryDayOrder.Controls.Add(this.btnSanitary);
            this.tpEveryDayOrder.Controls.Add(this.chkVegetableFood);
            this.tpEveryDayOrder.Controls.Add(this.lblVegetableFood);
            this.tpEveryDayOrder.Controls.Add(this.rdoNightOrder);
            this.tpEveryDayOrder.Controls.Add(this.chklstName);
            this.tpEveryDayOrder.Controls.Add(this.btnSend);
            this.tpEveryDayOrder.Controls.Add(this.btnCancel);
            this.tpEveryDayOrder.Controls.Add(this.cboDepart);
            this.tpEveryDayOrder.Controls.Add(this.lblSelectDeport);
            this.tpEveryDayOrder.Controls.Add(this.lblOrderPeople);
            this.tpEveryDayOrder.Controls.Add(this.lblSelectOrder);
            this.tpEveryDayOrder.Controls.Add(this.rdoPmOrder);
            this.tpEveryDayOrder.Controls.Add(this.rdoAmOrder);
            this.tpEveryDayOrder.Location = new System.Drawing.Point(4, 22);
            this.tpEveryDayOrder.Name = "tpEveryDayOrder";
            this.tpEveryDayOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tpEveryDayOrder.Size = new System.Drawing.Size(792, 535);
            this.tpEveryDayOrder.TabIndex = 0;
            this.tpEveryDayOrder.Text = "每日報餐登記表";
            this.tpEveryDayOrder.UseVisualStyleBackColor = true;
            // 
            // btnSendSunday
            // 
            this.btnSendSunday.Enabled = false;
            this.btnSendSunday.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendSunday.Location = new System.Drawing.Point(636, 494);
            this.btnSendSunday.Name = "btnSendSunday";
            this.btnSendSunday.Size = new System.Drawing.Size(150, 35);
            this.btnSendSunday.TabIndex = 56;
            this.btnSendSunday.Text = "None";
            this.btnSendSunday.UseVisualStyleBackColor = true;
            // 
            // btnSendM
            // 
            this.btnSendM.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendM.Location = new System.Drawing.Point(391, 494);
            this.btnSendM.Name = "btnSendM";
            this.btnSendM.Size = new System.Drawing.Size(170, 35);
            this.btnSendM.TabIndex = 55;
            this.btnSendM.Text = "報餐送出(無時段管控)";
            this.btnSendM.UseVisualStyleBackColor = true;
            this.btnSendM.Click += new System.EventHandler(this.btnSendM_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSelectAll.Location = new System.Drawing.Point(324, 367);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnSelectAll.Size = new System.Drawing.Size(50, 35);
            this.btnSelectAll.TabIndex = 54;
            this.btnSelectAll.Text = "全選";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // txtCompanyCellPhone
            // 
            this.txtCompanyCellPhone.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtCompanyCellPhone.Location = new System.Drawing.Point(97, 438);
            this.txtCompanyCellPhone.Name = "txtCompanyCellPhone";
            this.txtCompanyCellPhone.Size = new System.Drawing.Size(150, 27);
            this.txtCompanyCellPhone.TabIndex = 53;
            // 
            // txtCompanyTel
            // 
            this.txtCompanyTel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtCompanyTel.Location = new System.Drawing.Point(97, 405);
            this.txtCompanyTel.Name = "txtCompanyTel";
            this.txtCompanyTel.Size = new System.Drawing.Size(150, 27);
            this.txtCompanyTel.TabIndex = 52;
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtCompanyName.Location = new System.Drawing.Point(97, 372);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(150, 27);
            this.txtCompanyName.TabIndex = 51;
            // 
            // btnBentoTelChangeSave
            // 
            this.btnBentoTelChangeSave.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnBentoTelChangeSave.Location = new System.Drawing.Point(187, 471);
            this.btnBentoTelChangeSave.Name = "btnBentoTelChangeSave";
            this.btnBentoTelChangeSave.Size = new System.Drawing.Size(60, 25);
            this.btnBentoTelChangeSave.TabIndex = 50;
            this.btnBentoTelChangeSave.Text = "保存";
            this.btnBentoTelChangeSave.UseVisualStyleBackColor = true;
            this.btnBentoTelChangeSave.Click += new System.EventHandler(this.btnBentoTelChangeSave_Click);
            // 
            // btnBentoTelChange
            // 
            this.btnBentoTelChange.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnBentoTelChange.Location = new System.Drawing.Point(97, 471);
            this.btnBentoTelChange.Name = "btnBentoTelChange";
            this.btnBentoTelChange.Size = new System.Drawing.Size(60, 25);
            this.btnBentoTelChange.TabIndex = 49;
            this.btnBentoTelChange.Text = "修改";
            this.btnBentoTelChange.UseVisualStyleBackColor = true;
            this.btnBentoTelChange.Click += new System.EventHandler(this.btnBentoTelChange_Click);
            // 
            // lblCompanyTel
            // 
            this.lblCompanyTel.AutoSize = true;
            this.lblCompanyTel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCompanyTel.Location = new System.Drawing.Point(7, 375);
            this.lblCompanyTel.Name = "lblCompanyTel";
            this.lblCompanyTel.Size = new System.Drawing.Size(84, 19);
            this.lblCompanyTel.TabIndex = 45;
            this.lblCompanyTel.Text = "訂餐電話：";
            // 
            // rtbOrderTimeIllustrate
            // 
            this.rtbOrderTimeIllustrate.Enabled = false;
            this.rtbOrderTimeIllustrate.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtbOrderTimeIllustrate.Location = new System.Drawing.Point(502, 142);
            this.rtbOrderTimeIllustrate.Name = "rtbOrderTimeIllustrate";
            this.rtbOrderTimeIllustrate.Size = new System.Drawing.Size(274, 224);
            this.rtbOrderTimeIllustrate.TabIndex = 44;
            this.rtbOrderTimeIllustrate.Text = "注意：\n每日報餐時段，不可超過下述時間！\n超過規定時段，即不可報餐和取消.....\n中餐 09:50\n晚餐 14:50\n宵夜 21:50";
            // 
            // btnSanitary
            // 
            this.btnSanitary.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSanitary.Location = new System.Drawing.Point(270, 494);
            this.btnSanitary.Name = "btnSanitary";
            this.btnSanitary.Size = new System.Drawing.Size(115, 35);
            this.btnSanitary.TabIndex = 43;
            this.btnSanitary.Text = "聚豐-清潔人員";
            this.btnSanitary.UseVisualStyleBackColor = true;
            this.btnSanitary.Click += new System.EventHandler(this.btnSanitary_Click);
            // 
            // chkVegetableFood
            // 
            this.chkVegetableFood.AutoSize = true;
            this.chkVegetableFood.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkVegetableFood.Location = new System.Drawing.Point(324, 30);
            this.chkVegetableFood.Name = "chkVegetableFood";
            this.chkVegetableFood.Size = new System.Drawing.Size(58, 23);
            this.chkVegetableFood.TabIndex = 42;
            this.chkVegetableFood.Text = "素食";
            this.chkVegetableFood.UseVisualStyleBackColor = true;
            // 
            // lblVegetableFood
            // 
            this.lblVegetableFood.AutoSize = true;
            this.lblVegetableFood.Font = new System.Drawing.Font("微軟正黑體", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblVegetableFood.Location = new System.Drawing.Point(320, 68);
            this.lblVegetableFood.Name = "lblVegetableFood";
            this.lblVegetableFood.Size = new System.Drawing.Size(257, 24);
            this.lblVegetableFood.TabIndex = 41;
            this.lblVegetableFood.Text = "＊素食人員麻煩請另外報餐！";
            // 
            // rdoNightOrder
            // 
            this.rdoNightOrder.AutoSize = true;
            this.rdoNightOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoNightOrder.Location = new System.Drawing.Point(261, 29);
            this.rdoNightOrder.Name = "rdoNightOrder";
            this.rdoNightOrder.Size = new System.Drawing.Size(57, 23);
            this.rdoNightOrder.TabIndex = 38;
            this.rdoNightOrder.TabStop = true;
            this.rdoNightOrder.Text = "宵夜";
            this.rdoNightOrder.UseVisualStyleBackColor = true;
            // 
            // chklstName
            // 
            this.chklstName.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chklstName.FormattingEnabled = true;
            this.chklstName.Location = new System.Drawing.Point(6, 142);
            this.chklstName.MultiColumn = true;
            this.chklstName.Name = "chklstName";
            this.chklstName.Size = new System.Drawing.Size(490, 224);
            this.chklstName.TabIndex = 5;
            // 
            // btnSend
            // 
            this.btnSend.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSend.Location = new System.Drawing.Point(324, 408);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(105, 35);
            this.btnSend.TabIndex = 37;
            this.btnSend.Text = "報餐送出";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.Location = new System.Drawing.Point(380, 367);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnCancel.Size = new System.Drawing.Size(50, 35);
            this.btnCancel.TabIndex = 36;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboDepart
            // 
            this.cboDepart.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboDepart.FormattingEnabled = true;
            this.cboDepart.Location = new System.Drawing.Point(97, 68);
            this.cboDepart.Name = "cboDepart";
            this.cboDepart.Size = new System.Drawing.Size(121, 27);
            this.cboDepart.TabIndex = 5;
            this.cboDepart.SelectedIndexChanged += new System.EventHandler(this.cboDepart_SelectedIndexChanged);
            // 
            // lblSelectDeport
            // 
            this.lblSelectDeport.AutoSize = true;
            this.lblSelectDeport.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSelectDeport.Location = new System.Drawing.Point(6, 71);
            this.lblSelectDeport.Name = "lblSelectDeport";
            this.lblSelectDeport.Size = new System.Drawing.Size(99, 19);
            this.lblSelectDeport.TabIndex = 4;
            this.lblSelectDeport.Text = "請選擇部門：";
            // 
            // lblOrderPeople
            // 
            this.lblOrderPeople.AutoSize = true;
            this.lblOrderPeople.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOrderPeople.Location = new System.Drawing.Point(6, 114);
            this.lblOrderPeople.Name = "lblOrderPeople";
            this.lblOrderPeople.Size = new System.Drawing.Size(189, 19);
            this.lblOrderPeople.TabIndex = 3;
            this.lblOrderPeople.Text = "請勾選下列要報餐的人員：";
            // 
            // lblSelectOrder
            // 
            this.lblSelectOrder.AutoSize = true;
            this.lblSelectOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSelectOrder.Location = new System.Drawing.Point(6, 31);
            this.lblSelectOrder.Name = "lblSelectOrder";
            this.lblSelectOrder.Size = new System.Drawing.Size(129, 19);
            this.lblSelectOrder.TabIndex = 2;
            this.lblSelectOrder.Text = "請選擇報餐項目：";
            // 
            // rdoPmOrder
            // 
            this.rdoPmOrder.AutoSize = true;
            this.rdoPmOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoPmOrder.Location = new System.Drawing.Point(198, 29);
            this.rdoPmOrder.Name = "rdoPmOrder";
            this.rdoPmOrder.Size = new System.Drawing.Size(57, 23);
            this.rdoPmOrder.TabIndex = 1;
            this.rdoPmOrder.TabStop = true;
            this.rdoPmOrder.Text = "晚餐";
            this.rdoPmOrder.UseVisualStyleBackColor = true;
            // 
            // rdoAmOrder
            // 
            this.rdoAmOrder.AutoSize = true;
            this.rdoAmOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoAmOrder.Location = new System.Drawing.Point(135, 29);
            this.rdoAmOrder.Name = "rdoAmOrder";
            this.rdoAmOrder.Size = new System.Drawing.Size(57, 23);
            this.rdoAmOrder.TabIndex = 0;
            this.rdoAmOrder.TabStop = true;
            this.rdoAmOrder.Text = "中餐";
            this.rdoAmOrder.UseVisualStyleBackColor = true;
            // 
            // tpOrderRefer
            // 
            this.tpOrderRefer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tpOrderRefer.Controls.Add(this.btnDepartOrder);
            this.tpOrderRefer.Controls.Add(this.btnNewPeople1);
            this.tpOrderRefer.Controls.Add(this.lblInputName);
            this.tpOrderRefer.Controls.Add(this.txtInputName);
            this.tpOrderRefer.Controls.Add(this.btnNewPeople0);
            this.tpOrderRefer.Controls.Add(this.btnCancelOrderM);
            this.tpOrderRefer.Controls.Add(this.btnOrderStatistics);
            this.tpOrderRefer.Controls.Add(this.btnReferNoOrder);
            this.tpOrderRefer.Controls.Add(this.btnCancelOrder);
            this.tpOrderRefer.Controls.Add(this.lblOrderNumShow);
            this.tpOrderRefer.Controls.Add(this.lblOrderNum);
            this.tpOrderRefer.Controls.Add(this.dgvBentoDataShow);
            this.tpOrderRefer.Controls.Add(this.btnRefer);
            this.tpOrderRefer.Controls.Add(this.rdoReferNight);
            this.tpOrderRefer.Controls.Add(this.rdoReferPm);
            this.tpOrderRefer.Controls.Add(this.rdoReferAm);
            this.tpOrderRefer.Controls.Add(this.lblReferOrder);
            this.tpOrderRefer.Controls.Add(this.cboSelectDepartid);
            this.tpOrderRefer.Controls.Add(this.lblSelectDepartid);
            this.tpOrderRefer.Location = new System.Drawing.Point(4, 22);
            this.tpOrderRefer.Name = "tpOrderRefer";
            this.tpOrderRefer.Padding = new System.Windows.Forms.Padding(3);
            this.tpOrderRefer.Size = new System.Drawing.Size(792, 535);
            this.tpOrderRefer.TabIndex = 1;
            this.tpOrderRefer.Text = "每日訂餐查詢、取消";
            this.tpOrderRefer.UseVisualStyleBackColor = true;
            // 
            // btnNewPeople1
            // 
            this.btnNewPeople1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnNewPeople1.Location = new System.Drawing.Point(727, 386);
            this.btnNewPeople1.Name = "btnNewPeople1";
            this.btnNewPeople1.Size = new System.Drawing.Size(50, 35);
            this.btnNewPeople1.TabIndex = 60;
            this.btnNewPeople1.Text = "素";
            this.btnNewPeople1.UseVisualStyleBackColor = true;
            this.btnNewPeople1.Click += new System.EventHandler(this.btnNewPeople1_Click);
            // 
            // lblInputName
            // 
            this.lblInputName.AutoSize = true;
            this.lblInputName.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblInputName.Location = new System.Drawing.Point(492, 345);
            this.lblInputName.Name = "lblInputName";
            this.lblInputName.Size = new System.Drawing.Size(159, 19);
            this.lblInputName.TabIndex = 59;
            this.lblInputName.Text = "請輸入新進人員姓名：";
            // 
            // txtInputName
            // 
            this.txtInputName.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtInputName.Location = new System.Drawing.Point(657, 342);
            this.txtInputName.Name = "txtInputName";
            this.txtInputName.Size = new System.Drawing.Size(120, 27);
            this.txtInputName.TabIndex = 58;
            // 
            // btnNewPeople0
            // 
            this.btnNewPeople0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnNewPeople0.Location = new System.Drawing.Point(657, 386);
            this.btnNewPeople0.Name = "btnNewPeople0";
            this.btnNewPeople0.Size = new System.Drawing.Size(50, 35);
            this.btnNewPeople0.TabIndex = 57;
            this.btnNewPeople0.Text = "葷";
            this.btnNewPeople0.UseVisualStyleBackColor = true;
            this.btnNewPeople0.Click += new System.EventHandler(this.btnNewPeople0_Click);
            // 
            // btnCancelOrderM
            // 
            this.btnCancelOrderM.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancelOrderM.Location = new System.Drawing.Point(496, 215);
            this.btnCancelOrderM.Name = "btnCancelOrderM";
            this.btnCancelOrderM.Size = new System.Drawing.Size(170, 35);
            this.btnCancelOrderM.TabIndex = 46;
            this.btnCancelOrderM.Text = "取消訂餐(無時段管控)";
            this.btnCancelOrderM.UseVisualStyleBackColor = true;
            this.btnCancelOrderM.Click += new System.EventHandler(this.btnCancelOrderM_Click);
            // 
            // btnOrderStatistics
            // 
            this.btnOrderStatistics.BackColor = System.Drawing.Color.Crimson;
            this.btnOrderStatistics.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOrderStatistics.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOrderStatistics.Location = new System.Drawing.Point(475, 58);
            this.btnOrderStatistics.Name = "btnOrderStatistics";
            this.btnOrderStatistics.Size = new System.Drawing.Size(120, 35);
            this.btnOrderStatistics.TabIndex = 45;
            this.btnOrderStatistics.Text = "全廠報餐統計";
            this.btnOrderStatistics.UseVisualStyleBackColor = false;
            this.btnOrderStatistics.Click += new System.EventHandler(this.btnOrderStatistics_Click);
            // 
            // btnReferNoOrder
            // 
            this.btnReferNoOrder.BackColor = System.Drawing.Color.DarkMagenta;
            this.btnReferNoOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnReferNoOrder.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnReferNoOrder.Location = new System.Drawing.Point(475, 17);
            this.btnReferNoOrder.Name = "btnReferNoOrder";
            this.btnReferNoOrder.Size = new System.Drawing.Size(120, 35);
            this.btnReferNoOrder.TabIndex = 44;
            this.btnReferNoOrder.Text = "尚未報餐部門";
            this.btnReferNoOrder.UseVisualStyleBackColor = false;
            this.btnReferNoOrder.Click += new System.EventHandler(this.btnReferNoOrder_Click);
            // 
            // btnCancelOrder
            // 
            this.btnCancelOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancelOrder.Location = new System.Drawing.Point(496, 165);
            this.btnCancelOrder.Name = "btnCancelOrder";
            this.btnCancelOrder.Size = new System.Drawing.Size(80, 35);
            this.btnCancelOrder.TabIndex = 43;
            this.btnCancelOrder.Text = "取消訂餐";
            this.btnCancelOrder.UseVisualStyleBackColor = true;
            this.btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
            // 
            // lblOrderNumShow
            // 
            this.lblOrderNumShow.AutoSize = true;
            this.lblOrderNumShow.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOrderNumShow.Location = new System.Drawing.Point(54, 121);
            this.lblOrderNumShow.Name = "lblOrderNumShow";
            this.lblOrderNumShow.Size = new System.Drawing.Size(24, 19);
            this.lblOrderNumShow.TabIndex = 41;
            this.lblOrderNumShow.Text = "個";
            // 
            // lblOrderNum
            // 
            this.lblOrderNum.AutoSize = true;
            this.lblOrderNum.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOrderNum.Location = new System.Drawing.Point(6, 121);
            this.lblOrderNum.Name = "lblOrderNum";
            this.lblOrderNum.Size = new System.Drawing.Size(54, 19);
            this.lblOrderNum.TabIndex = 40;
            this.lblOrderNum.Text = "數量：";
            // 
            // dgvBentoDataShow
            // 
            this.dgvBentoDataShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBentoDataShow.Location = new System.Drawing.Point(10, 143);
            this.dgvBentoDataShow.Name = "dgvBentoDataShow";
            this.dgvBentoDataShow.RowTemplate.Height = 24;
            this.dgvBentoDataShow.Size = new System.Drawing.Size(460, 350);
            this.dgvBentoDataShow.TabIndex = 39;
            // 
            // btnRefer
            // 
            this.btnRefer.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRefer.Location = new System.Drawing.Point(298, 58);
            this.btnRefer.Name = "btnRefer";
            this.btnRefer.Size = new System.Drawing.Size(80, 35);
            this.btnRefer.TabIndex = 38;
            this.btnRefer.Text = "查詢";
            this.btnRefer.UseVisualStyleBackColor = true;
            this.btnRefer.Click += new System.EventHandler(this.btnRefer_Click);
            // 
            // rdoReferNight
            // 
            this.rdoReferNight.AutoSize = true;
            this.rdoReferNight.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoReferNight.Location = new System.Drawing.Point(297, 23);
            this.rdoReferNight.Name = "rdoReferNight";
            this.rdoReferNight.Size = new System.Drawing.Size(57, 23);
            this.rdoReferNight.TabIndex = 5;
            this.rdoReferNight.TabStop = true;
            this.rdoReferNight.Text = "宵夜";
            this.rdoReferNight.UseVisualStyleBackColor = true;
            // 
            // rdoReferPm
            // 
            this.rdoReferPm.AutoSize = true;
            this.rdoReferPm.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoReferPm.Location = new System.Drawing.Point(234, 23);
            this.rdoReferPm.Name = "rdoReferPm";
            this.rdoReferPm.Size = new System.Drawing.Size(57, 23);
            this.rdoReferPm.TabIndex = 4;
            this.rdoReferPm.TabStop = true;
            this.rdoReferPm.Text = "晚餐";
            this.rdoReferPm.UseVisualStyleBackColor = true;
            // 
            // rdoReferAm
            // 
            this.rdoReferAm.AutoSize = true;
            this.rdoReferAm.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoReferAm.Location = new System.Drawing.Point(171, 23);
            this.rdoReferAm.Name = "rdoReferAm";
            this.rdoReferAm.Size = new System.Drawing.Size(57, 23);
            this.rdoReferAm.TabIndex = 3;
            this.rdoReferAm.TabStop = true;
            this.rdoReferAm.Text = "中餐";
            this.rdoReferAm.UseVisualStyleBackColor = true;
            // 
            // lblReferOrder
            // 
            this.lblReferOrder.AutoSize = true;
            this.lblReferOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblReferOrder.Location = new System.Drawing.Point(6, 25);
            this.lblReferOrder.Name = "lblReferOrder";
            this.lblReferOrder.Size = new System.Drawing.Size(159, 19);
            this.lblReferOrder.TabIndex = 2;
            this.lblReferOrder.Text = "請選擇要查詢的餐別：";
            // 
            // cboSelectDepartid
            // 
            this.cboSelectDepartid.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboSelectDepartid.FormattingEnabled = true;
            this.cboSelectDepartid.Location = new System.Drawing.Point(171, 63);
            this.cboSelectDepartid.Name = "cboSelectDepartid";
            this.cboSelectDepartid.Size = new System.Drawing.Size(121, 27);
            this.cboSelectDepartid.TabIndex = 1;
            // 
            // lblSelectDepartid
            // 
            this.lblSelectDepartid.AutoSize = true;
            this.lblSelectDepartid.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSelectDepartid.Location = new System.Drawing.Point(6, 66);
            this.lblSelectDepartid.Name = "lblSelectDepartid";
            this.lblSelectDepartid.Size = new System.Drawing.Size(159, 19);
            this.lblSelectDepartid.TabIndex = 0;
            this.lblSelectDepartid.Text = "請選擇要查詢的部門：";
            // 
            // tpAccount
            // 
            this.tpAccount.Controls.Add(this.btnSendToExcel);
            this.tpAccount.Controls.Add(this.rdoSelectDepart);
            this.tpAccount.Controls.Add(this.cboSelectDepart);
            this.tpAccount.Controls.Add(this.btnReferOrderNight);
            this.tpAccount.Controls.Add(this.btnReferOrderPm);
            this.tpAccount.Controls.Add(this.btnReferOrderAm);
            this.tpAccount.Controls.Add(this.btnSavePrice);
            this.tpAccount.Controls.Add(this.btnChangePrice);
            this.tpAccount.Controls.Add(this.txtTotalPriceAll);
            this.tpAccount.Controls.Add(this.lblTotalPriceAll);
            this.tpAccount.Controls.Add(this.txtTotalPrice1);
            this.tpAccount.Controls.Add(this.txtTotalPrice0);
            this.tpAccount.Controls.Add(this.lblTotalPrice1);
            this.tpAccount.Controls.Add(this.lblTotalPrice0);
            this.tpAccount.Controls.Add(this.txtTotalOrder1);
            this.tpAccount.Controls.Add(this.txtTotalOrder0);
            this.tpAccount.Controls.Add(this.lblTotalOrder1);
            this.tpAccount.Controls.Add(this.lblTotalOrder0);
            this.tpAccount.Controls.Add(this.txtBentoPrice);
            this.tpAccount.Controls.Add(this.lblBentoPrice);
            this.tpAccount.Controls.Add(this.dgvReferOrderAll);
            this.tpAccount.Controls.Add(this.rdoAllCompany);
            this.tpAccount.Controls.Add(this.rdoTwoCompany);
            this.tpAccount.Controls.Add(this.rdOneCompany);
            this.tpAccount.Controls.Add(this.btnReferOrderAll);
            this.tpAccount.Controls.Add(this.lblSelectReferCompany);
            this.tpAccount.Controls.Add(this.lblSelectReferDate);
            this.tpAccount.Controls.Add(this.dtpEndDate);
            this.tpAccount.Controls.Add(this.dtpStartDate);
            this.tpAccount.Location = new System.Drawing.Point(4, 22);
            this.tpAccount.Name = "tpAccount";
            this.tpAccount.Padding = new System.Windows.Forms.Padding(3);
            this.tpAccount.Size = new System.Drawing.Size(792, 535);
            this.tpAccount.TabIndex = 2;
            this.tpAccount.Text = "月結統計";
            this.tpAccount.UseVisualStyleBackColor = true;
            // 
            // btnSendToExcel
            // 
            this.btnSendToExcel.BackColor = System.Drawing.Color.Green;
            this.btnSendToExcel.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendToExcel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnSendToExcel.Location = new System.Drawing.Point(380, 313);
            this.btnSendToExcel.Name = "btnSendToExcel";
            this.btnSendToExcel.Size = new System.Drawing.Size(80, 35);
            this.btnSendToExcel.TabIndex = 77;
            this.btnSendToExcel.Text = "滙出";
            this.btnSendToExcel.UseVisualStyleBackColor = false;
            this.btnSendToExcel.Click += new System.EventHandler(this.btnSendToExcel_Click);
            // 
            // rdoSelectDepart
            // 
            this.rdoSelectDepart.AutoSize = true;
            this.rdoSelectDepart.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoSelectDepart.Location = new System.Drawing.Point(390, 85);
            this.rdoSelectDepart.Name = "rdoSelectDepart";
            this.rdoSelectDepart.Size = new System.Drawing.Size(72, 23);
            this.rdoSelectDepart.TabIndex = 76;
            this.rdoSelectDepart.TabStop = true;
            this.rdoSelectDepart.Text = "依部門";
            this.rdoSelectDepart.UseVisualStyleBackColor = true;
            // 
            // cboSelectDepart
            // 
            this.cboSelectDepart.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboSelectDepart.FormattingEnabled = true;
            this.cboSelectDepart.Location = new System.Drawing.Point(466, 84);
            this.cboSelectDepart.Name = "cboSelectDepart";
            this.cboSelectDepart.Size = new System.Drawing.Size(121, 27);
            this.cboSelectDepart.TabIndex = 75;
            // 
            // btnReferOrderNight
            // 
            this.btnReferOrderNight.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnReferOrderNight.Location = new System.Drawing.Point(665, 29);
            this.btnReferOrderNight.Name = "btnReferOrderNight";
            this.btnReferOrderNight.Size = new System.Drawing.Size(80, 35);
            this.btnReferOrderNight.TabIndex = 74;
            this.btnReferOrderNight.Text = "宵夜";
            this.btnReferOrderNight.UseVisualStyleBackColor = true;
            this.btnReferOrderNight.Click += new System.EventHandler(this.btnReferOrderNight_Click);
            // 
            // btnReferOrderPm
            // 
            this.btnReferOrderPm.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnReferOrderPm.Location = new System.Drawing.Point(579, 29);
            this.btnReferOrderPm.Name = "btnReferOrderPm";
            this.btnReferOrderPm.Size = new System.Drawing.Size(80, 35);
            this.btnReferOrderPm.TabIndex = 73;
            this.btnReferOrderPm.Text = "晚餐";
            this.btnReferOrderPm.UseVisualStyleBackColor = true;
            this.btnReferOrderPm.Click += new System.EventHandler(this.btnReferOrderPm_Click);
            // 
            // btnReferOrderAm
            // 
            this.btnReferOrderAm.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnReferOrderAm.Location = new System.Drawing.Point(493, 29);
            this.btnReferOrderAm.Name = "btnReferOrderAm";
            this.btnReferOrderAm.Size = new System.Drawing.Size(80, 35);
            this.btnReferOrderAm.TabIndex = 72;
            this.btnReferOrderAm.Text = "中餐";
            this.btnReferOrderAm.UseVisualStyleBackColor = true;
            this.btnReferOrderAm.Click += new System.EventHandler(this.btnReferOrderAm_Click);
            // 
            // btnSavePrice
            // 
            this.btnSavePrice.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSavePrice.Location = new System.Drawing.Point(608, 141);
            this.btnSavePrice.Name = "btnSavePrice";
            this.btnSavePrice.Size = new System.Drawing.Size(60, 25);
            this.btnSavePrice.TabIndex = 71;
            this.btnSavePrice.Text = "保存";
            this.btnSavePrice.UseVisualStyleBackColor = true;
            this.btnSavePrice.Click += new System.EventHandler(this.btnSavePrice_Click);
            // 
            // btnChangePrice
            // 
            this.btnChangePrice.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnChangePrice.Location = new System.Drawing.Point(542, 141);
            this.btnChangePrice.Name = "btnChangePrice";
            this.btnChangePrice.Size = new System.Drawing.Size(60, 25);
            this.btnChangePrice.TabIndex = 70;
            this.btnChangePrice.Text = "修改";
            this.btnChangePrice.UseVisualStyleBackColor = true;
            this.btnChangePrice.Click += new System.EventHandler(this.btnChangePrice_Click);
            // 
            // txtTotalPriceAll
            // 
            this.txtTotalPriceAll.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTotalPriceAll.Location = new System.Drawing.Point(575, 257);
            this.txtTotalPriceAll.Name = "txtTotalPriceAll";
            this.txtTotalPriceAll.ReadOnly = true;
            this.txtTotalPriceAll.Size = new System.Drawing.Size(120, 33);
            this.txtTotalPriceAll.TabIndex = 69;
            this.txtTotalPriceAll.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTotalPriceAll
            // 
            this.lblTotalPriceAll.AutoSize = true;
            this.lblTotalPriceAll.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalPriceAll.Location = new System.Drawing.Point(474, 260);
            this.lblTotalPriceAll.Name = "lblTotalPriceAll";
            this.lblTotalPriceAll.Size = new System.Drawing.Size(105, 24);
            this.lblTotalPriceAll.TabIndex = 68;
            this.lblTotalPriceAll.Text = "金額總計：";
            // 
            // txtTotalPrice1
            // 
            this.txtTotalPrice1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTotalPrice1.Location = new System.Drawing.Point(625, 214);
            this.txtTotalPrice1.Name = "txtTotalPrice1";
            this.txtTotalPrice1.ReadOnly = true;
            this.txtTotalPrice1.Size = new System.Drawing.Size(70, 27);
            this.txtTotalPrice1.TabIndex = 67;
            this.txtTotalPrice1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTotalPrice0
            // 
            this.txtTotalPrice0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTotalPrice0.Location = new System.Drawing.Point(625, 177);
            this.txtTotalPrice0.Name = "txtTotalPrice0";
            this.txtTotalPrice0.ReadOnly = true;
            this.txtTotalPrice0.Size = new System.Drawing.Size(70, 27);
            this.txtTotalPrice0.TabIndex = 66;
            this.txtTotalPrice0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTotalPrice1
            // 
            this.lblTotalPrice1.AutoSize = true;
            this.lblTotalPrice1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalPrice1.Location = new System.Drawing.Point(540, 217);
            this.lblTotalPrice1.Name = "lblTotalPrice1";
            this.lblTotalPrice1.Size = new System.Drawing.Size(79, 19);
            this.lblTotalPrice1.TabIndex = 65;
            this.lblTotalPrice1.Text = "金額(素)：";
            // 
            // lblTotalPrice0
            // 
            this.lblTotalPrice0.AutoSize = true;
            this.lblTotalPrice0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalPrice0.Location = new System.Drawing.Point(540, 180);
            this.lblTotalPrice0.Name = "lblTotalPrice0";
            this.lblTotalPrice0.Size = new System.Drawing.Size(79, 19);
            this.lblTotalPrice0.TabIndex = 64;
            this.lblTotalPrice0.Text = "金額(葷)：";
            // 
            // txtTotalOrder1
            // 
            this.txtTotalOrder1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTotalOrder1.Location = new System.Drawing.Point(466, 214);
            this.txtTotalOrder1.Name = "txtTotalOrder1";
            this.txtTotalOrder1.ReadOnly = true;
            this.txtTotalOrder1.Size = new System.Drawing.Size(70, 27);
            this.txtTotalOrder1.TabIndex = 63;
            this.txtTotalOrder1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTotalOrder0
            // 
            this.txtTotalOrder0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTotalOrder0.Location = new System.Drawing.Point(466, 177);
            this.txtTotalOrder0.Name = "txtTotalOrder0";
            this.txtTotalOrder0.ReadOnly = true;
            this.txtTotalOrder0.Size = new System.Drawing.Size(70, 27);
            this.txtTotalOrder0.TabIndex = 62;
            this.txtTotalOrder0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTotalOrder1
            // 
            this.lblTotalOrder1.AutoSize = true;
            this.lblTotalOrder1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalOrder1.Location = new System.Drawing.Point(381, 217);
            this.lblTotalOrder1.Name = "lblTotalOrder1";
            this.lblTotalOrder1.Size = new System.Drawing.Size(79, 19);
            this.lblTotalOrder1.TabIndex = 61;
            this.lblTotalOrder1.Text = "數量(素)：";
            // 
            // lblTotalOrder0
            // 
            this.lblTotalOrder0.AutoSize = true;
            this.lblTotalOrder0.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalOrder0.Location = new System.Drawing.Point(381, 180);
            this.lblTotalOrder0.Name = "lblTotalOrder0";
            this.lblTotalOrder0.Size = new System.Drawing.Size(79, 19);
            this.lblTotalOrder0.TabIndex = 60;
            this.lblTotalOrder0.Text = "數量(葷)：";
            // 
            // txtBentoPrice
            // 
            this.txtBentoPrice.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtBentoPrice.Location = new System.Drawing.Point(466, 141);
            this.txtBentoPrice.Name = "txtBentoPrice";
            this.txtBentoPrice.ReadOnly = true;
            this.txtBentoPrice.Size = new System.Drawing.Size(70, 27);
            this.txtBentoPrice.TabIndex = 59;
            this.txtBentoPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBentoPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNum_keyPress);
            // 
            // lblBentoPrice
            // 
            this.lblBentoPrice.AutoSize = true;
            this.lblBentoPrice.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBentoPrice.Location = new System.Drawing.Point(406, 144);
            this.lblBentoPrice.Name = "lblBentoPrice";
            this.lblBentoPrice.Size = new System.Drawing.Size(54, 19);
            this.lblBentoPrice.TabIndex = 58;
            this.lblBentoPrice.Text = "單價：";
            // 
            // dgvReferOrderAll
            // 
            this.dgvReferOrderAll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReferOrderAll.Location = new System.Drawing.Point(6, 141);
            this.dgvReferOrderAll.Name = "dgvReferOrderAll";
            this.dgvReferOrderAll.RowTemplate.Height = 24;
            this.dgvReferOrderAll.Size = new System.Drawing.Size(320, 380);
            this.dgvReferOrderAll.TabIndex = 57;
            // 
            // rdoAllCompany
            // 
            this.rdoAllCompany.AutoSize = true;
            this.rdoAllCompany.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoAllCompany.Location = new System.Drawing.Point(327, 86);
            this.rdoAllCompany.Name = "rdoAllCompany";
            this.rdoAllCompany.Size = new System.Drawing.Size(57, 23);
            this.rdoAllCompany.TabIndex = 56;
            this.rdoAllCompany.TabStop = true;
            this.rdoAllCompany.Text = "全部";
            this.rdoAllCompany.UseVisualStyleBackColor = true;
            // 
            // rdoTwoCompany
            // 
            this.rdoTwoCompany.AutoSize = true;
            this.rdoTwoCompany.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoTwoCompany.Location = new System.Drawing.Point(264, 86);
            this.rdoTwoCompany.Name = "rdoTwoCompany";
            this.rdoTwoCompany.Size = new System.Drawing.Size(57, 23);
            this.rdoTwoCompany.TabIndex = 55;
            this.rdoTwoCompany.TabStop = true;
            this.rdoTwoCompany.Text = "二廠";
            this.rdoTwoCompany.UseVisualStyleBackColor = true;
            // 
            // rdOneCompany
            // 
            this.rdOneCompany.AutoSize = true;
            this.rdOneCompany.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdOneCompany.Location = new System.Drawing.Point(201, 86);
            this.rdOneCompany.Name = "rdOneCompany";
            this.rdOneCompany.Size = new System.Drawing.Size(57, 23);
            this.rdOneCompany.TabIndex = 54;
            this.rdOneCompany.TabStop = true;
            this.rdOneCompany.Text = "一廠";
            this.rdOneCompany.UseVisualStyleBackColor = true;
            // 
            // btnReferOrderAll
            // 
            this.btnReferOrderAll.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnReferOrderAll.Location = new System.Drawing.Point(407, 29);
            this.btnReferOrderAll.Name = "btnReferOrderAll";
            this.btnReferOrderAll.Size = new System.Drawing.Size(80, 35);
            this.btnReferOrderAll.TabIndex = 53;
            this.btnReferOrderAll.Text = "全部";
            this.btnReferOrderAll.UseVisualStyleBackColor = true;
            this.btnReferOrderAll.Click += new System.EventHandler(this.btnReferOrder_Click);
            // 
            // lblSelectReferCompany
            // 
            this.lblSelectReferCompany.AutoSize = true;
            this.lblSelectReferCompany.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSelectReferCompany.Location = new System.Drawing.Point(21, 88);
            this.lblSelectReferCompany.Name = "lblSelectReferCompany";
            this.lblSelectReferCompany.Size = new System.Drawing.Size(174, 19);
            this.lblSelectReferCompany.TabIndex = 46;
            this.lblSelectReferCompany.Text = "請選擇要查詢的公司別：";
            // 
            // lblSelectReferDate
            // 
            this.lblSelectReferDate.AutoSize = true;
            this.lblSelectReferDate.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSelectReferDate.Location = new System.Drawing.Point(6, 20);
            this.lblSelectReferDate.Name = "lblSelectReferDate";
            this.lblSelectReferDate.Size = new System.Drawing.Size(189, 19);
            this.lblSelectReferDate.TabIndex = 3;
            this.lblSelectReferDate.Text = "請選擇要查詢的日期區間：";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtpEndDate.Location = new System.Drawing.Point(201, 47);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(200, 27);
            this.dtpEndDate.TabIndex = 1;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtpStartDate.Location = new System.Drawing.Point(201, 14);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(200, 27);
            this.dtpStartDate.TabIndex = 0;
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDateTime.Location = new System.Drawing.Point(403, 9);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(84, 19);
            this.lblDateTime.TabIndex = 1;
            this.lblDateTime.Text = "目前時間：";
            // 
            // lblDateTimeShow
            // 
            this.lblDateTimeShow.AutoSize = true;
            this.lblDateTimeShow.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDateTimeShow.Location = new System.Drawing.Point(478, 9);
            this.lblDateTimeShow.Name = "lblDateTimeShow";
            this.lblDateTimeShow.Size = new System.Drawing.Size(114, 19);
            this.lblDateTimeShow.TabIndex = 2;
            this.lblDateTimeShow.Text = "DateTimeShow";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUserName.Location = new System.Drawing.Point(681, 9);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(84, 19);
            this.lblUserName.TabIndex = 3;
            this.lblUserName.Text = "登記人員：";
            // 
            // lblUserNameShow
            // 
            this.lblUserNameShow.AutoSize = true;
            this.lblUserNameShow.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUserNameShow.Location = new System.Drawing.Point(756, 9);
            this.lblUserNameShow.Name = "lblUserNameShow";
            this.lblUserNameShow.Size = new System.Drawing.Size(122, 19);
            this.lblUserNameShow.TabIndex = 4;
            this.lblUserNameShow.Text = "UserNameShow";
            // 
            // tmrDateTime
            // 
            this.tmrDateTime.Tick += new System.EventHandler(this.tmrDateTime_Tick);
            // 
            // btnDepartOrder
            // 
            this.btnDepartOrder.BackColor = System.Drawing.Color.DarkViolet;
            this.btnDepartOrder.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDepartOrder.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDepartOrder.Location = new System.Drawing.Point(475, 99);
            this.btnDepartOrder.Name = "btnDepartOrder";
            this.btnDepartOrder.Size = new System.Drawing.Size(120, 35);
            this.btnDepartOrder.TabIndex = 61;
            this.btnDepartOrder.Text = "各課報餐統計";
            this.btnDepartOrder.UseVisualStyleBackColor = false;
            this.btnDepartOrder.Click += new System.EventHandler(this.btnDepartOrder_Click);
            // 
            // BentoOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(824, 601);
            this.Controls.Add(this.lblUserNameShow);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblDateTimeShow);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.tabBentoOrder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BentoOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "長鴻電子-每日報餐登記表 v1.1";
            this.Load += new System.EventHandler(this.BentoOrder_Load);
            this.tabBentoOrder.ResumeLayout(false);
            this.tpEveryDayOrder.ResumeLayout(false);
            this.tpEveryDayOrder.PerformLayout();
            this.tpOrderRefer.ResumeLayout(false);
            this.tpOrderRefer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBentoDataShow)).EndInit();
            this.tpAccount.ResumeLayout(false);
            this.tpAccount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReferOrderAll)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabPage tpEveryDayOrder;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblDateTimeShow;
        private System.Windows.Forms.Label lblUserName;
        public System.Windows.Forms.Label lblUserNameShow;
        public System.Windows.Forms.TabControl tabBentoOrder;
        private System.Windows.Forms.TabPage tpOrderRefer;
        private System.Windows.Forms.Timer tmrDateTime;
        private System.Windows.Forms.Label lblSelectOrder;
        private System.Windows.Forms.RadioButton rdoPmOrder;
        private System.Windows.Forms.RadioButton rdoAmOrder;
        private System.Windows.Forms.Label lblOrderPeople;
        private System.Windows.Forms.ComboBox cboDepart;
        private System.Windows.Forms.Label lblSelectDeport;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckedListBox chklstName;
        private System.Windows.Forms.RadioButton rdoNightOrder;
        private System.Windows.Forms.TabPage tpAccount;
        private System.Windows.Forms.Label lblVegetableFood;
        private System.Windows.Forms.CheckBox chkVegetableFood;
        public System.Windows.Forms.Button btnSanitary;
        private System.Windows.Forms.RadioButton rdoReferNight;
        private System.Windows.Forms.RadioButton rdoReferPm;
        private System.Windows.Forms.RadioButton rdoReferAm;
        private System.Windows.Forms.Label lblReferOrder;
        private System.Windows.Forms.ComboBox cboSelectDepartid;
        private System.Windows.Forms.Label lblSelectDepartid;
        private System.Windows.Forms.Button btnRefer;
        private System.Windows.Forms.DataGridView dgvBentoDataShow;
        private System.Windows.Forms.Label lblOrderNumShow;
        private System.Windows.Forms.Label lblOrderNum;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.RichTextBox rtbOrderTimeIllustrate;
        private System.Windows.Forms.Button btnReferNoOrder;
        private System.Windows.Forms.Label lblCompanyTel;
        public System.Windows.Forms.Button btnBentoTelChange;
        public System.Windows.Forms.Button btnBentoTelChangeSave;
        private System.Windows.Forms.TextBox txtCompanyCellPhone;
        private System.Windows.Forms.TextBox txtCompanyTel;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblSelectReferDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblSelectReferCompany;
        private System.Windows.Forms.Button btnReferOrderAll;
        private System.Windows.Forms.RadioButton rdoAllCompany;
        private System.Windows.Forms.RadioButton rdoTwoCompany;
        private System.Windows.Forms.RadioButton rdOneCompany;
        private System.Windows.Forms.Button btnOrderStatistics;
        private System.Windows.Forms.DataGridView dgvReferOrderAll;
        private System.Windows.Forms.Label lblBentoPrice;
        private System.Windows.Forms.TextBox txtBentoPrice;
        private System.Windows.Forms.TextBox txtTotalPriceAll;
        private System.Windows.Forms.Label lblTotalPriceAll;
        private System.Windows.Forms.TextBox txtTotalPrice1;
        private System.Windows.Forms.TextBox txtTotalPrice0;
        private System.Windows.Forms.Label lblTotalPrice1;
        private System.Windows.Forms.Label lblTotalPrice0;
        private System.Windows.Forms.TextBox txtTotalOrder1;
        private System.Windows.Forms.TextBox txtTotalOrder0;
        private System.Windows.Forms.Label lblTotalOrder1;
        private System.Windows.Forms.Label lblTotalOrder0;
        public System.Windows.Forms.Button btnSavePrice;
        public System.Windows.Forms.Button btnChangePrice;
        public System.Windows.Forms.Button btnCancelOrderM;
        private System.Windows.Forms.Button btnReferOrderAm;
        private System.Windows.Forms.Button btnReferOrderNight;
        private System.Windows.Forms.Button btnReferOrderPm;
        private System.Windows.Forms.ComboBox cboSelectDepart;
        private System.Windows.Forms.RadioButton rdoSelectDepart;
        public System.Windows.Forms.Button btnSendM;
        private System.Windows.Forms.Button btnSendToExcel;
        private System.Windows.Forms.Button btnSendSunday;
        private System.Windows.Forms.Label lblInputName;
        public System.Windows.Forms.Button btnNewPeople1;
        public System.Windows.Forms.Button btnNewPeople0;
        public System.Windows.Forms.TextBox txtInputName;
        private System.Windows.Forms.Button btnDepartOrder;
    }
}

