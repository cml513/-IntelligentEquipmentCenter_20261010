namespace IntelligentEquipmentCenter.UiForm
{
    partial class Frm_Repice
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_recipe = new System.Windows.Forms.TextBox();
            this.txt_produc = new System.Windows.Forms.TextBox();
            this.txt_tooling = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_num = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.uiStyleManager1 = new Sunny.UI.UIStyleManager(this.components);
            this.btn_select = new Sunny.UI.UIButton();
            this.btn_add = new Sunny.UI.UIButton();
            this.btn_del = new Sunny.UI.UIButton();
            this.btn_save = new Sunny.UI.UIButton();
            this.btn_addRow = new Sunny.UI.UIButton();
            this.btn_delRow = new Sunny.UI.UIButton();
            this.dgvParams = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUpLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDownLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPlcAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDataType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShowOnUI = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colResultPlcAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNeedLimit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colStringLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uiLine2 = new Sunny.UI.UILine();
            this.uiLine1 = new Sunny.UI.UILine();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pageCommunication1 = new IntelligentEquipmentCenter.UPage.PageCommunication();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_Save = new System.Windows.Forms.Button();
            this.tb_MaterialCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "配方名称：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "产线名称：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(320, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "工位名称：";
            // 
            // txt_recipe
            // 
            this.txt_recipe.Location = new System.Drawing.Point(140, 19);
            this.txt_recipe.Name = "txt_recipe";
            this.txt_recipe.Size = new System.Drawing.Size(160, 29);
            this.txt_recipe.TabIndex = 1;
            // 
            // txt_produc
            // 
            this.txt_produc.Location = new System.Drawing.Point(140, 59);
            this.txt_produc.Name = "txt_produc";
            this.txt_produc.Size = new System.Drawing.Size(160, 29);
            this.txt_produc.TabIndex = 1;
            // 
            // txt_tooling
            // 
            this.txt_tooling.Location = new System.Drawing.Point(430, 19);
            this.txt_tooling.Name = "txt_tooling";
            this.txt_tooling.Size = new System.Drawing.Size(160, 29);
            this.txt_tooling.TabIndex = 1;
            this.txt_tooling.Text = "工位1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(320, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 19);
            this.label10.TabIndex = 0;
            this.label10.Text = "工装编号：";
            // 
            // txt_num
            // 
            this.txt_num.Location = new System.Drawing.Point(430, 59);
            this.txt_num.Name = "txt_num";
            this.txt_num.Size = new System.Drawing.Size(160, 29);
            this.txt_num.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(630, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 19);
            this.label11.TabIndex = 0;
            this.label11.Text = "当前配方：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(740, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(183, 27);
            this.comboBox1.TabIndex = 4;
            // 
            // btn_select
            // 
            this.btn_select.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_select.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_select.Location = new System.Drawing.Point(394, 566);
            this.btn_select.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(147, 45);
            this.btn_select.TabIndex = 5;
            this.btn_select.Text = "切换配方";
            this.btn_select.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
            // 
            // btn_add
            // 
            this.btn_add.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_add.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_add.Location = new System.Drawing.Point(199, 566);
            this.btn_add.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(147, 45);
            this.btn_add.TabIndex = 5;
            this.btn_add.Text = "添加配方";
            this.btn_add.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // btn_del
            // 
            this.btn_del.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_del.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_del.Location = new System.Drawing.Point(567, 566);
            this.btn_del.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_del.Name = "btn_del";
            this.btn_del.Size = new System.Drawing.Size(147, 45);
            this.btn_del.TabIndex = 5;
            this.btn_del.Text = "删除配方";
            this.btn_del.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_del.Click += new System.EventHandler(this.btn_del_Click);
            // 
            // btn_save
            // 
            this.btn_save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_save.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_save.Location = new System.Drawing.Point(740, 566);
            this.btn_save.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(147, 45);
            this.btn_save.TabIndex = 5;
            this.btn_save.Text = "修改保存";
            this.btn_save.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_addRow
            // 
            this.btn_addRow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_addRow.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_addRow.Location = new System.Drawing.Point(30, 100);
            this.btn_addRow.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_addRow.Name = "btn_addRow";
            this.btn_addRow.Size = new System.Drawing.Size(110, 32);
            this.btn_addRow.TabIndex = 5;
            this.btn_addRow.Text = "添加一行";
            this.btn_addRow.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_addRow.Click += new System.EventHandler(this.btn_addRow_Click);
            // 
            // btn_delRow
            // 
            this.btn_delRow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_delRow.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_delRow.Location = new System.Drawing.Point(150, 100);
            this.btn_delRow.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_delRow.Name = "btn_delRow";
            this.btn_delRow.Size = new System.Drawing.Size(110, 32);
            this.btn_delRow.TabIndex = 5;
            this.btn_delRow.Text = "删除选中行";
            this.btn_delRow.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_delRow.Click += new System.EventHandler(this.btn_delRow_Click);
            // 
            // dgvParams
            // 
            this.dgvParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colUpLimit,
            this.colDownLimit,
            this.colPlcAddress,
            this.colDataType,
            this.colUnit,
            this.colShowOnUI,
            this.colResultPlcAddress,
            this.colNeedLimit,
            this.colStringLength});
            this.dgvParams.Location = new System.Drawing.Point(30, 140);
            this.dgvParams.Name = "dgvParams";
            this.dgvParams.RowTemplate.Height = 23;
            this.dgvParams.Size = new System.Drawing.Size(970, 390);
            this.dgvParams.TabIndex = 6;
            // 
            // colName
            // 
            this.colName.HeaderText = "名字";
            this.colName.Name = "colName";
            this.colName.Width = 130;
            // 
            // colUpLimit
            // 
            this.colUpLimit.HeaderText = "上限值";
            this.colUpLimit.Name = "colUpLimit";
            // 
            // colDownLimit
            // 
            this.colDownLimit.HeaderText = "下限值";
            this.colDownLimit.Name = "colDownLimit";
            // 
            // colPlcAddress
            // 
            this.colPlcAddress.HeaderText = "实际值PLC地址";
            this.colPlcAddress.Name = "colPlcAddress";
            this.colPlcAddress.Width = 110;
            // 
            // colDataType
            // 
            this.colDataType.HeaderText = "数据类型";
            this.colDataType.Items.AddRange(new object[] {
            "int",
            "float",
            "string"});
            this.colDataType.Name = "colDataType";
            this.colDataType.Width = 80;
            // 
            // colUnit
            // 
            this.colUnit.HeaderText = "单位";
            this.colUnit.Name = "colUnit";
            this.colUnit.Width = 60;
            // 
            // colShowOnUI
            // 
            this.colShowOnUI.HeaderText = "显示到界面";
            this.colShowOnUI.Name = "colShowOnUI";
            this.colShowOnUI.Width = 80;
            // 
            // colResultPlcAddress
            // 
            this.colResultPlcAddress.HeaderText = "结果PLC地址";
            this.colResultPlcAddress.Name = "colResultPlcAddress";
            this.colResultPlcAddress.Width = 120;
            // 
            // colNeedLimit
            // 
            this.colNeedLimit.HeaderText = "需上下限";
            this.colNeedLimit.Name = "colNeedLimit";
            this.colNeedLimit.Width = 80;
            // 
            // colStringLength
            // 
            this.colStringLength.HeaderText = "字符串长度";
            this.colStringLength.Name = "colStringLength";
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 14F);
            this.tabControl1.ItemSize = new System.Drawing.Size(120, 45);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1040, 682);
            this.tabControl1.TabIndex = 19;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.uiLine2);
            this.tabPage1.Controls.Add(this.uiLine1);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btn_save);
            this.tabPage1.Controls.Add(this.btn_del);
            this.tabPage1.Controls.Add(this.btn_add);
            this.tabPage1.Controls.Add(this.btn_select);
            this.tabPage1.Controls.Add(this.btn_delRow);
            this.tabPage1.Controls.Add(this.btn_addRow);
            this.tabPage1.Controls.Add(this.dgvParams);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.txt_recipe);
            this.tabPage1.Controls.Add(this.txt_produc);
            this.tabPage1.Controls.Add(this.txt_tooling);
            this.tabPage1.Controls.Add(this.txt_num);
            this.tabPage1.Location = new System.Drawing.Point(4, 49);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1032, 629);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "配方功能";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // uiLine2
            // 
            this.uiLine2.BackColor = System.Drawing.Color.Transparent;
            this.uiLine2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLine2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine2.Location = new System.Drawing.Point(-12, 3);
            this.uiLine2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine2.Name = "uiLine2";
            this.uiLine2.Size = new System.Drawing.Size(1048, 10);
            this.uiLine2.TabIndex = 19;
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(-19, 545);
            this.uiLine1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(1048, 10);
            this.uiLine1.TabIndex = 19;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.pageCommunication1);
            this.tabPage3.Location = new System.Drawing.Point(4, 49);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1032, 629);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "通讯设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pageCommunication1
            // 
            this.pageCommunication1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageCommunication1.Location = new System.Drawing.Point(3, 3);
            this.pageCommunication1.Name = "pageCommunication1";
            this.pageCommunication1.Size = new System.Drawing.Size(1026, 623);
            this.pageCommunication1.TabIndex = 0;
            this.pageCommunication1.Load += new System.EventHandler(this.pageCommunication1_Load);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button_Save);
            this.tabPage2.Controls.Add(this.tb_MaterialCode);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 49);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1032, 629);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "产品参数";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(812, 503);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(135, 65);
            this.button_Save.TabIndex = 5;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // tb_MaterialCode
            // 
            this.tb_MaterialCode.Location = new System.Drawing.Point(128, 34);
            this.tb_MaterialCode.Name = "tb_MaterialCode";
            this.tb_MaterialCode.Size = new System.Drawing.Size(232, 29);
            this.tb_MaterialCode.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "物料编码";
            // 
            // Frm_Repice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "Frm_Repice";
            this.Size = new System.Drawing.Size(1040, 682);
            this.Load += new System.EventHandler(this.Frm_Repice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_recipe;
        private System.Windows.Forms.TextBox txt_produc;
        private System.Windows.Forms.TextBox txt_tooling;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_num;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBox1;
        private Sunny.UI.UIStyleManager uiStyleManager1;
        private Sunny.UI.UIButton btn_select;
        private Sunny.UI.UIButton btn_add;
        private Sunny.UI.UIButton btn_del;
        private Sunny.UI.UIButton btn_save;
        private Sunny.UI.UIButton btn_addRow;
        private Sunny.UI.UIButton btn_delRow;
        private System.Windows.Forms.DataGridView dgvParams;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUpLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDownLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPlcAddress;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShowOnUI;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResultPlcAddress;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNeedLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStringLength;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Sunny.UI.UILine uiLine2;
        private Sunny.UI.UILine uiLine1;
        private System.Windows.Forms.TabPage tabPage3;
        private UPage.PageCommunication pageCommunication1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.TextBox tb_MaterialCode;
        private System.Windows.Forms.Label label4;
    }
}
