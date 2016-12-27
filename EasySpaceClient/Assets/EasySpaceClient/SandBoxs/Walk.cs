using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;

public class Walk : MonoBehaviour {

    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];

    //玩家列表
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    public GameObject prefab;
    public string host = "127.0.0.1";
    public int port = 1234;
    public string id;

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    public void AddPalayer(string id,Vector3 pos)
    {
        GameObject player = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
        textMesh.text = id;
        players.Add(id, player);
    }

    /// <summary>
    /// 发送位置协议
    /// </summary>
    public void SendPos()
    {
        GameObject player = players[id];
        Vector3 pos = player.transform.position;

        //组装协议
        string str = "POS ";
        str += id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";
        str += pos.z.ToString() + " ";

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        socket.Send(bytes);

        print("发送" + str);
    }

    /// <summary>
    /// 发送离开协议
    /// </summary>
    public void SendLeave()
    {
        //组装协议
        string str = "LEAVE ";
        str += id + " ";

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        socket.Send(bytes);

        print("发送" + str);
    }

    /// <summary>
    /// 移动
    /// </summary>
    public void Move()
    {
        if (id == "") return;
        GameObject player = players[id];

        //上
        if (Input.GetKey(KeyCode.UpArrow))
        {
            player.transform.position += new Vector3(0, 0, 1);
            SendPos();
        }
        //下
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            player.transform.position += new Vector3(0, 0, -1);
            SendPos();
        }
        //左
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            player.transform.position += new Vector3(-1, 0, 0); SendPos();
        }
        //右
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            player.transform.position += new Vector3(1, 0, 0);
            SendPos();
        }
    }

    private void OnDestroy()
    {
        SendLeave();
    }
}
