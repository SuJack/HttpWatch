namespace Standard
{

    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class ImageViewer
    {
        private bool m_bReadOnly;
        private byte[] m_entityBody;
   
        private ImageInspector myControl;

        public ImageViewer()
        {
            this.myControl = new ImageInspector();
        }

        public  void AddToTab(TabPage o)
        {
            o.Text = "ImageView";
            o.Controls.Add(this.myControl);
            o.Controls[0].Dock = DockStyle.Fill;
        }

        public void Clear()
        {
            this.myControl.ClearImage();
         
            this.m_entityBody = null;
        }

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
 
        public  int GetOrder()
        {
            return -200;
        }

        public byte[] getOriginalBody()
        {
            return this.m_entityBody;
        }

      

        public  int ScoreForContentType(string sMIMEType)
        {
            if (sMIMEType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && !sMIMEType.StartsWith("image/svg+xml", StringComparison.OrdinalIgnoreCase))
            {
                return 80;
            }
            return -1;
        }

        public  void SetFontSize(float flSizeInPoints)
        {
            if (this.myControl != null)
            {
                this.myControl.txtProperties.Font = new Font(this.myControl.txtProperties.Font.FontFamily, flSizeInPoints);
            }
        }

        public   void ShowAboutBox()
        {
            MessageBox.Show("Standard.dll::ImageViewer", "About Inspector");
            if (GetAsyncKeyState(0x10) < 0)
            {
                this.myControl.RecreatePictureBox();
            }
        }

        public bool bDirty
        {
            get
            {
                return false;
            }
        }

        public byte[] body
        {
            get
            {
                return null;
            }
            set
            {
                this.m_entityBody = value;
                if ((this.m_entityBody == null) || (this.m_entityBody.Length == 0))
                {
                    this.Clear();
                    this.myControl.txtProperties.Text = "No body data";
                }
                else
                {
                    try
                    {
                      
                            MemoryStream oStream = new MemoryStream(this.m_entityBody);
                            this.myControl.SetImage(oStream);
                  
                    }
                    catch
                    {
                        this.myControl.ClearImage();
                  
                    }
                }
            }
        }

        public bool bReadOnly
        {
            get
            {
                return this.m_bReadOnly;
            }
            set
            {
                this.m_bReadOnly = value;
            }
        }

    }
}

