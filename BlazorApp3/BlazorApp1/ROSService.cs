using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RosbridgeNet.RosbridgeClient.Common;
using RosbridgeNet.RosbridgeClient.Common.Attributes;
using RosbridgeNet.RosbridgeClient.Common.Interfaces;
using RosbridgeNet.RosbridgeClient.ProtocolV2;
using RosbridgeNet.RosbridgeClient.ProtocolV2.Generics;
using RosbridgeNet.RosbridgeClient.ProtocolV2.Generics.Interfaces;

public class ROSService
{
    private readonly ILogger<ROSService> _logger;
    private readonly IRosbridgeMessageDispatcher _messageDispatcher;
    private readonly RosPublisher<Twist> _publisher;
    private readonly CancellationTokenSource _cts;

    public ROSService(string webSocketUri, ILogger<ROSService> logger)
    {
        _logger = logger;
        _cts = new CancellationTokenSource();
        _messageDispatcher = Connect(new Uri(webSocketUri), _cts);
        _publisher = CreatePublisher();
    }

    public void Subscribe(Action<Twist> onMessageReceived)
    {
        RosSubscriber<Twist> subscriber = new RosSubscriber<Twist>(_messageDispatcher, "/cmd_vel");
        subscriber.RosMessageReceived += (s, e) => { onMessageReceived?.Invoke(e.RosMessage); };
        subscriber.SubscribeAsync();
    }

    public RosPublisher<Twist> CreatePublisher()
    {
        RosPublisher<Twist> publisher = new RosPublisher<Twist>(_messageDispatcher, "/cmd_vel");
        publisher.AdvertiseAsync();
        return publisher;
    }


    public void MoveForward()
    {
        _logger.LogInformation("MF method call");
        var twist = new Twist
        {
            linear = new Vector { x = 0.5f , y = 0, z = 0 },
            angular = new Vector { x = 0, y = 0, z = 0 }
        };
        _publisher.PublishAsync(twist);
    }

    public void MoveBackward()
    {
        var twist = new Twist
        {
            linear = new Vector { x = -0.5f, y = 0, z = 0 },
            angular = new Vector { x = 0, y = 0, z = 0 }
        };
        _publisher.PublishAsync(twist);
    }

    public void TurnLeft()
    {
        var twist = new Twist
        {
            linear = new Vector { x = 0, y = 0, z = 0 },
            angular = new Vector { x = 0, y = 0, z = 0.5f }
        };
        _publisher.PublishAsync(twist);
    }

    public void TurnRight()
    {
        var twist = new Twist
        {
            linear = new Vector { x = 0, y = 0, z = 0 },
            angular = new Vector { x = 0, y = 0, z = -0.5f }
        };
        _publisher.PublishAsync(twist);
    }

    private IRosbridgeMessageDispatcher Connect(Uri webSocketAddress, CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            ISocket socket = new Socket(new ClientWebSocket(), webSocketAddress, cancellationTokenSource);
            IRosbridgeMessageSerializer messageSerializer = new RosbridgeMessageSerializer();
            IRosbridgeMessageDispatcher messageDispatcher = new RosbridgeMessageDispatcher(socket, messageSerializer);
            messageDispatcher.StartAsync().Wait();
            _logger.LogInformation("Connected to ROS bridge server at {WebSocketAddress}", webSocketAddress);
            return messageDispatcher;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to ROS bridge server at {WebSocketAddress}", webSocketAddress);
            throw;
        }
    }
}

[RosMessageType("geometry_msgs/Twist")]
public class Twist
{
    public Vector linear { get; set; }
    public Vector angular { get; set; }
    public override string ToString()
    {
        return string.Format("linear: {0}, angular: {1}", linear.ToString(), angular.ToString());
    }
}

public class Vector
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public override string ToString()
    {
        return string.Format("x: {0}, y: {1}, z: {2}", x, y, z);
    }
}
