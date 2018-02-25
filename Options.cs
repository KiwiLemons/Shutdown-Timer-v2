using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shutdown_Timer
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }
        //Form load
        private void Options_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = Properties.Settings.Default.duplicateAbort;
        }
        //Duplicate abort checkbox state change
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.duplicateAbort = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
