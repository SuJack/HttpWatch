namespace Standard
{

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class TextViewer 
    {
        private string _SuggestedFilename = "nofile.htm";
        private bool m_bShowHeaders;
        public Encoding m_Encoding = Encoding.UTF8;
        private byte[] m_entityBody;

        private TextView myControl;

        public TextViewer()
        {
            this.myControl = new TextView();
            this.myControl.btnSpawnOther.Click += new EventHandler(this.btnSpawnOther_Click);
            this.myControl.btnSpawnTextEditor.Click += new EventHandler(this.btnSpawnTextEditor_Click);
        }

        public  void AddToTab(TabPage o)
        {
            this.myControl.txtBody.Modified = false;
            o.Text = "TextView";
            this.myControl.txtBody.Font = o.Font;
            o.Controls.Add(this.myControl);
            o.Controls[0].Dock = DockStyle.Fill;
        }

        private void btnSpawnOther_Click(object sender, EventArgs e)
        {
            string sFilename =Path.GetTempPath() + @"\" + this._SuggestedFilename;
            this.WriteFile(sFilename);
            using (Process.Start("rundll32", "shell32.dll,OpenAs_RunDLL " + sFilename))
            {
            }
        }

        private void btnSpawnTextEditor_Click(object sender, EventArgs e)
        {
            string sFilename = Path.GetTempPath()  + @"\" +System.DateTime.Now.ToFileTime()+".txt" ;
            try
            {
               
                this.WriteFile(sFilename);
                using (Process.Start(sFilename))
                {
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + sFilename, "Failure Notice");
            }
        }

        public void Clear()
        {
            this.m_entityBody = null;
  
            this.m_Encoding = Encoding.UTF8;
            this.myControl.txtBody.Clear();
            this.myControl.txtBody.Modified = false;
            this.myControl.btnSpawnOther.Enabled = false;
            this.myControl.btnSpawnTextEditor.Enabled = false;
        }

        public   int GetOrder()
        {
            return -400;
        }
        public void SetText(string text)
        {
            myControl.txtBody.WordWrap = false;
           
     
            this.myControl.txtBody.Text = text;
           
            this.myControl.txtBody.Modified = true;
       
            this.myControl.btnSpawnOther.Enabled = true;
            this.myControl.btnSpawnTextEditor.Enabled = true;
        }
        private void Recalc()
        {
           
        }

        public  int ScoreForContentType(string sMIMEType)
        {
            if (sMIMEType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
            {
                return 50;
            }
            return -1;
        }

        public   void SetFontSize(float flSizeInPoints)
        {
            this.myControl.txtBody.Font = new Font(this.myControl.txtBody.Font.FontFamily, flSizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void WriteFile(string sFilename)
        {
          StreamWriter sw=  System.IO.File.CreateText(sFilename);
          sw.Write(this.myControl.txtBody.Text.Replace("\n",Environment.NewLine));
          sw.Close();

        }

        public bool bDirty
        {
            get
            {
                return this.myControl.txtBody.Modified;
            }
        }

        public byte[] body
        {
            get
            {
                if (this.m_Encoding != null)
                {
                    this.m_entityBody = this.m_Encoding.GetBytes(this.myControl.txtBody.Text);
                }
                else
                {
                    this.m_entityBody = Encoding.UTF8.GetBytes(this.myControl.txtBody.Text);
                }
                return this.m_entityBody;
            }
            set
            {
                this.m_entityBody = value;
                this.Recalc();
            }
        }

        public bool bReadOnly
        {
            get
            {
                return this.myControl.txtBody.ReadOnly;
            }
            set
            {
                if (this.m_bShowHeaders)
                {
                    value = true;
                }
                this.myControl.txtBody.ReadOnly = value;
                if (value)
                {
                    this.myControl.txtBody.BackColor = System.Drawing.Color.AliceBlue;
                    this.myControl.txtBody.AcceptsTab = false;
                }
                else
                {
                    this.myControl.txtBody.BackColor = Color.FromKnownColor(KnownColor.Window);
                    this.myControl.txtBody.AcceptsTab = true;
                }
            }
        }

        public bool bShowHeaders
        {
            get
            {
                return this.m_bShowHeaders;
            }
            set
            {
                this.m_bShowHeaders = value;
                this.Recalc();
            }
        }

    }
}

