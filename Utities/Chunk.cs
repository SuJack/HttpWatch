using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace HttpWatch
{
    public class Chunk
    {
        public static byte[] doUnchunk(byte[] writeData)
        {
            if ((writeData == null) || (writeData.Length == 0))
            {
                return new byte[0];
            }
            MemoryStream stream = new MemoryStream(writeData.Length);
            int index = 0;
            bool flag = false;
            while (!flag && (index <= (writeData.Length - 3)))
            {
                string s = Encoding.ASCII.GetString(writeData, index, Math.Min(0x20, writeData.Length - index));
                int length = s.IndexOf("\r\n", StringComparison.Ordinal);
                if (length <= 0)
                {
                    throw new InvalidDataException("HTTP Error: The chunked entity body is corrupt. Cannot find Chunk-Length in expected location. Offset: " + index.ToString());
                }
                index += length + 2;
                s = s.Substring(0, length);
                length = s.IndexOf(';');
                if (length > 0)
                {
                    s = s.Substring(0, length);
                }
                int count = int.Parse(s, System.Globalization.NumberStyles.HexNumber);
                if (count == 0)
                {
                    flag = true;
                }
                else
                {
                    if (writeData.Length < (count + index))
                    {
                        throw new InvalidDataException("HTTP Error: The chunked entity body is corrupt. The final chunk length is greater than the number of bytes remaining.");
                    }
                    stream.Write(writeData, index, count);
                    index += count + 2;
                }
            }

            byte[] buffer = new byte[stream.Length];
            stream.Position = 0L;
            stream.Read(buffer, 0, (int)stream.Length);
            return buffer;
        }


    }
}
