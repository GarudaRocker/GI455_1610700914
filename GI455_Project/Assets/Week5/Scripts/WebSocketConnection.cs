using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWeek05
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

        public class MessageData
        {
            public string userName;
            public string message;
        }

        public GameObject rootConnection;
        public GameObject rootIDPassword;
        public GameObject rootRegister;
        public GameObject rootCreateAndJoin;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootMessenger;
        public GameObject errorPopUp;

        public InputField inputUserID;
        public InputField inputUserPassword;
        public InputField inputRegisterID;
        public InputField inputRegisterName;
        public InputField inputRegisterPassword;
        public InputField inputRegisterRePassword;
        public InputField inputRoomNameCreate;
        public InputField inputRoomNameJoin;
        public InputField inputText;
        public Text UserName;
        public Text userRoomName;
        public Text sendText;
        public Text receiveText;
        public Text ErrorText;

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnUserLogin;
        public DelegateHandle OnUserRegister;
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;
        public DelegateHandle OnisRoomFail;
        public DelegateHandle OnSendMessage;

        private void Start()
        {
            rootConnection.SetActive(true);
            rootIDPassword.SetActive(false);
            rootRegister.SetActive(false);
            rootCreateAndJoin.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootMessenger.SetActive(false);
            errorPopUp.SetActive(false);

            OnUserLogin += UserLoginDelegate;
            OnUserRegister += UserRegisterDelegate;
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
            rootIDPassword.SetActive(true);
        }

        public void UserLogin()
        {
            UserLoginDatatoServer();          
        }

        public void UserRegister()
        {
            rootIDPassword.SetActive(false);
            rootRegister.SetActive(true);
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

        public void UserLoginDelegate(SocketEvent status)
        {
            if (status.data == "fail")
            {
                errorPopUp.SetActive(true);
                ErrorText.text = "Login Fail.";
            }
            else
            {
                UserName.text = status.data;

                rootIDPassword.SetActive(false);
                rootCreateAndJoin.SetActive(true);
            }
        }

        public void UserRegisterDelegate(SocketEvent status)
        {
            Debug.Log("Room : " + status.data);

            if (status.data == "fail")
            {
                errorPopUp.SetActive(true);
                ErrorText.text = "Register Fail.";
            }
            else
            {
                errorPopUp.SetActive(true);
                ErrorText.text = "Register Successfully.";

                rootRegister.SetActive(false);
                rootIDPassword.SetActive(true);               
            }
        }

        public void CreateRoomDelegate(SocketEvent status)
        {
            Debug.Log("Room : " + status.data);

            if (status.data == "fail")
            {
                errorPopUp.SetActive(true);
                ErrorText.text = "Room already Exist.";
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
                ErrorText.text = "Room not Exist.";
            }

            else
            {
                rootMessenger.SetActive(true);
                rootJoinRoom.SetActive(false);
            }
        }

        public void UserLoginDatatoServer()
        {
            // Userlogin Here
            if (inputUserID.text == "" || inputUserPassword.text == "")
            {
                errorPopUp.SetActive(true);
                ErrorText.text = "Please input ID or Password.";
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("UserLogin", "#" + inputUserID.text + "#" + inputUserPassword.text);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);              
            }          
        }

        public void UserRegisterDatatoServer()
        {
            // User Register Here

                if (inputRegisterID.text == "" || inputRegisterName.text == "" || inputRegisterPassword.text == "" || inputRegisterRePassword.text == "")
                {
                    errorPopUp.SetActive(true);
                    ErrorText.text = "Please input all Field.";
                }

                else if (inputRegisterPassword.text != inputRegisterRePassword.text)
                {
                    errorPopUp.SetActive(true);
                    ErrorText.text = "Password is not match.";
                }

                else
                {
                    SocketEvent socketEvent = new SocketEvent("UserRegister", "#" + inputRegisterID.text + "#" + inputRegisterPassword.text + "#" + inputRegisterName.text);

                    string toJsonStr = JsonUtility.ToJson(socketEvent);

                    ws.Send(toJsonStr);
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
            if (inputText.text == "" )
                  return;

            MessageData newMessageData = new MessageData();

            newMessageData.userName = UserName.text;
            newMessageData.message = inputText.text;

            string toJsonStrMessage = JsonUtility.ToJson(newMessageData);

            SocketEvent socketEvent = new SocketEvent("SendMessage", toJsonStrMessage);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            inputText.text = "";

            Debug.Log("Send to Server : " + toJsonStr);
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveSocketEvent = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveSocketEvent.eventName == "UserLogin")
                {
                    if (OnUserLogin != null)
                        OnUserLogin(receiveSocketEvent);
                }
                else if (receiveSocketEvent.eventName == "UserRegister")
                {
                    if (OnUserRegister != null)
                        OnUserRegister(receiveSocketEvent);
                }
                else if (receiveSocketEvent.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveSocketEvent);
                }
                else if (receiveSocketEvent.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveSocketEvent);
                }
                else if (receiveSocketEvent.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveSocketEvent);
                }
                else if (receiveSocketEvent.eventName == "SendMessage")
                {
                    if (OnSendMessage != null)
                        OnSendMessage(receiveSocketEvent);

                    MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(receiveSocketEvent.data);

                    Debug.Log("Sender User Name : " + receiveMessageData.userName);

                    if (receiveMessageData.userName == UserName.text)
                    {
                        Debug.Log("Sender User Name : " + receiveMessageData.userName);
                        receiveText.text += "\n";
                        sendText.text += receiveMessageData.userName + " : " + receiveMessageData.message + "\n";
                    }
                    else
                    {
                        Debug.Log("Receive User Name : " + receiveMessageData.userName);
                        sendText.text += "\n";
                        receiveText.text += receiveMessageData.userName + " : " + receiveMessageData.message + "\n";
                    }

                }

                tempMessageString = "";
            }
        }       

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log("Recieve from Server : " + messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }

    
        private void UpdateMessageData()
        {
            {
                MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                Debug.Log("Sender User Name : " + receiveMessageData.userName);

                if (receiveMessageData.userName == UserName.text)
                {
                    Debug.Log("Sender User Name : " + receiveMessageData.userName);
                    receiveText.text += "\n";
                    sendText.text += receiveMessageData.userName + ": " + receiveMessageData.message + "\n";
                }
                else
                {
                    Debug.Log("Receive User Name : " + receiveMessageData.userName);
                    sendText.text += "\n";
                    receiveText.text += receiveMessageData.userName + ": " + receiveMessageData.message + "\n";
                }

                tempMessageString = "";
            }
        }
    }
}


