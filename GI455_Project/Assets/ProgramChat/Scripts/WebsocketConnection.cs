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

        public Transform chatDialog;
        public Text inputChat;
        public Text chatDisplay;

        [SerializeField]
        List<Message> messagesList = new List<Message>();

        public void UserConnect()
        {
            iP = inputIP.GetComponent<Text>().text;
            port = inputport.GetComponent<Text>().text;

            websocket = new WebSocket("ws://" + iP + ":" + port + "/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            ConnectLayout.SetActive(false);
            ChatLayout.SetActive(true);
        }

        public void SendMessage()
        {


            if (websocket.ReadyState == WebSocketState.Open)
            {


                //chatSend = inputChat.GetComponent<Text>().text;

                chatSend = inputChat.text;

                //inputChat.text = "";

                if (chatSend != "")
                {
                    websocket.Send(chatSend);
                }

            }

        }

        //public void SendMessageTochat(MessageEventArgs messageEventArgs)
        //{

        //    if (messagesList.Count >= maxMessages)
        //    {
        //        messagesList.Remove(messagesList[0]);
        //    }



        //    Message newMessage = new Message();

        //    newMessage.text = chatReceive;

        //    GameObject newText = Instantiate(textObject, sad);

        //    newMessage.text = newText.GetComponent<Text>();

        //    newMessage

        //    messagesList.Add(newMessage);
        //}

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log("Receive msg : " + messageEventArgs.Data);

            chatDisplay.text += messageEventArgs.Data + "\n";

            //chatReceive = messageEventArgs.Data;

            //SendMessageTochat(messageEventArgs);

            chatSend = messageEventArgs.Data;

            ReceiveMessage();
        }

        public void ReceiveMessage()
        {
            chatSend = inputChat.GetComponent<Text>().text;

            var text = Instantiate(inputChat, chatDialog.position, Quaternion.identity);

            //chatDisplay.alignment = TextAnchor.UpperLeft;

            text.transform.parent = chatDialog;


        }




        private void OnDestroy()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }



        //public void SendMessage()
        //{
        //    if (websocket.ReadyState == WebSocketState.Open)
        //    {
        //        if (!string.IsNullOrEmpty(chatSend.text) && chatSend.text.Trim().Length > 0)
        //        {

        //            websocket.Send(chatSend.text);
        //        }

        //    }

        //}

    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public Text textObject;
    }

}

