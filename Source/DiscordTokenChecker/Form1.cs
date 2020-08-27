using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace DiscordTokenChecker
{
    public partial class Form1 : Form
    {
        public static bool bool1 = false;
        public static int int_0;
        public static int int_1;
        public static int int_2;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Select();
            if (!File.Exists("verified.txt"))
            {
                File.Create("verified.txt");
            }
            if (!File.Exists("unverified.txt"))
            {
                File.Create("unverified.txt");
            }
            if (!File.Exists("invalid.txt"))
            {
                File.Create("invalid.txt");
            }
            if (!File.Exists("tokens.txt"))
            {
                File.Create("tokens.txt");
                MessageBox.Show("The app is restarting to create the tokens files.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = "tokens.txt";
            if (!File.Exists("tokens.txt"))
            {
                MessageBox.Show("Please restart the app to create the tokens files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                File.WriteAllText("verified.txt", "");
                File.WriteAllText("unverified.txt", "");
                File.WriteAllText("invalid.txt", "");
                using (FileStream fileStream = File.OpenRead(path))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128))
                    {
                        string text;
                        while ((text = streamReader.ReadLine()) != null)
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                NameValueCollection nameValueCollection = new NameValueCollection();
                                nameValueCollection[""] = "";
                                webClient.Headers.Add("Authorization", text);
                                try
                                {
                                    webClient.UploadValues("https://discordapp.com/api/v6/invite/discordtokenchecker", nameValueCollection);
                                }
                                catch (WebException ex)
                                {
                                    string text2 = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                                    if (text2.Contains("401: Unauthorized"))
                                    {
                                        int_2++;
                                        File.AppendAllText("invalid.txt", text + Environment.NewLine);
                                    }
                                    else if (text2.Contains("You need to verify your account in order to perform this action."))
                                    {
                                        int_1++;
                                        File.AppendAllText("unverified.txt", text + Environment.NewLine);
                                    }
                                    else
                                    {
                                        int_0++;
                                        File.AppendAllText("verified.txt", text + Environment.NewLine);
                                    }
                                }
                            }
                        }
                        int num = int_0 + int_2 + int_1;
                        MessageBox.Show(string.Concat(new object[]
                        {
                            "Verified Tokens: ",
                            int_0,
                            "\nUnverified Tokens: ",
                            int_1,
                            "\nInvalid Tokens: ",
                            int_2,
                            "\nTotal: ",
                            num
                        }), "Tokens Checker", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        int_0 = 0;
                        int_1 = 0;
                        int_2 = 0;
                    }
                }
            }
        }

    }
}
