namespace IntelligentEquipmentCenter.UiForm
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.PublicPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.txtTitleLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripTimeLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_max = new System.Windows.Forms.Button();
            this.btn_min = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_userex = new Sunny.UI.UIButton();
            this.btn_config = new Sunny.UI.UIButton();
            this.btn_datafind = new Sunny.UI.UIButton();
            this.btn_user = new Sunny.UI.UIButton();
            this.btn_armres = new Sunny.UI.UIButton();
            this.btn_run = new Sunny.UI.UIButton();
            this.btn_exit = new Sunny.UI.UIButton();
            this.lt_PLC = new IntelligentEquipmentCenter.UControl.LightStat();
            this.lt_mes = new IntelligentEquipmentCenter.UControl.LightStat();
            this.lt_vision = new IntelligentEquipmentCenter.UControl.LightStat();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // PublicPanel
            // 
            this.PublicPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PublicPanel.Location = new System.Drawing.Point(3, 71);
            this.PublicPanel.Margin = new System.Windows.Forms.Padding(2);
            this.PublicPanel.Name = "PublicPanel";
            this.PublicPanel.Size = new System.Drawing.Size(1317, 698);
            this.PublicPanel.TabIndex = 12;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.PublicPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1323, 840);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1321, 67);
            this.tableLayoutPanel2.TabIndex = 13;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 5;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.lt_PLC, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.lt_mes, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.lt_vision, 2, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(439, 65);
            this.tableLayoutPanel7.TabIndex = 6;
            this.tableLayoutPanel7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Frm_MouseDown);
            this.tableLayoutPanel7.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Frm_MouseMove);
            this.tableLayoutPanel7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Frm_MouseUp);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.txtTitleLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(441, 1);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(439, 65);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // txtTitleLabel
            // 
            this.txtTitleLabel.AutoSize = true;
            this.txtTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTitleLabel.Font = new System.Drawing.Font("宋体", 24F);
            this.txtTitleLabel.ForeColor = System.Drawing.Color.White;
            this.txtTitleLabel.Location = new System.Drawing.Point(3, 1);
            this.txtTitleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtTitleLabel.Name = "txtTitleLabel";
            this.txtTitleLabel.Size = new System.Drawing.Size(433, 42);
            this.txtTitleLabel.TabIndex = 0;
            this.txtTitleLabel.Text = "丝杆打螺丝";
            this.txtTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.txtTitleLabel.DoubleClick += new System.EventHandler(this.txtTitleLabel_Click);
            this.txtTitleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Frm_MouseDown);
            this.txtTitleLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Frm_MouseMove);
            this.txtTitleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Frm_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(433, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "版本号:    1.0               ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel5.Controls.Add(this.toolStripTimeLabel, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(884, 4);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(433, 59);
            this.tableLayoutPanel5.TabIndex = 7;
            // 
            // toolStripTimeLabel
            // 
            this.toolStripTimeLabel.AutoSize = true;
            this.toolStripTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripTimeLabel.Font = new System.Drawing.Font("宋体", 14F);
            this.toolStripTimeLabel.ForeColor = System.Drawing.Color.White;
            this.toolStripTimeLabel.Location = new System.Drawing.Point(3, 20);
            this.toolStripTimeLabel.Margin = new System.Windows.Forms.Padding(3, 20, 3, 0);
            this.toolStripTimeLabel.Name = "toolStripTimeLabel";
            this.toolStripTimeLabel.Size = new System.Drawing.Size(297, 39);
            this.toolStripTimeLabel.TabIndex = 7;
            this.toolStripTimeLabel.Text = "00:00:00";
            this.toolStripTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel6.Controls.Add(this.btn_close, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.btn_max, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.btn_min, 0, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(338, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(95, 32);
            this.tableLayoutPanel6.TabIndex = 8;
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btn_close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_close.BackgroundImage")));
            this.btn_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_close.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_close.Location = new System.Drawing.Point(62, 0);
            this.btn_close.Margin = new System.Windows.Forms.Padding(0);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(33, 32);
            this.btn_close.TabIndex = 0;
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_max
            // 
            this.btn_max.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btn_max.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_max.BackgroundImage")));
            this.btn_max.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_max.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_max.Location = new System.Drawing.Point(31, 0);
            this.btn_max.Margin = new System.Windows.Forms.Padding(0);
            this.btn_max.Name = "btn_max";
            this.btn_max.Size = new System.Drawing.Size(31, 32);
            this.btn_max.TabIndex = 1;
            this.btn_max.UseVisualStyleBackColor = false;
            this.btn_max.Click += new System.EventHandler(this.btn_max_Click);
            // 
            // btn_min
            // 
            this.btn_min.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btn_min.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_min.BackgroundImage")));
            this.btn_min.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_min.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_min.FlatAppearance.BorderColor = System.Drawing.SystemColors.HotTrack;
            this.btn_min.Location = new System.Drawing.Point(0, 0);
            this.btn_min.Margin = new System.Windows.Forms.Padding(0);
            this.btn_min.Name = "btn_min";
            this.btn_min.Size = new System.Drawing.Size(31, 32);
            this.btn_min.TabIndex = 2;
            this.btn_min.UseVisualStyleBackColor = false;
            this.btn_min.Click += new System.EventHandler(this.btn_min_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 10;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.btn_userex, 5, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_config, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_datafind, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_user, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_armres, 6, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_run, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_exit, 8, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 775);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1315, 61);
            this.tableLayoutPanel4.TabIndex = 14;
            // 
            // btn_userex
            // 
            this.btn_userex.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_userex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_userex.FillColor = System.Drawing.Color.Olive;
            this.btn_userex.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_userex.Location = new System.Drawing.Point(587, 7);
            this.btn_userex.Margin = new System.Windows.Forms.Padding(7);
            this.btn_userex.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_userex.Name = "btn_userex";
            this.btn_userex.Radius = 14;
            this.btn_userex.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn_userex.Size = new System.Drawing.Size(106, 47);
            this.btn_userex.TabIndex = 17;
            this.btn_userex.Text = "用户登录";
            this.btn_userex.TipsFont = new System.Drawing.Font("宋体", 12F);
            this.btn_userex.Click += new System.EventHandler(this.btn_userex_Click);
            // 
            // btn_config
            // 
            this.btn_config.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_config.FillColor = System.Drawing.Color.Olive;
            this.btn_config.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_config.Location = new System.Drawing.Point(227, 7);
            this.btn_config.Margin = new System.Windows.Forms.Padding(7);
            this.btn_config.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_config.Name = "btn_config";
            this.btn_config.Radius = 14;
            this.btn_config.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn_config.Size = new System.Drawing.Size(106, 47);
            this.btn_config.TabIndex = 14;
            this.btn_config.Text = "配置信息";
            this.btn_config.TipsFont = new System.Drawing.Font("宋体", 12F);
            this.btn_config.Click += new System.EventHandler(this.btn_config_Click);
            // 
            // btn_datafind
            // 
            this.btn_datafind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_datafind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_datafind.FillColor = System.Drawing.Color.Olive;
            this.btn_datafind.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_datafind.Location = new System.Drawing.Point(347, 7);
            this.btn_datafind.Margin = new System.Windows.Forms.Padding(7);
            this.btn_datafind.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_datafind.Name = "btn_datafind";
            this.btn_datafind.Radius = 14;
            this.btn_datafind.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn_datafind.Size = new System.Drawing.Size(106, 47);
            this.btn_datafind.TabIndex = 14;
            this.btn_datafind.Text = "数据查询";
            this.btn_datafind.TipsFont = new System.Drawing.Font("宋体", 12F);
            this.btn_datafind.Click += new System.EventHandler(this.btn_datafind_Click);
            // 
            // btn_user
            // 
            this.btn_user.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_user.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_user.FillColor = System.Drawing.Color.Olive;
            this.btn_user.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_user.Location = new System.Drawing.Point(467, 7);
            this.btn_user.Margin = new System.Windows.Forms.Padding(7);
            this.btn_user.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_user.Name = "btn_user";
            this.btn_user.Radius = 14;
            this.btn_user.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn_user.Size = new System.Drawing.Size(106, 47);
            this.btn_user.TabIndex = 14;
            this.btn_user.Text = "用户管理";
            this.btn_user.TipsFont = new System.Drawing.Font("宋体", 12F);
            this.btn_user.Click += new System.EventHandler(this.btn_user_Click);
            // 
            // btn_armres
            // 
            this.btn_armres.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_armres.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_armres.FillColor = System.Drawing.Color.Olive;
            this.btn_armres.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_armres.Location = new System.Drawing.Point(707, 7);
            this.btn_armres.Margin = new System.Windows.Forms.Padding(7);
            this.btn_armres.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_armres.Name = "btn_armres";
            this.btn_armres.Radius = 14;
            this.btn_armres.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn_armres.Size = new System.Drawing.Size(106, 47);
            this.btn_armres.TabIndex = 14;
            this.btn_armres.Text = "报警复位";
            this.btn_armres.TipsFont = new System.Drawing.Font("宋体", 12F);
            // 
            // btn_run
            // 
            this.btn_run.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_run.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_run.FillColor = System.Drawing.Color.Olive;
            this.btn_run.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_run.Location = new System.Drawing.Point(107, 7);
            this.btn_run.Margin = new System.Windows.Forms.Padding(7);
            this.btn_run.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_run.Name = "btn_run";
            this.btn_run.Radius = 14;
            this.btn_run.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn_run.Size = new System.Drawing.Size(106, 47);
            this.btn_run.TabIndex = 14;
            this.btn_run.Text = "主界面";
            this.btn_run.TipsFont = new System.Drawing.Font("宋体", 12F);
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_exit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_exit.FillColor = System.Drawing.Color.Red;
            this.btn_exit.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_exit.Location = new System.Drawing.Point(1030, 3);
            this.btn_exit.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Radius = 50;
            this.btn_exit.Size = new System.Drawing.Size(74, 55);
            this.btn_exit.TabIndex = 15;
            this.btn_exit.Text = "退出";
            this.btn_exit.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // lt_PLC
            // 
            this.lt_PLC.ForeColor = System.Drawing.Color.White;
            this.lt_PLC.lightMode = false;
            this.lt_PLC.Location = new System.Drawing.Point(3, 3);
            this.lt_PLC.Name = "lt_PLC";
            this.lt_PLC.Size = new System.Drawing.Size(46, 59);
            this.lt_PLC.TabIndex = 0;
            this.lt_PLC.TitleName = "PLC";
            // 
            // lt_mes
            // 
            this.lt_mes.ForeColor = System.Drawing.Color.White;
            this.lt_mes.lightMode = false;
            this.lt_mes.Location = new System.Drawing.Point(63, 3);
            this.lt_mes.Name = "lt_mes";
            this.lt_mes.Size = new System.Drawing.Size(54, 59);
            this.lt_mes.TabIndex = 1;
            this.lt_mes.TitleName = "MES";
            // 
            // lt_vision
            // 
            this.lt_vision.ForeColor = System.Drawing.Color.White;
            this.lt_vision.lightMode = false;
            this.lt_vision.Location = new System.Drawing.Point(123, 3);
            this.lt_vision.Name = "lt_vision";
            this.lt_vision.Size = new System.Drawing.Size(54, 59);
            this.lt_vision.TabIndex = 2;
            this.lt_vision.TitleName = "视觉";
            this.lt_vision.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1323, 840);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "智能装备中心 ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PublicPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label txtTitleLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label toolStripTimeLabel;
        private Sunny.UI.UIButton btn_run;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Sunny.UI.UIButton btn_config;
        private Sunny.UI.UIButton btn_datafind;
        private Sunny.UI.UIButton btn_user;
        private Sunny.UI.UIButton btn_armres;
        private UControl.LightStat lt_PLC;
        private UControl.LightStat lt_mes;
        private UControl.LightStat lt_vision;
        private Sunny.UI.UIButton btn_exit;
        private Sunny.UI.UIButton btn_userex;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_max;
        private System.Windows.Forms.Button btn_min;
    }
}