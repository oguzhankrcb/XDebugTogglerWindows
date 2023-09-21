using System.Windows.Forms;
using Microsoft.Win32;
using System.Resources;
using XDebugTogglerWindows.Config;

namespace XDebugTogglerWindows
{
    public partial class MainForm : Form
    {
        private readonly IConfigManager configManager;
        private IniParser? iniParser = null;
        private bool xDebugStatus = false;
        public MainForm()
        {
            InitializeComponent();
            this.configManager = new ConfigManager();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please select php.ini file";
            openFileDialog.Filter = "php.ini|php.ini";
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            openFileDialog.ShowDialog();

            txtIniPath.Text = openFileDialog.FileName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            configManager.WriteToConfigFile(txtIniPath.Text, "php_ini_path");
            if (iniParser == null)
            {
                iniParser = new IniParser(txtIniPath.Text);
                if (iniParser.KeyExists("zend_extension", "XDebug"))
                {
                    changeNotifyIconStatus(true);
                    configManager.WriteToConfigFile(iniParser.Read("zend_extension", "XDebug", "xdebug"), "xdebug_extension_path");
                }
                else
                {
                    changeNotifyIconStatus(false);
                }
            }
            notifyIcon.ShowBalloonTip(5, "Hey!", "XDebugToggler works in system tray!", ToolTipIcon.Info);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (configManager.ReadFromConfigFile("start_with_windows") == "1")
            {
                chkStartWithWindows.Checked = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }

            txtIniPath.Text = configManager.ReadFromConfigFile("php_ini_path");
            notifyIcon.Visible = true;

            if (!String.IsNullOrEmpty(txtIniPath.Text))
            {
                iniParser = new IniParser(txtIniPath.Text);
                if (iniParser.KeyExists("zend_extension", "XDebug"))
                {
                    changeNotifyIconStatus(true);
                    configManager.WriteToConfigFile(iniParser.Read("zend_extension", "XDebug", "xdebug"), "xdebug_extension_path");
                }
                else
                {
                    changeNotifyIconStatus(false);
                }
            }
        }

        private void changeNotifyIconStatus(bool changeNotifyIconStatus)
        {
            if (changeNotifyIconStatus == true)
            {
                xDebugStatus = true;
                notifyIcon.Icon = Resource.check;
                notifyIcon.Text = "XDebug is currently active";
                notifyIcon.Visible = true;
            }
            else
            {
                xDebugStatus = false;
                notifyIcon.Icon = Resource.xmark;
                notifyIcon.Text = "XDebug is currently NOT active";
                notifyIcon.Visible = true;
            }
        }
        private void setStartup(bool status)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey != null)
            {
                if (status)
                    registryKey.SetValue("XdebugToggler", Application.ExecutablePath);
                else
                    registryKey.DeleteValue("XdebugToggler", false);
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtIniPath.Clear();

            configManager.WriteToConfigFile("", "php_ini_path");
        }

        private void chkStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStartWithWindows.Checked == true)
            {
                setStartup(true);
                configManager.WriteToConfigFile("1", "start_with_windows");
            }
            else
            {
                setStartup(false);
                configManager.WriteToConfigFile("0", "start_with_windows");
            }
        }

        private void notifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (xDebugStatus == true)
                {
                    iniParser?.DeleteKey("zend_extension", "XDebug");
                    changeNotifyIconStatus(false);
                }
                else
                {
                    iniParser?.Write("zend_extension", configManager.ReadFromConfigFile("xdebug_extension_path"), "XDebug");
                    changeNotifyIconStatus(true);
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Written By Oðuzhan KARACABAY\ngithub.com/oguzhankrcb", "About", MessageBoxButtons.OK);
        }
    }
}