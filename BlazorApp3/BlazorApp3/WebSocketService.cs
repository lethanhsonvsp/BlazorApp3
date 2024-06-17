using RosbridgeNet.RosbridgeClient.Common;
using RosbridgeNet.RosbridgeClient.Common.Interfaces;
using RosbridgeNet.RosbridgeClient.ProtocolV2;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp3
{
    public class WebSocketService
    {
        private readonly string _webSocketUri;
        private CancellationTokenSource _cancellationTokenSource;
        public IRosbridgeMessageDispatcher? MessageDispatcher { get; private set; }

        public WebSocketService(string webSocketUri)
        {
            _webSocketUri = webSocketUri;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            if (MessageDispatcher == null)
            {
                ISocket socket = new Socket(new ClientWebSocket(), new Uri(_webSocketUri), _cancellationTokenSource);
                IRosbridgeMessageSerializer messageSerializer = new RosbridgeMessageSerializer();
                MessageDispatcher = new RosbridgeMessageDispatcher(socket, messageSerializer);

                await MessageDispatcher.StartAsync();
            }
        }
    }
}
