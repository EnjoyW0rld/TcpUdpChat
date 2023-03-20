using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChatManager : MonoBehaviour
{
    private Server _server;
    private Client _client;

    [SerializeField] private GameObject _connectionScreen;
    [SerializeField] private GameObject _gameScreen;
    [SerializeField] private TextMeshProUGUI _gameText;

    [HideInInspector] public UnityEvent<string> OnMessageAppeared;

    string IP = "192.168.56.1";

    public void OnConnetButtonClick()
    {
        GameObject clientObj = new GameObject();
        clientObj.name = "Client";
        _client = clientObj.AddComponent<Client>();
        _client.SetChatManager(this);
        _client.Connect(IP,55555);
        SetGameScreen();
    }
    public void OnHostButtonClick()
    {
        GameObject hostObj = new GameObject();
        hostObj.name = "Server";
        _server = hostObj.AddComponent<Server>();
        _server.SetChatManager(this);
        _server.StartServer();
        SetGameScreen();
    }

    private void SetGameScreen()
    {
        _connectionScreen.SetActive(false);
        _gameScreen.SetActive(true);
    }

    public void ShowMessage(string message)
    {
        _gameText.text = message;
    }
    public void NewMessage(string message)
    {
        OnMessageAppeared?.Invoke(message);
    }
}
