using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWeek04
{
    public class WebSocketConnection : MonoBehaviour
    {
        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        public GameObject rootConnection;
        public GameObject rootCreateAndJoin;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootMessenger;
        public GameObject errorPopUp;

        public InputField inputUserName;
        public InputField inputRoomNameCreate;
        public InputField inputRoomNameJoin;
        public InputField inputText;
        public Text userRoomName;
        public Text sendText;
        public Text receiveText;

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;
        public DelegateHandle OnisRoomFail;

        private void Start()
        {
            rootConnection.SetActive(true);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootMessenger.SetActive(false);
            errorPopUp.SetActive(false);

            OnCreateRoom += CreateRoomDelegate;
            OnJoinRoom += JoinRoomDelegate;
        }

        private void Update()
        {          
            UpdateNotifyMessage();
        }

        public void Connect()
        {
            string url = "ws://127.0.0.1:25500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootCreateAndJoin.SetActive(true);
        }

        public void UserCreateRoom()
        {
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(true);
        }

        public void UserJoinRoom()
        {
            rootCreateAndJoin.SetActive(false);
            rootJoinRoom.SetActive(true);
        }

        public void CloseErrorPopUp()
        {
            errorPopUp.SetActive(false);
        }

        public void CreateRoom(string roomName)
        {
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(true);

            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);          

        }

        public void JoinRoom(string roomName)
        {
            rootCreateAndJoin.SetActive(false);
            rootJoinRoom.SetActive(true);

            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void CreateRoomDelegate(SocketEvent status)
        {
            Debug.Log(status.data);

            if (status.data == "fail")
            {
                errorPopUp.SetActive(true);
            }
            else
            {
                rootMessenger.SetActive(true);
                rootCreateRoom.SetActive(false);
            }
        }

        public void JoinRoomDelegate(SocketEvent status)
        {
            Debug.Log(status.data);

            if (status.data == "fail")
            {
                errorPopUp.SetActive(true);
            }

            else
            {
                rootMessenger.SetActive(true);
                rootJoinRoom.SetActive(false);
            }
        }

        public void CreateRoomButton()
        {
            SocketEvent socketEvent = new SocketEvent("CreateRoom", inputRoomNameCreate.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            userRoomName.text = "Room : [" + inputRoomNameCreate.text + "]";
        }

        public void JoinRoomButton()
        {
            SocketEvent socketEvent = new SocketEvent("JoinRoom", inputRoomNameJoin.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            userRoomName.text = "Room : [" + inputRoomNameJoin.text + "]";
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            rootMessenger.SetActive(false);
            rootCreateAndJoin.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }

        public void SendMessage(/*string message*/)
        {

        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }               

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }
    }
}


