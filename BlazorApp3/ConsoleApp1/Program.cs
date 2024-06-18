using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (ClientWebSocket ws = new ClientWebSocket())
            {
                Uri serverUri = new Uri("ws://192.168.137.46:9090");
                await ws.ConnectAsync(serverUri, CancellationToken.None);

                Console.WriteLine("Connected!");

                string msg = "{\"op\": \"advertise\", \"topic\": \"/chat\", \"type\": \"std_msgs/String\"}";
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

                Console.WriteLine("Message sent!");

                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                Console.WriteLine("Message received: " + Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));

                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                Console.WriteLine("Disconnected!");
            }
        }
    }
}
