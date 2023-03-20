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

    private UdpClient _udpClient;
    private MovementManager _movementManager;

    private float timeToUpdate;

    private void Update()
    {
        if (timeToUpdate <= 0)
        {
            UpdateNetwork();
            UpdateNetworkUdp();
            timeToUpdate = Server.COOLDOWN;
        }
        timeToUpdate -= Time.deltaTime;
    }

    public void SetChatManager(ChatManager chatManager)
    {
        _chatManager = chatManager;
        _chatManager.OnMessageAppeared.AddListener(SendNewMessage);
    }
    IPEndPoint _endPoint;
    public void Connect(string IP, int Port)
    {
        timeToUpdate = Server.COOLDOWN;
        _movementManager = FindObjectOfType<MovementManager>();
        _movementManager.OnPositionChange.AddListener(OnPlayerMove);
        _movementManager.SetOwner(1);
        try
        {
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse(IP), Port);

            _udpClient = new UdpClient();
            _udpClient.Connect(IPAddress.Parse(IP), 55555);
            _endPoint = new IPEndPoint(IPAddress.Parse(IP), 55555);
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine("Could not connect to server");
        }

    }

    private void UpdateNetworkUdp()
    {
        if (_udpClient.Available > 0)
        {
            byte[] bytes = _udpClient.Receive(ref _endPoint);
            Packet pack = new Packet(bytes);
            PlayerMoved playerMove = pack.Read<PlayerMoved>();
            _movementManager.MovePlayer(playerMove.ID, new Vector3(playerMove.pos[0], playerMove.pos[1], playerMove.pos[2]));
            Debug.Log("Recieved new message UDP");
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
    private void SendNewMessage(string message)
    {
        ChatMessage newMessage = new ChatMessage(message);
        StreamUtil.Write(_client.GetStream(), newMessage);
    }
    private void OnPlayerMove(int id, Transform pos)
    {
        PlayerMoved playerMove = new PlayerMoved(pos.position, id);
        Packet pack = new Packet();
        pack.Write(playerMove);
        byte[] bytes = pack.GetBytes();
        _udpClient.Send(bytes, bytes.Length);
    }
}
