namespace HttpWatch.Control
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    internal sealed class NativeMethods
    {
        public const int WM_CHAR = 0x102;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WS_BORDER = 0x800000;
        public const int WS_EX_CLIENTEDGE = 0x200;

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool DestroyCaret();
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool SetCaretPos(int X, int Y);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool ShowCaret(IntPtr hWnd);

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public RECT(int left_, int top_, int right_, int bottom_)
            {
                this.Left = left_;
                this.Top = top_;
                this.Right = right_;
                this.Bottom = bottom_;
            }

            public int Height
            {
                get
                {
                    return ((this.Bottom - this.Top) + 1);
                }
            }
            public int Width
            {
                get
                {
                    return ((this.Right - this.Left) + 1);
                }
            }
            public System.Drawing.Size Size
            {
                get
                {
                    return new System.Drawing.Size(this.Width, this.Height);
                }
            }
            public Point Location
            {
                get
                {
                    return new Point(this.Left, this.Top);
                }
            }
            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
            }

            public static HttpWatch.Control.NativeMethods.RECT FromRectangle(Rectangle rectangle)
            {
                return new HttpWatch.Control.NativeMethods.RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return (((this.Left ^ ((this.Top << 13) | (this.Top >> 0x13))) ^ ((this.Width << 0x1a) | (this.Width >> 6))) ^ ((this.Height << 7) | (this.Height >> 0x19)));
            }

            public static implicit operator Rectangle(HttpWatch.Control.NativeMethods.RECT rect)
            {
                return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            public static implicit operator HttpWatch.Control.NativeMethods.RECT(Rectangle rect)
            {
                return new HttpWatch.Control.NativeMethods.RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
        }
    }
}

