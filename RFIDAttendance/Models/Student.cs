/*
 * Jonathan McLatcher, Harrison Boyd
 * RFID Class Attendance
 * 2020
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
