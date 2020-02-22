/*
 * Jonathan McLatcher, Harrison Boyd
 * RFID Class Attendance
 * 2020
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
        private readonly StudentDbContext _context;
        private readonly IHubContext<StudentHub> _hubContext;

        public StudentsController(StudentDbContext context, IHubContext<StudentHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            ViewBag.CurrentFilter = "All Periods";
            // TODO: update period view based on time
            //CheckPeriod();
        }



        private IActionResult CheckPeriod()
        {
            string index = "";
            // change period based on time


            return FilterPeriod(index);
        }

        public IActionResult FilterPeriod(string link)
        {
            var filteredStudents = from s in _context.Student select s;
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
            var students = from s in _context.Student select s;
            Student studentObject = null;
            foreach (Student s in students)
            {
                System.Diagnostics.Debug.WriteLine(s);
                if (s.StudentID == studentID/*Convert.ToInt64(search)*/)
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
            _context.SaveChanges(); // update DB
            return studentObject;
        }

        // new day handling
        public IActionResult NewDay()
        {
            var students = from s in _context.Student select s;
            foreach (Student s in students)
            {
                s.AttendaceStatus = "ABSENT";
                s.InClass = false;
                s.TimeLastCheckedIn = null;
                s.TimeLastCheckedOut = null;
            }
            _context.SaveChanges(); // update DB
            return RedirectToAction("Index");
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string search)
        {
            // sorting with hyperlinks - append on top of period
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";

            var students = from s in _context.Student select s;

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

            var student = await _context.Student
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
                _context.Add(student);
                await _context.SaveChangesAsync();
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

            var student = await _context.Student.FindAsync(id);
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
                    _context.Update(student);
                    await _context.SaveChangesAsync();
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

            var student = await _context.Student
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
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: API/api
        [HttpPost]
        [Route("api/API")]
        public HttpResponseMessage Rfid([FromBody] JArray body)
        {
            long studentID = body.First().SelectToken("studentID").Value<long>();
            System.Diagnostics.Debug.WriteLine((int)studentID);
            if (_context.Student.Any(e => e.StudentID == studentID))
            {
                Student s = CheckIn(studentID);
                _hubContext.Clients.All.SendAsync("ReceiveCheckIn", studentID, s.InClass, s.TimeLastCheckedIn.Value.ToShortTimeString(), s.TimeLastCheckedOut.Value.ToShortTimeString(), s.AttendaceStatus);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
