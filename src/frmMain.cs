using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Configuration;
using DriveFrame.Properties;
using System.Management;
using System.Text.RegularExpressions;

namespace DriveFrame
{
    public partial class frmMain : Form
    {
        PictureBox[] picDrives;
        Label[] lbDrive, lbPercent, lbDisk;
        KProgressBar[] percentBar;
        List<Volumn> listDrive = new List<Volumn>();

        public frmMain()
        {
            InitializeComponent();

            //if (DwmAPI.DwmIsCompositionEnabled())
            //{
            //    // Paint the glass effect.
            //    DwmAPI.MARGINS margins = new DwmAPI.MARGINS { Left = -1 };
            //    DwmAPI.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            //}

            // generate control.
            int maxdrive = 10;
            picDrives = new PictureBox[maxdrive];
            percentBar = new KProgressBar[maxdrive];
            lbDrive = new Label[maxdrive];
            lbPercent = new Label[maxdrive];
            lbDisk = new Label[maxdrive];

            for (int i = 0; i < maxdrive; ++i)
            {
                picDrives[i] = new PictureBox();
                picDrives[i].Image = picDrive.Image;
                picDrives[i].SizeMode = PictureBoxSizeMode.AutoSize;
                picDrives[i].Location = new Point(picDrive.Left, 60 * i + 30);
                picDrives[i].Visible = false;
                this.Controls.Add(picDrives[i]);

                percentBar[i] = new KProgressBar();
                percentBar[i].Height = progBarSample.Height;
                percentBar[i].Width = progBarSample.Width;
                percentBar[i].Location = new Point(picDrive.Left, picDrives[i].Bottom + 5);
                percentBar[i].Anchor = progBarSample.Anchor;
                percentBar[i].Visible = false;
                this.Controls.Add(percentBar[i]);

                lbDrive[i] = new Label();
                lbDrive[i].AutoSize = true;
                lbDrive[i].ForeColor = Color.White;
                lbDrive[i].Location = new Point(lbDriveInfoSample.Left, picDrives[i].Top);
                this.Controls.Add(lbDrive[i]);

                lbPercent[i] = new Label();
                lbPercent[i].AutoSize = true;
                lbPercent[i].ForeColor = Color.White;
                lbPercent[i].Location = new Point(lbDriveInfoSample.Left, picDrives[i].Bottom - lbPercentSample.Height);
                this.Controls.Add(lbPercent[i]);

                lbDisk[i] = new Label();
                lbDisk[i].Left = lbSampleDisk.Left;
                lbDisk[i].AutoSize = true;
                lbDisk[i].ForeColor = Color.White;
                lbDisk[i].Anchor = lbSampleDisk.Anchor;
                lbDisk[i].TextAlign = lbSampleDisk.TextAlign;
                lbDisk[i].Font = lbSampleDisk.Font;
                this.Controls.Add(lbDisk[i]);
            }

            // check if autorun mode.
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            mnAutorun.Checked = regKey.GetValue(Application.ProductName) != null;
            regKey.Close();

            this.Left = Settings.Default.left;
            this.Top = Settings.Default.top;
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
            listDrive.AddRange(GetVolumns());

            int i = 0;
            string currentDisk = "";
            int currentDiskIndex = -1;
            int currentTop = -10;
            for (; i < listDrive.Count; ++i)
            {
                if (listDrive[i].Drive != currentDisk)
                {
                    ++currentDiskIndex;
                    lbDisk[currentDiskIndex].Text = currentDisk = listDrive[i].Drive;
                    lbDisk[currentDiskIndex].Visible = true;

                    currentTop += 20;
                    lbDisk[currentDiskIndex].Top = currentTop;
                    currentTop = lbDisk[currentDiskIndex].Bottom;
                }

                picDrives[i].Top = currentTop + 10;
                picDrives[i].Visible = true;
                percentBar[i].Visible = true;

                lbDrive[i].Top = picDrives[i].Top;
                lbDrive[i].Text = string.Format("({0}) {1}", listDrive[i].Name.Substring(0, 2), listDrive[i].VolumeLabel);
                //lbDriveInfoSample.Top = picDrives[i].Top;

                //Win32API.DrawTextOnGlass(this.Handle, lbDriveInfoSample.Text, lbDriveInfoSample.Font, lbDriveInfoSample.Bounds, 10);

                lbPercent[i].Top = picDrives[i].Bottom - lbDrive[i].Height;
                lbPercent[i].Text = string.Format("{0} / {1}", ToReadableSize(listDrive[i].AvailableFreeSpace), ToReadableSize(listDrive[i].TotalSize));

                percentBar[i].Top = picDrives[i].Bottom + 3;
                percentBar[i].Value = 100 - (int)(listDrive[i].AvailableFreeSpace * 100 / listDrive[i].TotalSize);
                //lbPercentSample.Top = picDrives[i].Bottom - lbPercentSample.Height;
                //Win32API.DrawTextOnGlass(this.Handle, lbPercentSample.Text, lbPercentSample.Font, lbPercentSample.Bounds, 10);

                currentTop = percentBar[i].Bottom;
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

        private List<Volumn> GetVolumns()
        {
            var searcher = new ManagementObjectSearcher("SELECT Index,Model FROM Win32_DiskDrive");

            Dictionary<uint, string> dicDisks = new Dictionary<uint, string>();
            foreach (var queryObj in searcher.Get())
                dicDisks[(uint)queryObj["Index"]] = queryObj["Index"] + " - " + queryObj["Model"];

            searcher = new ManagementObjectSearcher("SELECT DeviceID,FreeSpace,Size,VolumeName,DriveType FROM Win32_LogicalDisk");

            List<Volumn> vols = new List<Volumn>();
            Dictionary<string, Volumn> dicVols = new Dictionary<string, Volumn>();
            foreach (var queryObj in searcher.Get())
            {
                if (queryObj["Size"] == null) continue;

                var vol = new Volumn
                {
                    Drive = "UNKNOWN DEVICE",
                    Name = (string)queryObj["DeviceID"],
                    AvailableFreeSpace = (ulong)queryObj["FreeSpace"],
                    TotalSize = (ulong)queryObj["Size"],
                    VolumeLabel = (string)queryObj["VolumeName"]
                };
                vols.Add(vol);
                dicVols[vol.Name] = vol;
            }

            searcher = new ManagementObjectSearcher("SELECT Antecedent,Dependent FROM Win32_LogicalDiskToPartition");
            foreach (var queryObj in searcher.Get())
            {
                string Antecedent = (string)queryObj["Antecedent"];
                string Dependent = (string)queryObj["Dependent"];

                Match m = Regex.Match(Antecedent, "Disk #(\\d+),");
                uint driveindex = uint.Parse(m.Groups[1].Value);

                m = Regex.Match(Dependent, "\"(.:)\"");
                string letter = m.Groups[1].Value;

                dicVols[letter].Drive = dicDisks[driveindex];
            }
            foreach (var vol in vols)
                Console.WriteLine(vol);

            vols = vols.OrderBy(v => v.Drive).ThenBy(v => v.Name).ToList();

            return vols;
        }

        string ToReadableSize(ulong size)
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

    class Volumn
    {
        public string Drive { get; set; }
        public string Name { get; set; }
        public string VolumeLabel { get; set; }
        public ulong AvailableFreeSpace { get; set; }
        public ulong TotalSize { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3}/{4}", Drive, Name, VolumeLabel, AvailableFreeSpace, TotalSize);
        }
    }
}
