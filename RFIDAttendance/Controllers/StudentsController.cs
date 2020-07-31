/*
Project: RFID Class Attendance
Original Authors: Jonathan McLatcher, Harrison Boyd
Description: This class controlls the students page


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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RFIDAttendance.Data;
using RFIDAttendance.Hubs;
using RFIDAttendance.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RFIDAttendance.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentDbContext StudentDatabaseContext;
        private readonly IHubContext<StudentHub> AttendanceHubContext;

        public StudentsController(StudentDbContext studentDatabaseContext, IHubContext<StudentHub> attendanceHubContext)
        {
            StudentDatabaseContext = studentDatabaseContext;
            AttendanceHubContext = attendanceHubContext;
            ViewBag.CurrentFilter = "All Periods";
        }

        // TODO: implementation of this
        // returns filter action based on current period
        public IActionResult CheckPeriod()
        {
            System.Diagnostics.Debug.WriteLine("CheckPeriod called");
            DateTime time = DateTime.Now;
            // change period based on time
            if ((time.Hour == 8 && time.Minute >= 15) || (time.Hour == 9 && time.Minute <= 20))
            {
                return FilterPeriod("1");
            }
            else if ((time.Hour == 9 && time.Minute >= 21) || (time.Hour == 10 && time.Minute <= 15))
            {
                return FilterPeriod("2");
            }
            else if ((time.Hour == 10 && time.Minute >= 50) || (time.Hour == 11 && time.Minute <= 42))
            {
                return FilterPeriod("3");
            }
            else if ((time.Hour == 11 && time.Minute >= 43) || (time.Hour == 12 && time.Minute <= 39))
            {
                return FilterPeriod("4");
            }
            else if ((time.Hour == 1 && time.Minute >= 36) || (time.Hour == 2 && time.Minute <= 32))
            {
                System.Diagnostics.Debug.WriteLine("It's 5th Period.");
                return FilterPeriod("5");
            }
            else if ((time.Hour == 2 && time.Minute >= 33) || (time.Hour == 3 && time.Minute <= 30))
            {
                return FilterPeriod("6");
            }
            else
            {
                return FilterPeriod("");
            }
        }

        public IActionResult FilterPeriod(string link)
        {
            System.Diagnostics.Debug.WriteLine("FiterPeriod called");
            var filteredStudents = from s in StudentDatabaseContext.Student select s;
            // select only the correct period from db
            switch (link)
            {
                case "1":
                    filteredStudents = filteredStudents.Where(s => s.Period.Contains("1"));
                    ViewBag.CurrentFilter = "Period 1";
                    break;
                case "2":
                    filteredStudents = filteredStudents.Where(s => s.Period.Contains("2"));
                    ViewBag.CurrentFilter = "Period 2";
                    break;
                case "3":
                    filteredStudents = filteredStudents.Where(s => s.Period.Contains("3"));
                    ViewBag.CurrentFilter = "Period 3";
                    break;
                case "4":
                    filteredStudents = filteredStudents.Where(s => s.Period.Contains("4"));
                    ViewBag.CurrentFilter = "Period 4";
                    break;
                case "5":
                    filteredStudents = filteredStudents.Where(s => s.Period.Contains("5"));
                    ViewBag.CurrentFilter = "Period 5";
                    break;
                case "6":
                    filteredStudents = filteredStudents.Where(s => s.Period.Contains("6"));
                    ViewBag.CurrentFilter = "Period 6";
                    break;
                default:
                    ViewBag.CurrentFilter = "All Periods";
                    break;
            }
            return View("Index", filteredStudents);
        }

        // Check in button event handling
        public Student CheckIn(long studentID)
        {
            var students = from s in StudentDatabaseContext.Student select s;
            Student studentObject = null;
            foreach (Student s in students)
            {
                if (s.StudentID == studentID)
                {
                    // TODO: update of tardy, present, absent with bell schedule -- BASED ON STUDENT PERIOD
                    // PRESENT - if checking in by late bell
                    // TARDY - will be changed from ABSENT to tardy when 
                    // ABSENT - will be the default when late bell rings and havent checked in
                    if (s.InClass)
                    {
                        // check out student
                        s.TimeLastCheckedOut = DateTime.Now;
                        s.InClass = false;
                    }
                    else
                    {
                        // check if has checked in before
                        if (!s.TimeLastCheckedIn.HasValue)
                        {
                            // mark tardy if after late bell and before early bell of next period
                            if ((Convert.ToInt32(DateTime.Now.Hour) > 8) && (Convert.ToInt32(DateTime.Now.Minute) > 4))
                            {
                                s.AttendaceStatus = "TARDY";
                            }
                            else
                            {
                                s.AttendaceStatus = "PRESENT";
                            }
                        }
                        // check in student
                        s.TimeLastCheckedIn = DateTime.Now;
                        s.InClass = true;
                    }
                    studentObject = s;
                    break;
                }
            }
            if (studentObject == null)
            {
                throw new System.InvalidOperationException("Logfile cannot be read-only");
            }
            StudentDatabaseContext.SaveChanges(); // update DB
            return studentObject;
        }

        // new day handling
        public IActionResult NewDay()
        {
            var students = from s in StudentDatabaseContext.Student select s;
            foreach (Student s in students)
            {
                s.AttendaceStatus = "ABSENT";
                s.InClass = false;
                s.TimeLastCheckedIn = null;
                s.TimeLastCheckedOut = null;
            }
            StudentDatabaseContext.SaveChanges(); // update DB
            return RedirectToAction("Index");
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string search)
        {
            // sorting with hyperlinks - append on top of period
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";

            var students = from s in StudentDatabaseContext.Student select s;

            // TOFIX: searching won't work immediately after filtering (link still active)
            // search by name or student ID
            if (!string.IsNullOrEmpty(search))
            {
                students = students.Where(s => s.Name.Contains(search)
                                       || s.StudentID.ToString().Contains(search));
            }

            students = sortOrder switch
            {
                "name_desc" => students.OrderByDescending(s => s.Name),
                "status" => students.OrderBy(s => s.AttendaceStatus),
                "status_desc" => students.OrderByDescending(s => s.AttendaceStatus),
                // name ascending 
                _ => students.OrderBy(s => s.Name),
            };
            return View(await students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await StudentDatabaseContext.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentID,Name,Period,InClass,TimeLastCheckedIn,TimeLastCheckedOut,AttendaceStatus")] Student student)
        {
            if (ModelState.IsValid)
            {
                StudentDatabaseContext.Add(student);
                await StudentDatabaseContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await StudentDatabaseContext.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentID,Name,Period,InClass,TimeLastCheckedIn,TimeLastCheckedOut,AttendaceStatus")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    StudentDatabaseContext.Update(student);
                    await StudentDatabaseContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await StudentDatabaseContext.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await StudentDatabaseContext.Student.FindAsync(id);
            StudentDatabaseContext.Student.Remove(student);
            await StudentDatabaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: API/api
        [HttpPost]
        [Route("api/API")]
        public HttpResponseMessage RFIDScanReciever([FromBody] JArray postBody)
        {
            long studentID = postBody.First().SelectToken("studentID").Value<long>();
            if (StudentDatabaseContext.Student.Any(e => e.StudentID == studentID))
            {
                Student student = CheckIn(studentID);
                AttendanceHubContext.Clients.All.SendAsync("ReceiveCheckIn", studentID, student.InClass, student.TimeLastCheckedIn.Value.ToShortTimeString(), student.TimeLastCheckedOut.Value.ToShortTimeString(), student.AttendaceStatus);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        private bool StudentExists(int id)
        {
            return StudentDatabaseContext.Student.Any(e => e.Id == id);
        }
    }
}
