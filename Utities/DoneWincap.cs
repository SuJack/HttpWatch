using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace CnCerT.Net.Winpcap
{

    /// <summary>
    /// 
    /// </summary>
    public class Device
    {
 
       private string name;
        private string description;
        private string address;
        private string netmask;
        public override string ToString()
        {
            return description;
        }
        public Device()
        {
            this.name = null;
            this.description = null;
            this.address = null;
            this.netmask = null;
        }


        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }


        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
            }
        }


        public string Netmask
        {
            get
            {
                return this.netmask;
            }
            set
            {
                this.netmask = value;
            }
        }


    }

    public class dotnetWinpCap
    {

        [DllImport("Kernel32")]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("wpcap.dll")]
        private static extern void pcap_close(IntPtr p);
        [DllImport("wpcap.dll")]
        private static extern void pcap_dump(IntPtr dumper, IntPtr h, IntPtr data);
        [DllImport("wpcap.dll")]
        private static extern void pcap_dump_close(IntPtr dumper);
        [DllImport("wpcap.dll")]
        private static extern IntPtr pcap_dump_open(IntPtr p, string filename);
        [DllImport("wpcap.dll")]
        private static extern int pcap_findalldevs(ref IntPtr devicelist, StringBuilder errbuf);
        [DllImport("wpcap.dll")]
        private static extern void pcap_freealldevs(IntPtr devicelist);
        [DllImport("wpcap.dll")]
        private static extern int pcap_live_dump(IntPtr p, string filename, int maxsize, int maxpacks);
        [DllImport("wpcap.dll")]
        private static extern int pcap_live_dump_ended(IntPtr p, int sync);
        [DllImport("wpcap.dll")]
        private static extern int pcap_loop(IntPtr p, int cnt, packet_handler callback, IntPtr user);
        [DllImport("wpcap.dll")]
        private static extern byte[] pcap_next(IntPtr p, IntPtr pkt_header);
        [DllImport("wpcap.dll")]
        private static extern int pcap_next_ex(IntPtr p, ref IntPtr pkt_header, ref IntPtr packetdata);
        [DllImport("wpcap.dll")]
        private static extern IntPtr pcap_open(string source, int snaplen, int flags, int read_timeout, IntPtr auth, StringBuilder errbuf);
        [DllImport("wpcap.dll")]
        private static extern IntPtr pcap_open_live(string device, int snaplen, int promisc, int to_ms, StringBuilder ebuf);
        [DllImport("wpcap.dll")]
        private static extern int pcap_sendpacket(IntPtr p, byte[] buff, int size);
        [DllImport("wpcap.dll")]
        private static extern int pcap_setbuff(IntPtr p, int kernelbufferbytes);
        [DllImport("wpcap.dll")]
        private static extern int pcap_setmintocopy(IntPtr p, int size);


        // Fields
        private packet_handler callback;
        private bool disposed;
        private IntPtr dumper;
        private StringBuilder errbuf;
        private string fname;
        private Thread ListenThread;
        private string m_attachedDevice;
        private bool m_islistening;
        private bool m_isopen;
        private int maxb;
        private int maxp;
        public  DumpEnded OnDumpEnded;
        public  ReceivePacket OnReceivePacket;
        public  ReceivePacketInternal OnReceivePacketInternal;
        private IntPtr pcap_t;


        private delegate void packet_handler(IntPtr param, IntPtr header, IntPtr pkt_data);



        public enum PCAP_NEXT_EX_STATE
        {
            // Fields
            EOF = -2,
            ERROR = -1,
            SUCCESS = 1,
            TIMEOUT = 0,
            UNKNOWN = -3
        }

        public delegate void ReceivePacket(object sender, PacketHeader p, byte[] s);

        public delegate void DumpEnded(object sender);

        public delegate void ReceivePacketInternal(object sender, IntPtr header, IntPtr data);



        [StructLayout(LayoutKind.Sequential)]
        public struct pcap_pkthdr
        {
            public dotnetWinpCap.timeval ts;
            public uint caplen;
            public uint len;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct pcap_rmtauth
        {
            private int type;
            private string username;
            private string password;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct sockaddr
        {
            public short family;
            public ushort port;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] addr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct timeval
        {
            public uint tv_sec;
            public uint tv_usec;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct in_addr
        {
            public char b1;
            public char b2;
            public char b3;
            public char b4;
            public ushort w1;
            public ushort w2;
            public ulong addr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct pcap_addr
        {
            public IntPtr next;
            public IntPtr addr;
            public IntPtr netmask;
            public IntPtr broadaddr;
            public IntPtr dstaddr;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct pcap_if
        {
            public IntPtr next;
            public string name;
            public string description;
            public IntPtr addresses;
            public uint flags;
        }

        public dotnetWinpCap()
        {
            this.ListenThread = null;
            this.disposed = false;
            this.callback = null;
            this.fname = "";
            this.maxb = 0;
            this.maxp = 0;
            this.m_islistening = false;
            this.m_isopen = false;
            this.m_attachedDevice = null;
            this.pcap_t = IntPtr.Zero;
            this.dumper = IntPtr.Zero;
            this.errbuf = new StringBuilder(0x100);
        }

        public static ArrayList FindAllDevs()
        {
            try
            {
                dotnetWinpCap.pcap_if _if1;
                ArrayList list1 = new ArrayList();
                _if1.addresses = IntPtr.Zero;
                _if1.description = new StringBuilder().ToString();
                _if1.flags = 0;
                _if1.name = new StringBuilder().ToString();
                _if1.next = IntPtr.Zero;
                IntPtr ptr1 = IntPtr.Zero;
                StringBuilder builder1 = new StringBuilder(0x100);
                IntPtr ptr2 = IntPtr.Zero;
                if (dotnetWinpCap.pcap_findalldevs(ref ptr1, builder1) == -1)
                {
                    return null;
                }
                ptr2 = ptr1;
                while (ptr1.ToInt32() != 0)
                {
                    Device device1 = new Device();
                    list1.Add(device1);
                    _if1 = (dotnetWinpCap.pcap_if)Marshal.PtrToStructure(ptr1, typeof(dotnetWinpCap.pcap_if));
                    device1.Name = _if1.name;
                    device1.Description = _if1.description;
                    if (_if1.addresses.ToInt32() != 0)
                    {
                        dotnetWinpCap.pcap_addr _addr1 = (dotnetWinpCap.pcap_addr)Marshal.PtrToStructure(_if1.addresses, typeof(dotnetWinpCap.pcap_addr));
                        if (_addr1.addr.ToInt32() != 0)
                        {
                            dotnetWinpCap.sockaddr sockaddr1 = (dotnetWinpCap.sockaddr)Marshal.PtrToStructure(_addr1.addr, typeof(dotnetWinpCap.sockaddr));
                            device1.Address = sockaddr1.addr[0].ToString() + "." + sockaddr1.addr[1].ToString() + "." + sockaddr1.addr[2].ToString() + "." + sockaddr1.addr[3].ToString();
                        }
                        if (_addr1.netmask.ToInt32() != 0)
                        {
                            dotnetWinpCap.sockaddr sockaddr2 = (dotnetWinpCap.sockaddr)Marshal.PtrToStructure(_addr1.netmask, typeof(dotnetWinpCap.sockaddr));
                            device1.Netmask = sockaddr2.addr[0].ToString() + "." + sockaddr2.addr[1].ToString() + "." + sockaddr2.addr[2].ToString() + "." + sockaddr2.addr[3].ToString();
                        }
                    }
                    ptr1 = _if1.next;
                } 
                dotnetWinpCap.pcap_freealldevs(ptr2);
                return list1;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public class AlreadyOpenException : Exception
        {
            // Methods
            public AlreadyOpenException()
            {
            }




            public override string Message
            {
                get
                {
                    return "Device attached to object already open. Close first before reopening";
                }
            }


        }



        public bool Open(string source, int snaplen, int flags, int read_timeout)
        {
            if (this.pcap_t != IntPtr.Zero)
            {
                throw new dotnetWinpCap.AlreadyOpenException();
            }
            this.pcap_t = dotnetWinpCap.pcap_open(source, snaplen, flags, read_timeout, IntPtr.Zero, this.errbuf);
            if (this.pcap_t.ToInt32() != 0)
            {
                this.m_isopen = true;
                this.m_attachedDevice = source;
                return true;
            }
            this.m_isopen = false;
            this.m_attachedDevice = null;
            return false;
        }



        private void Loop()
        {
            this.callback = new dotnetWinpCap.packet_handler(this.LoopCallback);
            IntPtr ptr1 = IntPtr.Zero;
            new HandleRef(this.callback, ptr1);
            dotnetWinpCap.pcap_loop(this.pcap_t, 0, this.callback, IntPtr.Zero);
        }


        private void LoopCallback(IntPtr param, IntPtr header, IntPtr pkt_data)
        {
            Marshal.PtrToStringAnsi(param);
            dotnetWinpCap.pcap_pkthdr _pkthdr1 = (dotnetWinpCap.pcap_pkthdr)Marshal.PtrToStructure(header, typeof(dotnetWinpCap.pcap_pkthdr));
            byte[] buffer1 = new byte[_pkthdr1.caplen];
            Marshal.Copy(pkt_data, buffer1, 0, (int)_pkthdr1.caplen);
            Marshal.PtrToStringAnsi(pkt_data);
        }


        private bool OpenLive(string source, int snaplen, int promisc, int to_ms)
        {
            this.pcap_t = dotnetWinpCap.pcap_open_live(source, snaplen, promisc, to_ms, this.errbuf);
            if (this.pcap_t.ToInt32() != 0)
            {
                return true;
            }
            return false;
        }



        private dotnetWinpCap.PCAP_NEXT_EX_STATE ReadNextInternal(out PacketHeader p, out byte[] packet_data, out IntPtr pkthdr, out IntPtr pktdata)
        {
            pkthdr = IntPtr.Zero;
            pktdata = IntPtr.Zero;
            p = null;
            packet_data = null;
            if (this.pcap_t.ToInt32() == 0)
            {
                this.errbuf = new StringBuilder("No adapter is currently open");
                return dotnetWinpCap.PCAP_NEXT_EX_STATE.ERROR;
            }
            switch (dotnetWinpCap.pcap_next_ex(this.pcap_t, ref pkthdr, ref pktdata))
            {
                case 0:
                    return dotnetWinpCap.PCAP_NEXT_EX_STATE.TIMEOUT;

                case 1:
                    {
                        dotnetWinpCap.pcap_pkthdr _pkthdr1 = (dotnetWinpCap.pcap_pkthdr)Marshal.PtrToStructure(pkthdr, typeof(dotnetWinpCap.pcap_pkthdr));
                        p = new PacketHeader();
                        p.Caplength = (int)_pkthdr1.caplen;
                        p.Length = (int)_pkthdr1.len;
                        p.ts = _pkthdr1.ts;
                        packet_data = new byte[p.Length];
                        Marshal.Copy(pktdata, packet_data, 0, p.Length);
                        return dotnetWinpCap.PCAP_NEXT_EX_STATE.SUCCESS;
                    }
                case -1:
                    return dotnetWinpCap.PCAP_NEXT_EX_STATE.ERROR;

                case -2:
                    return dotnetWinpCap.PCAP_NEXT_EX_STATE.EOF;
            }
            return dotnetWinpCap.PCAP_NEXT_EX_STATE.UNKNOWN;
        }


        public dotnetWinpCap.PCAP_NEXT_EX_STATE ReadNextInternal(out PacketHeader p, out byte[] packet_data)
        {
            IntPtr ptr1;
            return this.ReadNextInternal(out p, out packet_data, out ptr1, out ptr1);
        }




        public bool SendPacket(byte[] packet_data)
        {
            if (dotnetWinpCap.pcap_sendpacket(this.pcap_t, packet_data, packet_data.Length) == 0)
            {
                return true;
            }
            return false;
        }



        private void MonitorDump()
        {
            if ((dotnetWinpCap.pcap_live_dump_ended(this.pcap_t, 1) != 0) && (this.OnDumpEnded != null))
            {
                this.OnDumpEnded(this);
            }
        }



        private void DumpPacket(object sender, IntPtr header, IntPtr data)
        {
            if (this.dumper != IntPtr.Zero)
            {
                dotnetWinpCap.pcap_dump(this.dumper, header, data);
            }
        }


        public void StopDump()
        {
            this.OnReceivePacketInternal = (dotnetWinpCap.ReceivePacketInternal)Delegate.Remove(this.OnReceivePacketInternal, new dotnetWinpCap.ReceivePacketInternal(this.DumpPacket));
            if (this.dumper != IntPtr.Zero)
            {
                dotnetWinpCap.pcap_dump_close(this.dumper);
                this.dumper = IntPtr.Zero;
            }
        }



        private bool StartLiveDump(string filename, int maxbytes, int maxpackets)
        {
            this.fname = filename;
            this.maxb = maxbytes;
            this.maxp = maxpackets;
            if (dotnetWinpCap.pcap_live_dump(this.pcap_t, this.fname, this.maxb, this.maxp) == 0)
            {
                new Thread(new ThreadStart(this.MonitorDump));
                return true;
            }
            return false;
        }





        private void ReadNextLoop()
        {
            while (true)
            {
                IntPtr ptr1;
                IntPtr ptr2;
                PacketHeader header1 = null;
                byte[] buffer1 = null;
                dotnetWinpCap.PCAP_NEXT_EX_STATE pcap_next_ex_state1 = this.ReadNextInternal(out header1, out buffer1, out ptr1, out ptr2);
                if (pcap_next_ex_state1 == dotnetWinpCap.PCAP_NEXT_EX_STATE.SUCCESS)
                {
                    if (this.OnReceivePacket != null)
                    {
                   
                        this.OnReceivePacket(this, header1, buffer1);
                    }
                    if (this.OnReceivePacketInternal != null)
                    {
                        this.OnReceivePacketInternal(this, ptr1, ptr2);
                    }
                }
            }
        }



        public bool SetKernelBuffer(int bytes)
        {
            if (dotnetWinpCap.pcap_setbuff(this.pcap_t, bytes) != 0)
            {
                return false;
            }
            return true;
        }



        public void StartListen()
        {
            if (this.ListenThread != null)
            {
                this.ListenThread.Abort();
            }
            this.ListenThread = new Thread(new ThreadStart(this.ReadNextLoop));
            this.ListenThread.Start();
            this.m_islistening = true;
        }



        public void StopListen()
        {
            if (this.ListenThread != null)
            {
                if (this.ListenThread.IsAlive)
                {
                    this.ListenThread.Abort();
                }
                this.ListenThread = null;
            }
            this.m_islistening = false;
        }

        public bool IsListening
        {
            get
            {
                return this.m_islistening;
            }
        }
       public bool IsOpen
{
      get
      {
            return this.m_isopen;
      }
}
 
        public string AttachedDevice
        {
            get
            {
                return this.m_attachedDevice;
            }
        }


        public bool SetMinToCopy(int size)
        {
            if (dotnetWinpCap.pcap_setmintocopy(this.pcap_t, size) == 0)
            {
                return true;
            }
            return false;
        }

 

        public string LastError
        {
            get
            {
                return this.errbuf.ToString();
            }
        }


        public void Close()
        {
            this.StopDump();
            if (this.IsListening)
            {
                this.StopListen();
            }
            this.m_isopen = false;
            this.m_attachedDevice = null;
            if (this.pcap_t != IntPtr.Zero)
            {
                dotnetWinpCap.pcap_close(this.pcap_t);
                this.pcap_t = IntPtr.Zero;
            }
        }



        private void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing && (this.ListenThread != null))
                {
                    if (this.ListenThread.IsAlive)
                    {
                        this.ListenThread.Abort();
                    }
                    this.ListenThread = null;
                }
                if (this.pcap_t != IntPtr.Zero)
                {
                    dotnetWinpCap.pcap_close(this.pcap_t);
                    this.pcap_t = IntPtr.Zero;
                }
            }
            this.disposed = true;
        }


     
    }

    public class PacketHeader
    {
        public dotnetWinpCap.timeval ts;
        private int caplen;
        private int len;

        public PacketHeader(dotnetWinpCap.timeval ts, int caplen, int len)
        {
            caplen = 0;
            len = 0;
            ts = ts;
            caplen = caplen;
            len = len;
            return;
        }

        public PacketHeader()
        {
            caplen = 0;
            len = 0;
            return;
        }
        public virtual int Caplength
        {
            get
            {
                return caplen;
            }
            set
            {
                caplen = value;
                return;
            }
        }
        public virtual int Length
        {
            get
            {
                return len;
            }
            set
            {
                len = value;
                return;
            }
        }
        public virtual DateTime TimeStamp
        {
            get
            {
                DateTime dateTime;
                DateTime dateTime1;
                dateTime1 = new DateTime(1970, 1, 1);
                dateTime = dateTime1.AddSeconds(((System.Double)(ts.tv_sec)));
                dateTime.AddMilliseconds(((System.Double)(ts.tv_usec)));
                return dateTime;
            }
        }
    }
}