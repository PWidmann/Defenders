using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Concurrent;

public class Client : MonoBehaviour
{
    ClientState clientState = null;

    public void StartClient(string _serverIP, int _serverPort, string _userName)
    {
        IPAddress serverAddress = IPAddress.Parse(_serverIP);
        IPEndPoint serverEndPoint = new IPEndPoint(serverAddress, _serverPort);

        Socket socket = new Socket(serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        clientState = new ClientState(socket);
        clientState.Username = _userName;


        clientState.Socket.BeginConnect(serverEndPoint, new AsyncCallback(ConnectCallback), clientState);
    }

    public void SetPlayerClass(PlayerClass playerClass)
    {
        TCPHelper.SendPlayerClass(clientState, playerClass, MessageType.PlayerClass);
    }

    public void SetPlayerReady()
    {
        TCPHelper.SendPlayerReady(clientState);
    }

    public void DisconnectClient()
    {
        TCPHelper.SendText(clientState, clientState.Username, MessageType.ClientDisconnect);
    }

    void ConnectCallback(IAsyncResult asyncResult)
    {
        ClientState clientState = (ClientState)asyncResult.AsyncState; // server
        clientState.Socket.EndConnect(asyncResult);
        TCPHelper.SendText(clientState, clientState.Username, MessageType.Username);
        Debug.Log($"Client Successfully connected to the server {clientState.Socket.RemoteEndPoint} with name {clientState.Username}");

        clientState.Socket.BeginReceive(clientState.Buffer, 0, ClientState.BufferSize, 0, new AsyncCallback(ReceiveCallback), clientState);
    }

    

    void ReceiveCallback(IAsyncResult asyncResult)
    {
        ClientState client = asyncResult.AsyncState as ClientState;
        int receivedBytes = client.Socket.EndReceive(asyncResult);

        if (receivedBytes > 0)
        {

            client.Socket.BeginReceive(client.Buffer, 0, ClientState.BufferSize, 0, new AsyncCallback(ReceiveCallback), client);
        }
    }


}