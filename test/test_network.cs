using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using CWLEngine.Core.Network;
using System.Linq;



public class test_network : MonoBehaviour
{

    InputField IpInput;
    InputField PortInput;

    void Start()
    {
        Button btnConnect = GameObject.Find("connect").GetComponent<Button>();
        btnConnect.onClick.AddListener(ClickConnect);

        Button btnSend = GameObject.Find("send").GetComponent<Button>();
        btnSend.onClick.AddListener(ClickSend);

        Button btnClose = GameObject.Find("close").GetComponent<Button>();
        btnClose.onClick.AddListener(ClickClose);

        //Button btnServer = GameObject.Find("server").GetComponent<Button>();
        //btnServer.onClick.AddListener(ClickClose);

        IpInput = GameObject.Find("IP").GetComponent<InputField>();
        PortInput = GameObject.Find("Port").GetComponent<InputField>();

        NetworkMgr.Instance.AddEventListener(NetworkMgr.NetEvent.CONNECT_SUCCESS, OnConnectSuccess);
        NetworkMgr.Instance.AddEventListener(NetworkMgr.NetEvent.CONNECT_FAIL, OnConnectFail);
        NetworkMgr.Instance.AddEventListener(NetworkMgr.NetEvent.CLOSE, OnConnectClose);

        NetworkMgr.Instance.AddMsgListener(1001, LoginServerTest);

        JsonTest();
    }
    
    [Serializable]
    public class User
    {
        public string name;
        public int age;
    }

    void JsonTest()
    {
        User user = new User();
        user.name = "sad";
        user.age = 12;
        string ss = "{\"name\":\"sad\",\"age\":\"asd\"}"; /*JsonUtility.ToJson(user);*/
        Debug.Log(ss);
        
        try
        {
            User user2 = JsonUtility.FromJson<User>(ss);
            Debug.Log(user2.age);
            Debug.Log(user2.name);
        }
        catch(ArgumentException ex)
        {
            Debug.Log("json format error. " + ex.ToString());
        }
    }

    void LoginServerTest(Message msg)
    {
        Debug.Log(msg.DebugString());
    }

    void OnConnectSuccess(string err)
    {
        Debug.Log("Success");
    }

    void OnConnectFail(string err)
    {
        Debug.Log("connect fail" + err);
    }

    void OnConnectClose(string err)
    {
        Debug.Log("close");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IpInput.isFocused)
            {
                PortInput.ActivateInputField();
            }
            else if (PortInput.isFocused)
            {
                IpInput.ActivateInputField();
            }
        }
        NetworkMgr.Instance.Update();
    }

    void ClickClose()
    {
        Debug.Log("click close");
        NetworkMgr.Instance.Close();
    }

    void ClickConnect()
    {
        try
        {
            string ip = IpInput.text.ToString();
            int port = Convert.ToInt32(PortInput.text.ToString());
            NetworkMgr.Instance.Connect(ip, port);
        }
        catch(Exception ex)
        {
            Debug.Log("ip or port error. " + ex.ToString());
        }
    }

    void ClickSend()
    {
        try
        {
            Message msg = new Message();
            msg.PackBuffer(1001, "{\"name\":\"cwl\"}");
            NetworkMgr.Instance.Send(msg);
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("" + ex.ToString());
        }
        catch (SocketException ex)
        {
            // 连接掉线，需要重连接
            Debug.Log(ex.ToString());
        }
    }

}
