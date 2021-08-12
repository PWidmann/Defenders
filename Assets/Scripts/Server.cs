using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class Server : MonoBehaviour
{
    private int serverPort;
    Socket listener = null;
    private bool gameStarted = false;

    List<ClientState> connectedClients = new List<ClientState>();

    public void StartServer(int _serverPort)
    {
        serverPort = _serverPort;
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, serverPort);
        listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(ipEndPoint);
        listener.Listen(5);

        Debug.Log($"Started server on port {_serverPort}");
        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
    }

    public void CloseServer()
    {
        listener.Close();
    }

    void AcceptCallback(IAsyncResult asyncResult)
    {
        Socket listener = (Socket)asyncResult.AsyncState;
        Socket clientSocket = listener.EndAccept(asyncResult);
        ClientState client = new ClientState(clientSocket);

        connectedClients.Add(client);

        client.Socket.BeginReceive(client.Buffer, 0, ClientState.BufferSize, 0, new AsyncCallback(ReceiveCallback), client);
        // Restart Accept process on server socket
        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
    }

    void ReceiveCallback(IAsyncResult asyncResult)
    {
        ClientState client = asyncResult.AsyncState as ClientState;
        int receivedBytes = client.Socket.EndReceive(asyncResult);

        if (receivedBytes > 0)
        {
            switch ((MessageType)client.Buffer[0])
            {
                case MessageType.Text:
                    string message = Encoding.UTF8.GetString(client.Buffer, 1, receivedBytes - 1);
                    Debug.Log($"{client.Username}: {message}");
                    foreach (ClientState clientState in connectedClients)
                    {
                        //Send(clientState, $"{client.Username}: {message}");
                    }
                    break;
                case MessageType.Username:
                    client.Username = TCPHelper.ReceivedText(client.Buffer, receivedBytes);
                    Debug.Log($"Username {client.Username} allocated");
                    break;
                case MessageType.MovementInput:
                    if (TCPHelper.TryReadMovementInput(client.Buffer, receivedBytes, out NetworkInput input))
                    {
                        //client.InputQueue.Enqueue(input);
                    }
                    break;
                case MessageType.Position:
                    if (TCPHelper.TryReadPosition(client.Buffer, receivedBytes, out NetworkPosition position))
                    {
                        //client.PositionQueue.Enqueue(position);
                    }
                    break;
                case MessageType.Shoot:
                    if (TCPHelper.TryReadShoot(client.Buffer, receivedBytes, out Vector3 shootPosition, out Vector3 direction, out long timestamp))
                    {
                        //client.ShootQueue.Enqueue(new NetworkShoot(shootPosition, direction, timestamp));
                    }
                    break;
                case MessageType.ClientDisconnect:
                    connectedClients.Remove(client);
                    Debug.Log($"Client {client.Username} disconnected");
                    break;
                case MessageType.PlayerClass:
                    switch (client.Buffer[1])
                    {
                        case 5:
                            client.PlayerClass = PlayerClass.Gunner;
                            Debug.Log($"Player {client.Username} chose class Gunner");
                            break;
                        case 6:
                            client.PlayerClass = PlayerClass.Knight;
                            Debug.Log($"Player {client.Username} chose class Knight");
                            break;
                        default:
                            break;
                    }
                    break;
                case MessageType.PlayerReady:
                    client.ReadyToPlay = true; ;
                    Debug.Log($"Client {client.Username} is ready");
                    break;
                default:
                    break;
            }

            //string allUsers = "";
            //foreach (var user in connectedClients)
            //{
            //    allUsers += " " + user.Username ;
            //}
            //Debug.Log("All connected users: " + allUsers);

            client.Socket.BeginReceive(client.Buffer, 0, ClientState.BufferSize, 0, new AsyncCallback(ReceiveCallback), client);
        }
    }
}
