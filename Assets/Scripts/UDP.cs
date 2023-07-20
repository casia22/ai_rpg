using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;

public class UDPClass{

    IPAddress ipAddress;
    int port;
    UdpClient client;
    IPEndPoint pythonEndPoint;

    public UDPClass(IPAddress ipAddress, int port, UdpClient client, IPEndPoint pythonEndPoint)
    {
        this.ipAddress = ipAddress;
        this.port = port;
        this.client = client;
        this.pythonEndPoint = pythonEndPoint;
    }

    public void UDPSendJson(object content)
    {
        string json = "";// JsonConvert.SerializeObject(content);

        byte[] bytes = Encoding.UTF8.GetBytes(json); // ��JSON�ַ���ת��Ϊ�ֽ�����

        client.Send(bytes, bytes.Length, pythonEndPoint);
    }
}

public class Listener
{
    private UdpClient client;
    private CancellationTokenSource cancellationTokenSource;

    public Listener(UdpClient c) //���������ʱ��ʼ������
    {
        client = c;
    }

    public void StartListening()
    {
        cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Task.Run(async () => await ReceiveLoop(cancellationToken));
    }

    public void StopListening()
    {
        cancellationTokenSource?.Cancel();
        client?.Close();
    }

    private async Task ReceiveLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await client.ReceiveAsync();

                // ������յ�������
                byte[] receiveBytes = result.Buffer;
                IPEndPoint remoteEP = result.RemoteEndPoint;

                // ����������Զ����߼�����

                // ʾ�����룺��ӡ���յ������ݺͷ��ͷ��� IP ��ַ
                string message = System.Text.Encoding.UTF8.GetString(receiveBytes);
                Debug.Log($"Received: {message} from {remoteEP.Address}:{remoteEP.Port}");
            }
            catch (SocketException)
            {
                // ������չ����е��쳣
            }
        }
    }
}