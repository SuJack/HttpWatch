namespace Standard
{

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class ImageInspector : UserControl
    {
        private ComboBox cbxAutosize;
        private IContainer components;
        public PictureBox imgPicture;
        private Image m_imgOriginal;

        private ToolStripMenuItem miCopyBitmap;
        private ToolStripMenuItem miSaveToDesktop;
        private ContextMenuStrip mnusContext;
        private Panel panel1;
        public RichTextBox txtProperties;

        internal ImageInspector()
        {
            this.InitializeComponent();
           
            try
            {
                this.cbxAutosize.SelectedIndex = 0;
            }
            catch
            {
            }
            //this.txtProperties.Font = new Font(this.txtProperties.Font.FontFamily,FontStyle.Regular);
        }

        private void actDumpImage()
        {
            if (this.m_imgOriginal != null)
            {
                try
                {
                    this.txtProperties.BackColor = Color.Red;
                    Application.DoEvents();
               
                    string  str = DateTime.Now.ToString("hh-mm-ss") + "." + this.ImageFormatToExt(this.m_imgOriginal.RawFormat);
               
                    string filename = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\" + str;
                    this.m_imgOriginal.Save(filename);
                    this.txtProperties.BackColor = Color.LightSlateGray;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Failed to dump image");
                }
            }
        }

        private void cbxAutosize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DoBestFit(this.cbxAutosize.SelectedIndex);
        //    FiddlerApplication.Prefs.SetInt32Pref("fiddler.extensions.inspectors.images.viewmode", this.cbxAutosize.SelectedIndex);
        }

        public void ClearImage()
        {
            this.m_imgOriginal = null;
            this.imgPicture.Image = null;
            this.txtProperties.Clear();
        }

        protected   void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoBestFit(int iScaleType)
        {
            if (this.m_imgOriginal == null)
            {
                this.imgPicture.Image = null;
            }
            else
            {
                try
                {
                    if ((iScaleType == 2) || (((iScaleType == 0) && (this.imgPicture.Width >= this.m_imgOriginal.Width)) && (this.imgPicture.Height >= this.m_imgOriginal.Height)))
                    {
                        this.imgPicture.Image = null;
                        this.imgPicture.Image = this.m_imgOriginal;
                    }
                    else
                    {
                        Size size = this.GetBestFit(this.m_imgOriginal.Width, this.m_imgOriginal.Height, this.imgPicture.Width, this.imgPicture.Height);
                        Bitmap bitmap = new Bitmap(this.m_imgOriginal, size.Width, size.Height);
                        this.imgPicture.Image = null;
                        this.imgPicture.Image = bitmap;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private Size GetBestFit(int originWidth, int originHeight, int targetWidth, int targetHeight)
        {
            double num = Math.Min((double) (((double) targetWidth) / ((double) originWidth)), (double) (((double) targetHeight) / ((double) originHeight)));
            return new Size((int) (originWidth * num), (int) (originHeight * num));
        }

        public static string GetMimeTypeFromImage(Image i)
        {
            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageDecoders())
            {
                if (info.FormatID == i.RawFormat.Guid)
                {
                    return info.MimeType;
                }
            }
            return "image/unknown";
        }

        private string ImageFormatToExt(ImageFormat o)
        {
            string str = ".dat";
            if (o.Equals(ImageFormat.Bmp))
            {
                return "bmp";
            }
            if (o.Equals(ImageFormat.Png))
            {
                return "png";
            }
            if (o.Equals(ImageFormat.Jpeg))
            {
                return "jpg";
            }
            if (o.Equals(ImageFormat.Gif))
            {
                return "gif";
            }
            if (o.Equals(ImageFormat.MemoryBmp))
            {
                return "bmp";
            }
            if (o.Equals(ImageFormat.Icon))
            {
                return "ico";
            }
            if (o.Equals(ImageFormat.Emf))
            {
                return "emf";
            }
            if (o.Equals(ImageFormat.Exif))
            {
                return "exif";
            }
            if (o.Equals(ImageFormat.Tiff))
            {
                return "tif";
            }
            if (o.Equals(ImageFormat.Wmf))
            {
                str = "wmf";
            }
            return str;
        }

        private string ImageFormatToString(ImageFormat o)
        {
            string str = "<unknown>";
            if (o.Equals(ImageFormat.Bmp))
            {
                return "BMP";
            }
            if (o.Equals(ImageFormat.Emf))
            {
                return "EMF";
            }
            if (o.Equals(ImageFormat.Exif))
            {
                return "EXIF";
            }
            if (o.Equals(ImageFormat.Gif))
            {
                return "GIF";
            }
            if (o.Equals(ImageFormat.Icon))
            {
                return "Icon";
            }
            if (o.Equals(ImageFormat.Jpeg))
            {
                return "JPEG";
            }
            if (o.Equals(ImageFormat.MemoryBmp))
            {
                return "MemoryBMP";
            }
            if (o.Equals(ImageFormat.Png))
            {
                return "PNG";
            }
            if (o.Equals(ImageFormat.Tiff))
            {
                return "TIFF";
            }
            if (o.Equals(ImageFormat.Wmf))
            {
                str = "WMF";
            }
            return str;
        }

        private void Images_Resize(object sender, EventArgs e)
        {
            if (this.cbxAutosize.SelectedIndex != 2)
            {
                this.DoBestFit(this.cbxAutosize.SelectedIndex);
            }
        }

        private void imgPicture_DoubleClick(object sender, EventArgs e)
        {
            if (this.m_imgOriginal != null)
            {
                Form oNewForm = new Form();
                oNewForm.BackColor = Color.White;
                PictureBox oBigPic = new PictureBox();
                oBigPic.Dock = DockStyle.Fill;
                oBigPic.Cursor = Cursors.Cross;
                oBigPic.Image = this.m_imgOriginal;
                oBigPic.SizeMode = PictureBoxSizeMode.Zoom;
                oBigPic.Parent = oNewForm;
                oNewForm.FormBorderStyle = FormBorderStyle.None;
                oNewForm.WindowState = FormWindowState.Maximized;
                oNewForm.KeyPreview = true;
                oNewForm.KeyUp += delegate (object s, KeyEventArgs k) {
                    if (k.KeyCode == Keys.Escape)
                    {
                        k.Handled = true;
                        k.SuppressKeyPress = true;
                        (s as Form).Close();
                    }
                };
                oNewForm.KeyDown += delegate (object s, KeyEventArgs k) {
                    Keys keyCode = k.KeyCode;
                    if (keyCode <= Keys.H)
                    {
                        if (keyCode != Keys.Return)
                        {
                            if (keyCode == Keys.H)
                            {
                                try
                                {
                                    oBigPic.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                    oBigPic.Refresh();
                                }
                                catch (Exception)
                                {
                                }
                            }
                            return;
                        }
                    }
                    else
                    {
                        if (keyCode == Keys.R)
                        {
                            try
                            {
                                oBigPic.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                oBigPic.Refresh();
                            }
                            catch (Exception)
                            {
                            }
                            return;
                        }
                        if (keyCode != Keys.Z)
                        {
                            return;
                        }
                    }
                    this.TogglePictureBoxZoom(oBigPic);
                };
                oNewForm.MouseWheel += delegate (object s, MouseEventArgs args) {
                    if (args.Delta > 0)
                    {
                        oBigPic.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        oBigPic.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                };
                oBigPic.MouseUp += delegate (object s, MouseEventArgs args) {
                    if (args.Button != MouseButtons.Left)
                    {
                        oNewForm.Close();
                    }
                };
                oNewForm.ShowDialog();
                oNewForm.Dispose();
            }
        }

        private void imgPicture_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                this.actDumpImage();
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtProperties = new System.Windows.Forms.RichTextBox();
            this.cbxAutosize = new System.Windows.Forms.ComboBox();
            this.imgPicture = new System.Windows.Forms.PictureBox();
            this.mnusContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyBitmap = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveToDesktop = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPicture)).BeginInit();
            this.mnusContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtProperties);
            this.panel1.Controls.Add(this.cbxAutosize);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(96, 280);
            this.panel1.TabIndex = 3;
            // 
            // txtProperties
            // 
            this.txtProperties.BackColor = System.Drawing.Color.LightSlateGray;
            this.txtProperties.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProperties.ForeColor = System.Drawing.Color.White;
            this.txtProperties.Location = new System.Drawing.Point(0, 0);
            this.txtProperties.Name = "txtProperties";
            this.txtProperties.ReadOnly = true;
            this.txtProperties.Size = new System.Drawing.Size(96, 260);
            this.txtProperties.TabIndex = 2;
            this.txtProperties.Text = "Not an image";
            // 
            // cbxAutosize
            // 
            this.cbxAutosize.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbxAutosize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAutosize.Items.AddRange(new object[] {
            "Autoshrink",
            "Scale to fit",
            "No scaling"});
            this.cbxAutosize.Location = new System.Drawing.Point(0, 260);
            this.cbxAutosize.Name = "cbxAutosize";
            this.cbxAutosize.Size = new System.Drawing.Size(96, 20);
            this.cbxAutosize.TabIndex = 0;
            this.cbxAutosize.SelectedIndexChanged += new System.EventHandler(this.cbxAutosize_SelectedIndexChanged);
            // 
            // imgPicture
            // 
            this.imgPicture.BackColor = System.Drawing.Color.AliceBlue;
            this.imgPicture.ContextMenuStrip = this.mnusContext;
            this.imgPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgPicture.Location = new System.Drawing.Point(96, 0);
            this.imgPicture.Name = "imgPicture";
            this.imgPicture.Size = new System.Drawing.Size(272, 280);
            this.imgPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imgPicture.TabIndex = 4;
            this.imgPicture.TabStop = false;
            this.imgPicture.DoubleClick += new System.EventHandler(this.imgPicture_DoubleClick);
            this.imgPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imgPicture_MouseUp);
            // 
            // mnusContext
            // 
            this.mnusContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyBitmap,
            this.miSaveToDesktop});
            this.mnusContext.Name = "mnusContext";
            this.mnusContext.Size = new System.Drawing.Size(173, 70);
            this.mnusContext.Opening += new System.ComponentModel.CancelEventHandler(this.mnusContext_Opening);
            // 
            // miCopyBitmap
            // 
            this.miCopyBitmap.Name = "miCopyBitmap";
            this.miCopyBitmap.Size = new System.Drawing.Size(172, 22);
            this.miCopyBitmap.Text = "&Copy Bitmap";
            this.miCopyBitmap.Click += new System.EventHandler(this.miCopyImage_Click);
            // 
            // miSaveToDesktop
            // 
            this.miSaveToDesktop.Name = "miSaveToDesktop";
            this.miSaveToDesktop.Size = new System.Drawing.Size(172, 22);
            this.miSaveToDesktop.Text = "&Save to Desktop";
            this.miSaveToDesktop.Click += new System.EventHandler(this.miSaveToDesktop_Click);
            // 
            // ImageInspector
            // 
            this.Controls.Add(this.imgPicture);
            this.Controls.Add(this.panel1);
            this.Name = "ImageInspector";
            this.Size = new System.Drawing.Size(368, 280);
            this.Resize += new System.EventHandler(this.Images_Resize);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgPicture)).EndInit();
            this.mnusContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void miCopyDataURI_Click(object sender, EventArgs e)
        {
            
        }

        private void miCopyImage_Click(object sender, EventArgs e)
        {
            if (this.m_imgOriginal != null)
            {
                DataObject oData = new DataObject(this.m_imgOriginal);
                Clipboard.SetDataObject(oData);
               // Utilities.CopyToClipboard(oData);
            }
        }

        private void miSaveToDesktop_Click(object sender, EventArgs e)
        {
            this.actDumpImage();
        }

        private void mnusContext_Opening(object sender, CancelEventArgs e)
        {
            this.miCopyBitmap.Enabled = true ;
            this.miSaveToDesktop.Enabled = true;
        }

        internal void RecreatePictureBox()
        {
            base.Controls.Remove(this.imgPicture);
            base.SuspendLayout();
            this.imgPicture = new PictureBox();
         //   this.imgPicture.BackColor = System.Drawing.Color.AliceBlue;
            this.imgPicture.Location = new Point(0x60, 0);
            this.imgPicture.Name = "imgPicture";
            this.imgPicture.Size = new Size(0x110, 280);
            this.imgPicture.SizeMode = PictureBoxSizeMode.CenterImage;
            this.imgPicture.TabIndex = 4;
            this.imgPicture.TabStop = false;
            this.imgPicture.MouseUp += new MouseEventHandler(this.imgPicture_MouseUp);
            base.Controls.Add(this.imgPicture);
            base.ResumeLayout(true);
            this.imgPicture.BringToFront();
            this.imgPicture.Dock = DockStyle.Fill;
        }

        public void SetImage(MemoryStream oStream)
        {
            this.m_imgOriginal = new Bitmap(oStream);
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("{0:N0} bytes\n\n", oStream.Length);
            builder.AppendFormat("{0}w x {1}h\n\n", this.m_imgOriginal.Width, this.m_imgOriginal.Height);
            builder.AppendFormat("Format: {0}\n\n", this.ImageFormatToString(this.m_imgOriginal.RawFormat));
            this.txtProperties.Text = builder.ToString();
            this.DoBestFit(this.cbxAutosize.SelectedIndex);
        }

        private void TogglePictureBoxZoom(PictureBox oPic)
        {
            if (oPic.SizeMode == PictureBoxSizeMode.Zoom)
            {
                oPic.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                oPic.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
    }
}

