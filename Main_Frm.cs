using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO.Compression;
using CnCerT.Net.Winpcap;
using Newtonsoft.Json;
using System.IO;

namespace HttpWatch
{
    public partial class Main_Frm : Form
    {
        public Main_Frm()
        {
            InitializeComponent();
            InitUiParms();
            this.Text += " [" + Application.ProductVersion + "]";
        }
        int rid;
        long pktcount;
        private delegate void RecApacket(httpsession osession);
        private delegate void updatemsg(string msg);
        private updatemsg myupdatemsg;
        private RecApacket Myrecvie;

        private Socket mainSocket;                          //The socket which captures all incoming packets

        private bool bContinueCapturing = false;            //A flag to check if packets are to be captured or not
        const int SIO_RCVALL = unchecked((int)0x98000001);
        private delegate void updae_state_del(int id, int code, double times);
        private updae_state_del myupdatestatehandle;

        System.IO.StreamWriter debugsw = System.IO.File.AppendText(System.DateTime.Now.ToString("yyyyMMdd") + "_debug_.txt");
        private dotnetWinpCap.ReceivePacket rcvPackDownflux = null;
        private dotnetWinpCap mypcap = new dotnetWinpCap();
        private int maxqps = 1;
        private int qpscount = 0;
        private long totalcount = 0;
        private delegate void updatechart_del(string s);
        private updatechart_del myupchart;



        private Dictionary<string, List<byte>> dic_senddata = new Dictionary<string, List<byte>>();
        private Dictionary<string, httpsession> dic_ack_httpsession = new Dictionary<string, httpsession>();


        Standard.HexViewer hexview_request = new Standard.HexViewer();

        Standard.ImageViewer imgview = new Standard.ImageViewer();
        //Standard.RequestHeaderView headview_request = new Standard.RequestHeaderView();
        Standard.TextViewer textview_responce = new Standard.TextViewer();
        // Standard.ResponseHeaderView headview_responce = new Standard.ResponseHeaderView();
        Standard.TextViewer textview_requst = new Standard.TextViewer();
        Standard.HexViewer hexview_responce = new Standard.HexViewer();

        #region 更新UI
        private void update_statecode_instance(int id, int code, double times)
        {
            try
            {
                if (dataGridView1.Rows.Count > id && dataGridView1.Rows[id].Cells[0].Value.ToString() == id.ToString())
                {
                    dataGridView1.Rows[id].Cells[5].Value = code;
                    dataGridView1.Rows[id].Cells[6].Value = (int)times;
                }
            }
            catch (Exception e)
            {
            }
        }
        private void updatemsginstance(string msg)
        {
            this.textBox_log.AppendText(DateTime.Now.ToString() + "\t" + msg + "\r\n");
            Application.DoEvents();
        }

        private void debugmsg(string msg)
        {
            if (Config.isdebug)
            {
                this.textBox_log.BeginInvoke(myupdatemsg, new object[] { msg });
                debugsw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + msg);
                debugsw.Flush();
            }

        }
        private void UpdateRecPacket(httpsession osession)
        {


            if (rid > 40)
            {
                dataGridView1.VirtualMode = true;
                dataGridView1.RowCount = dic_ack_httpsession.Count;
            }
            else
            {
                dataGridView1.VirtualMode = false;
                //  log(method + "\t" + url); ;
                //debugmsg(string.Format("[{0}]  创建 {1}", seq, url));

                dataGridView1.Rows.Add(osession.id, osession.ack, osession.method, osession.url, Encoding.ASCII.GetString(osession.sendraw.ToArray()), osession.statucode);
            }


        }

        private void updatechat_ins(string s)
        {
            dataChart1.UpdateChart(qpscount);
            if (maxqps < qpscount) maxqps = qpscount;
            totalcount = totalcount + qpscount;
            this.toolStripStatusLabel1.Text = string.Format("QPS:{0} MAX:{2} TotalQuery:{1} TotalPacket:{3}", qpscount, totalcount, maxqps, pktcount);
            qpscount = 0;

        }
        private void updatechart()
        {
            while (bContinueCapturing || pktcache.Count > 0)
            {
                try
                {
                    this.textBox_log.Invoke(myupchart, new object[] { " " });
                    Thread.Sleep(1000);
                }
                catch
                {

                }
               
            }
        }
        ArrayList devlist;
        private void Getsetting()//加载网卡设置
        {
            try
            {
                devlist = dotnetWinpCap.FindAllDevs();
                for (int i = 0; i <= devlist.Count - 1; i++)
                {
                    comboBox1.Items.Add(((Device)devlist[i]).Description);
                }
                if (comboBox1.Items.Count > 0)
                    this.comboBox1.Text = this.comboBox1.Items[0].ToString();
                else
                {
                    MessageBox.Show("不能检测到有效的网卡，请确认您使用了以太网卡而非无线网卡", "错误");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("不能获取网卡，请检查是否安装winpcap且已管理员身份运行！", "错误");
            }

        }

        #endregion
        private void InitUiParms()
        {

            hexview_request.AddToTab(this.tabPage7);

            imgview.AddToTab(tabPage9);//iii

            textview_requst.AddToTab(this.tabPage_text_request);
            textview_responce.AddToTab(this.tabPage_rec_text);
            //headview_request.AddToTab(this.tabPage_headview_requst);
            //textview_responce.AddToTab(this.tabPage_headview_responce);

            hexview_responce.AddToTab(this.tabPage5);



            Config.isusepcap = System.IO.File.Exists(".pcap");


            if (Config.isusepcap)
            {
                this.toolStripLabel1.Text = "Device";
                toolStripStatusLabel2.Text = "Mode:Pcap";
                Getsetting();
            }
            else
            {
                toolStripStatusLabel2.Text = "Mode:RawSock";
                toolStripLabel1.Text = "IP";
                string strIP = null;
                IPHostEntry HosyEntry = Dns.GetHostEntry((Dns.GetHostName()));
                if (HosyEntry.AddressList.Length > 0)
                {
                    foreach (IPAddress ip in HosyEntry.AddressList)
                    {
                        if (ip.IsIPv6LinkLocal || ip.IsIPv6Multicast || ip.IsIPv6SiteLocal || ip.IsIPv6SiteLocal)
                            continue;
                        strIP = ip.ToString();
                        comboBox1.Items.Add(strIP);
                    }
                }
            }

            dataChart1.UpdateChart(0);

            Myrecvie = new RecApacket(UpdateRecPacket);
            myupdatemsg = new updatemsg(updatemsginstance);
            myupdatestatehandle = new updae_state_del(update_statecode_instance);
            myupchart = new updatechart_del(updatechat_ins);

        }

        Device usedev = null;

        private void MonitorData_Pcap()
        {
            if (mypcap.IsOpen)
            {
                mypcap.Close();
            }

            if (!mypcap.Open(usedev.Name, 65536, 1, 0))
            {

                MessageBox.Show("Error at:" + ((Device)devlist[this.comboBox1.SelectedIndex]).Description + mypcap.LastError, "Message");
            }
            mypcap.SetMinToCopy(100);

            if (rcvPackDownflux == null)
            {
                rcvPackDownflux = new dotnetWinpCap.ReceivePacket(this.ReceivePacket_pcap);//这里是一个回调，PCAP每收到一个数据包都就产生一个事件，避免了do loop的方式
                mypcap.OnReceivePacket += rcvPackDownflux;
            }

            mypcap.StartListen();
        }
        private void ReceivePacket_pcap(object sender, PacketHeader p, byte[] s)
        {


            if (s[23] != 0x06) return;
            if (s[12] != 0x08 && s[13] != 0x00) return;

            if (s.Length <= 60) return;
            int offset = 34;
            int srcport = CnCerT.Net.Packet.PkFunction.Get2Bytes(s, ref offset, 0);
            int desport = CnCerT.Net.Packet.PkFunction.Get2Bytes(s, ref offset, 0);

            bool isok = false;
            if (iswhiteport(desport))
            {
                isok = true;
                if (s[47] == 0x18) qpscount++;
            }
            else if (iswhiteport(srcport))
            {

                isok = true;
            }
            if (!isok) return;



            byte[] t = new byte[s.Length - 14];
            Array.Copy(s, 14, t, 0, t.Length);
            pktcache.Enqueue(t);
        }



        private void ParcePkt_Cache(byte[] byteData, int nReceived)
        {


            IPHeader ipHeader = new IPHeader(byteData, nReceived);


            switch (ipHeader.ProtocolType)
            {
                case Protocol.TCP:
                    TCPHeader tcpHeader = new TCPHeader(ipHeader.Data, ipHeader.MessageLength);//Length of the data field          
                    int headlen = ipHeader.HeaderLength + tcpHeader.HeaderLength;

                    if (iswhiteport(tcpHeader.DestinationPort))
                    {
                        pktcount++;
                        if (headlen >= nReceived) return;
                        if (tcpHeader.Flags == 0x18)
                            BuildPacket(true, tcpHeader.AcknowledgementNumber, tcpHeader.SequenceNumber, byteData, headlen, nReceived);
                        else if (tcpHeader.Flags == 0x10)
                        {
                            byte[] dataArray = new byte[nReceived - headlen];
                            Array.Copy(byteData, headlen, dataArray, 0, dataArray.Length);

                            if (dic_senddata.ContainsKey(tcpHeader.AcknowledgementNumber))
                            {
                                log("包含 tcp：" + tcpHeader.AcknowledgementNumber);
                                dic_senddata[tcpHeader.AcknowledgementNumber].AddRange(dataArray);

                            }
                            else
                            {
                                List<byte> listtmp = new List<byte>();
                                listtmp.AddRange(dataArray);
                                log("tcp：" + tcpHeader.AcknowledgementNumber);
                                dic_senddata.Add(tcpHeader.AcknowledgementNumber, listtmp);

                            }
                        }
                    }
                    else if (iswhiteport(tcpHeader.SourcePort))
                    {
                        pktcount++;
                        if (headlen >= nReceived) return;
                        BuildPacket(false, tcpHeader.AcknowledgementNumber, tcpHeader.SequenceNumber, byteData, headlen, nReceived);

                    }
                    break;




            }


        }





        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int nReceived = mainSocket.EndReceive(ar);

                byte[] byteData = ar.AsyncState as byte[];
                //Analyze the bytes received...

                ParcePkt_Cache(byteData, nReceived);
                byteData = new byte[40960];
                if (bContinueCapturing)
                {
                    //    Thread.Sleep(1);
                    byteData = new byte[40960];

                    //Another call to BeginReceive so that we continue to receive the incoming
                    //packets
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), byteData);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "sniffer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool iswhiteport(int port)
        {

            for (int i = 0; i < ports.Length; i++)
            {
                if (port == ports[i])
                    return true;
            }
            return false;
        }

        private void ParserCacheThread()
        {
            while (bContinueCapturing)
            {
                while (pktcache.Count > 0)
                {

                    byte[] t = pktcache.Dequeue();
                    ParcePkt_Cache(t, t.Length);
                    //Application.DoEvents();
                    //   Thread.Sleep(10);

                }

                System.Threading.Thread.Sleep(1000);

            }
        }
        private Queue<byte[]> pktcache = new Queue<byte[]>();

        byte[] byteData = new byte[40960];
        private void MonitorData_raw()//监控线程
        {
            while (bContinueCapturing)
            {
                // byteData.Initialize();
                byte[] buf = new byte[40960];
                int size = mainSocket.Receive(buf, 0, buf.Length, SocketFlags.None);


                //   if (size <= 40) continue;
                if (buf[9] != 0x06) continue;
                int offset = 20;
                int srcport = CnCerT.Net.Packet.PkFunction.Get2Bytes(buf, ref offset, 0);
                int desport = CnCerT.Net.Packet.PkFunction.Get2Bytes(buf, ref offset, 0);

                bool isok = false;
                if (iswhiteport(desport))
                {

                    isok = true;
                    if (buf[33] == 0x18) qpscount++;
                }
                else if (iswhiteport(srcport))
                {

                    isok = true;
                }
                if (!isok) continue;

                byte[] t = new byte[size];
                Array.Copy(buf, 0, t, 0, t.Length);

                pktcache.Enqueue(t);

            }



        }

        string localip = string.Empty;
        int[] ports;
        //IPAddress ipwhite = null;

        Regex rhost = new Regex(@"\bhost:.(\S*)", RegexOptions.IgnoreCase);
        private string Gethost(string http)
        {

            Match m = rhost.Match(http);
            return m.Groups[1].Value;

        }

        private void log(string msg)
        {

            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.DateTime.Now.ToString("yyyyMMdd_") + "log.txt"))
            {
                sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + msg);
                sw.Flush();
                sw.Close();
            }
        }
        private Dictionary<string, string> dic_ack_seq = new Dictionary<string, string>();

        private List<string> list_ack = new List<string>();

        /// <summary>
        /// 组包
        /// </summary>
        /// <param name="isout"></param>
        /// <param name="ack"></param>
        /// <param name="seq"></param>
        /// <param name="PacketData"></param>
        /// <param name="start"></param>
        /// <param name="reclen"></param>
        /// 

        private void BuildPacket(bool isout, string ack, string seq, byte[] PacketData, int start, int reclen)
        {

            try
            {
                if (reclen <= start) return;
                byte[] dataArray = new byte[reclen - start];
                Array.Copy(PacketData, start, dataArray, 0, dataArray.Length);
                if (isout)//如果是请求包
                {
                    httpsession osesion = new httpsession();
                    osesion.id = rid;
                    osesion.senddtime = DateTime.Now;
                    osesion.ack = ack;

                    log(ack);
                    if (dic_senddata.ContainsKey(ack))
                    {
                        log("包含："+ack);
                        osesion.sendraw = dic_senddata[ack];
                    }
                    else
                    {
                        osesion.sendraw = new List<byte>();
                    }

                    osesion.sendraw.AddRange(dataArray);


                    string http_str = System.Text.Encoding.ASCII.GetString(osesion.sendraw.ToArray());

                    log(http_str);
                    string host = Gethost(http_str);

                    if (filtedomain == "" || host.IndexOf(filtedomain) >= 0)
                    {


                        ////  this.dataGridView1.Rows.Add(vr.rid, vr.seq, vr.method, vr.url,http_str);
                        //else
                        int fltag = http_str.IndexOf("\r\n");
                        if (fltag > 0)
                        {
                            string fline = http_str.Substring(0, fltag);
                            int fblacktag = fline.IndexOf(" ");
                            if (fblacktag > 0)
                            {
                                osesion.method = fline.Substring(0, fline.IndexOf(" "));
                                int urllen = fline.LastIndexOf(" ") - fblacktag - 1;
                                if (urllen > 0)
                                    osesion.url = String.Format("http://{0}{1}", host, fline.Substring(fblacktag + 1, urllen));
                            }
                        }
                        if (!this.dic_ack_httpsession.ContainsKey(osesion.ack))
                        {
                            this.dic_ack_httpsession.Add(osesion.ack, osesion);
                            this.list_ack.Add(ack);
                        }
                        rid++;
                        this.dataGridView1.BeginInvoke(Myrecvie, new object[] { osesion });

                        debugmsg(string.Format("[{0}]  创建 {1}", seq, osesion.url));
                    }

                }
                else//如果是返回数据包
                {

                    if (dic_ack_httpsession.ContainsKey(seq))//如果第一次匹配
                    {
                        //    log(ack + ":" + seq + " 第一次返回匹配，添加映射");
                        debugmsg(string.Format("[{0}]  开始接受 {1}：{2}", seq, ack, seq));
                        httpsession osession = dic_ack_httpsession[seq];
                        if (osession.responseraw == null) osession.responseraw = new List<byte>();
                        osession.responseraw.AddRange(dataArray);
                        osession.responoversetime = DateTime.Now;

                        string headb = System.Text.Encoding.ASCII.GetString(dataArray);
                        int flinetag = headb.IndexOf("\r\n");
                        if (flinetag > 0)
                        {
                            headb = headb.Substring(0, flinetag);
                            string[] p3 = headb.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (p3.Length >= 2)
                            {
                                osession.statucode = int.Parse(p3[1]);

                                this.dataGridView1.BeginInvoke(myupdatestatehandle, new object[] { osession.id, osession.statucode, (osession.responoversetime - osession.senddtime).TotalMilliseconds });
                                log(osession.method + "\t" + osession.url + "\t" + osession.statucode); ;
                            }
                        }
                        dic_ack_httpsession[seq] = osession;
                        if (!dic_ack_seq.ContainsKey(ack))
                        {
                            dic_ack_seq.Add(ack, seq);
                        }
                        //    if (osession.id<=40)

                    }//后面的数据包
                    else
                    {
                        if (dic_ack_seq.ContainsKey(ack))
                        {

                            httpsession osession = dic_ack_httpsession[dic_ack_seq[ack]];

                            osession.responseraw.AddRange(dataArray);
                            dic_ack_httpsession[dic_ack_seq[ack]] = osession;
                            debugmsg(string.Format("[{0}]  继续接受 {1}：{2}  总长度 {3} ", dic_ack_seq[ack], ack, seq, osession.responseraw.Count));
                        }
                        else
                        {
                            debugmsg(string.Format("[未识别]  接受 {1}：{2}", 0, ack, seq));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debugmsg(string.Format("[异常]  {0}", ex.ToString()));
                log(ex.ToString());
            }
        }



        string filtedomain;
        Thread main_raw;
        Thread main_pcap;
        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (comboBox1.SelectedItem == null) return;
                filtedomain = textBox_host.Text;
                rid = 0;
                pktcount = 0;

                if (button_start.Text == "Start")
                {

                    dic_ack_seq.Clear();
                    dic_ack_httpsession.Clear();
                    string[] portstring = textBox_ports.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    ports = new int[portstring.Length];
                    for (int i = 0; i < portstring.Length; i++)
                    {
                        ports[i] = Convert.ToInt32(portstring[i]);
                    }

                    if (!Config.isusepcap)
                    {
                        localip = comboBox1.Text;
                        if (textBox_host.Text.Length > 0)
                            filtedomain = textBox_host.Text;
                        bContinueCapturing = true;
                        mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                        mainSocket.Bind(new IPEndPoint(IPAddress.Parse(localip), 0));
                        mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                        byte[] IN = new byte[4] { 1, 0, 0, 0 };
                        byte[] OUT = new byte[4];
                        mainSocket.IOControl(SIO_RCVALL, IN, OUT);
                        main_raw = new Thread(new ThreadStart(MonitorData_raw));
                        main_raw.Priority = ThreadPriority.Highest;
                        main_raw.Start();
                    }
                    else
                    {
                        usedev = (Device)devlist[this.comboBox1.SelectedIndex];
                        bContinueCapturing = true;
                        main_pcap = new Thread(new ThreadStart(MonitorData_Pcap));
                        main_pcap.Priority = ThreadPriority.Highest;
                        main_pcap.Start();
                    }

                    Thread ucth = new Thread(new ThreadStart(updatechart));
                    ucth.IsBackground = true;
                    ucth.Start();

                    Thread ppcthread = new Thread(new ThreadStart(ParserCacheThread));
                    ppcthread.IsBackground = true;
                    ppcthread.Start();
                    button_start.Text = "Stop";
                }
                else
                {


                    while (this.pktcache.Count > 0)
                        Application.DoEvents();

                    if (mypcap != null) mypcap.Close();
                    bContinueCapturing = false;
                    Thread.Sleep(1100);
                    button_start.Text = "Start";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }



        private void Frm_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bContinueCapturing = false;

        }




        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string url = dataGridView1.SelectedRows[0].Cells["URL"].Value.ToString();
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }




        private bool isimage(string head)
        {
            if (head.ToLower().IndexOf("image") > 0)

                return true;

            return false;
        }
        private string getencompress(string head)
        {
            if (head.ToLower().IndexOf("gzip") > 0)

                return "gzip";
            if (head.ToLower().IndexOf("deflate") > 0)

                return "deflate";

            return "";
        }
        Regex regexencode = new Regex("charset=([\\w|-]+)", RegexOptions.IgnoreCase);
        private string getencfromhead(string head)
        {
            Match mc = regexencode.Match(head);
            if (mc.Success)
            {
                return mc.Groups[1].Value;
            }
            return string.Empty;
        }


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int tt = 0;
            try
            {

                if (dataGridView1.SelectedRows.Count > 0)
                {
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        resetui();

                        string seq = dataGridView1.SelectedRows[0].Cells["seq"].Value.ToString();
                        if (dic_ack_httpsession.ContainsKey(seq))
                        {
                            httpsession osession = dic_ack_httpsession[seq];

                            var requestBody = dataGridView1.SelectedRows[0].Cells["RAW"].Value.ToString();
                            this.textview_requst.SetText(requestBody);


                            if (!string.IsNullOrWhiteSpace(requestBody))
                            {
                                try
                                {
                                    jsonViewerRequest.Json = requestBody;
                                }
                                catch
                                {

                                }
                            }
                            if (osession.responseraw == null)
                            {
                                debugmsg("没有数据");
                                return;
                            }
                            byte[] dataall = osession.responseraw.ToArray();
                            this.hexview_responce.body = dataall;
                            this.hexview_request.body = osession.sendraw.ToArray();

                            int tag = 0;
                            for (int i = 0; i < dataall.Length - 3; i++)
                            {
                                if (dataall[i] == 0x0D && dataall[i + 1] == 0x0A && dataall[i + 2] == 0x0D && dataall[i + 3] == 0x0A)
                                {
                                    tag = i;
                                    break;
                                }
                            }
                            byte[] headbyte = new byte[tag + 4];
                            Array.Copy(dataall, 0, headbyte, 0, tag + 4);
                            byte[] recbyte = new byte[dataall.Length - headbyte.Length];
                            Array.Copy(dataall, tag + 4, recbyte, 0, recbyte.Length);
                            string headstring = Encoding.ASCII.GetString(headbyte).Trim(new char[] { '\0' });

                            if (headstring.IndexOf("chunk") >= 0)
                            {
                                recbyte = Chunk.doUnchunk(recbyte);
                            }

                            //    this.hexview_responce.body = headbyte;

                            System.IO.Stream responseStream = new System.IO.MemoryStream(recbyte);
                            string compress = getencompress(headstring);
                            if (compress == "gzip")
                            {
                                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                            }
                            else if (compress == "deflate")// if (headstring.ToLower().Contains("deflate"))
                            {
                                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                            }


                            if (isimage(headstring)) //图形
                            {
                                this.imgview.body = recbyte;
                            }
                            else
                            {

                                string encstring = getencfromhead(headstring);
                                if (encstring == "")
                                {
                                    string prs = System.Text.Encoding.ASCII.GetString(recbyte);
                                    encstring = getencfromhead(prs);
                                }
                                if (encstring == "")
                                    encstring = "utf-8";
                                Encoding enc = Encoding.GetEncoding(encstring);
                                #region
                               // HttpWatch.Control.ByteCollection bytecollection = new HttpWatch.Control.ByteCollection();


                               // while (true)
                               // {
                               //     byte[] buf = new byte[1024];
                               //     int l = responseStream.Read(buf, 0, buf.Length);
                               //     if (l <= 0) break;
                               //     if (l < buf.Length)
                               //     {
                               //         byte[] n = new byte[l];
                               //         Array.Copy(buf, 0, n, 0, n.Length);
                               //         bytecollection.AddRange(n);

                               //     }
                               //     else
                               //     {
                               //         bytecollection.AddRange(buf);
                               //     }
                               // }
                               // //textview_responce.m_headers = new Fiddler.HTTPResponseHeaders(enc);
                               //// textview_responce..
                               // //textview_responce.m_headers.Add("Content-Type", "text/xml; charset=" + encstring);
                               // textview_responce.body = bytecollection.GetBytes();
                                #endregion



                                System.IO.StreamReader sr = new System.IO.StreamReader(responseStream, enc);

                                string html = sr.ReadToEnd();
                                //string body = html.Substring(html.IndexOf(headstring));
                                if(!string.IsNullOrWhiteSpace(html))
                                {
                                    try
                                    {
                                        if(headstring.ToLower().Contains("application/json"))
                                        {
                                            jsonViewResponse.Json = html;
                                        }
                                        
                                    }
                                    catch
                                    {

                                    }
                                }
                                textview_responce.SetText(headstring + html);
                                this.webBrowser1.DocumentText = html;
                            }

                        }

                        //  }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("[异常]   {0}", ex.ToString()));
                debugmsg(string.Format("[异常]   {0}", ex.ToString()));
            }
            //this.dataGridView1.Enabled = true;
        }
        public Encoding GetResponseEncoding(HttpWebResponse websp)
        {
            Encoding encoding = Encoding.Default;
            if (websp.CharacterSet != null)
            {
                try
                {
                    encoding = Encoding.GetEncoding(websp.CharacterSet);
                }
                catch
                {
                }
            }
            else
            {
                if (websp.Headers["Content-Type"] != null)
                {
                    if (websp.Headers["Content-Type"].IndexOf("charset") >= 0)
                    {

                        string[] a = websp.Headers["Content-Type"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < a.Length; i++)
                        {
                            if (a[i].IndexOf("charset") >= 0)
                            {
                                encoding = Encoding.GetEncoding(a[i].Substring(a[i].IndexOf("charset") + 8));
                                break;
                            }
                        }
                    }
                }
            }
            return encoding;
        }
        private void resetui()
        {

            this.webBrowser1.DocumentText = "";
            hexview_responce.Clear();
            hexview_request.Clear();
            textview_responce.Clear();
            textview_requst.Clear();
            jsonViewResponse.Clear();
            imgview.Clear();

        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            this.pktcount = 0;
            this.pktcache.Clear();
            dic_ack_httpsession.Clear();
            list_ack.Clear();
            dic_ack_seq.Clear();
            rid = 0;
            this.textBox_log.Clear();
            resetui();
        }

        private void Frm_main_Shown(object sender, EventArgs e)
        {

        }

        private void textBox_host_TextChanged(object sender, EventArgs e)
        {
            this.filtedomain = this.textBox_host.Text;
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            //if ((!dic_rid_ack.ContainsKey(e.RowIndex)) || (!dic_ack_httpsession.ContainsKey(dic_rid_ack[e.RowIndex]))) 
            //    return;

            if (list_ack.Count <= e.RowIndex || !dic_ack_httpsession.ContainsKey(list_ack[e.RowIndex]))
                return;
            // vrow vr= _dic_rid_virow[e.RowIndex];
            httpsession osession = dic_ack_httpsession[list_ack[e.RowIndex]];
            switch (e.ColumnIndex)
            {
                case 0:
                    e.Value = osession.id;
                    break;
                case 1:
                    e.Value = osession.ack;
                    break;
                case 2:
                    e.Value = osession.method;
                    break;
                case 3:
                    e.Value = osession.url;
                    break;

                case 4:
                    e.Value = System.Text.Encoding.ASCII.GetString(osession.sendraw.ToArray());
                    break;

                case 5:
                    e.Value = osession.statucode;
                    break;

                case 6:
                    e.Value = (int)((osession.responoversetime - osession.senddtime).TotalMilliseconds);
                    break;
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void chanagemode(string mode)
        {
            if (mode == "pcap")
            {

            }
        }
        private void toolStripButton_mode_Click(object sender, EventArgs e)
        {

            if (System.IO.File.Exists(".pcap"))
            {
                System.IO.File.Delete(".pcap");
                if (MessageBox.Show("已切换为RawSocket模式 请以管理员运行，并将本程序添加到防火墙例外。\r\n重启后生效,是否立即重启进程？", "HttpWach", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Application.Restart();
                }
            }
            else
            {
                System.IO.File.CreateText(".pcap").Close();
                if (MessageBox.Show("已切换为Pcap模式 ,请确定已安装wincap.\r\n 重启后生效, 是否立即重启进程？", "HttpWach", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    Application.Restart();
            }
        }

        private void Frm_main_SizeChanged(object sender, EventArgs e)
        {
            int sieze = this.Width - this.toolStripLabel1.Width - comboBox1.Width - toolstrip_IncludeDomain.Width - toolStripLabel2.Width - textBox_ports.Width - button_start.Width - toolStripButton_mode.Width - 30;
            textBox_host.Size = new Size(sieze, textBox_host.Size.Height);
        }

        private void toolStripLabel1_DoubleClick(object sender, EventArgs e)
        {
            //   Config.isusepcap = !Config.isusepcap;

        }

       
    }
}