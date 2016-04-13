using System;
using System.Collections.Generic;
using System.Text;

namespace HttpWatch
{

        [Serializable()]
        public struct httpsession
        {
            public string url;
            public string method;
            public DateTime senddtime;
            public DateTime responoversetime;
            public int id;
            public string ack;
            public List<byte> sendraw;
            public List<byte> responseraw;
            public int statucode;
        }
        public enum Protocol
        {
            TCP = 6,
            UDP = 17,
            Unknown = -1
        };
}
