using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using shared;

public class Server : MonoBehaviour
{
    public const int COOLDOWN = 1;

    private TcpListener _listener;
    private TcpClient _client;

    private ChatManager _chatManager;

    private float timeToUpdate;

    public void SetChatManager(ChatManager chatManager)
    {
        _chatManager = chatManager;
        _chatManager.OnMessageAppeared.AddListener(SendChatMessage);
    }
    public void StartServer()
    {
        timeToUpdate = COOLDOWN;
        _listener = new TcpListener(IPAddress.Any, 55555);
        _listener.Start();
    }

    private void Update()
    {
        if (timeToUpdate <= 0)
        {
            ExecuteNetworking();
            timeToUpdate = COOLDOWN;
        }
        timeToUpdate -= Time.deltaTime;
    }

    private void ExecuteNetworking()
    {
        if (_client == null && _listener.Pending())
        {
            _client = _listener.AcceptTcpClient();
        }
        if (StreamUtil.Available(_client))
        {
            Packet packet = new Packet(StreamUtil.Read(_client.GetStream()));
            ChatMessage message = packet.Read<ChatMessage>();
            _chatManager.ShowMessage(message.message);
        }

    }
    private void SendChatMessage(string message)
    {
        ChatMessage newMessage = new ChatMessage(message);
        StreamUtil.Write(_client.GetStream(), newMessage);
    }
}
