using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using IWshRuntimeLibrary;

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
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                MessageBox.Show("Why do you want me in fullscreen so badly?");
            }
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
            if (validator() && Int32.TryParse(textBox1.Text, out int Time))
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
                    setStartInfo.Arguments = String.Format("/c shutdown -r -t {0}", Time);
                else
                    //Logoff args
                    setStartInfo.Arguments = String.Format("/c shutdown -l -t {0}", Time);
                process.StartInfo = setStartInfo;
                process.Start();
                StreamReader streamreaderE = process.StandardError;
                if (streamreaderE.ReadToEnd().Contains("1190"))
                {
                    switch (Properties.Settings.Default.duplicateAbort)
                    {
                        case true:
                            button2.PerformClick();
                            button1.PerformClick();
                            break;
                        case false:
                            if (MessageBox.Show("A timer is already in place!\nWould you like to abort it and set your new one?", "Duplicate Timer", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                            {
                                button2.PerformClick();
                                button1.PerformClick();
                            }
                            break;
                    }
                }
                streamreaderE.Close();
            }
        }
        //Abort button
        private void button2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo abortstartinfo = new ProcessStartInfo();
            abortstartinfo.FileName = "cmd";
            abortstartinfo.Arguments = "/c shutdown -a";
            //Below works instead of setting the window style to hidden (Having the "UseShellExecute" property set to true stops window styles from working for some reason)
            abortstartinfo.CreateNoWindow = true;
            abortstartinfo.RedirectStandardError = true;
            abortstartinfo.UseShellExecute = false;
            process.StartInfo = abortstartinfo;
            process.Start();
            StreamReader streamreader = process.StandardError;
            if (streamreader.ReadToEnd().Contains("1116"))
                MessageBox.Show("A shutdown timer was never set", "No Timer", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            streamreader.Close();
        }
        //Options button click
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form options = new Options();
            options.ShowDialog();
        }
        //Crete shortcut button
        private void button4_Click(object sender, EventArgs e)
        {
            if (validator() && Int32.TryParse(textBox1.Text, out int Time))
            {
                bool hours = true;
                //If hours is checked
                if (radioButton2.Checked)
                    //Convert hours to seconds
                    Time *= 3600;
                //If minutes is checked
                else
                {                    
                    //convet mins to seconds
                    Time *= 60;
                    hours = false;
                }
                //Start creation of shortcut
                WshShell shell = new WshShell();
                IWshShortcut shortcut = null;
                if (comboBox1.SelectedIndex == 0)
                {
                    //Shutdown args
                    if (hours)
                    {
                        shortcut = (IWshShortcut)shell.CreateShortcut(String.Format("{0}\\{1} Hour Shutdown Timer.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Time / 3600));
                    }
                    else
                    {
                        shortcut = (IWshShortcut)shell.CreateShortcut(String.Format("{0}\\{1} Minute Shutdown Timer.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Time / 60));
                    }
                    shortcut.Arguments = String.Format("-s -t {0}",Time);
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    //Reboot args
                    if (hours)
                    {
                        shortcut = (IWshShortcut)shell.CreateShortcut(String.Format("{0}\\{1} Hour Restart Timer.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Time / 3600));
                    }
                    else
                    {
                        shortcut = (IWshShortcut)shell.CreateShortcut(String.Format("{0}\\{1} Minute Restart Timer.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Time / 60));
                    }
                    shortcut.Arguments = String.Format("-r -t {0}", Time);
                }
                else
                {
                    //Logoff args
                    if (hours)
                    {
                        shortcut = (IWshShortcut)shell.CreateShortcut(String.Format("{0}\\{1} Hour Logoff Timer.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop),Time/3600));
                    }
                    else
                    {
                        shortcut = (IWshShortcut)shell.CreateShortcut(String.Format("{0}\\{1} Minute Logoff Timer.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop),Time/60));
                    }
                    shortcut.Arguments = String.Format("-l -t {0}", Time);
                }
                shortcut.TargetPath = String.Format("{0}\\shutdown.exe", Environment.GetFolderPath(Environment.SpecialFolder.System));
                shortcut.Save();
            }
        }
        //used to check if all text boxes are properly filled out
        private bool validator()
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please do not leave the textbox empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (textBox1.Text.IndexOf('0') == 0)
            {
                MessageBox.Show("Zero is a bad number to use as a timer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (!Int32.TryParse(textBox1.Text, out int Time))
            {
                MessageBox.Show("The textbox can only contain numbers!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
                return true;
        }
    }
}