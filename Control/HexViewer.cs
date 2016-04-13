namespace Standard
{
    using HttpWatch.Control;

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class HexViewer 
    {
        private bool _bShowHeaders = true;
        private Label lblLoading;
        private byte[] m_entityBody;
     /// <summary>
     /// 
     /// </summary>
 //  private HTTPResponseHeaders m_Headers;
        private HexBoxView myControl;
        private TabPage myTabPage;

        public  void AddToTab(TabPage o)
        {
            this.myTabPage = o;
            this.lblLoading = new Label();
            this.lblLoading.Size = new Size(150, 0x19);
            this.lblLoading.ForeColor = Color.Red;
            this.lblLoading.Font = new Font("Tahoma", 10f);
            this.lblLoading.Location = new Point(0, 0);
            this.lblLoading.Text = "Loading...";
            this.myTabPage.Controls.Add(this.lblLoading);
            this.myTabPage.Text = "HexView";
        }

        public void Clear()
        {
            this.m_entityBody = null;
            this.EnsureReady();
            if (this.myControl.hexViewer.ByteProvider != null)
            {
                IDisposable byteProvider = this.myControl.hexViewer.ByteProvider as IDisposable;
                if (byteProvider != null)
                {
                    byteProvider.Dispose();
                }
                this.myControl.hexViewer.ByteProvider = null;
            }
        }

        private void EnsureReady()
        {
            if (this.myControl == null)
            {
                this.lblLoading.Refresh();
                this.myControl = new HexBoxView();
                this.myControl.tsmiShowHeaders.CheckedChanged += new EventHandler(this.tsmiShowHeaders_CheckedChanged);
                this.myControl.hexViewer.HeaderColor = Color.Green;
                this.myTabPage.Controls.Add(this.myControl);
                this.myControl.Dock = DockStyle.Fill;
                this.myControl.UpdateBytesPerLine();
                this.lblLoading.Visible = false;
            }
        }

        public  int GetOrder()
        {
            return -100;
        }

        public   int ScoreForContentType(string sMIMEType)
        {
            if (sMIMEType.StartsWith("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            {
                return 50;
            }
            return 0;
        }

        public   void SetFontSize(float flSizeInPoints)
        {
            if (this.myControl != null)
            {
                this.myControl.hexViewer.Font = new Font(this.myControl.hexViewer.Font.FontFamily, flSizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
                this.myControl.UpdateBytesPerLine();
            }
        }

        public   void ShowAboutBox()
        {
            MessageBox.Show("Standard.dll::HexResponseViewer\n\nBe.HexEditor Control\nhttp://sourceforge.net/projects/hexbox/", "About Inspector");
        }

        private void tsmiShowHeaders_CheckedChanged(object sender, EventArgs e)
        {
            this._bShowHeaders = this.myControl.tsmiShowHeaders.Checked;
            this.UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            byte[] buffer;
       
                this.myControl.hexViewer.BodyOffset = 0;
                buffer = new byte[0];
        
            if (this.m_entityBody == null)
            {
                this.m_entityBody = new byte[0];
            }
            byte[] array = new byte[buffer.Length + this.m_entityBody.Length];
            buffer.CopyTo(array, 0);
            this.m_entityBody.CopyTo(array, buffer.Length);
            DynamicByteProvider provider = new DynamicByteProvider(array);
            this.myControl.hexViewer.ByteProvider = provider;
            this.myControl.hexViewer.ByteProvider.ApplyChanges();
        }

        public bool bDirty
        {
            get
            {
                if (this.myControl == null)
                {
                    return false;
                }
                if (this.myControl.hexViewer.ByteProvider == null)
                {
                    return false;
                }
                return this.myControl.hexViewer.ByteProvider.HasChanges();
            }
        }

        public byte[] body
        {
            get
            {
                this.EnsureReady();
                if (this.bDirty && (this.myControl.hexViewer.ByteProvider != null))
                {
                    this.m_entityBody = (this.myControl.hexViewer.ByteProvider as DynamicByteProvider).Bytes.ToArray();
                }
                return this.m_entityBody;
            }
            set
            {
                this.EnsureReady();
                this.m_entityBody = value;
                this.UpdateDisplay();
            }
        }

        public bool bReadOnly
        {
            get
            {
                return ((this.myControl == null) || this.myControl.hexViewer.ReadOnly);
            }
            set
            {
                this.EnsureReady();
                this.myControl.hexViewer.ReadOnly = value;
                if (value)
                {
                    this.myControl.hexViewer.BackColor = System.Drawing.Color.AliceBlue;
                    this.myControl.sbpInsertMode.Text = "Read only";
                    this.myControl.tsmiShowHeaders.Enabled = true;
                }
                else
                {
                    this._bShowHeaders = false;
                    this.myControl.tsmiShowHeaders.Checked = this.myControl.tsmiShowHeaders.Enabled = false;
                    this.myControl.hexViewer.BackColor = Color.FromKnownColor(KnownColor.Window);
                    this.myControl.sbpInsertMode.Text = this.myControl.hexViewer.InsertActive ? "Insert" : "Overwrite";
                }
            }
        }

    }
}

