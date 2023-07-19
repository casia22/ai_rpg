using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

[Serializable]
public struct NPCData
{
    public string name;
    public string desc;
    public string mood;
    public string location;
    public List<string> memory;
}
[Serializable]
public struct KnowledgeData
{
    public string[] actions;
    public string[] places;
    public string[] moods;
    public string[] people;
}
public struct InitFormat
{
    public string func;
    public NPCData[] npc;
    public KnowledgeData knowledge;
    public string language;
}

[Serializable]
public struct ActionFormat
{
    public string type;
    public string args;
}
[Serializable]
public struct LinesFormat
{
    public string type;
    public string state;
    public string name;
    public string mood;
    public string words;
    public ActionFormat action;
}
public struct ReceiveConvFormat
{
    public string name;
    public string location;
    public string id;
    public int length;
    public List<LinesFormat> lines;
}
public struct GenConvFormat
{
    public string func;
    public List<string> npc;
    public string location;
    public string topic;
    public string observations;
    //玩家
    public string starting;
    public string player_desc;

    public string length;
}
public struct ConfirmFormat
{
    public string func;
    public string conversation_id;
    public int index;
}

public class DialogScene
{
    private string targetUrl;
    private int targetPort;
    private int listenPort;
    public UdpClient sock;
    private Thread listenThread;
    private bool thread_stop;

    public bool new_receive;
    public List<ReceiveConvFormat> receiveConvs = new List<ReceiveConvFormat>();

    public DialogScene(string targetUrl = "::1", int targetPort = 8199, int listenPort = 8084)
    {
        this.targetUrl = targetUrl;
        this.targetPort = targetPort;
        this.listenPort = listenPort;
        this.sock = new UdpClient(AddressFamily.InterNetworkV6);
        //this.sock.Client.ReceiveTimeout = 5000;
        this.listenThread = new Thread(Listen);
        this.listenThread.Start();
        this.thread_stop = false;
    }

    public void Listen()
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, this.listenPort);
        this.sock.Client.Bind(localEndPoint);

        string receivedData = "";
        while (!this.thread_stop)
        {
            UnityEngine.Debug.Log("listening");
            byte[] data = this.sock.Receive(ref localEndPoint);
            string packet = Encoding.UTF8.GetString(data); UnityEngine.Debug.LogFormat("get: {0}", packet);
            string[] parts = packet.Split('@');
            string lastPart = parts[parts.Length - 1];
            receivedData+=lastPart;
            if (receivedData.EndsWith("}"))
            {
                UnityEngine.Debug.LogFormat("receivedData: {0}", receivedData);
                ReceiveConvFormat json_data = JsonUtility.FromJson<ReceiveConvFormat>(receivedData);
                receivedData = ""; UnityEngine.Debug.LogFormat("receivedData: {0}", receivedData);
                if (json_data.name == "inited") { UnityEngine.Debug.Log("Successful initialization."); continue; }
                else
                {
                    UnityEngine.Debug.Log(json_data.location);
                    foreach (LinesFormat line in json_data.lines) UnityEngine.Debug.Log(line.words);
                    if (json_data.name == "conversation") { receiveConvs.Add(json_data); new_receive = true; }
                }
                /*}
                catch (Exception ex)
                {
                    // JSON decoding error
                    UnityEngine.Debug.Log("连接失败"+ex.Message);
                    continue;
                }*/
            }
        }
        UnityEngine.Debug.Log(this.thread_stop);
    }

    public void InitEngine()
    {
        InitFormat init_data = new InitFormat
        {
            func = "init",
            npc = new NPCData[]
            {
                new NPCData
                {
                    name = "武大郎",
                    desc = "是个老实人，是潘金莲的丈夫",
                    mood = "正常",
                    location = "武大郎家",
                    memory = new List<string>()
                },
                new NPCData
                {
                    name = "潘金莲",
                    desc = "是个坏人，是武大郎的夫人，但很讨厌武大郎，经常骂武大郎，希望摆脱他，偷偷喜欢西门庆",
                    mood = "严肃",
                    location = "沙地",
                    memory = new List<string>()
                },
                new NPCData
                {
                    name = "西门庆",
                    desc = "是个坏人，喜欢潘金莲，想杀掉武大郎",
                    mood = "焦急",
                    location = "草地",
                    memory = new List<string>()
                }
            },
            knowledge = new KnowledgeData
            {
                actions = new[] { "stay", "move", "chat" },
                places = new[] { "武大郎家", "王婆家", "广场", "瓜田", "酒吧", "警局" },
                moods = new[] { "正常", "焦急", "严肃", "开心", "伤心" },
                people = new[] { "武大郎", "潘金莲", "西门庆", "警长" }
            },
            language="C"
        };

        SendData(init_data);
    }

    public GenConvFormat GenerateConversation(IEnumerable<string> npc, string location, string topic, string obs="", string player_start="",string player_desc="",string length="S")
    {
        GenConvFormat conversation_data = new GenConvFormat
        {
            func = "create_conversation",
            npc = new List<string>(npc),
            location = location,
            topic = topic,
            observations = obs,
            starting = player_start,
            player_desc = player_desc,
            length = length
        };

        SendData(conversation_data);
        return conversation_data;
    }

    public void ConfirmConversation(string conversationId, int index)
    {
        ConfirmFormat confirm_data = new ConfirmFormat
        {
            func = "confirm_conversation_line",
            conversation_id = conversationId,
            index = index
        };

        SendData(confirm_data);
    }

    private void SendData(object data)
    {
        string json = JsonUtility.ToJson(data);
        UnityEngine.Debug.Log(json);
        json = $"@1@1@{json}";
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        //add seperator
        /*
        byte[] separator = Encoding.UTF8.GetBytes("@@@");
        byte[] dataWithSeparator = new byte[bytes.Length + separator.Length];
        bytes.CopyTo(dataWithSeparator, 0);
        separator.CopyTo(dataWithSeparator, bytes.Length);*/

        this.sock.Send(bytes, bytes.Length, this.targetUrl, this.targetPort);
    }

    public void Dispose()
    {
        this.sock.Close();
        this.thread_stop = true;
        this.listenThread.Join();
    }
}

public class GodsHand : MonoBehaviour
{
    // 程序
    Process process;
    public DialogScene scene;
    public List<ChatRoom> rooms;

    void runProcess()
        {
            // 设置Python脚本路径和参数
            string scriptPath = Application.dataPath + "/" + "Scripts/NPCEngine.py";
            string arguments = "";  // 传递给Python脚本的参数，以空格分隔

            // 创建一个进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "python";  // Python解释器的路径，如果已经设置了环境变量，可以直接使用"python"
            startInfo.Arguments = $"\"{scriptPath}\" {arguments}"; ;  // 构造命令行参数
            startInfo.UseShellExecute = false;  // 不使用操作系统的shell启动进程
            startInfo.CreateNoWindow = true;  // 不创建新窗口

            // 启动Python进程
            process = Process.Start(startInfo);
            UnityEngine.Debug.Log("运行外部程序成功！");
        }
    
    void genConversations()
    {
        foreach(ChatRoom room in rooms)
        {
            if (room.chatRequest)
            {
                scene.GenerateConversation(room.members.Keys, room.location, room.topic, room.interruptSpeech);
                room.chatRequest = false;
            }
        }
    }

    void Start()
    {
        //runProcess();
        Task.Delay(2000).Wait();
        scene = new DialogScene();
        scene.InitEngine();
        
        //GenConvFormat res = scene.GenerateConversation(new[] { "潘金莲", "武大郎" }, "酒吧", "喝酒吗？", "");
        //scene.ConfirmConversation("6e273930-af5c-4c50-bf40-be906af91f9a", 24);
        UnityEngine.Debug.Log("initialization Done.");
    }

    void Update()
    {
        genConversations();
        if (scene.new_receive)
        {
            List<ReceiveConvFormat> t = new List<ReceiveConvFormat>();
            rooms.RemoveAll(item => item == null);//清空丢失的元素
            foreach (ReceiveConvFormat conv in scene.receiveConvs)
            {
                foreach(ChatRoom room in rooms)
                {
                    if (conv.location == room.location)
                    {
                        UnityEngine.Debug.Log($"分配到{room.location}");
                        room.receiveConv=conv; room.getConv = true;
                        t.Add(conv);
                    }
                }
            }
            foreach(ReceiveConvFormat conv in t)
            {
                scene.receiveConvs.Remove(conv);
            }
            scene.new_receive = false;
        }
    }

    private void OnDestroy()
    {
        UnityEngine.Debug.Log("OnDestroy");
        scene.Dispose();
        //scene.sock.Close();
        process.Close();
    }
    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("OnApplicationQuit");
        scene.Dispose();
        //scene.sock.Close();
        process.Close();
    }
}
