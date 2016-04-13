
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SystemMonitor
{
	/// <summary>
	/// Summary description for DataChart.
	/// </summary>
	public class DataChart : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ArrayList _arrayList;

		private Color _colorLine;
		private Color _colorGrid;
		
		private int  _yMaxInit;
		private int  _gridPixel;
		private ChartType _chartType;

        private Color _leftcolor;
        private Panel panel1;
        private Panel panel2;
        private Label label2;
        private Label label1;
        private Panel panel3;

        public Color LeftColor
        {
            set { _leftcolor = value; }
            get { return _leftcolor; }
        }

        private int _maxvalue;

        public int MaxValue
        {
            get { return _maxvalue; }
        }
		#region Constructor/Dispose
		public DataChart()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			BackColor = Color.Silver;

			_colorLine = Color.DarkBlue;
			_colorGrid = Color.Yellow;

			_yMaxInit = 1000;
			_gridPixel = 0;
			_chartType = ChartType.Stick;

			_arrayList = new ArrayList();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

       
		public void UpdateChart(double d)
		{
            if (d > _maxvalue)
            {
                _maxvalue =(int) d;

            }
            if (_maxvalue > _yMaxInit)

                _yMaxInit = (int)_maxvalue;
            this.label2.Text = _yMaxInit.ToString();

			Rectangle rt = this.panel2.ClientRectangle;
			int dataCount = rt.Width/2;

			if (_arrayList.Count >= dataCount) 
				_arrayList.RemoveAt(0);

			_arrayList.Add(d);
       //     this.panel1.BackColor = _colorGrid;
          //  panel1.Invalidate();
			panel2. Invalidate();
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(33, 40);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Yellow;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(32, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1, 40);
            this.panel3.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "200";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(20, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "0";
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(33, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(338, 40);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // DataChart
            // 
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DataChart";
            this.Size = new System.Drawing.Size(371, 40);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region "Properties"

		[Description("Gets or sets the current Line Color in chart"), Category("DataChart")]
		public Color LineColor
		{
			get { return _colorLine; }
			set { _colorLine = value; }
		}

		[Description("Gets or sets the current Grid Color in chart"), Category("DataChart")]
		public Color GridColor
		{
			get { return _colorGrid; }
			set { _colorGrid = value; }
		}

		[Description("Gets or sets the initial maximum Height for sticks in chart"), Category("DataChart")]
		public int InitialHeight
		{
			get { return _yMaxInit; }
			set { _yMaxInit = value; }
		}

		[Description("Gets or sets the current chart Type for stick or Line"), Category("DataChart")]
		public ChartType ChartType
		{
			get { return _chartType; }
			set { _chartType = value; }
		}

		[Description("Enables grid drawing with spacing of the Pixel number"), Category("DataChart")]
		public int GridPixels
		{
			get { return _gridPixel; }
			set { _gridPixel = value; }
		}

		#endregion
  //      private int xoffset = 0;
		#region Drawing

		#endregion

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            int count = _arrayList.Count;
            if (count == 0) return;

          
            double y = 0, yMax = InitialHeight;
            for (int i = 0; i < count; i++)
            {
                y = Convert.ToDouble(_arrayList[i]);
                if (y > yMax) yMax = y;
            }

            Rectangle rt = this.panel2.ClientRectangle;
            y = yMax == 0 ? 1 : rt.Height / yMax;		// y ratio

            int xStart = rt.Width ;
            int yStart = rt.Height;
            int nX, nY;

            Pen pen = null;
            e.Graphics.Clear(BackColor);

            if (GridPixels != 0)
            {
                pen = new Pen(GridColor, 1);
                nX = rt.Width / GridPixels;
                nY = rt.Height / GridPixels;

                //»­ÊúÏß

                for (int i = 1; i <= nX; i++)
                    e.Graphics.DrawLine(pen,  i * GridPixels, 0,  i * GridPixels, yStart);



                //»­ºáÏß
                for (int i = 1; i < nY; i++)
                    e.Graphics.DrawLine(pen, 0, i * GridPixels, xStart, i * GridPixels);


  

          //      e.Graphics.DrawString("0", new Font("SimSun", 9, FontStyle.Regular), Brushes.Yellow, xoffset - 11, yStart - 14);


          //      e.Graphics.DrawString(MaxValue.ToString(), new Font("SimSun", 9, FontStyle.Regular), Brushes.Yellow, xoffset - 11, yStart);
            }

            // From the most recent data, so X <--------------|	
            // Get data from _arrayList	 a[0]..<--...a[count-1]

            if (ChartType == ChartType.Stick)
            {
                pen = new Pen(LineColor, 2);

                for (int i = count - 1; i >= 0; i--)
                {
                    nX = xStart - 2 * (count - i);
                    if (nX <= 0) break;

                    nY = (int)(yStart - y * Convert.ToDouble(_arrayList[i]));
                    e.Graphics.DrawLine(pen, nX, yStart, nX, nY);
                }
            }
            else
                if (ChartType == ChartType.Line)
                {
                    pen = new Pen(LineColor, 1);

                    int nX0 = xStart - 2;
                    int nY0 = (int)(yStart - y * Convert.ToDouble(_arrayList[count - 1]));
                    for (int i = count - 2; i >= 0; i--)
                    {
                        nX = xStart - 2 * (count - i);
                        if (nX <= 0) break;

                    

                        nY = (int)(yStart - y * Convert.ToDouble(_arrayList[i]));
                        e.Graphics.DrawLine(pen, nX0, nY0, nX, nY);
                        if ((double)_arrayList[i] >= (double)_maxvalue && _maxvalue>0)
                        {

                            e.Graphics.DrawString(_arrayList[i].ToString(), new Font("SimSun", 9, FontStyle.Regular), Brushes.Yellow, nX - 11, nY - 14);
                        }

                        nX0 = nX;
                        nY0 = nY;
                    }
                }
        }
	}

	public enum ChartType { Stick, Line }

}
