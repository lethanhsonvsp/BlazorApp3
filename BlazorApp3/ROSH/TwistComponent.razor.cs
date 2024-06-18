using Microsoft.AspNetCore.Components;
using RosbridgeNet.RosbridgeClient.Common.Interfaces;
using RosbridgeNet.RosbridgeClient.ProtocolV2.Generics;
using System.Numerics;
using System.Threading.Tasks;

namespace ROSH
{
    public partial class TwistComponent : ComponentBase
    {
        [Inject]
        public WebSocketService? WebSocketService { get; set; }

        public RosPublisher<TwistMessage> publisher;
        public string StatusMessage { get; private set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await Connect();
        }

        private async Task Connect()
        {
            await WebSocketService.ConnectAsync();
            if (WebSocketService.MessageDispatcher != null)
            {
                publisher = CreatePublisher(WebSocketService.MessageDispatcher);
                await publisher.AdvertiseAsync();
                StatusMessage = "Connected to WebSocket.";
            }
            else
            {
                StatusMessage = "Failed to connect to WebSocket.";
            }
        }

        private RosPublisher<TwistMessage> CreatePublisher(IRosbridgeMessageDispatcher messageDispatcher)
        {
            return new RosPublisher<TwistMessage>(messageDispatcher, "/cmd_vel");
        }

        public async Task SendCommand(string command)
        {
            if (publisher != null)
            {
                var message = command switch
                {
                    "forward" => new TwistMessage { linear = new Vector { x = 1 }, angular = new Vector() },
                    "backward" => new TwistMessage { linear = new Vector { x = -1 }, angular = new Vector() },
                    "stop" => new TwistMessage { linear = new Vector { x = 0 }, angular = new Vector { z = 0 } },
                    _ => throw new ArgumentException("Invalid command")
                };
                await publisher.PublishAsync(message);
                StatusMessage = $"Sent {command} command.";
            }
            else
            {
                StatusMessage = "Publisher is not initialized.";
            }
        }
    }
}
