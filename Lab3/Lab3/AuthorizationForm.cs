using System;
using System.Windows.Forms;
using System.IO;

namespace Lab3
{
    public partial class AuthorizationForm : Form
    {
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void AuthorizationForm_Load(object sender, EventArgs e)
        {
                GetToken.DocumentCompleted += webBrowser1_DocumentCompleted;
                GetToken.Navigate("https://oauth.vk.com/authorize?client_id=5527856&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=friends&response_type=token&v=5.52");
        }
        private void GetUserToken()
        {
            char[] Symbols = { '=', '&' };
            string[] URL = GetToken.Url.ToString().Split(Symbols);
            File.WriteAllText("UserInf.txt", URL[1] + "\n");
            File.AppendAllText("UserInf.txt", URL[5]);
            this.Visible = false;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (GetToken.Url.ToString().IndexOf("access_token=") != -1)
            {
                GetUserToken();
            }
        }
    }
}
