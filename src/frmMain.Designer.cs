namespace DriveFrame
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbDriveInfoSample = new System.Windows.Forms.Label();
            this.progBarSample = new System.Windows.Forms.ProgressBar();
            this.picDrive = new System.Windows.Forms.PictureBox();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.lbPercentSample = new System.Windows.Forms.Label();
            this.ctMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnAutorun = new System.Windows.Forms.ToolStripMenuItem();
            this.mnExit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picDrive)).BeginInit();
            this.ctMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDriveInfoSample
            // 
            this.lbDriveInfoSample.AutoSize = true;
            this.lbDriveInfoSample.BackColor = System.Drawing.Color.Transparent;
            this.lbDriveInfoSample.ForeColor = System.Drawing.Color.White;
            this.lbDriveInfoSample.Location = new System.Drawing.Point(50, 12);
            this.lbDriveInfoSample.Name = "lbDriveInfoSample";
            this.lbDriveInfoSample.Size = new System.Drawing.Size(78, 15);
            this.lbDriveInfoSample.TabIndex = 1;
            this.lbDriveInfoSample.Text = "(C:) Windows";
            this.lbDriveInfoSample.Visible = false;
            // 
            // progBarSample
            // 
            this.progBarSample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progBarSample.BackColor = System.Drawing.Color.Black;
            this.progBarSample.Cursor = System.Windows.Forms.Cursors.Default;
            this.progBarSample.Location = new System.Drawing.Point(12, 50);
            this.progBarSample.MarqueeAnimationSpeed = 0;
            this.progBarSample.Name = "progBarSample";
            this.progBarSample.Size = new System.Drawing.Size(137, 10);
            this.progBarSample.TabIndex = 2;
            this.progBarSample.Value = 70;
            this.progBarSample.Visible = false;
            // 
            // picDrive
            // 
            this.picDrive.BackColor = System.Drawing.Color.Transparent;
            this.picDrive.Image = global::DriveFrame.Properties.Resources.drive_fix;
            this.picDrive.Location = new System.Drawing.Point(12, 12);
            this.picDrive.Name = "picDrive";
            this.picDrive.Size = new System.Drawing.Size(32, 32);
            this.picDrive.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDrive.TabIndex = 3;
            this.picDrive.TabStop = false;
            this.picDrive.Visible = false;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 1000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // lbPercentSample
            // 
            this.lbPercentSample.AutoSize = true;
            this.lbPercentSample.BackColor = System.Drawing.Color.Transparent;
            this.lbPercentSample.ForeColor = System.Drawing.Color.White;
            this.lbPercentSample.Location = new System.Drawing.Point(50, 32);
            this.lbPercentSample.Name = "lbPercentSample";
            this.lbPercentSample.Size = new System.Drawing.Size(90, 15);
            this.lbPercentSample.TabIndex = 1;
            this.lbPercentSample.Text = "120 GB / 500 GB";
            this.lbPercentSample.Visible = false;
            // 
            // ctMenu
            // 
            this.ctMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnAutorun,
            this.mnExit});
            this.ctMenu.Name = "contextMenuStrip1";
            this.ctMenu.Size = new System.Drawing.Size(122, 48);
            // 
            // mnAutorun
            // 
            this.mnAutorun.Name = "mnAutorun";
            this.mnAutorun.Size = new System.Drawing.Size(121, 22);
            this.mnAutorun.Text = "Auto run";
            this.mnAutorun.Click += new System.EventHandler(this.mnAutorun_Click);
            // 
            // mnExit
            // 
            this.mnExit.Name = "mnExit";
            this.mnExit.Size = new System.Drawing.Size(121, 22);
            this.mnExit.Text = "Exit";
            this.mnExit.Click += new System.EventHandler(this.mnExit_Click);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(161, 262);
            this.ContextMenuStrip = this.ctMenu;
            this.ControlBox = false;
            this.Controls.Add(this.picDrive);
            this.Controls.Add(this.progBarSample);
            this.Controls.Add(this.lbPercentSample);
            this.Controls.Add(this.lbDriveInfoSample);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMain";
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMain_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.picDrive)).EndInit();
            this.ctMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbDriveInfoSample;
        private System.Windows.Forms.ProgressBar progBarSample;
        private System.Windows.Forms.PictureBox picDrive;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Label lbPercentSample;
        private System.Windows.Forms.ContextMenuStrip ctMenu;
        private System.Windows.Forms.ToolStripMenuItem mnExit;
        private System.Windows.Forms.ToolStripMenuItem mnAutorun;
    }
}