const { json } = require('express');

const sqlite = require('sqlite3').verbose();

let db = new sqlite.Database('./db.chat.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

    console.log('Connected to Database.');

    var id = "Test8888";
    var password = "88888";
    var name = "Text08"

    var sqlSelect = "SELECT * FROM UserData WHERE UserID='"+id+"' AND Password='"+password+"' ";
    var sqlInsert = "INSERT INTO UserData (UserID, Password, Name, Money) VALUES ('"+id+"', '"+password+"', '"+name+"', '100')";
    var sqlUpdate = "UPDATE UserData SET Money='500' WHERE UserID='"+id+"'";


    db.all("SELECT Money FROM UserData WHERE UserID='"+id+"'", (err,rows)=>{

        if(err){

            console.log(err);
        }
        else
        {
            if(rows.length > 0)
            {
                var currentMoney = rows[0].Money;
                currentMoney += 100;

                db.all("UPDATE UserData SET Money='"+currentMoney+"' WHRER UserID='"+id+"'", (err,rows)=>{

                    if(err)
                    {
                        console.log(err);
                    }

                    else
                    {
                        var result = {
                            status:true,
                            Money:currentMoney                 
                        }

                        console.log(JSON.stringify(result));
                    }

                });            
            }               
            else
            {
                console.log("UserID not Found");
            }
            
        }
        
        console.log(rows);
    });

});