const { json } = require('express');

const sqlite = require('sqlite3').verbose();

const websocket = require('ws');

const wss = new websocket.Server({port:25500}, ()=>{
     console.log("Server is running");
});

let db = new sqlite.Database('./db.chat.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

    console.log('Connected to Database.');

});

var roomList = [];

wss.on("connection", (ws)=>{
    
    //Lobby
    console.log("client connected.");
    //Reception
    ws.on("message", (data)=>{
        console.log("send from client :"+ data);       

        //========== Convert jsonStr into jsonObj =======

        //toJsonObj = JSON.parse(data);

        // I change to line below for prevent confusion
        var toJsonObj = { 
            roomName:"",
            data:""
        }
        toJsonObj = JSON.parse(data);
        //===============================================

        if(toJsonObj.eventName == "UserLogin") //Login
        {
            var dataFromClient = {
                eventName:"UserLogin",
                data:toJsonObj.data
            }

            var splitStr = dataFromClient.data.split('#');
            var id = splitStr[1];
            var password = splitStr[2];
        
            var sqlSelect = "SELECT * FROM UserData WHERE UserID='"+id+"' AND Password='"+password+"' "; //Check Database to Login
        
            db.all(sqlSelect, (err,rows)=>{
        
                if(err){
        
                    console.log("Login[0]" + err);
                }
                else
                {
                    if(rows.length > 0)
                    {
                        //User id found
                        console.log("------Login[1]-------");
                        console.log(rows);
                        console.log("------Login[1]-------");

                        var callbackMsg = {
                            eventName:"UserLogin",
                            data:rows[0].Name

                        }

                        //console.log("Login Success");
                        // Send Name

                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        console.log("Login[2]" + toJsonStr);

                        console.log("------UserName-------");
                        console.log(rows[0].Name);                    
                        console.log("------Login[3]-------");

                    }
                    else
                    {
                        //User id not found
                        var callbackMsg = {
                            eventName:"UserLogin",
                            data:"fail"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        console.log("Loginfail[4]" + toJsonStr);

                    }
            
                }
        
        
            });

        }

        else if(toJsonObj.eventName == "UserRegister") //Register
        {
            var dataFromClient = {
                eventName:"UserRegister",
                data:toJsonObj.data
            }

            var splitStr = dataFromClient.data.split('#');
            var id = splitStr[1];
            var password = splitStr[2];
            var name = splitStr[3];

            var sqlInsert = "INSERT INTO UserData (UserID, Password, Name, Money) VALUES ('"+id+"', '"+password+"', '"+name+"', '0')"; //Register to Database

            db.all(sqlInsert, (err,rows)=>{
        
                if(err)
                {       
                    var callbackMsg = {
                        eventName:"UserRegister",
                        data:"fail"
                    }

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Register[0]" + toJsonStr);
                }
                else
                {                   
                    var callbackMsg = {
                        eventName:"UserRegister",
                        data:"success"
                    }

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Register[1]" + toJsonStr);

                }

            });

        }



        //===============================================

        else if(toJsonObj.eventName == "CreateRoom")//CreateRoom
        {
            //============= Find room with roomName from Client =========
            var isFoundRoom = false;
            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    isFoundRoom = true;
                    break;
                }
            }
            //===========================================================

            if(isFoundRoom == true)// Found room
            {
                //Can't create room because roomName is exist.
                //========== Send callback message to Client ============

                //ws.send("CreateRoomFail"); 

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("client create room fail.");
            }
            else
            {
                //============ Create room and Add to roomList ==========
                var newRoom = {
                    roomName: toJsonObj.data,
                    wsList: []
                }

                newRoom.wsList.push(ws);

                roomList.push(newRoom);
                //=======================================================

                //========== Send callback message to Client ============

                //ws.send("CreateRoomSuccess");

                //I need to send roomName into client too. I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:toJsonObj.data
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================
                console.log("client create room success.");
                console.log("Room Name : [" + toJsonObj.data + "]");
            }

            //console.log("client request CreateRoom ["+toJsonObj.data+"]");
            
        }
        else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
        {
            //============= Home work ================
            // Implementation JoinRoom event when have request from client.

            console.log("client request JoinRoom [" + toJsonObj.data + "].");            

            //============= Find room with roomName from Client =========
            var isFoundRoom = false;
            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    isFoundRoom = true;
                    break;
                }
            }
            //===========================================================

            if(isFoundRoom == true)// Found room
            {
                //Can join room because roomName is exist.                            

                roomList[i].wsList.push(ws);

                //========== Send callback message to Client ============

                //I need to send roomName into client too. I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"JoinRoom",
                    data:toJsonObj.data
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("client join room success.");
            }
            else
            {
                //============ Can't join room because roomName isn't exist ==========

                //========== Send callback message to Client ============

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"JoinRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================
                console.log("client join room fail, Room isn't exist.");
            }
            
            // do something here ===============================================================================================================

            //================= Hint =================
            //roomList[i].wsList.push(ws);

            //console.log("client request JoinRoom");
            //========================================
        }
        else if(toJsonObj.eventName == "LeaveRoom")//LeaveRoom
        {
            //============ Find client in room for remove client out of room ================
            var isLeaveSuccess = false;//Set false to default.
            for(var i = 0; i < roomList.length; i++)//Loop in roomList
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                {
                    if(ws == roomList[i].wsList[j])//If founded client.
                    {
                        roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                        if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                        {
                            roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                        }
                        isLeaveSuccess = true;
                        break;
                    }
                }
            }
            //===============================================================================

            if(isLeaveSuccess)
            {
                //========== Send callback message to Client ============

                //ws.send("LeaveRoomSuccess");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room success");
            }
            else
            {
                //========== Send callback message to Client ============

                //ws.send("LeaveRoomFail");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room fail");
            }
        }

        else if(toJsonObj.eventName == "SendMessage")//SendMessage
        {
            console.log("SendMessage is Working");
            
            console.log(toJsonObj);
 /*
            var toJsonMessage = { 
                eventName:"",
                data:""
            }

            toJsonMessage = JSON.parse(toJsonObj.data);

            console.log(toJsonMessage);
 */
            Boardcast(ws, data); 
            //Boardcast(ws, toJsonMessage.message);                               
        }           
    });


    ws.on("close", ()=>{

        console.log("client disconnected.");

        //============ Find client in room for remove client out of room ================
        for(var i = 0; i < roomList.length; i++)//Loop in roomList
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
            {
                if(ws == roomList[i].wsList[j])//If founded client.
                {
                    roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                    if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                    {
                        roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                    }
                    break;
                }
            }
        }
        //===============================================================================
    });
});

function Boardcast(ws, message)
            {
                console.log("Boardcast is Working");
                var selectRoomIndex = -1;

                for(var i = 0; i < roomList.length; i++)
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)
                    {
                        if(ws == roomList[i].wsList[j])
                        {
                            selectRoomIndex = i;
                            break;
                        }
                    }
                }

                for(var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
                {
                    /*
                    var callbackMsg = {
                        eventName :"SendMessage",
                        data:message
                    }
                    */
                   
                    roomList[selectRoomIndex].wsList[i].send(message);

                    console.log("========message========");
                    console.log(message);

                    //var toJsonStr = roomList[selectRoomIndex].wsList[i].send(JSON.stringify(callbackMsg));

                    //console.log("========message========");
                    //console.log(toJsonStr);

                }
            }

