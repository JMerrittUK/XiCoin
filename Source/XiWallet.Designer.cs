namespace XiCoin
{
    partial class XiWallet
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XiWallet));
            this.payment = new System.Windows.Forms.Button();
            this.XiTitle = new System.Windows.Forms.Label();
            this.fundsLabel = new System.Windows.Forms.Label();
            this.totalValue = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gbpValue = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.wallets = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // payment
            // 
            this.payment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.payment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.payment.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.payment.Location = new System.Drawing.Point(344, 260);
            this.payment.Name = "payment";
            this.payment.Size = new System.Drawing.Size(98, 23);
            this.payment.TabIndex = 1;
            this.payment.Text = "Make a Payment";
            this.payment.UseVisualStyleBackColor = false;
            // 
            // XiTitle
            // 
            this.XiTitle.AutoSize = true;
            this.XiTitle.BackColor = System.Drawing.Color.Transparent;
            this.XiTitle.Font = new System.Drawing.Font("Constantia", 42F);
            this.XiTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.XiTitle.Location = new System.Drawing.Point(196, 2);
            this.XiTitle.Name = "XiTitle";
            this.XiTitle.Size = new System.Drawing.Size(64, 68);
            this.XiTitle.TabIndex = 3;
            this.XiTitle.Text = "Ξ";
            this.XiTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.XiTitle.Click += new System.EventHandler(this.label1_Click);
            // 
            // fundsLabel
            // 
            this.fundsLabel.AutoSize = true;
            this.fundsLabel.BackColor = System.Drawing.Color.Transparent;
            this.fundsLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.fundsLabel.Location = new System.Drawing.Point(312, 216);
            this.fundsLabel.Name = "fundsLabel";
            this.fundsLabel.Size = new System.Drawing.Size(47, 13);
            this.fundsLabel.TabIndex = 5;
            this.fundsLabel.Text = "Total   Ξ";
            this.fundsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fundsLabel.Click += new System.EventHandler(this.fundsLabel_Click);
            // 
            // totalValue
            // 
            this.totalValue.AutoSize = true;
            this.totalValue.BackColor = System.Drawing.Color.Transparent;
            this.totalValue.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.totalValue.Location = new System.Drawing.Point(402, 216);
            this.totalValue.Name = "totalValue";
            this.totalValue.Size = new System.Drawing.Size(22, 13);
            this.totalValue.TabIndex = 6;
            this.totalValue.Text = "0.0";
            this.totalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(259, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "GBP Value (approx)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // gbpValue
            // 
            this.gbpValue.AutoSize = true;
            this.gbpValue.BackColor = System.Drawing.Color.Transparent;
            this.gbpValue.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.gbpValue.Location = new System.Drawing.Point(402, 235);
            this.gbpValue.Name = "gbpValue";
            this.gbpValue.Size = new System.Drawing.Size(22, 13);
            this.gbpValue.TabIndex = 8;
            this.gbpValue.Text = "0.0";
            this.gbpValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.gbpValue.Click += new System.EventHandler(this.label4_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(3, 260);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(231, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Value = 75;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.Location = new System.Drawing.Point(240, 260);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Create Wallets";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // wallets
            // 
            this.wallets.BackColor = System.Drawing.Color.Transparent;
            this.wallets.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.wallets.Location = new System.Drawing.Point(19, 66);
            this.wallets.Name = "wallets";
            this.wallets.Size = new System.Drawing.Size(411, 141);
            this.wallets.TabIndex = 11;
            this.wallets.TabStop = false;
            this.wallets.Text = "Wallets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Constantia", 8F);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(6, 238);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Developed by Jared Merritt";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // XiWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.wallets);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.gbpValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.totalValue);
            this.Controls.Add(this.fundsLabel);
            this.Controls.Add(this.XiTitle);
            this.Controls.Add(this.payment);
            this.Name = "XiWallet";
            this.Size = new System.Drawing.Size(445, 286);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button payment;
        private System.Windows.Forms.Label XiTitle;
        private System.Windows.Forms.Label fundsLabel;
        private System.Windows.Forms.Label totalValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label gbpValue;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox wallets;
        private System.Windows.Forms.Label label2;
    }
}
