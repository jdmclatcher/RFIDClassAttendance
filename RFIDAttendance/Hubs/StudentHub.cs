/*
 * Jonathan McLatcher, Harrison Boyd
 * RFID Class Attendance
 * 2020
 */
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace RFIDAttendance.Hubs
{
    public class StudentHub : Hub
    {
        public async Task SendCheckIn(string studentID, bool InCLass, DateTime? TimeLastCheckedIn, DateTime? TimeLastCheckedOut, string AttendaceStatus)
        {
            await Clients.All.SendAsync("ReceiveCheckIn", studentID, InCLass, TimeLastCheckedIn, TimeLastCheckedOut, AttendaceStatus);
        }
    }
}
