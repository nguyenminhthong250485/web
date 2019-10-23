using System;
using OpenQA.Selenium;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FaceAdsTool
{
    public partial class MainForm : Form
    {
        SeleniumHelper ChromeFB = new SeleniumHelper();
        struct fanpage
        {
            public string link;
            public string name;
        }
        List<fanpage> Trangs = new List<fanpage>();
        public MainForm()
        {
            InitializeComponent();
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            Button.CheckForIllegalCrossThreadCalls = false;
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        private void bt_quit_Click(object sender, EventArgs e)
        {
            ChromeFB.Close();
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ChromeFB.GotoURL("https://www.facebook.com");
        }

        private void tb_username_MouseClick(object sender, MouseEventArgs e)
        {
            tb_username.Text = "0866462750";
        }

        private void tb_password_MouseClick(object sender, MouseEventArgs e)
        {
            tb_password.Text = "Meyeu150458@";
            tb_password.PasswordChar = '*';
        }

        private void bt_login_Click(object sender, EventArgs e)
        {
            string str_alert = "";
            Thread ThreadLogin = new Thread(delegate ()
            {
                try
                {
                    string userFB = tb_username.Text;
                    string passFB = tb_password.Text;
                    ChromeFB.SendKeys(ChromeFB.FindElement(By.XPath("//input[@id='email']")), "0866462750");
                    ChromeFB.SendKeys(ChromeFB.FindElement(By.XPath("//input[@id='pass']")), "Meyeu150458@");//loginbutton
                    ChromeFB.FindElement(By.XPath("//label[@id='loginbutton']")).Click();//driver.switchTo().alert().accept();
                    str_alert += "Đăng Nhập Tài Khoản Thành Công...\n";
                }
                catch
                {
                    str_alert += "Đăng Nhập Tài Khoản Thất Bại...\n";
                    return;
                }
                rtb_main.Text = str_alert;
                Thread.Sleep(1000);
                try
                {
                    //IWebElement Tag_Trang = ChromeFB.FindElement(By.XPath("//a[@title='AIFU GROUP']"));
                    //string Link_Trang = Tag_Trang.GetAttribute("href");
                    ChromeFB.GotoURL("https://www.facebook.com/pages/?category=your_pages&ref=bookmarks");
                    str_alert += "Xin Mời Bạn Chọn Trang Cần Quảng Cáo...\n";
                }
                catch
                {
                    str_alert += "Truy Cập Vào Trang Thất Bại...\n";
                }
                Thread.Sleep(1000);
                rtb_main.Text = str_alert;
                IWebElement Div_Trang = ChromeFB.FindElement(By.XPath("//div[@id='page_browser_your_pages']"));
                ReadOnlyCollection<IWebElement> links_Trang = Div_Trang.FindElements(By.XPath("//div[@class='_1vgt ellipsis _349g']/a"));
                foreach(IWebElement link_Trang in links_Trang)
                {
                    string href = link_Trang.GetAttribute("href");
                    string inner = link_Trang.Text;
                    fanpage Trang = new fanpage();
                    Trang.link = href;
                    Trang.name = inner;
                    Trangs.Add(Trang);
                    dmud_page.Items.Add(inner);
                }
                dmud_page.SelectedIndex = 0;
                if(dmud_page.Items.Count==1)
                {
                    str_alert += "Đang Vào Trang " + Trangs[0].name + "...";
                    rtb_main.Text = str_alert;
                }
            });
            ThreadLogin.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ChromeFB.FindElement(By.XPath("//a[@title='AIFU GROUP']")).Click();
        }
    }
}
