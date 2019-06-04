﻿using System;
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

namespace bank_parser
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (!CheckForInternetConnection())
            {
                MetroFramework.MetroMessageBox.Show(this, "Отсутствует интернет соединение, пожалуйста проверьте подключение к интернету!", "Отсутствует интернет соединение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if(CheckForInternetConnection())
            {
                
                MainForm mainForm = new MainForm();
                Hide();
                mainForm.ShowDialog();
                Close();
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
    }
}
