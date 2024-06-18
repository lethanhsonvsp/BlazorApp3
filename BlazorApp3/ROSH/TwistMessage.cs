//TwistMessage.cs
using RosbridgeNet.RosbridgeClient.Common.Attributes;


namespace ROSH
{
    [RosMessageType("geometry_msgs/Twist")]
    public class TwistMessage
    {
        public Vector linear { get; set; } = new();
        public Vector angular { get; set; } = new();

        public override string ToString() => $"linear: {linear}, angular: {angular}";
    }

    public class Vector
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public override string ToString() => $"x: {x}, y: {y}, z: {z}";
    }
}

