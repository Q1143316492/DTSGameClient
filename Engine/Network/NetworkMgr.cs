using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CWLEngine.Core.Network
{
    public class NetworkMgr : Singleton<NetworkMgr>
    {
        public enum NetEvent
        {
            CONNECT_SUCCESS,
            CONNECT_FAIL,
            CLOSE,
        }

        public NetworkMgr()
        {
            MonoMgr.Instance.AddUpdateEvent(Update);
        }

        // 检测网络状态，重连
        public void CheckNetwork()
        {
            string ip = string.Empty;
            int port = -1;
            try
            {
                ip = MemeryCacheMgr.Instance.Get("ip") as string;
                port = (int)MemeryCacheMgr.Instance.Get("port");
            }
            catch (Exception ex)
            {
                Debug.Log("ip or port not set. " + ex.ToString());
                ip = NetworkMacro.LOCAL_IP;
                port = NetworkMacro.LOCAL_PORT;
            }

            Connect(ip, port);
        }

        private Socket socket = null;

        private bool isConnecting = false;      // 是否正在连接，标记BeginConnect到回调函数还未返回的阶段
        private bool isClosing = false;         // 是否正在关闭, 保证发送队列都发完才关

        private Queue<ByteArray> writeQueue;

        public delegate void EventListener(string err);
        public delegate void MsgListener(Message msg);

        private Queue<UnityAction> connectCallbackList = new Queue<UnityAction>();

        private Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
        private Dictionary<int, MsgListener> msgListeners = new Dictionary<int, MsgListener>();
        
        private ByteArray readBuff = null;
        private Message readMessage = null;
        private List<Message> msgList = new List<Message>();
        private int msgCount = 0;
        private readonly int MAX_MESSAGE_FIRE = 100;

        public void AddEventListener(NetEvent netEvent, EventListener eventListener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += eventListener;
            }
            else
            {
                eventListeners[netEvent] = eventListener;
            }
        }

        public void RemoveEventListener(NetEvent netEvent, EventListener eventListener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= eventListener;
                if (eventListeners[netEvent] == null)
                {
                    eventListeners.Remove(netEvent);
                }
            }
        }

        public void AddMsgListener(int msgHandler, MsgListener msgListener)
        {
            if (msgListeners.ContainsKey(msgHandler))
            {
                msgListeners[msgHandler] += msgListener;
            }
            else
            {
                msgListeners[msgHandler] = msgListener;
            }
        }

        public void RemoveMsgListener(int msgHandler, MsgListener msgListener)
        {
            if (msgListeners.ContainsKey(msgHandler))
            {
                msgListeners[msgHandler] -= msgListener;
                if (msgListeners[msgHandler] == null)
                {
                    msgListeners.Remove(msgHandler);
                }
            }
        }

        private void FireEvent(NetEvent netEvent, string err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        private void FireMessage(int msgHandler, Message msg)
        {
            if (msgListeners.ContainsKey(msgHandler))
            {
                msg.UnEncryption();
                msgListeners[msgHandler](msg);
            }
        }

        public string GetDesc()
        {
            if (socket == null) return "";
            if (!socket.Connected) return "";
            return socket.LocalEndPoint.ToString();
        }
        
        private void InitState()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            isConnecting = false;
            isClosing = false;

            readBuff = new ByteArray();
            readMessage = new Message();

            writeQueue = new Queue<ByteArray>();
            msgList = new List<Message>();

            msgCount = 0;
        }

        public void Connect(string ip, int port, UnityAction callback = null)
        {
            if (socket != null && socket.Connected)
            {
                callback?.Invoke();
                Debug.Log("Connect fail, already connected");
                return;
            }

            if (isConnecting)
            {
                if (callback != null)
                {
                    lock(connectCallbackList)
                    {
                        connectCallbackList.Enqueue(callback);
                    }
                }
                Debug.Log("Connect fail, isConnecting");
                return;
            }
            InitState();

            socket.NoDelay = true;
            isConnecting = true;
           
            MonoMgr.Instance.StartDelayEvent(2000, () =>
            {
                if (socket == null || !socket.Connected)
                {
                    UIType<string> warnText = new UIType<string>(UICacheKeys.MESSAGE_STRING, "网络连接失败");
                    UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
                }

            });

            if (callback != null)
            {
                lock (connectCallbackList)
                {
                    connectCallbackList.Enqueue(callback);
                }
            }

            socket.BeginConnect(ip, port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket) ar.AsyncState;
                socket.EndConnect(ar);
                Debug.Log("Socket Connect Succ");
                FireEvent(NetEvent.CONNECT_SUCCESS, "");
                isConnecting = false;

                socket.BeginReceive(readBuff.bytes, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);

                if (connectCallbackList.Count > 0)
                {
                    lock (connectCallbackList)
                    {
                        if (connectCallbackList.Count > 0)
                        {
                            UnityAction callback = connectCallbackList.Dequeue();
                            callback();
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket connect fail" + ex.ToString());
                FireEvent(NetEvent.CONNECT_FAIL, ex.ToString());
                isConnecting = false;
            }
        }

        public void Close()
        {
            if (socket == null || !socket.Connected) {
                return ;
            }
            if (isConnecting) {
                return ;
            }

            if (writeQueue.Count > 0)
            {
                isClosing = true;
            }
            else
            {
                socket.Close();
                FireEvent(NetEvent.CLOSE, "");
            }
        }

        public void Send(Message msg)
        {
            if (socket == null || !socket.Connected) return;
            if (isConnecting) return;
            if (isClosing) return;

            //Debug.Log("begin:" + msg.DebugString());
            msg.Encryption();
            //Debug.Log("end:" + msg.DebugString());
            byte[] sendBytes = msg.GetBytes();
            ByteArray byteArray = new ByteArray(sendBytes);

            int writeQueueLength = 0;

            lock (writeQueue)
            {
                writeQueue.Enqueue(byteArray);
                writeQueueLength = writeQueue.Count;
            }
            if (writeQueueLength == 1)
            {
                socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket) ar.AsyncState;
                
                if (socket == null || ! socket.Connected)
                {
                    return;
                }

                int count = socket.EndSend(ar);
                ByteArray byteArray = null;

                lock(writeQueue)
                {
                    // 有数据才会 SendCallback, 所以这里一定有数据
                    byteArray = writeQueue.First();
                }

                byteArray.readIndex += count;
                if (byteArray.length == 0)
                {
                    lock(writeQueue)
                    {
                        writeQueue.Dequeue();
                        if (writeQueue.Count > 0)
                        {
                            byteArray = writeQueue.First();
                        }
                    }
                }
                else
                {
                    byteArray = null;
                }

                if (byteArray != null)
                {
                    socket.BeginSend(byteArray.bytes, byteArray.readIndex, byteArray.length, 0, SendCallback, socket);
                }
                else if (isClosing)
                {
                    socket.Close();
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket send fail" + ex.ToString());
            }
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket) ar.AsyncState;
                int count = socket.EndReceive(ar);

                if (count == 0)
                {
                    Close();
                    return;
                }

                readBuff.writeIndex += count;

                // 尽可能的组装消息
                while (readBuff.length != 0)
                {
                    int packLength = readMessage.Recv(readBuff.bytes, readBuff.readIndex, readBuff.writeIndex);
                    readBuff.readIndex += packLength;
                    if (readMessage.Finish())
                    {
                        lock(msgList)
                        {
                            // TODO
                            //readMessage.UnEncryption();
                            msgList.Add(readMessage);
                            msgCount = msgList.Count;
                        }
                        readMessage = new Message();
                    }
                }
                readBuff = new ByteArray();
                socket.BeginReceive(readBuff.bytes, readBuff.writeIndex, readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Receive fail" + ex.ToString());
            }
        }

        /**
         这个函数会一直 监听 客户端异步读的消息 是否有
             */
        public void MsgUpdate()
        {
            if (msgCount == 0) return ;

            for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
            {
                Message msg = null;
                if (msgList.Count > 0)
                {
                    lock (msgList)
                    {
                        msg = msgList[0];
                        msgList.RemoveAt(0);
                        msgCount = msgList.Count;
                    }
                }
                if (msg != null)
                {
                    //Debug.Log("NetworkMgr get msg:" + msg.DebugString());
                    FireMessage(msg.Handler, msg);
                }
                else
                {
                    break;
                }
            }
        }

        public void Update()
        {
            MsgUpdate();
        }

        public static T ParseCallback<T>(Message message)
        {
            try
            {
                T response = default;

                response = JsonTools.UnSerializeFromString<T>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("ParseCallback parse error. " + ex.ToString());
                return default;
            }
        }
    }
}
