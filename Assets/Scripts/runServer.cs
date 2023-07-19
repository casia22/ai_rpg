using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class runServer : MonoBehaviour
{
    Process process;
    //
    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    int port;
    UdpClient client;
    IPEndPoint pythonEndPoint;

    void runProcess()
    {
        //string output = "runPython";
        // 设置Python脚本路径和参数
        string scriptPath = Application.dataPath + "/" + "Scripts/test.py";
        string arguments = "";  // 传递给Python脚本的参数，以空格分隔

        // 创建一个进程启动信息
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "python";  // Python解释器的路径，如果已经设置了环境变量，可以直接使用"python"
        startInfo.Arguments = $"\"{scriptPath}\" {arguments}"; ;  // 构造命令行参数
        //startInfo.RedirectStandardOutput = true;  // 重定向输出流
        startInfo.UseShellExecute = false;  // 不使用操作系统的shell启动进程
        startInfo.CreateNoWindow = true;  // 不创建新窗口
        //startInfo.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;

        // 启动Python进程
        process = Process.Start(startInfo);
        UnityEngine.Debug.Log("运行外部程序成功！");
        /*
        using (Process process = Process.Start(startInfo))
        {
            // 读取Python脚本的输出
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();  // 等待进程结束
        }
        */
        //return output;
    }

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
    void UDPturn(string sendData)
    {
        string output = "UDPPython_start";

        // 向Python端发送数据
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);
        client.Send(sendBytes, sendBytes.Length, pythonEndPoint);
    }

    // Start is called before the first frame update
    void Awake()
    {
        runProcess();
        UDPset();
    }

    private void OnApplicationQuit()
    {
        UDPturn($"game;关闭;");
        process.Close();
    }
}
