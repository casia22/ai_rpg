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
        // ����Python�ű�·���Ͳ���
        string scriptPath = Application.dataPath + "/" + "Scripts/test.py";
        string arguments = "";  // ���ݸ�Python�ű��Ĳ������Կո�ָ�

        // ����һ������������Ϣ
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "python";  // Python��������·��������Ѿ������˻�������������ֱ��ʹ��"python"
        startInfo.Arguments = $"\"{scriptPath}\" {arguments}"; ;  // ���������в���
        //startInfo.RedirectStandardOutput = true;  // �ض��������
        startInfo.UseShellExecute = false;  // ��ʹ�ò���ϵͳ��shell��������
        startInfo.CreateNoWindow = true;  // �������´���
        //startInfo.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;

        // ����Python����
        process = Process.Start(startInfo);
        UnityEngine.Debug.Log("�����ⲿ����ɹ���");
        /*
        using (Process process = Process.Start(startInfo))
        {
            // ��ȡPython�ű������
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();  // �ȴ����̽���
        }
        */
        //return output;
    }

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
    void UDPturn(string sendData)
    {
        string output = "UDPPython_start";

        // ��Python�˷�������
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
        UDPturn($"game;�ر�;");
        process.Close();
    }
}
