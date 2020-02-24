/*
Project: RFID Class Attendance
Original Authors: Jonathan McLatcher, Harrison Boyd
Description: This class is the hub that manages the communication between client and server for live updating of the table


Copyright (c) 2020, Jonathan McLatcher, Harrison Boyd

This file is part of RFID Class Attendance.

    RFID Class Attendance is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    RFID Class Attendance is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with RFID Class Attendance.  If not, see <https://www.gnu.org/licenses/>.
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
