using UnityEngine;
using System;
using System.Text;

public static class TCPHelper
{
    public static void SendText(ClientState destinationClient, string message, MessageType messageType = MessageType.Text)
    {
        byte[] data = new byte[Encoding.UTF8.GetByteCount(message) + 1];
        data[0] = (byte)messageType;
        Encoding.UTF8.GetBytes(message, 0, message.Length, data, 1);
        Send(destinationClient, data, new AsyncCallback(SendCallback));
    }

    public static void SendPlayerClass(ClientState destinationClient, PlayerClass playerClass, MessageType messageType = MessageType.PlayerClass)
    {
        byte[] data = new byte[2];
        data[0] = (byte)messageType;
        switch (playerClass)
        {
            case PlayerClass.Gunner:
                data[1] = 5;
                break;
            case PlayerClass.Knight:
                data[1] = 6;
                break;
            default:
                break;
        }
        Send(destinationClient, data, new AsyncCallback(SendCallback));
    }

    public static void SendPlayerReady(ClientState destinationClient, MessageType messageType = MessageType.PlayerReady)
    {
        byte[] data = new byte[2];
        data[0] = (byte)messageType;
        data[1] = 1; // Send 1 for player ready state
        Send(destinationClient, data, new AsyncCallback(SendCallback));
    }

    public static void SendCallback(IAsyncResult asyncResult)
    {
        ClientState client = (ClientState)asyncResult.AsyncState;
        int bytesSent = client.Socket.EndSend(asyncResult);
        //Debug.Log($"{client.Username} sent {bytesSent} bytes");
    }

    public static string ReceivedText(byte[] buffer, int receivedBytes)
    {
        return Encoding.UTF8.GetString(buffer, 1, receivedBytes - 1); ;
    }

    public static void Send(ClientState target, byte[] data, AsyncCallback sendCallback)
    {
        target.Socket.BeginSend(data, 0, data.Length, 0, sendCallback, target);
    }

    public static void SendMovementInput(ClientState target, NetworkInput movementInput, AsyncCallback sendCallback)
    {
        byte[] data = new byte[1 + sizeof(float) * 2 + sizeof(long)];
        data[0] = (byte)MessageType.MovementInput;
        movementInput.Input.GetBytes().CopyTo(data, 1);
        BitConverter.GetBytes(movementInput.Timestamp).CopyTo(data, 1 + sizeof(float) * 2);
        Send(target, data, sendCallback);
    }

    public static bool TryReadMovementInput(byte[] data, int receivedBytes, out NetworkInput input)
    {
        if ((MessageType)data[0] == MessageType.MovementInput && receivedBytes == 1 + 2 * sizeof(float) + sizeof(long))
        {
            input = new NetworkInput(data.GetVector2(1), BitConverter.ToInt64(data, 1 + 2 * sizeof(float)));
            return true;
        }
        else
        {
            input = new NetworkInput(Vector2.zero, 0);
            return false;
        }
    }

    public static void SendPosition(ClientState target, NetworkPosition position, AsyncCallback sendCallback)
    {
        byte[] data = new byte[1 + sizeof(float) * 3 + sizeof(long)];
        data[0] = (byte)MessageType.Position;
        position.Position.GetBytes().CopyTo(data, 1);
        BitConverter.GetBytes(position.Timestamp).CopyTo(data, 1 + sizeof(float) * 3);
        Send(target, data, sendCallback);
    }

    public static bool TryReadPosition(byte[] data, int receivedBytes, out NetworkPosition position)
    {
        if ((MessageType)data[0] == MessageType.Position && receivedBytes == 1 + 3 * sizeof(float) + sizeof(long))
        {
            position = new NetworkPosition(data.GetVector3(1), BitConverter.ToInt64(data, 1 + sizeof(float) * 3));
            return true;
        }
        else
        {
            position = new NetworkPosition(Vector3.zero, 0);
            return false;
        }
    }

    public static void SendShoot(ClientState target, Vector3 position, Vector3 direction, long timestamp)
    {
        byte[] data = new byte[1 + sizeof(float) * 6 + sizeof(long)];
        data[0] = (byte)MessageType.Shoot;
        position.GetBytes().CopyTo(data, 1);
        direction.GetBytes().CopyTo(data, 1 + sizeof(float) * 3);
        BitConverter.GetBytes(timestamp).CopyTo(data, 1 + 6 * sizeof(float));
        Send(target, data, new AsyncCallback(SendCallback));
    }

    public static bool TryReadShoot(byte[] data, int receivedBytes, out Vector3 position, out Vector3 direction, out long timestamp)
    {
        if (MessageType.Shoot == (MessageType)data[0] && receivedBytes == 1 + 6 * sizeof(float) + sizeof(long))
        {
            position = data.GetVector3(1);
            direction = data.GetVector3(1 + 3 * sizeof(float));
            timestamp = BitConverter.ToInt64(data, 1 + 6 * sizeof(float));
            return true;
        }
        else
        {
            position = direction = Vector3.zero;
            timestamp = 0;
            return false;
        }
    }
}

public enum MessageType : byte
{
    Text,
    Username,
    PlayerClass,
    PlayerReady,
    MovementInput,
    Position,
    Shoot,
    ClientDisconnect
}


public static class UnityTypesExtensions
{

    public static byte[] GetBytes(this Vector2 vec)
    {
        byte[] data = new byte[2 * sizeof(float)];
        BitConverter.GetBytes(vec.x).CopyTo(data, 0);
        BitConverter.GetBytes(vec.y).CopyTo(data, sizeof(float));
        return data;
    }

    public static Vector2 GetVector2(this byte[] data, int startIndex = 0)
    {
        return new Vector2(BitConverter.ToSingle(data, startIndex), BitConverter.ToSingle(data, startIndex + sizeof(float)));
    }

    public static byte[] GetBytes(this Vector3 vec)
    {
        byte[] data = new byte[3 * sizeof(float)];
        BitConverter.GetBytes(vec.x).CopyTo(data, 0);
        BitConverter.GetBytes(vec.y).CopyTo(data, sizeof(float));
        BitConverter.GetBytes(vec.z).CopyTo(data, sizeof(float) * 2);
        return data;
    }

    public static Vector3 GetVector3(this byte[] data, int startIndex = 0)
    {
        return new Vector3(BitConverter.ToSingle(data, startIndex), BitConverter.ToSingle(data, startIndex + sizeof(float)), BitConverter.ToSingle(data, startIndex + sizeof(float) * 2));
    }
}

public struct NetworkInput
{
    public Vector2 Input;
    public long Timestamp;

    public NetworkInput(Vector2 input, long timestamp)
    {
        Input = input;
        Timestamp = timestamp;
    }
}
public struct NetworkPosition
{
    public Vector3 Position;
    public long Timestamp;

    public NetworkPosition(Vector3 position, long timestamp)
    {
        Position = position;
        Timestamp = timestamp;
    }
}

public struct NetworkShoot
{
    public Vector3 Position;
    public Vector3 Direction;
    public long Timestamp;

    public NetworkShoot(Vector3 position, Vector3 direction, long timestamp)
    {
        Position = position;
        Direction = direction;
        Timestamp = timestamp;
    }
}