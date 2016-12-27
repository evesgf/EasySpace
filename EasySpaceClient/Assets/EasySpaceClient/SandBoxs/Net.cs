using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using UnityEngine.UI;
using System;

public class Net : MonoBehaviour {

    //与服务端的套接字
    Socket socket;

    //服务端的IP和端口
    public InputField hostInput;
    public InputField portInput;

    //显示客户端收到的消息
    public Text recvText;
    public string recvStr;

    //显示客户端IP和端口
    public Text clientText;

    //聊天输入框
    public InputField textInput;

    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];

    /// <summary>
    /// 因为只有主线程能修改UI，所以在Update中修改文本
    /// </summary>
    private void Update()
    {
        recvText.text = recvStr;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
    }

    public void Connetion()
    {
        //清理text
        recvText.text = "";

        //Socket
        socket = new Socket(AddressFamily.InterNetwork,                      SocketType.Stream, ProtocolType.Tcp);

        //Connect
        string host = hostInput.text;
        int port = int.Parse(portInput.text);
        socket.Connect(host, port);
        clientText.text = "客户端地址 " + socket.LocalEndPoint.ToString();

        //Recv
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
    }

    /// <summary>
    /// 接收回调
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            //count是接收数据的大小
            int count = socket.EndReceive(ar);

            //数据处理
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (recvStr.Length > 300) recvStr = "";
            recvStr += str + "\n";
            //继续接收
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            recvText.text += "连接已断开："+e.Message;
            socket.Close();
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    public void Send()
    {
        string str = textInput.text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch (Exception e)
        {
            recvText.text += "发送失败：" + e.Message;
            //socket.Close();
        }
    } 
}
