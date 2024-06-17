using Microsoft.AspNetCore.Components;
using RosbridgeNet.RosbridgeClient.Common.Interfaces;
using RosbridgeNet.RosbridgeClient.ProtocolV2.Generics;
using System.Threading.Tasks;

namespace BlazorApp3
{
    public partial class TwistComponent : ComponentBase
    {
        [Inject]
        public WebSocketService? WebSocketService { get; set; }

        private RosPublisher<TwistMessage>? publisher;

        public string StatusMessage { get; private set; } = string.Empty;

        public async Task Connect()
        {
            if (WebSocketService != null)
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
        }

        private RosPublisher<TwistMessage> CreatePublisher(IRosbridgeMessageDispatcher messageDispatcher)
        {
            return new RosPublisher<TwistMessage>(messageDispatcher, "/cmd_vel");
        }

        public async Task MoveForward()
        {
            if (publisher != null)
            {
                await publisher.PublishAsync(new TwistMessage
                {
                    linear = new Vector { x = 1, y = 0, z = 0 },
                    angular = new Vector { x = 0, y = 0, z = 0 }
                });
                StatusMessage = "Sent move forward command.";
            }
            else
            {
                StatusMessage = "Publisher is not initialized.";
            }
        }

        public async Task MoveBackward()
        {
            if (publisher != null)
            {
                await publisher.PublishAsync(new TwistMessage
                {
                    linear = new Vector { x = -1, y = 0, z = 0 },
                    angular = new Vector { x = 0, y = 0, z = 0 }
                });
                StatusMessage = "Sent move backward command.";
            }
            else
            {
                StatusMessage = "Publisher is not initialized.";
            }
        }

        public async Task TurnLeft()
        {
            if (publisher != null)
            {
                await publisher.PublishAsync(new TwistMessage
                {
                    linear = new Vector { x = 0, y = 0, z = 0 },
                    angular = new Vector { x = 0, y = 0, z = 1 }
                });
                StatusMessage = "Sent turn left command.";
            }
            else
            {
                StatusMessage = "Publisher is not initialized.";
            }
        }

        public async Task TurnRight()
        {
            if (publisher != null)
            {
                await publisher.PublishAsync(new TwistMessage
                {
                    linear = new Vector { x = 0, y = 0, z = 0 },
                    angular = new Vector { x = 0, y = 0, z = -1 }
                });
                StatusMessage = "Sent turn right command.";
            }
            else
            {
                StatusMessage = "Publisher is not initialized.";
            }
        }
    }

    [RosbridgeNet.RosbridgeClient.Common.Attributes.RosMessageType("geometry_msgs/Twist")]
    public class TwistMessage
    {
        public Vector linear { get; set; }
        public Vector angular { get; set; }

        public TwistMessage()
        {
            linear = new Vector();
            angular = new Vector();
        }

        public override string ToString()
        {
            return $"linear: {linear}, angular: {angular}";
        }
    }

    public class Vector
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public override string ToString()
        {
            return $"x: {x}, y: {y}, z: {z}";
        }
    }
}
