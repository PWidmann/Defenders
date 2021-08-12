using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Concurrent;

public class ClientState
{
    public Socket Socket { get; private set; } = null;
    public const int BufferSize = 1024;
    public byte[] Buffer = new byte[BufferSize];
    public string Username = "DefaultName";
    public PlayerClass PlayerClass;
    public bool ReadyToPlay = false;
    public bool GameStarted = false;

    //public ConcurrentQueue<NetworkPosition> PositionQueue = new ConcurrentQueue<NetworkPosition>();
    //public ConcurrentQueue<NetworkInput> InputQueue = new ConcurrentQueue<NetworkInput>();
    //public ConcurrentQueue<NetworkShoot> ShootQueue = new ConcurrentQueue<NetworkShoot>();
    //public Queue<NetworkPosition> hitCheckPositionQueue = new Queue<NetworkPosition>();
    public Vector3 currentPositionOnRemote = Vector3.zero;

    public ClientState(Socket socket)
    {
        Socket = socket;
    }
}

public enum PlayerClass
{
    Gunner,
    Knight
}
