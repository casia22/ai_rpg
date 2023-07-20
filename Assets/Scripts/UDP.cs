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

        byte[] bytes = Encoding.UTF8.GetBytes(json); // 将JSON字符串转换为字节数组

        client.Send(bytes, bytes.Length, pythonEndPoint);
    }
}

public class Listener
{
    private UdpClient client;
    private CancellationTokenSource cancellationTokenSource;

    public Listener(UdpClient c) //调用这个类时初始化变量
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

                // 处理接收到的数据
                byte[] receiveBytes = result.Buffer;
                IPEndPoint remoteEP = result.RemoteEndPoint;

                // 在这里进行自定义逻辑处理

                // 示例代码：打印接收到的数据和发送方的 IP 地址
                string message = System.Text.Encoding.UTF8.GetString(receiveBytes);
                Debug.Log($"Received: {message} from {remoteEP.Address}:{remoteEP.Port}");
            }
            catch (SocketException)
            {
                // 处理接收过程中的异常
            }
        }
    }
}