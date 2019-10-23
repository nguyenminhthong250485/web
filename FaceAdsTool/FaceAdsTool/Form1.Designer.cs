namespace FaceAdsTool
{
    partial class MainForm
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
            this.flp_tb = new System.Windows.Forms.FlowLayoutPanel();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.flp_bt = new System.Windows.Forms.FlowLayoutPanel();
            this.bt_quit = new System.Windows.Forms.Button();
            this.bt_login = new System.Windows.Forms.Button();
            this.rtb_main = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dmud_page = new System.Windows.Forms.DomainUpDown();
            this.flp_tb.SuspendLayout();
            this.flp_bt.SuspendLayout();
            this.SuspendLayout();
            // 
            // flp_tb
            // 
            this.flp_tb.Controls.Add(this.tb_username);
            this.flp_tb.Controls.Add(this.tb_password);
            this.flp_tb.Controls.Add(this.dmud_page);
            this.flp_tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flp_tb.Location = new System.Drawing.Point(167, 19);
            this.flp_tb.Margin = new System.Windows.Forms.Padding(10);
            this.flp_tb.Name = "flp_tb";
            this.flp_tb.Size = new System.Drawing.Size(314, 570);
            this.flp_tb.TabIndex = 4;
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(10, 10);
            this.tb_username.Margin = new System.Windows.Forms.Padding(10);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(285, 26);
            this.tb_username.TabIndex = 3;
            this.tb_username.Text = "Tên đăng nhập facebook";
            this.tb_username.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tb_username_MouseClick);
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(10, 56);
            this.tb_password.Margin = new System.Windows.Forms.Padding(10);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(285, 26);
            this.tb_password.TabIndex = 4;
            this.tb_password.Text = "Mật khẩu đăng nhập facebook";
            this.tb_password.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tb_password_MouseClick);
            // 
            // flp_bt
            // 
            this.flp_bt.Controls.Add(this.bt_quit);
            this.flp_bt.Controls.Add(this.bt_login);
            this.flp_bt.Controls.Add(this.button1);
            this.flp_bt.Location = new System.Drawing.Point(19, 19);
            this.flp_bt.Margin = new System.Windows.Forms.Padding(10);
            this.flp_bt.Name = "flp_bt";
            this.flp_bt.Size = new System.Drawing.Size(127, 570);
            this.flp_bt.TabIndex = 3;
            // 
            // bt_quit
            // 
            this.bt_quit.BackColor = System.Drawing.SystemColors.Highlight;
            this.bt_quit.ForeColor = System.Drawing.Color.White;
            this.bt_quit.Location = new System.Drawing.Point(10, 10);
            this.bt_quit.Margin = new System.Windows.Forms.Padding(10);
            this.bt_quit.Name = "bt_quit";
            this.bt_quit.Size = new System.Drawing.Size(106, 34);
            this.bt_quit.TabIndex = 0;
            this.bt_quit.Text = "THOÁT";
            this.bt_quit.UseVisualStyleBackColor = false;
            this.bt_quit.Click += new System.EventHandler(this.bt_quit_Click);
            // 
            // bt_login
            // 
            this.bt_login.BackColor = System.Drawing.SystemColors.Highlight;
            this.bt_login.ForeColor = System.Drawing.Color.White;
            this.bt_login.Location = new System.Drawing.Point(10, 64);
            this.bt_login.Margin = new System.Windows.Forms.Padding(10);
            this.bt_login.Name = "bt_login";
            this.bt_login.Size = new System.Drawing.Size(106, 34);
            this.bt_login.TabIndex = 1;
            this.bt_login.Text = "VÀO FB";
            this.bt_login.UseVisualStyleBackColor = false;
            this.bt_login.Click += new System.EventHandler(this.bt_login_Click);
            // 
            // rtb_main
            // 
            this.rtb_main.Location = new System.Drawing.Point(879, 19);
            this.rtb_main.Name = "rtb_main";
            this.rtb_main.Size = new System.Drawing.Size(485, 658);
            this.rtb_main.TabIndex = 5;
            this.rtb_main.Text = "";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Highlight;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(10, 118);
            this.button1.Margin = new System.Windows.Forms.Padding(10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 34);
            this.button1.TabIndex = 2;
            this.button1.Text = "VÀO FB";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dmud_page
            // 
            this.dmud_page.Location = new System.Drawing.Point(10, 102);
            this.dmud_page.Margin = new System.Windows.Forms.Padding(10);
            this.dmud_page.Name = "dmud_page";
            this.dmud_page.Size = new System.Drawing.Size(285, 26);
            this.dmud_page.TabIndex = 5;
            this.dmud_page.Text = "Chọn Trang";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1390, 709);
            this.Controls.Add(this.rtb_main);
            this.Controls.Add(this.flp_tb);
            this.Controls.Add(this.flp_bt);
            this.Name = "MainForm";
            this.Text = "FaceAdsTool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.flp_tb.ResumeLayout(false);
            this.flp_tb.PerformLayout();
            this.flp_bt.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp_tb;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.FlowLayoutPanel flp_bt;
        private System.Windows.Forms.Button bt_quit;
        private System.Windows.Forms.Button bt_login;
        private System.Windows.Forms.RichTextBox rtb_main;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DomainUpDown dmud_page;
    }
}

