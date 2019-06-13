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
        BackgroundWorker bg;

        public Form1()
        {
            InitializeComponent();
            bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (!CheckForInternetConnection())
            {
                MetroFramework.MetroMessageBox.Show(this, "Отсутствует интернет соединение, пожалуйста проверьте подключение к интернету!", "Отсутствует интернет соединение", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else if(CheckForInternetConnection())
            {
                
                metroButton1.Visible = false;
                metroButton1.Visible = false;

                startParsing();

                
            }
        }

        private delegate void updateProgressDelegate();

        private async void startParsing()
        {

            //Thread t = new Thread(new ThreadStart(StartNewStaThread));
            //// Make sure to set the apartment state BEFORE starting the thread. 
            //t.ApartmentState = ApartmentState.STA;
            //t.Start();   
            bg.RunWorkerAsync();

        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!bg.IsBusy)
            {
                MetroMessageBox.Show(this, "Загрузка началась!", "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
                
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Invoke(new updateProgressDelegate(doUpdate));
            Thread t = new Thread(new ThreadStart(StartNewStaThread));
            // Make sure to set the apartment state BEFORE starting the thread. 
            t.ApartmentState = ApartmentState.STA;
            t.Start();
            
        }

        void doUpdate()
        {
            metroLabel1.Visible = true;
            metroProgressSpinner1.Visible = true;
            metroProgressSpinner1.Enabled = true;
        }

        private void StartNewStaThread()
        {
            Application.Run(new MainForm());
            try
            {
                Invoke((MethodInvoker)delegate () { Close(); });
            }
            catch (Exception e)
            {

            }
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

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        
    }
}
