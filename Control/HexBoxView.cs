namespace Standard
{
    using HttpWatch.Control;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Globalization;

    public class HexBoxView : UserControl
    {
        private IContainer components;
        public HexBox hexViewer;

        private ToolStripMenuItem miContextSetWidth;
        private ToolStripMenuItem miFindBytes;
        private ToolStripMenuItem miGotoByte;
        private ToolStripMenuItem miInsertFile;
        private ToolStripMenuItem miSaveSelected;
        private ToolStripMenuItem miSelectBytes;
        private ContextMenuStrip mnuHexContext;
        public StatusBarPanel sbpInsertMode;
        private StatusBarPanel sbpLocation;
        private StatusBarPanel sbpSelection;
        private StatusBar sbTextInfo;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
        public ToolStripMenuItem tsmiShowHeaders;

        public HexBoxView()
        {
            this.InitializeComponent();
          //  this.m_owner = o;
            if (8.25f != this.hexViewer.Font.Size)
            {
                this.hexViewer.Font = new Font(this.hexViewer.Font.FontFamily, 8.25f);
            }
            this.UpdateBytesPerLine();
        }

        protected   void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private static byte[] FileToByteArray(string sFilename)
        {
            FileStream stream = File.OpenRead(sFilename);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int) stream.Length);
            stream.Close();
            return buffer;
        }

        private void HexBoxView_SizeChanged(object sender, EventArgs e)
        {
            //if (FiddlerApplication.Prefs.GetInt32Pref("fiddler.extensions.inspectors.hexview.bytesperline", 0) < 4)
            //{
                this.SetBytesPerLine(0);
          //  }
        }

        private void hexViewer_InsertActiveChanged(object sender, EventArgs e)
        {
            if (!this.hexViewer.ReadOnly)
            {
                this.sbpInsertMode.Text = this.hexViewer.InsertActive ? "Insert" : "Overwrite";
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.sbTextInfo = new StatusBar();
            this.sbpLocation = new StatusBarPanel();
            this.sbpSelection = new StatusBarPanel();
            this.sbpInsertMode = new StatusBarPanel();
            this.hexViewer = new HexBox();
            this.mnuHexContext = new ContextMenuStrip(this.components);
            this.miInsertFile = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripSeparator();
            this.miSelectBytes = new ToolStripMenuItem();
            this.miSaveSelected = new ToolStripMenuItem();
            this.miGotoByte = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripSeparator();
            this.tsmiShowHeaders = new ToolStripMenuItem();
            this.miContextSetWidth = new ToolStripMenuItem();
            this.miFindBytes = new ToolStripMenuItem();
            this.sbpLocation.BeginInit();
            this.sbpSelection.BeginInit();
            this.sbpInsertMode.BeginInit();
            this.mnuHexContext.SuspendLayout();
            base.SuspendLayout();
            this.sbTextInfo.Location = new Point(0, 590);
            this.sbTextInfo.Name = "sbTextInfo";
            this.sbTextInfo.Panels.AddRange(new StatusBarPanel[] { this.sbpLocation, this.sbpSelection, this.sbpInsertMode });
            this.sbTextInfo.ShowPanels = true;
            this.sbTextInfo.Size = new Size(0x300, 0x16);
            this.sbTextInfo.SizingGrip = false;
            this.sbTextInfo.TabIndex = 3;
            this.sbpLocation.Alignment = HorizontalAlignment.Center;
            this.sbpLocation.Name = "sbpLocation";
            this.sbpLocation.Width = 120;
            this.sbpSelection.Alignment = HorizontalAlignment.Center;
            this.sbpSelection.Name = "sbpSelection";
            this.sbpSelection.Width = 70;
            this.sbpInsertMode.Alignment = HorizontalAlignment.Center;
            this.sbpInsertMode.Name = "sbpInsertMode";
            this.sbpInsertMode.Text = "Read only";
            this.hexViewer.BackColor = Color.LightGray;
            this.hexViewer.BodyOffset = 0;
        this.hexViewer.BorderStyle = BorderStyle.FixedSingle;
            this.hexViewer.BytesPerLine = 0x18;
            this.hexViewer.ContextMenuStrip = this.mnuHexContext;
            this.hexViewer.Dock = DockStyle.Fill;
            this.hexViewer.Font = new Font("Courier New", 8.25f);
            this.hexViewer.HeaderColor = Color.Maroon;
            this.hexViewer.LineInfoVisible = true;
            this.hexViewer.Location = new Point(0, 0);
            this.hexViewer.Name = "hexViewer";
            this.hexViewer.ReadOnly = true;
            this.hexViewer.SelectionLength = 0L;
            this.hexViewer.SelectionStart = -1L;
            this.hexViewer.ShadowSelectionColor = Color.FromArgb(100, 60, 0xbc, 0xff);
            this.hexViewer.Size = new Size(0x300, 590);
            this.hexViewer.StringViewVisible = true;
            this.hexViewer.TabIndex = 4;
            this.hexViewer.UseFixedBytesPerLine = true;
            this.hexViewer.VScrollBarVisible = true;
            this.hexViewer.InsertActiveChanged += new EventHandler(this.hexViewer_InsertActiveChanged);
            this.hexViewer.SelectionStartChanged += new EventHandler(this.UpdateSelectionInfo);
            this.hexViewer.SelectionLengthChanged += new EventHandler(this.UpdateSelectionInfo);
            this.hexViewer.CurrentLineChanged += new EventHandler(this.UpdateCaretLocation);
            this.hexViewer.CurrentPositionInLineChanged += new EventHandler(this.UpdateCaretLocation);
            this.mnuHexContext.Items.AddRange(new ToolStripItem[] { this.miInsertFile, this.toolStripMenuItem1, this.miSelectBytes, this.miSaveSelected, this.miGotoByte, this.miFindBytes, this.toolStripMenuItem2, this.tsmiShowHeaders, this.miContextSetWidth });
            this.mnuHexContext.Name = "mnuHexContext";
            this.mnuHexContext.Size = new Size(0xbb, 0xc0);
            this.mnuHexContext.Opening += new CancelEventHandler(this.mnuHexContext_Opening);
            this.miInsertFile.Name = "miInsertFile";
            this.miInsertFile.Size = new Size(0xba, 0x16);
            this.miInsertFile.Text = "&Insert File Here...";
            this.miInsertFile.Click += new EventHandler(this.miInsertFile_Click);
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(0xb7, 6);
            this.miSelectBytes.Name = "miSelectBytes";
            this.miSelectBytes.Size = new Size(0xba, 0x16);
            this.miSelectBytes.Text = "&Select Bytes...";
            this.miSelectBytes.Click += new EventHandler(this.miSelectBytes_Click);
            this.miSaveSelected.Name = "miSaveSelected";
            this.miSaveSelected.Size = new Size(0xba, 0x16);
            this.miSaveSelected.Text = "S&ave Selected Bytes...";
            this.miSaveSelected.Click += new EventHandler(this.miSaveSelected_Click);
            this.miGotoByte.Name = "miGotoByte";
            this.miGotoByte.ShortcutKeys = Keys.Control | Keys.G;
            this.miGotoByte.Size = new Size(0xba, 0x16);
            this.miGotoByte.Text = "&Goto Offset...";
            this.miGotoByte.Click += new EventHandler(this.miGotoByte_Click);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new Size(0xb7, 6);
            this.tsmiShowHeaders.Checked = true;
            this.tsmiShowHeaders.CheckOnClick = true;
            this.tsmiShowHeaders.CheckState = CheckState.Checked;
            this.tsmiShowHeaders.Name = "tsmiShowHeaders";
            this.tsmiShowHeaders.Size = new Size(0xba, 0x16);
            this.tsmiShowHeaders.Text = "Show &Headers";
            this.miContextSetWidth.Name = "miContextSetWidth";
            this.miContextSetWidth.Size = new Size(0xba, 0x16);
            this.miContextSetWidth.Text = "Set &Width...";
            this.miContextSetWidth.Click += new EventHandler(this.miContextSetWidth_Click);
            this.miFindBytes.Name = "miFindBytes";
            this.miFindBytes.Size = new Size(0xba, 0x16);
            this.miFindBytes.Text = "&Find Bytes...";
            this.miFindBytes.Click += new EventHandler(this.miFindBytes_Click);
            base.Controls.Add(this.hexViewer);
            base.Controls.Add(this.sbTextInfo);
            this.Font = new Font("Tahoma", 8.25f);
            base.Name = "HexBoxView";
            base.Size = new Size(0x300, 0x264);
            base.SizeChanged += new EventHandler(this.HexBoxView_SizeChanged);
            this.sbpLocation.EndInit();
            this.sbpSelection.EndInit();
            this.sbpInsertMode.EndInit();
            this.mnuHexContext.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void miContextSetWidth_Click(object sender, EventArgs e)
        {
            int num;
            string s =  frmPrompt.GetUserString("Column Width", "Enter an integer between 4-80. Enter 0 to Autosize.", this.hexViewer.BytesPerLine.ToString(), true);
            if ((s != null) && int.TryParse(s, out num))
            {
                if (num == 0)
                {
                  //  FiddlerApplication.Prefs.SetInt32Pref("fiddler.extensions.inspectors.hexview.bytesperline", 0);
                    this.SetBytesPerLine(num);
                }
                else
                {
                    if (num < 4)
                    {
                        num = 4;
                    }
                    if (num > 80)
                    {
                        num = 80;
                    }
               //     FiddlerApplication.Prefs.SetInt32Pref("fiddler.extensions.inspectors.hexview.bytesperline", num);
                    this.SetBytesPerLine(num);
                }
            }
        }
        public static bool TryHexParse(string sInput, out int iOutput)
        {
            return int.TryParse(sInput, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out iOutput);
        }
        private void miFindBytes_Click(object sender, EventArgs e)
        {
            string str;
        Label_0000:
            str = "a1 b2 c3";
            string str2 = frmPrompt.GetUserString("Find Bytes", "Enter a sequence of bytes in 2-character hex format, separated by spaces.", str, true);
            if (str2 != null)
            {
                string[] strArray = str2.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                byte[] bytes = new byte[strArray.Length];
                for (int i = 0; i < strArray.Length; i++)
                {
                    int num2;
                    if (TryHexParse(strArray[i], out num2))
                    {
                        bytes[i] = (byte) num2;
                    }
                    else
                    {
                        str = str2;
                        goto Label_0000;
                    }
                }
                long num3 = this.hexViewer.Find(bytes, this.hexViewer.CurrentByte);
                if (num3 < 0L)
                {
                    MessageBox.Show("Hex sequence was not found.", "Not Found");
                }
                else if (num3 < this.hexViewer.ByteProvider.Length)
                {
                    this.hexViewer.SelectionStart = num3;
                    this.hexViewer.SelectionLength = bytes.Length;
                    this.hexViewer.Focus();
                }
            }
        }

        private void miGotoByte_Click(object sender, EventArgs e)
        {
            string s = frmPrompt.GetUserString("Goto Byte", "Enter an integer between 0-" + ((this.hexViewer.ByteProvider.Length - 1L)).ToString() + "\n(Prefix hexadecimal numbers with $)", this.hexViewer.CurrentByte.ToString(), true);
            if (s != null)
            {
                int num;
                bool flag;
                if (s.StartsWith("$", StringComparison.Ordinal))
                {
                    flag = TryHexParse(s.Substring(1), out num);
                }
                else
                {
                    flag = int.TryParse(s, out num);
                }
                if (flag && (num < this.hexViewer.ByteProvider.Length))
                {
                    this.hexViewer.SelectionStart = num;
                    this.hexViewer.SelectionLength = 1L;
                    this.hexViewer.Focus();
                }
            }
        }

        private void miInsertFile_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.Title = "Insert from file";
            dialog.CheckFileExists = true;
            if (DialogResult.OK == dialog.ShowDialog())
            {
                byte[] bs = FileToByteArray(dialog.FileName);
                long currentByte = this.hexViewer.CurrentByte;
                this.hexViewer.ByteProvider.InsertBytes(currentByte, bs);
                this.hexViewer.Refresh();
            }
            dialog.Dispose();
        }

        private void miSaveSelected_Click(object sender, EventArgs e)
        {
            if (this.hexViewer.SelectionLength >= 2L)
            {
                FileDialog dialog = new SaveFileDialog();
                dialog.Title = "Save Selected Bytes To File";
                dialog.Filter = "All Files (*.*)|*.*";
                if (DialogResult.OK == dialog.ShowDialog())
                {
                    FileStream stream = new FileStream(dialog.FileName, FileMode.Create);
                    byte[] buffer = (this.hexViewer.ByteProvider as DynamicByteProvider).Bytes.ToArray();
                    stream.Write(buffer, (int) this.hexViewer.SelectionStart, (int) this.hexViewer.SelectionLength);
                    stream.Close();
                }
                dialog.Dispose();
            }
        }

        private void miSelectBytes_Click(object sender, EventArgs e)
        {
            int num;
            string s = frmPrompt.GetUserString("Select Bytes", "Select how many bytes, up to " + ((this.hexViewer.ByteProvider.Length - this.hexViewer.CurrentByte)).ToString() + "?", "0", true);
            if ((s != null) && (int.TryParse(s, out num) && (num <= (this.hexViewer.ByteProvider.Length - this.hexViewer.CurrentByte))))
            {
                this.hexViewer.SelectionStart = this.hexViewer.CurrentByte;
                this.hexViewer.SelectionLength = num;
                this.hexViewer.Refresh();
            }
        }

        private void mnuHexContext_Opening(object sender, CancelEventArgs e)
        {
            this.miInsertFile.Enabled = !this.hexViewer.ReadOnly;
            bool flag = (this.hexViewer.ByteProvider != null) && (this.hexViewer.ByteProvider.Length > 1L);
            this.miFindBytes.Enabled = this.miGotoByte.Enabled = this.miSelectBytes.Enabled = flag;
            this.miSaveSelected.Enabled = this.hexViewer.SelectionLength > 1L;
        }

        private void SetBytesPerLine(int iBytesPerLine)
        {
            if ((iBytesPerLine > 4) && (iBytesPerLine < 80))
            {
                this.hexViewer.BytesPerLine = iBytesPerLine;
            }
            else
            {
                this.hexViewer.BytesPerLine = Math.Max(4, this.hexViewer.CalculateMaxVisibleBytesPerLineForWidth(this.hexViewer.Width));
            }
        }

        internal void UpdateBytesPerLine()
        {
          //  int iBytesPerLine = FiddlerApplication.Prefs.GetInt32Pref("fiddler.extensions.inspectors.hexview.bytesperline", 0);
         //   this.SetBytesPerLine(iBytesPerLine);
        }

        private void UpdateCaretLocation(object sender, EventArgs e)
        {
            long currentByte = this.hexViewer.CurrentByte;
            if (currentByte < 0L)
            {
                currentByte = 0L;
            }
            if (currentByte >= this.hexViewer.BodyOffset)
            {
                currentByte -= this.hexViewer.BodyOffset;
                this.sbpLocation.Text = currentByte.ToString() + " [0x" + currentByte.ToString("x") + "] of body";
            }
            else
            {
                this.sbpLocation.Text = currentByte.ToString() + " [0x" + currentByte.ToString("x") + "]";
            }
        }

        private void UpdateSelectionInfo(object sender, EventArgs e)
        {
            this.sbpSelection.Text = this.hexViewer.SelectionLength.ToString();
        }
    }
}

