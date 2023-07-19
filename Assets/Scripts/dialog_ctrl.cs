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
    public string location = "����";
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
        // ����UDP�ͻ����׽���
        client = new UdpClient();
        client.Client.ReceiveTimeout = 5000;//���ܿ��Է�ֹһֱ�ȴ��ظ�����unity����Ӧ
        // ���ӵ�Python�˵�IP��ַ�Ͷ˿ں�
        pythonEndPoint = new IPEndPoint(ipAddress, port);
    }
    string UDPturn(string sendData)
    {
        string output = "UDPPython_start";

        // ��Python�˷�������
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);
        client.Send(sendBytes, sendBytes.Length, pythonEndPoint);

        // ����Python�˷��͵�����
        IPEndPoint remoteEP = null;
        byte[] receiveBytes = client.Receive(ref remoteEP);
        //byte[] receiveBytes = client.Receive(ref pythonEndPoint);
        output = Encoding.UTF8.GetString(receiveBytes);

        return output;
    }
    string UDPreceive()
    {
        string output = "UDPPython_start";
        // ����Python�˷��͵�����
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
            if ((neibors.Count>0 && !haveSent) || (haveSent && receive == "������"))
            {
                /*List<string> last;
                if (heard.Count >= 5) last = heard.GetRange(heard.Count - 5, 5);
                else last = heard;*/
                receive = UDPturn($"{name};�����;{string.Join("|", heard.GetRange(pre_head_lenth, heard.Count-pre_head_lenth))}");// ������  ��ʼ����ȡ  ��Ԫ��)}
                if (receive == "������") pre_head_lenth= heard.Count;
                UnityEngine.Debug.Log("�����");
                UnityEngine.Debug.Log(receive);
                haveSent = true;
            }
            if (haveSent)
            {
                receive = UDPturn($"{name};�ڵȴ�;");
                //UnityEngine.Debug.Log(receive);
                if (receive != "" && receive != "������" && receive != "δ���" && receive != "������" && receive != "?")
                {
                    haveSent = false;
                    UnityEngine.Debug.Log(receive);
                    if (long_term_heardSomeOthers)
                    {
                        long_term_heardSomeOthers = false;
                        patience *= tau;//�����仯̫���ͬ���ı�����ֵ
                    }
                    else //�ӷ���ָ�����û�����������仯
                    {
                        say = receive;
                        patience = 1f;//˵����ͣ�����ĶȻ�ԭ
                        if (tex)
                        {
                            tex.text = say;
                            tex.enabled = true;
                        }
                        long_term_heardSomeOthers = false;//���¼�ʱ
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
        if (long_term_heardSomeOthers || waitTurns>=3)//�ȴ�����(��ʾ����)��ֻ��״̬�ı�����ȥ˵
        {
            state = "chatting";
            long_term_heardSomeOthers = false;
            waitTurns = 0;
        }
        else if(waitTurns<3)//�����ٵ�
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
            UnityEngine.Debug.Log($"����һ���ھӣ�{other.gameObject}");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        dialog_ctrl other_dialog = other.gameObject.GetComponent<dialog_ctrl>();

        if (other_dialog && neibors.Contains(other.gameObject))
        {
            neibors.Remove(other.gameObject);
            UnityEngine.Debug.Log($"����һ���ھӣ�{other.gameObject}");
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
        if (say != "" && !heard.Contains($"��/��{location}˵/" + say))
        {
            heard.Add($"��/��{location}˵/" + say);
        }
        foreach (GameObject obj in neibors)
        {
            dialog_ctrl other_dialog = obj.GetComponent<dialog_ctrl>();
            string saying = $"{other_dialog.name}/��{other_dialog.location}˵/"+other_dialog.say;
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

