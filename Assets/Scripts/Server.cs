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
    private TcpClient _tcpClient;

    private UdpClient _udpClient;

    private ChatManager _chatManager;
    private MovementManager _movementManager;

    private float timeToUpdate;

    public void SetChatManager(ChatManager chatManager)
    {
        _chatManager = chatManager;
        _chatManager.OnMessageAppeared.AddListener(SendChatMessage);
    }
    public void StartServer()
    {
        _movementManager = FindObjectOfType<MovementManager>();
        _movementManager.OnPositionChange.AddListener(OnPositionChange);
        _movementManager.SetOwner(0);

        timeToUpdate = COOLDOWN;
        _listener = new TcpListener(IPAddress.Any, 55555);
        _listener.Start();
        _udpClient = new UdpClient(55555);

    }
    private void OnPositionChange(int id, Transform pTransform)
    {
        try
        {
            PlayerMoved playerMove = new PlayerMoved(pTransform.position, id);
            Packet pack = new Packet();
            pack.Write(playerMove);
            byte[] bytes = pack.GetBytes();
            _udpClient.Send(bytes, bytes.Length,_endPoint);
            Debug.Log("Packet sent");
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
    private void Update()
    {
        if (timeToUpdate <= 0)
        {
            ExecuteNetworkingTcp();
            ExecuteNetworkingUdp();
            timeToUpdate = COOLDOWN;
        }
        timeToUpdate -= Time.deltaTime;
    }

    private void ExecuteNetworkingTcp()
    {
        if (_tcpClient == null && _listener.Pending())
        {
            _tcpClient = _listener.AcceptTcpClient();
        }
        else return;

        if (StreamUtil.Available(_tcpClient))
        {
            Packet packet = new Packet(StreamUtil.Read(_tcpClient.GetStream()));
            ChatMessage message = packet.Read<ChatMessage>();
            _chatManager.ShowMessage(message.message);
        }

    }
    IPEndPoint _endPoint = new IPEndPoint(IPAddress.Any, 55555);
    private void ExecuteNetworkingUdp()
    {
        if (_udpClient.Available > 0)
        {
            byte[] message = _udpClient.Receive(ref _endPoint);
            Packet pack = new Packet(message);
            PlayerMoved move = pack.Read<PlayerMoved>();
            _movementManager.MovePlayer(move.ID,new Vector3(move.pos[0], move.pos[1],move.pos[2]));
        }
    }

    private void SendChatMessage(string message)
    {
        ChatMessage newMessage = new ChatMessage(message);
        StreamUtil.Write(_tcpClient.GetStream(), newMessage);
    }
}
