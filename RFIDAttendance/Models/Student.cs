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

using System;
using System.ComponentModel.DataAnnotations;

namespace RFIDAttendance.Models
{
    public class Student
    {
        // unique ID needed for database
        public int Id { get; set; }

        [Required(ErrorMessage = "A Student ID is required."), Range(1000000000, 4000000000, ErrorMessage = "Please enter a valid Student ID.")]
        public long StudentID { get; set; }

        [Required(ErrorMessage = "A {0} is required."), StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "A {0} is required.")]
        public string Period { get; set; }

        // status that marks if currently in class or not
        [Display(Name = "In Class?")]
        public bool InClass { get; set; } = false;

        [DataType(DataType.Time), Display(Name = "Last Checked In")]
        public DateTime? TimeLastCheckedIn { get; set; }

        [DataType(DataType.Time), Display(Name = "Last Checked Out")]
        public DateTime? TimeLastCheckedOut { get; set; }

        [Display(Name = "Attendance Status")]
        public string AttendaceStatus { get; set; } = "ABSENT";
    }
}
