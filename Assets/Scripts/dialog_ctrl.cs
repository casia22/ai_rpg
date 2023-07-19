using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class dialog_ctrl : MonoBehaviour
{
    public string name = "";
    public string description = "";
    public string location = "地球";
    [DisplayOnly]
    public string state = "chatting";
    public Text tex=null;
    public List<string> heard;
    public string say="";
    public string send;
    //
    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    int port;
    UdpClient client;
    IPEndPoint pythonEndPoint;
    //
    public List<GameObject> neibors;
    public bool haveSent;
    public int pre_head_lenth;
    public string receive;
    bool long_term_heardSomeOthers;
    public int waitTurns;


    void UDPset()
    {
        ipAddress = IPAddress.Parse("127.0.0.1");
        port = 8648;
        // 创建UDP客户端套接字
        client = new UdpClient();
        client.Client.ReceiveTimeout = 5000;//可能可以防止一直等待回复导致unity无响应
        // 连接到Python端的IP地址和端口号
        pythonEndPoint = new IPEndPoint(ipAddress, port);
    }
    string UDPturn(string sendData)
    {
        string output = "UDPPython_start";

        // 向Python端发送数据
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);
        client.Send(sendBytes, sendBytes.Length, pythonEndPoint);

        // 接收Python端发送的数据
        IPEndPoint remoteEP = null;
        byte[] receiveBytes = client.Receive(ref remoteEP);
        //byte[] receiveBytes = client.Receive(ref pythonEndPoint);
        output = Encoding.UTF8.GetString(receiveBytes);

        return output;
    }
    string UDPreceive()
    {
        string output = "UDPPython_start";
        // 接收Python端发送的数据
        IPEndPoint remoteEP = null;
        byte[] receiveBytes = client.Receive(ref remoteEP);
        //byte[] receiveBytes = client.Receive(ref pythonEndPoint);
        output = Encoding.UTF8.GetString(receiveBytes);

        return output;
    }

    // Use this for initialization
    void Start()
    {
        UDPset();
        pre_head_lenth = heard.Count;
        haveSent = false;
        long_term_heardSomeOthers = false;
        waitTurns = 0;
        Task.Delay(2000).Wait();
        string a = UDPturn($"{name};start;{description}");
        UnityEngine.Debug.Log(a);
    }
    // Update is called once per frame
    void Update()
    {
        ResetNeiors();
        Heard();
        if (state == "chatting")
        {
            tex.enabled = false;
            if ((neibors.Count>0 && !haveSent) || (haveSent && receive == "无任务"))
            {
                /*List<string> last;
                if (heard.Count >= 5) last = heard.GetRange(heard.Count - 5, 5);
                else last = heard;*/
                receive = UDPturn($"{name};请接收;{string.Join("|", heard.GetRange(pre_head_lenth, heard.Count-pre_head_lenth))}");// 从索引  开始，获取  个元素)}
                if (receive == "已启动") pre_head_lenth= heard.Count;
                UnityEngine.Debug.Log("请接收");
                UnityEngine.Debug.Log(receive);
                haveSent = true;
            }
            if (haveSent)
            {
                receive = UDPturn($"{name};在等待;");
                //UnityEngine.Debug.Log(receive);
                if (receive != "" && receive != "已启动" && receive != "未完成" && receive != "无任务" && receive != "?")
                {
                    haveSent = false;
                    UnityEngine.Debug.Log(receive);
                    if (long_term_heardSomeOthers)
                    {
                        long_term_heardSomeOthers = false;
                        patience *= tau;//环境变化太多次同样改变耐心值
                    }
                    else //从发送指令到现在没有遇到其他变化
                    {
                        say = receive;
                        patience = 1f;//说话的停顿耐心度还原
                        if (tex)
                        {
                            tex.text = say;
                            tex.enabled = true;
                        }
                        long_term_heardSomeOthers = false;//重新计时
                        waitSomeTime(3f, 5f);
                    }
                }
            }
        }
        else
        {
        }
        //temp:
        if (neibors.Count == 0) heard.Clear();
    }
    private void OnDestroy()
    {
        UnityEngine.Debug.Log("OnDestroy");
        client.Close();
    }
    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("OnApplicationQuit");
        client.Close();
    }
    float tau=0.8f;public float patience = 1f;
    private void waitSomeTime(float minTime,float maxTime)
    {
        state = "stay";
        CancelInvoke();
        System.Random random = new System.Random();
        float randomNumber = (float)(random.NextDouble() * (maxTime - minTime) + minTime);
        Invoke("restoreChatting", randomNumber * patience);
        if (patience > 0.1f) patience *= tau;
    }
    private void restoreChatting()
    {
        if (long_term_heardSomeOthers || waitTurns>=3)//等待过程(显示过程)中只有状态改变了再去说
        {
            state = "chatting";
            long_term_heardSomeOthers = false;
            waitTurns = 0;
        }
        else if(waitTurns<3)//否则再等
        {
            waitTurns++;
            waitSomeTime(1f, 3f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        dialog_ctrl other_dialog = other.gameObject.GetComponent<dialog_ctrl>();

        if (other_dialog && !neibors.Contains(other.gameObject))
        {
            neibors.Add(other.gameObject);
            UnityEngine.Debug.Log($"增加一个邻居：{other.gameObject}");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        dialog_ctrl other_dialog = other.gameObject.GetComponent<dialog_ctrl>();

        if (other_dialog && neibors.Contains(other.gameObject))
        {
            neibors.Remove(other.gameObject);
            UnityEngine.Debug.Log($"减少一个邻居：{other.gameObject}");
        }
    }

    void ResetNeiors()
    {
        foreach(GameObject obj in neibors)
        {
            if (Cac2DDist2(transform, obj.transform) > 4)
            {
                neibors.Remove(obj);
            }
        }
    }
    void Heard()
    {
        if (say != "" && !heard.Contains($"我/在{location}说/" + say))
        {
            heard.Add($"我/在{location}说/" + say);
        }
        foreach (GameObject obj in neibors)
        {
            dialog_ctrl other_dialog = obj.GetComponent<dialog_ctrl>();
            string saying = $"{other_dialog.name}/在{other_dialog.location}说/"+other_dialog.say;
            if (saying!="" && !heard.Contains(saying))
            {
                heard.Add(saying);
                long_term_heardSomeOthers = true;
            }
        }
    }

    float Cac2DDist2(Transform orin, Transform target)
    {
        Vector2 vector2D1 = new Vector2(orin.position.x, orin.position.y);
        Vector2 vector2D2 = new Vector2(target.position.x, target.position.y);

        return Vector2.Distance(vector2D1, vector2D2);
    }
}

