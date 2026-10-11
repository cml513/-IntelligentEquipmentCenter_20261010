namespace IntelligentEquipmentCenter.UiForm
{
    partial class BootScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BootScreen));
            this.TxtTitle = new System.Windows.Forms.Label();
            this.txtDateTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TxtTitle
            // 
            this.TxtTitle.AutoSize = true;
            this.TxtTitle.BackColor = System.Drawing.Color.Transparent;
            this.TxtTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TxtTitle.Font = new System.Drawing.Font("微软雅黑", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtTitle.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.TxtTitle.Location = new System.Drawing.Point(275, 69);
            this.TxtTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TxtTitle.Name = "TxtTitle";
            this.TxtTitle.Size = new System.Drawing.Size(419, 83);
            this.TxtTitle.TabIndex = 16;
            this.TxtTitle.Text = "智能装备中心";
            // 
            // txtDateTime
            // 
            this.txtDateTime.BackColor = System.Drawing.Color.Transparent;
            this.txtDateTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.txtDateTime.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDateTime.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.txtDateTime.Location = new System.Drawing.Point(228, 197);
            this.txtDateTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtDateTime.Name = "txtDateTime";
            this.txtDateTime.Size = new System.Drawing.Size(537, 73);
            this.txtDateTime.TabIndex = 17;
            // 
            // BootScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::IntelligentEquipmentCenter.Properties.Resources._1;
            this.ClientSize = new System.Drawing.Size(961, 531);
            this.Controls.Add(this.txtDateTime);
            this.Controls.Add(this.TxtTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BootScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BootScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TxtTitle;
        private System.Windows.Forms.Label txtDateTime;
    }
}