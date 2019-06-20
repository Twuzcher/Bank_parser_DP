using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using MetroFramework.Forms;
using System.Threading;
using MetroFramework;

namespace bank_parser
{
    public partial class Form1 : MetroForm
    {
        MainForm main;
        public bool check;

        public Form1()
        {
            InitializeComponent();
            check = false;
        }

        public Form1(MainForm main)
        {
            InitializeComponent();
            check = false;
            this.main = main;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (!CheckForInternetConnection())
            {
                MetroFramework.MetroMessageBox.Show(this, "Отсутствует интернет соединение, пожалуйста проверьте подключение к интернету!", "Отсутствует интернет соединение", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else
            {
                
                metroButton1.Visible = false;
                metroButton2.Visible = false;
                metroLabel1.Visible = true;
                metroProgressSpinner1.Visible = true;
                check = true;
                
                backgroundWorker1.RunWorkerAsync();
             
            }
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            main.start();
            while (check)
            {
                if (check == false)
                {
                    break;
                }
       
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!check)
            {
                MetroMessageBox.Show(this, "Загрузка завершена!", "Информация полученна!", MessageBoxButtons.OK, MessageBoxIcon.Question);
                backgroundWorker1.CancelAsync();
                Close();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
    }
}
