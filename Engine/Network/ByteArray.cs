using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWLEngine.Core.Network
{
    public class ByteArray
    {
        private const int DEFAULT_SIZE = 1024;

        public byte[] bytes;

        public int readIndex = 0;
        public int writeIndex = 0;

        private int capacity = 0; // 容量

        /**
         [       这部分有数据    ] [        空         ]
         readIndex .....  writeIndex ..........capacity
             
             */

        public int remain { get { return capacity - writeIndex;  } }    // 剩余空间
        public int length { get { return writeIndex - readIndex; } }    // 数据长度

        public byte this[int i]
        {
            get
            {
                return bytes[i];
            }
            set
            {
                bytes[i] = value;
            }
        }

        public ByteArray(int size = DEFAULT_SIZE)
        {
            Assign(size);
        }

        public ByteArray(byte[] bytes)
        {
            this.bytes = bytes;
            capacity = this.bytes.Length;
            readIndex = 0;
            writeIndex = this.bytes.Length;
        }

        public void Assign(int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            readIndex = 0;
            writeIndex = 0;
        }

        public void AddOneByte(byte b)
        {
            if (remain > 0)
            {
                bytes[writeIndex] = b;
                writeIndex += 1;
            }
            else
            {
                Resize();
                AddOneByte(b);
            }
        }

        public void Append(byte[] bytes, int offset, int count)
        {
            for (int i = 0; i < count; i++ )
            {
                AddOneByte(bytes[offset + i]);
            }
        }

        public void Resize()
        {
            capacity *= 2;
            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIndex, newBytes, 0, writeIndex - readIndex);
            bytes = newBytes;
            writeIndex = writeIndex - readIndex;
            readIndex = 0;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append((char) bytes[readIndex + i]);
            }
            return stringBuilder.ToString();
        }
    }
}
