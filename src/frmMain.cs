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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DriveFrame
{
    public partial class frmMain : Form
    {
        List<Volumn> listDrive = new List<Volumn>();
        Bitmap photo = null;

        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            // check if autorun mode.
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            mnAutorun.Checked = regKey.GetValue(Application.ProductName) != null;
            regKey.Close();

            this.Left = Settings.Default.left;
            this.Top = Settings.Default.top;
            this.Width = Settings.Default.width;

            int oldExStyle = Win32API.GetWindowLong(Handle, Win32API.GWL_EXSTYLE);
            Win32API.SetWindowLong(Handle, Win32API.GWL_EXSTYLE, oldExStyle | Win32API.WS_EX_LAYERED);
        }
        private void frmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Win32API.ReleaseCapture();
                Win32API.SendMessage(this.Handle, Win32API.WM_NCLBUTTONDOWN, Win32API.HT_CAPTION, 0);
            }
        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.left = this.Left;
            Settings.Default.top = this.Top;
            Settings.Default.width = this.Width;

            Settings.Default.Save();
        }
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            Size barsize = new Size(this.Width - picDrive.Left * 2, 10);
            photo = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(photo);
            DrawBorder(g);

            listDrive.Clear();
            listDrive.AddRange(GetVolumns());

            string currentDisk = "";
            int currentTop = 0;
            foreach (var vol in listDrive)
            {
                if (vol.Drive != currentDisk)
                {
                    currentTop += 20;
                    lbSampleDisk.Top = currentTop;
                    lbSampleDisk.Text = currentDisk = vol.Drive;
                    g.DrawString(currentDisk, lbSampleDisk.Font, Brushes.White, lbSampleDisk.Bounds);

                    currentTop = lbSampleDisk.Bottom;
                }

                picDrive.Top = currentTop + 10;
                g.DrawImageUnscaled(picDrive.Image, picDrive.Location);

                lbDriveInfoSample.Top = picDrive.Top;
                lbDriveInfoSample.Text = string.Format("({0}) {1}", vol.Name.Substring(0, 2), vol.VolumeLabel);
                g.DrawString(lbDriveInfoSample.Text, this.Font, Brushes.White, lbDriveInfoSample.Location);

                lbPercentSample.Top = picDrive.Bottom - lbDriveInfoSample.Height;
                lbPercentSample.Text = string.Format("{0} / {1}", ToReadableSize(vol.AvailableFreeSpace), ToReadableSize(vol.TotalSize));
                g.DrawString(lbPercentSample.Text, this.Font, Brushes.White, lbPercentSample.Location);

                currentTop = picDrive.Bottom + 3;
                int percent = 100 - (int)(vol.AvailableFreeSpace * 100 / vol.TotalSize);
                g.FillRectangle(new SolidBrush(Color.FromArgb(120, Color.White)), lbSampleDisk.Left, currentTop, barsize.Width, barsize.Height);
                g.FillRectangle(Brushes.DeepSkyBlue, lbSampleDisk.Left, currentTop, barsize.Width * percent / 100, barsize.Height);

                currentTop += barsize.Height;
            }
            this.ClientSize = new Size(this.ClientSize.Width, currentTop + 10);
            g.Dispose();
            Win32API.SetBitmap(this, photo, 200);
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

        private void DrawBorder2(Graphics g)
        {
            Size region = g.VisibleClipBounds.Size.ToSize();
            Bitmap border = Resources.border;
            int borderThick = border.Height;

            //g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, Color.Black)), 10, 10, region.Width - 20, region.Height - 20);
            Rectangle desRectW = new Rectangle(0, 0, region.Width - borderThick, borderThick);
            Rectangle desRectH = new Rectangle(0, 0, borderThick, region.Height - borderThick);
            Matrix m = g.Transform;

            //Top border
            g.DrawImage(border, desRectW, desRectW, GraphicsUnit.Pixel);

            //Bottom border
            border.RotateFlip(RotateFlipType.Rotate180FlipNone);
            g.TranslateTransform(borderThick, region.Height - borderThick);
            g.DrawImage(border, new Rectangle(0, 0, region.Width - borderThick, borderThick),
                new Rectangle(border.Width - region.Width + borderThick, 0, region.Width - borderThick, borderThick), GraphicsUnit.Pixel);

            //Right border
            border.RotateFlip(RotateFlipType.Rotate270FlipNone);
            g.Transform = m;
            g.TranslateTransform(region.Width - borderThick, 0);
            g.DrawImage(border, desRectH, desRectH, GraphicsUnit.Pixel);

            //Left border
            border.RotateFlip(RotateFlipType.Rotate180FlipNone);
            g.Transform = m;
            g.TranslateTransform(0, borderThick);
            g.DrawImage(border, new Rectangle(0, 0, borderThick, region.Height - borderThick),
                new Rectangle(0, border.Height - region.Height + borderThick, borderThick, region.Height - borderThick), GraphicsUnit.Pixel);

            g.Transform = m;
        }
        private void DrawBorder(Graphics g)
        {
            Size region = g.VisibleClipBounds.Size.ToSize();
            Bitmap border = Resources.border;
            int borderThick = border.Height;

            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.FillRectangle(Brushes.Black, 10, 10, region.Width - 20, region.Height - 20);
            Rectangle desRectW = new Rectangle(0, 0, region.Width - borderThick, borderThick);
            Rectangle desRectH = new Rectangle(0, 0, region.Height - borderThick, borderThick);

            //Top border
            g.DrawImage(border, desRectW, desRectW, GraphicsUnit.Pixel);

            //Right border
            g.TranslateTransform(region.Width, 0);
            g.RotateTransform(90);
            g.DrawImage(border, desRectH, desRectH, GraphicsUnit.Pixel);

            //Bottom border
            g.TranslateTransform(region.Height, 0);
            g.RotateTransform(90);
            g.DrawImage(border, desRectW, desRectW, GraphicsUnit.Pixel);

            //Left border
            g.TranslateTransform(region.Width, 0);
            g.RotateTransform(90);
            g.DrawImage(border, desRectH, desRectH, GraphicsUnit.Pixel);

            g.ResetTransform();
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
