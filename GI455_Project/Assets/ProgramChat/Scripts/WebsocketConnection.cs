using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;

        private string iP;
        private string port;

        public GameObject inputIP;
        public GameObject inputport;
        public GameObject ConnectLayout;
        public GameObject ChatLayout;

        private string chatSend;
        private string tempMessage;

        public Transform chatDialog;
        public Text inputChat;
        public Text chatDisplay;

        [SerializeField]
        List<Message> messagesList = new List<Message>();

        public void Start()
        {
            ConnectLayout.SetActive(true);
            ChatLayout.SetActive(false);
        }

        public void UserConnect()
        {
            iP = inputIP.GetComponent<Text>().text;
            port = inputport.GetComponent<Text>().text;

            string url = $"ws://" + iP + ":" + port + "/";

            websocket = new WebSocket(url);

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            ConnectLayout.SetActive(false);
            ChatLayout.SetActive(true);
        }

        private void Disconnect()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }

        public void SendMessage()
        {
            if (/*inputChat.text == "" || */websocket.ReadyState == WebSocketState.Open)
                //return;

            chatSend = inputChat.text;

            websocket.Send(chatSend);

            //inputChat.text = "";
            
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log("Receive msg : " + messageEventArgs.Data);

            tempMessage = messageEventArgs.Data;

            //chatDisplay.text += messageEventArgs.Data + "\n";

            //chatSend = messageEventArgs.Data;

            //ReceiveMessage();
        }

        private void OnDestroy()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }

        public void Update()
        {
            if (!string.IsNullOrEmpty(tempMessage) && tempMessage.Trim().Length > 0)
            {
               
                chatDisplay.text += tempMessage + "\n";

                //if (receiveMessageData.username == inputUserName.text)
                //{
                //    sendText.text += receiveMessageData.username + ": " + receiveMessageData.message + "\n";
                //}
                //else
                //{
                //    receiveText.text += receiveMessageData.username + ": " + receiveMessageData.message + "\n";
                //}

                tempMessage = "";
            }
        }

        //public void ReceiveMessage()
        //{
        //    chatSend = inputChat.GetComponent<Text>().text;

        //    var text = Instantiate(inputChat, chatDialog.position, Quaternion.identity);

        //    //chatDisplay.alignment = TextAnchor.UpperLeft;

        //    text.transform.parent = chatDialog;


        //}

    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public Text textObject;
    }

}

