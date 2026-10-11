namespace IntelligentEquipmentCenter.UiForm
{
    partial class Frm_MesModel
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb_dj = new Sunny.UI.UIRadioButton();
            this.rb_lx = new Sunny.UI.UIRadioButton();
            this.rb_zx = new Sunny.UI.UIRadioButton();
            this.btnok = new Sunny.UI.UIButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb_dj);
            this.groupBox1.Controls.Add(this.rb_lx);
            this.groupBox1.Controls.Add(this.rb_zx);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 14F);
            this.groupBox1.Location = new System.Drawing.Point(12, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(351, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MES模式选择";
            // 
            // rb_dj
            // 
            this.rb_dj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_dj.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_dj.Location = new System.Drawing.Point(62, 121);
            this.rb_dj.MinimumSize = new System.Drawing.Size(1, 1);
            this.rb_dj.Name = "rb_dj";
            this.rb_dj.Size = new System.Drawing.Size(124, 28);
            this.rb_dj.TabIndex = 15;
            this.rb_dj.Text = "单机模式";
            this.rb_dj.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ub_zx_MouseDown);
            // 
            // rb_lx
            // 
            this.rb_lx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_lx.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_lx.Location = new System.Drawing.Point(62, 80);
            this.rb_lx.MinimumSize = new System.Drawing.Size(1, 1);
            this.rb_lx.Name = "rb_lx";
            this.rb_lx.Size = new System.Drawing.Size(124, 28);
            this.rb_lx.TabIndex = 15;
            this.rb_lx.Text = "MES离线模式";
            this.rb_lx.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ub_zx_MouseDown);
            // 
            // rb_zx
            // 
            this.rb_zx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_zx.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_zx.Location = new System.Drawing.Point(62, 39);
            this.rb_zx.MinimumSize = new System.Drawing.Size(1, 1);
            this.rb_zx.Name = "rb_zx";
            this.rb_zx.Size = new System.Drawing.Size(124, 28);
            this.rb_zx.TabIndex = 15;
            this.rb_zx.Text = "MES在线模式";
            this.rb_zx.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ub_zx_MouseDown);
            // 
            // btnok
            // 
            this.btnok.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnok.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.btnok.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.btnok.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.btnok.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.btnok.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.btnok.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnok.Location = new System.Drawing.Point(99, 225);
            this.btnok.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnok.Name = "btnok";
            this.btnok.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.btnok.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.btnok.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.btnok.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.btnok.Size = new System.Drawing.Size(133, 41);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "确定";
            this.btnok.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // Frm_MesModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 278);
            this.Controls.Add(this.btnok);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Frm_MesModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模式选择";
            this.Load += new System.EventHandler(this.Frm_MesModel_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Sunny.UI.UIRadioButton rb_dj;
        private Sunny.UI.UIRadioButton rb_lx;
        private Sunny.UI.UIRadioButton rb_zx;
        private Sunny.UI.UIButton btnok;
    }
}