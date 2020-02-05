using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDAttendance.Models
{
    public class Student
    {
        // mandatory unique ID needed for database
        public int Id { get; set; }
        [Required(ErrorMessage = "A {0} is required")]
        public long StudentID { get; set; }

        [Required(ErrorMessage = "A {0} is required"), StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
        // status boolean that marks if currently in class or not
        public bool InClass { get; set; }
        [DataType(DataType.Date)]
        public string TimeLastCheckedIn { get; set; }
        [DataType(DataType.Date)]
        public string TimeLastCheckedOut { get; set; }
        // PRESENT - if checking in by late bell
        // TARDY - will be changed from ABSENT to tardy when 
        // ABSENT - will be the default when late bell rings and havent checked in
        public string AttendaceStatus { get; set; }
    }
}
