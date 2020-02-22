"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/studentHub").build();

connection.on("ReceiveCheckIn", function (studentID, inCLass, timeLastCheckedIn, timeLastCheckedOut, attendaceStatus) {
    console.log("OK RECIEVED CHECK IN");
    var studentTable = document.getElementById("studentTable");
    console.log("WE HAVE TABLE " + studentTable.rows[1].cells[0].inner);
    for (var i = 0, studentRow; studentRow = studentTable.rows[i]; i++) {
        if (studentRow.cells[1].innerHTML == studentID) {
            studentRow.cells[3].childNodes[0].checked = inCLass;
            studentRow.cells[4].innerHTML = timeLastCheckedIn;
            studentRow.cells[5].innerHTML = timeLastCheckedOut;
            studentRow.cells[6].innerHTML = attendaceStatus;
        }
    }
});

connection.start()
    .then(function () {
        console.log('connection started');
    });