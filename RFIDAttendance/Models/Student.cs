using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDAttendance.Models
{
    public class Student
    {
        // unique ID needed for database
        public int Id { get; set; }
        [Required(ErrorMessage = "A Student ID is required"), Range(1000000000, 4000000000, ErrorMessage = "Please enter a valid Student ID")]
        public long StudentID { get; set; }
        [Required(ErrorMessage = "A {0} is required"), StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
        // status that marks if currently in class or not
        [Display(Name = "In Class?")]
        public bool InClass { get; set; }
        [DataType(DataType.Time), Display(Name = "Last Checked In")]
        public string TimeLastCheckedIn { get; set; }
        [DataType(DataType.Time), Display(Name = "Last Checked Out")]
        public string TimeLastCheckedOut { get; set; }
        // PRESENT - if checking in by late bell
        // TARDY - will be changed from ABSENT to tardy when 
        // ABSENT - will be the default when late bell rings and havent checked in
        [Display(Name = "Attendance Status")]
        public string AttendaceStatus { get; set; }
    }
}
