using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat Instance;
    
    [SerializeField] private GameObject textObject;
    [SerializeField] private int maxMessages = 20;
    
    private ContentSizeFitter _chatContent;
    private InputField _chatInputField;
    private List<Message> _messageList = new List<Message>();
    private List<GameObject> _textObjectList = new List<GameObject>();

    private Animator _animator;

    private bool _isChatOpened = false;

    private int _animIDShowChat;
    private int _animIDHideChat;
    private int _animIDShowNewUnseenMessageImage;
    
    private class Message
    {
        public string Text;
        public Text TextObject;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _chatContent = GetComponentInChildren<ContentSizeFitter>();
        _chatInputField = GetComponentInChildren<InputField>();
        _animator = GetComponent<Animator>();

        _animIDShowChat = Animator.StringToHash("ShowChat");
        _animIDHideChat = Animator.StringToHash("HideChat");
        _animIDShowNewUnseenMessageImage = Animator.StringToHash("ShowNewUnseenMessageImage");
    }
    
    public void SendMessageToChat(string text, ulong fromWho, bool server)
    {
        if (!_isChatOpened)
        {
            _animator.SetTrigger(_animIDShowNewUnseenMessageImage);
        }
        if (_messageList.Count >= maxMessages)
        {
            Destroy(_messageList[0].TextObject.gameObject);
            _messageList.Remove(_messageList[0]);
        }
        Message newMessage = new Message();
        string senderName = "Server";

        if (!server)
        {
            senderName = SteamClient.Name;
            _chatInputField.ActivateInputField();
        }
        
        newMessage.Text = senderName + ": " + text;

        GameObject newText = Instantiate(textObject, _chatContent.transform);
        newMessage.TextObject = newText.GetComponent<Text>();
        newMessage.TextObject.text = newMessage.Text;

        _messageList.Add(newMessage);
        _textObjectList.Add(newText);
    }
    
    public void ClearChat()
    {
        _messageList.Clear();
        foreach (GameObject textGameObject in _textObjectList)
        {
            Destroy(textGameObject);
        }
        _textObjectList.Clear();
        Debug.Log("Clearing chat");
    }

    public void OpenOrCloseChat(PlayerInputActions inputActions)
    {
        if (_isChatOpened)
        {
            _chatInputField.text = "";
            _chatInputField.DeactivateInputField();
            _isChatOpened = false;
            inputActions.Player.Enable();
            Cursor.lockState = CursorLockMode.Confined;
            _animator.SetTrigger(_animIDHideChat);
        }
        else
        {
            _animator.SetTrigger(_animIDShowChat);
            _chatInputField.text = "";
            _isChatOpened = true;
            inputActions.Player.Disable();
            _chatInputField.ActivateInputField();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TryToSendMessage()
    {
        if (_chatInputField.text != "")
        {
            NetworkTransmission.Instance.IWishToSendAChatServerRpc(_chatInputField.text, NetworkManager.Singleton.LocalClientId);
            _chatInputField.text = "";
        }
    }
}

