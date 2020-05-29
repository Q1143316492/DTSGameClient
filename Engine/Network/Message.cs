using CWLEngine.Core.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.Core.Network
{
    public enum MessageState
    {
        PKG_RECV_HEAD,
        PKG_RECV_HANDLER,
        PKG_RECV_BODY,
        PKG_FINISH
    }

    public class Message
    {
        private readonly int headerLength = 4;
        private readonly int handlerLength = 4;

        private int header;
        private int handler;
        private ByteArray byteArray;
        private MessageState messageState;

        public int Handler
        {
            get { return handler; }
        }

        public Message()
        {
            Assign();
        }

        public void Assign()
        {
            header = 0;
            handler = 0;
            byteArray = new ByteArray();
            messageState = MessageState.PKG_RECV_HEAD;
        }

        private static bool IsBigEndian()
        {
            int val = 0x0102;
            byte[] bytes = System.BitConverter.GetBytes(val);
            return bytes[0] != 2;
        }

        private void BytesCopy(byte[] source, int sourceStart, byte[] dest, int destStart, int length)
        {
            for (int i = 0; i < length; i++)
            {
                dest[destStart + i] = source[sourceStart + i];
            }
        }

        private byte[] PackageBuffer()
        {
            byte[] package = new byte[8 + header];
            byte[] headerBytes = BitConverter.GetBytes(header);
            byte[] handlerBytes = BitConverter.GetBytes(handler);

            if (!IsBigEndian())
            {
                Array.Reverse(headerBytes);
                Array.Reverse(handlerBytes);
            }

            BytesCopy(headerBytes, 0, package, 0, 4);
            BytesCopy(handlerBytes, 0, package, 4, 4);

            for (int i = 0; i < header; i++)
            {
                package[i + 8] = byteArray[i];
            }
            
            return package;
        }

        private int Ehash(int i)
        {
            return i;
        }

        public void Encryption()
        {
            for (int i = 0; i < header; i++)
            {
                int val = byteArray[i];
                val += Ehash(i);
                val %= 128;
                byteArray[i] = Convert.ToByte(val);
            }
        }

        public void UnEncryption()
        {
            for (int i = 0; i < header; i++)
            {
                int val = byteArray[i];
                val -= Ehash(i);
                val = (val % 128 + 128) % 128;
                byteArray[i] = Convert.ToByte(val);
            }
        }

        // 构造一个数据包
        public byte[] PackBuffer(int handler, string message)
        {
            header = message.Length;
            this.handler = handler;
            byteArray = new ByteArray();

            // string 对于每个字符(unicode char)，忽略高8位转 byte[]
            bool asciiFlag = true;

            for (int i = 0; i < message.Length; i++)
            {
                byte[] bytes = BitConverter.GetBytes(message[i]);
                byteArray.AddOneByte(bytes[0]);

                if (bytes[1] != 0)
                {
                    asciiFlag = false;
                    break;
                }
            }

            // 失败，说明string中有 ascii无法表示的字符，当做非法包
            if (asciiFlag == false)
            {
                return null;
            }
            messageState = MessageState.PKG_FINISH;
            return PackageBuffer();
        }

        public byte[] GetBytes()
        {
            if (messageState != MessageState.PKG_FINISH)
            {
                return null;
            }
            return PackageBuffer();
        }

        public bool Finish()
        {
            return messageState == MessageState.PKG_FINISH;
        }

        public string GetMessageBuffer()
        {
            return byteArray.ToString();
        }

        public string DebugString()
        {
            return string.Format("[{0}][{1}][{2}]", header, handler, byteArray.ToString());
        }

        private int RecvInt32(byte[] bytes, int offset)
        {
            byte[] tmp = new byte[4];
            Array.Copy(bytes, offset, tmp, 0, 4);
            Array.Reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }

        private int RecvBytes(byte[] bytes, int startIndex, int endIndex, int needSize)
        {
            int readSize = Math.Min(needSize, endIndex - startIndex);
            byteArray.Append(bytes, startIndex, readSize);
            return readSize;
        }

        public int Recv(byte[] bytes, int startIndex, int endIndex)
        {
            int readSize = 0;

            if (messageState == MessageState.PKG_RECV_HEAD)
            {
                readSize = RecvBytes(bytes, startIndex, endIndex, headerLength - byteArray.length);
                if (byteArray.length == headerLength)
                {
                    messageState = MessageState.PKG_RECV_HANDLER;
                    header = RecvInt32(byteArray.bytes, 0);
                    if (header <= 0 || header > EngineMacro.MAX_MESSAGE_LENGTH)
                    {
                        return -1;
                    }
                    byteArray.Assign();
                }
            }
            else if(messageState == MessageState.PKG_RECV_HANDLER)
            {
                readSize = RecvBytes(bytes, startIndex, endIndex, handlerLength - byteArray.length);
                if (byteArray.length == handlerLength)
                {
                    messageState = MessageState.PKG_RECV_BODY;
                    handler = RecvInt32(byteArray.bytes, 0);
                    byteArray.Assign();
                }
            }
            else if(messageState == MessageState.PKG_RECV_BODY)
            {
                readSize = RecvBytes(bytes, startIndex, endIndex, header - byteArray.length);
                if (byteArray.length == header)
                {
                    messageState = MessageState.PKG_FINISH;
                }
            }
            return readSize;
        }
    }
}
