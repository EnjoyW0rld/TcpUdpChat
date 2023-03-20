using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using shared;

public class Client : MonoBehaviour
{
    private TcpClient _client;
    private ChatManager _chatManager;
    private float timeToUpdate;

    private void Update()
    {
        if(timeToUpdate <= 0)
        {
            UpdateNetwork();
            Debug.Log("Elapsed");
            timeToUpdate = Server.COOLDOWN;
        }
        timeToUpdate -= Time.deltaTime;
    }

    public void SetChatManager(ChatManager chatManager) => _chatManager = chatManager;
    public void Connect(string IP, int Port)
    {
        timeToUpdate = Server.COOLDOWN;
        try
        {
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse(IP), Port);

        }
        catch (System.Exception e)
        {
            System.Console.WriteLine("Could not connect to server");
        }

    }

    private void UpdateNetwork()
    {
        if (StreamUtil.Available(_client))
        {
            Packet packet = new Packet(StreamUtil.Read(_client.GetStream()));
            ChatMessage message = packet.Read<ChatMessage>();
            _chatManager.ShowMessage(message.message);
        }
    }
}
