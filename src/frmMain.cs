using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Configuration;
using DriveFrame.Properties;

namespace DriveFrame
{
    public partial class frmMain : Form
    {
        PictureBox[] picDrives;
        KProgressBar[] percentBar;
        List<DriveInfo> listDrive = new List<DriveInfo>();

        public frmMain()
        {
            InitializeComponent();

            if (DwmAPI.DwmIsCompositionEnabled())
            {
                // Paint the glass effect.
                DwmAPI.MARGINS margins = new DwmAPI.MARGINS { Left = -1 };
                DwmAPI.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }

            // generate control.
            int maxdrive = 10;
            picDrives = new PictureBox[maxdrive];
            percentBar = new KProgressBar[maxdrive];

            for (int i = 0; i < maxdrive; ++i)
            {
                picDrives[i] = new PictureBox();
                picDrives[i].Image = picDrive.Image;
                picDrives[i].SizeMode = PictureBoxSizeMode.AutoSize;
                picDrives[i].Location = new Point(picDrive.Left, 70 * i + 10);
                picDrives[i].Visible = false;
                this.Controls.Add(picDrives[i]);

                percentBar[i] = new KProgressBar();
                percentBar[i].Height = progBarSample.Height;
                percentBar[i].Width = progBarSample.Width;
                percentBar[i].Location = new Point(picDrive.Left, picDrives[i].Bottom + 5);
                percentBar[i].Anchor = progBarSample.Anchor;
                percentBar[i].Visible = false;
                this.Controls.Add(percentBar[i]);
            }

            // check if autorun mode.
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            mnAutorun.Checked = regKey.GetValue(Application.ProductName) != null;
            regKey.Close();

            this.Left = (int)Settings.Default["left"];
            this.Top = (int)Settings.Default["top"];
        }
        private void frmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Win32API.ReleaseCapture();
                Win32API.SendMessage(this.Handle, Win32API.WM_NCLBUTTONDOWN, Win32API.HT_CAPTION, 0);
            }
        }
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            listDrive.Clear();
            listDrive.AddRange(DriveInfo.GetDrives().Where(d => d.IsReady));

            int i = 0;
            for (; i < listDrive.Count; ++i)
            {
                picDrives[i].Visible = true;
                percentBar[i].Visible = true;
            
                lbDriveInfoSample.Text = string.Format("({0}) {1}", listDrive[i].Name.Substring(0, 2), listDrive[i].VolumeLabel);
                lbDriveInfoSample.Top = picDrives[i].Top;
                percentBar[i].Value = 100 - (int)(listDrive[i].AvailableFreeSpace * 100 / listDrive[i].TotalSize);
                Win32API.DrawTextOnGlass(this.Handle, lbDriveInfoSample.Text, lbDriveInfoSample.Font, lbDriveInfoSample.Bounds, 10);

                lbPercentSample.Text = string.Format("{0} / {1}", ToReadableSize(listDrive[i].AvailableFreeSpace), ToReadableSize(listDrive[i].TotalSize));
                lbPercentSample.Top = picDrives[i].Bottom - lbPercentSample.Height;
                Win32API.DrawTextOnGlass(this.Handle, lbPercentSample.Text, lbPercentSample.Font, lbPercentSample.Bounds, 10);
            }

            this.ClientSize = new Size(this.ClientSize.Width, percentBar[i - 1].Bottom + 10);
        }
        private void mnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void mnAutorun_Click(object sender, EventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (mnAutorun.Checked)
            {
                regKey.DeleteValue(Application.ProductName);
                mnAutorun.Checked = false;
            }
            else
            {
                regKey.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
                mnAutorun.Checked = true;
            }

            regKey.Close();
        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default["left"] = this.Left;
            Settings.Default["top"] = this.Top;
            Settings.Default.Save();
        }

        string ToReadableSize(long size)
        {
            const long ONE_GB = 1024 * 1024 * 1024;
            const long ONE_MB = 1024 * 1024;
            const long ONE_KB = 1024;

            if (size < ONE_KB)
                return string.Format("{0} Bytes", size);

            if (size < ONE_MB)
                return string.Format("{0} KB", size / ONE_KB);

            if (size < ONE_GB)
                return string.Format("{0} MB", size / ONE_MB);

            return string.Format("{0:0.##} GB", 1f * size / ONE_GB);
        }
    }
}
