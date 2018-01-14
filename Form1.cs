using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Shutdown_Timer
{
    public partial class Form1 : Form
    {
        Process process = new Process();
        public Form1()
        {
            InitializeComponent();
        }
        //Form load
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
        //Main textbox keypress
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
                button1.PerformClick();
        }
        //Set button
        private void button1_Click(object sender, EventArgs e)
        {
            if (Int32.TryParse(textBox1.Text, out int Time) && !textBox1.Text.StartsWith("0"))
            {
                //If hours is checked
                if (radioButton2.Checked)
                {
                    //We need to multiply by 3600 to convert however many hours desired into seconds as the command we are trying to execute only accepts seconds.
                    Time *= 3600;
                }
                //If minutes is checked
                else
                {
                    Time *= 60;
                }
                ProcessStartInfo setStartInfo = new ProcessStartInfo();
                setStartInfo.FileName = "cmd";
                setStartInfo.RedirectStandardError = true;
                setStartInfo.UseShellExecute = false;
                setStartInfo.CreateNoWindow = true;
                if (comboBox1.SelectedIndex == 0)
                    //Shutdown args
                    setStartInfo.Arguments = String.Format("/c shutdown -s -t {0}", Time);
                else if (comboBox1.SelectedIndex == 1)
                    //Reboot args
                    setStartInfo.Arguments = String.Format("/c shutdown -s -r {0}", Time);
                else
                    //Logoff args
                    setStartInfo.Arguments = String.Format("/c shutdown -s -l {0}", Time);
                process.StartInfo = setStartInfo;
                process.Start();
                StreamReader streamreader = process.StandardError;
                if (streamreader.ReadToEnd().Contains("1190"))
                    if (MessageBox.Show("A timer is already in place!\nWould you like to abort it?", "Duplicate Timer", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                        button2.PerformClick();
                streamreader.Close();
            }
            else if (textBox1.Text == "")
                MessageBox.Show("Please do not leave the textbox empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //Have to use index because a contains would not work or just a simple == "0" because the user can have multiple zeros and it would still instantly shutdown thier PC
            else if (textBox1.Text.IndexOf('0') == 0)
                MessageBox.Show("Zero is a bad number to use as a timer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("The textbox can only contain numbers!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        //Abort button
        private void button2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo abortstartinfo = new ProcessStartInfo();
            abortstartinfo.FileName = "cmd";
            abortstartinfo.Arguments = "/c shutdown -a";
            //Fixes works instead of setting the window style to hidden (Having the "UseShellExecute" property set to true stops window styles from working for some reason)
            abortstartinfo.CreateNoWindow = true;
            abortstartinfo.RedirectStandardError = true;
            abortstartinfo.UseShellExecute = false;
            process.StartInfo = abortstartinfo;
            process.Start();
            StreamReader streamreader = process.StandardError;
            if (streamreader.ReadToEnd().Contains("1116"))
                MessageBox.Show("A shutdown timer was never set","No Timer",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
            streamreader.Close();
        }
    }
}