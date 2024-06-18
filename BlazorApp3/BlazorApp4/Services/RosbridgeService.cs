// Services/RosbridgeService.cs
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp4.Services
{
    public class RosbridgeService
    {
        private ClientWebSocket? _webSocket;

        public async Task ConnectAsync(string uri)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("WebSocket is not connected.");

            var bytes = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("WebSocket is not connected.");

            var buffer = new byte[1024];
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _webSocket.Dispose();
            }
        }
    }
}
