namespace Standard
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class TextView : UserControl
    {
        public Button btnSpawnOther;
        public Button btnSpawnTextEditor;
        private Container components;

        private MenuItem menuItem1;
        private MenuItem menuItem4;
        private MenuItem miTextViewCopy;
        private MenuItem miTextViewCut;
        private MenuItem miTextViewPaste;
        private MenuItem miTextViewShowHeaders;
        private MenuItem miTextViewWordWrap;
        private ContextMenu mnuTextView;
        private StatusBarPanel sbpLinePos;
        private StatusBarPanel sbpLocation;
        private StatusBarPanel sbpSelection;
        private StatusBar sbTextInfo;
        public RichTextBox txtBody;
        private TextBox txtSearchFor;
        private const int WM_VSCROLL = 0x115;
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendCueTextMessage(IntPtr hWnd, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
      

        public static void SetCueText(Control control, string text)
        {
            SendCueTextMessage(control.Handle, 0x1501, IntPtr.Zero, text);
        }
        public TextView()
        {
            this.InitializeComponent();
            //this.m_owner = o;
            SetCueText(this.txtSearchFor, " Find...");
      
           // this.txtSearchFor.cu = "Find...";
        }

        private void CalcCaretLocation()
        {
            this.sbpLinePos.Text = this.txtBody.GetLineFromCharIndex(this.txtBody.SelectionStart).ToString() + ": " + ((this.txtBody.SelectionStart - this.txtBody.GetFirstCharIndexOfCurrentLine())).ToString();
            this.sbpLocation.Text = string.Format("{0}/{1}", this.txtBody.SelectionStart.ToString("N0"), this.txtBody.TextLength.ToString("N0"));
        }

        protected   void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mnuTextView = new ContextMenu();
            this.miTextViewCopy = new MenuItem();
            this.miTextViewCut = new MenuItem();
            this.miTextViewPaste = new MenuItem();
            this.menuItem4 = new MenuItem();
            this.miTextViewWordWrap = new MenuItem();
            this.menuItem1 = new MenuItem();
            this.miTextViewShowHeaders = new MenuItem();
            this.sbTextInfo = new StatusBar();
            this.sbpLinePos = new StatusBarPanel();
            this.sbpLocation = new StatusBarPanel();
            this.sbpSelection = new StatusBarPanel();
            this.txtBody = new RichTextBox();
            this.btnSpawnTextEditor = new Button();
            this.btnSpawnOther = new Button();
            this.txtSearchFor = new TextBox();
            this.sbpLinePos.BeginInit();
            this.sbpLocation.BeginInit();
            this.sbpSelection.BeginInit();
            base.SuspendLayout();
            this.mnuTextView.MenuItems.AddRange(new MenuItem[] { this.miTextViewCopy, this.miTextViewCut, this.miTextViewPaste, this.menuItem4, this.miTextViewWordWrap, this.menuItem1, this.miTextViewShowHeaders });
            this.mnuTextView.Popup += new EventHandler(this.mnuTextView_Popup);
            this.miTextViewCopy.Index = 0;
            this.miTextViewCopy.Text = "&Copy";
            this.miTextViewCopy.Click += new EventHandler(this.miTextViewCopy_Click);
            this.miTextViewCut.Index = 1;
            this.miTextViewCut.Text = "Cu&t";
            this.miTextViewCut.Click += new EventHandler(this.miTextViewCut_Click);
            this.miTextViewPaste.Index = 2;
            this.miTextViewPaste.Text = "&Paste";
            this.miTextViewPaste.Click += new EventHandler(this.miTextViewPaste_Click);
            this.menuItem4.Index = 3;
            this.menuItem4.Text = "-";
            this.miTextViewWordWrap.Index = 4;
            this.miTextViewWordWrap.Text = "&Word Wrap";
            this.miTextViewWordWrap.Click += new EventHandler(this.miTextViewWordWrap_Click);
            this.menuItem1.Index = 5;
            this.menuItem1.Text = "-";
            this.menuItem1.Visible = false;
            this.miTextViewShowHeaders.Index = 6;
            this.miTextViewShowHeaders.Text = "&Show Headers";
            this.miTextViewShowHeaders.Visible = false;
            this.miTextViewShowHeaders.Click += new EventHandler(this.miTextViewShowHeaders_Click);
            this.sbTextInfo.Location = new Point(0, 0x182);
            this.sbTextInfo.Name = "sbTextInfo";
            this.sbTextInfo.Panels.AddRange(new StatusBarPanel[] { this.sbpLinePos, this.sbpLocation, this.sbpSelection });
            this.sbTextInfo.ShowPanels = true;
            this.sbTextInfo.Size = new Size(560, 0x16);
            this.sbTextInfo.SizingGrip = false;
            this.sbTextInfo.TabIndex = 1;
            this.sbpLinePos.Alignment = HorizontalAlignment.Center;
            this.sbpLinePos.AutoSize = StatusBarPanelAutoSize.Contents;
            this.sbpLinePos.Name = "sbpLinePos";
            this.sbpLinePos.Text = "0: 0";
            this.sbpLinePos.Width = 0x21;
            this.sbpLocation.Alignment = HorizontalAlignment.Center;
            this.sbpLocation.AutoSize = StatusBarPanelAutoSize.Contents;
            this.sbpLocation.Name = "sbpLocation";
            this.sbpLocation.Text = "0 / 0";
            this.sbpLocation.Width = 0x24;
            this.sbpSelection.Alignment = HorizontalAlignment.Center;
            this.sbpSelection.AutoSize = StatusBarPanelAutoSize.Contents;
            this.sbpSelection.BorderStyle = StatusBarPanelBorderStyle.None;
            this.sbpSelection.Name = "sbpSelection";
            this.sbpSelection.Width = 10;
            this.txtBody.BackColor = SystemColors.Window;
            this.txtBody.ContextMenu = this.mnuTextView;
            this.txtBody.DetectUrls = false;
            this.txtBody.Dock = DockStyle.Fill;
            this.txtBody.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.txtBody.HideSelection = false;
            this.txtBody.Location = new Point(0, 0);
            this.txtBody.MaxLength = 0x5f5e100;
            this.txtBody.Name = "txtBody";
            this.txtBody.ReadOnly = true;
            this.txtBody.Size = new Size(560, 0x182);
            this.txtBody.TabIndex = 0;
            this.txtBody.Text = "";
            this.txtBody.KeyDown += new KeyEventHandler(this.txtBody_KeyDown);
            this.txtBody.SelectionChanged += new EventHandler(this.txtBody_SelectionChanged);
            this.txtBody.TextChanged += new EventHandler(this.txtBody_TextChanged);
            this.btnSpawnTextEditor.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnSpawnTextEditor.Enabled = false;
            this.btnSpawnTextEditor.FlatStyle = FlatStyle.Flat;
            this.btnSpawnTextEditor.Location = new Point(0x1a8, 0x184);
            this.btnSpawnTextEditor.Name = "btnSpawnTextEditor";
            this.btnSpawnTextEditor.Size = new Size(0x62, 20);
            this.btnSpawnTextEditor.TabIndex = 3;
            this.btnSpawnTextEditor.Text = "View in Notepad";
            this.btnSpawnOther.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnSpawnOther.Enabled = false;
            this.btnSpawnOther.FlatStyle = FlatStyle.Flat;
            this.btnSpawnOther.Location = new Point(0x20c, 0x184);
            this.btnSpawnOther.Name = "btnSpawnOther";
            this.btnSpawnOther.Size = new Size(0x18, 20);
            this.btnSpawnOther.TabIndex = 4;
            this.btnSpawnOther.Text = "...";
            this.txtSearchFor.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.txtSearchFor.BackColor = SystemColors.Window;
            this.txtSearchFor.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearchFor.Location = new Point(0xc0, 0x183);
            this.txtSearchFor.Name = "txtSearchFor";
            this.txtSearchFor.Size = new Size(0xe2, 0x15);
            this.txtSearchFor.TabIndex = 2;
            this.txtSearchFor.WordWrap = false;
            this.txtSearchFor.TextChanged += new EventHandler(this.txtSearchFor_TextChanged);
            this.txtSearchFor.KeyDown += new KeyEventHandler(this.txtSearchFor_KeyDown);
            this.txtSearchFor.Enter += new EventHandler(this.txtSearchFor_Enter);
            base.Controls.Add(this.txtBody);
            base.Controls.Add(this.txtSearchFor);
            base.Controls.Add(this.btnSpawnOther);
            base.Controls.Add(this.btnSpawnTextEditor);
            base.Controls.Add(this.sbTextInfo);
            this.Font = new Font("Tahoma", 8.25f);
            base.Name = "TextView";
            base.Size = new Size(560, 0x198);
            this.sbpLinePos.EndInit();
            this.sbpLocation.EndInit();
            this.sbpSelection.EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void miTextViewCopy_Click(object sender, EventArgs e)
        {
            this.txtBody.Copy();
        }

        private void miTextViewCut_Click(object sender, EventArgs e)
        {
            this.txtBody.Cut();
        }

        private void miTextViewPaste_Click(object sender, EventArgs e)
        {
            this.txtBody.Paste();
        }

        private void miTextViewShowHeaders_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void miTextViewWordWrap_Click(object sender, EventArgs e)
        {
            this.miTextViewWordWrap.Checked = !this.miTextViewWordWrap.Checked;
            this.SetWordWrapState(this.miTextViewWordWrap.Checked);
        }

        private void mnuTextView_Popup(object sender, EventArgs e)
        {
            this.miTextViewWordWrap.Checked = this.txtBody.WordWrap;
            this.miTextViewCopy.Enabled = this.txtBody.SelectionLength > 0;
            this.miTextViewCut.Enabled = (this.txtBody.SelectionLength > 0) && !this.txtBody.ReadOnly;
            this.miTextViewPaste.Enabled = !this.txtBody.ReadOnly && ((Clipboard.GetDataObject() != null) && Clipboard.GetDataObject().GetDataPresent("Text", true));
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private void SetWordWrapState(bool bWordWrap)
        {
            bool modified = this.txtBody.Modified;
            this.txtBody.WordWrap = bWordWrap;
            if (this.txtBody.Modified != modified)
            {
                this.txtBody.Modified = modified;
            }
        }

        private void txtBody_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                this.txtSearchFor.Focus();
                if (this.txtBody.TextLength > 0)
                {
                    int start = Math.Min(this.txtBody.SelectionStart + 1, this.txtBody.TextLength);
                    this.txtBody.Find(this.txtSearchFor.Text, start, RichTextBoxFinds.None);
                }
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.G) && e.Control)
            {
                this.SetWordWrapState(false);
                int lineFromCharIndex = this.txtBody.GetLineFromCharIndex(this.txtBody.SelectionStart);
                int length = this.txtBody.Lines.Length;
                if (length >= 1)
                {
                    int num4;
                    string s = frmPrompt.GetUserString("Goto Line", "Enter an integer between 0-" + ((length - 1)).ToString(), lineFromCharIndex.ToString(), true);
                    if ((s != null) && ((int.TryParse(s, out num4) && (num4 > -1)) && (num4 < length)))
                    {
                        int firstCharIndexFromLine = this.txtBody.GetFirstCharIndexFromLine(num4);
                        int num6 = this.txtBody.Lines[num4].Length;
                        this.txtBody.SelectionStart = firstCharIndexFromLine;
                        this.txtBody.SelectionLength = num6;
                    }
                }
            }
        }

        private void txtBody_SelectionChanged(object sender, EventArgs e)
        {
            this.CalcCaretLocation();
            this.sbpSelection.Text = (this.txtBody.SelectionLength > 0) ? this.txtBody.SelectionLength.ToString("N0") : string.Empty;
        }

        private void txtBody_TextChanged(object sender, EventArgs e)
        {
            this.CalcCaretLocation();
        }

        private void txtSearchFor_Enter(object sender, EventArgs e)
        {
            this.txtSearchFor.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            if (this.txtSearchFor.Text == " Find...")
            {
                this.txtSearchFor.Text = string.Empty;
            }
            this.txtSearchFor.SelectAll();
        }

        private void txtSearchFor_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            if (keyCode <= Keys.Escape)
            {
                if (keyCode != Keys.Return)
                {
                    if (keyCode == Keys.Escape)
                    {
                        this.txtSearchFor.Clear();
                        e.Handled = true;
                        e.Handled = e.SuppressKeyPress = true;
                    }
                    return;
                }
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.Up:
                        SendMessage(this.txtBody.Handle, 0x115, 0, 0);
                        e.Handled = e.SuppressKeyPress = true;
                        return;

                    case Keys.Right:
                        return;

                    case Keys.Down:
                        SendMessage(this.txtBody.Handle, 0x115, 1, 0);
                        e.Handled = e.SuppressKeyPress = true;
                        return;

                    case Keys.F3:
                        goto Label_00AA;

                    default:
                        return;
                }
            }
        Label_00AA:
            if (this.txtBody.TextLength > 0)
            {
                int start = Math.Min(this.txtBody.SelectionStart + 1, this.txtBody.TextLength);
                this.txtBody.Find(this.txtSearchFor.Text, start, RichTextBoxFinds.None);
            }
            e.Handled = e.SuppressKeyPress = true;
        }

        private void txtSearchFor_TextChanged(object sender, EventArgs e)
        {
            this.txtSearchFor.Font = this.txtSearchFor.Font;
            if (this.txtSearchFor.Text.Length > 0)
            {
                this.txtSearchFor.BackColor = (this.txtBody.Find(this.txtSearchFor.Text, 0, RichTextBoxFinds.None) > -1) ? Color.LightGreen : Color.OrangeRed;
            }
            else
            {
                this.txtSearchFor.BackColor = Color.FromKnownColor(KnownColor.Window);
                this.txtBody.Select(0, 0);
            }
        }
    }
}

