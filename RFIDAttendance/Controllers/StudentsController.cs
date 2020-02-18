/*
 * Jonathan McLatcher
 * RFID Class Attendance
 * 2020
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFIDAttendance.Data;
using RFIDAttendance.Models;

namespace RFIDAttendance.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentDbContext _context;
        
        public StudentsController(StudentDbContext context)
        {
            _context = context;
            ViewBag.CurrentFilter = "All Periods";
            // TODO: update period view based on time
            // CheckPeriod();
        }

        //private IActionResult CheckPeriod()
        //{
        //    string index = "";

        //    // change period based on time

        //    return FilterPeriod(index);
        //}

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
        public IActionResult CheckIn(long studentID)
        {
            var students = from s in _context.Student select s;
            foreach(Student s in students)
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
                    } else
                    {
                        // check if has checked in before
                        if (!s.TimeLastCheckedIn.HasValue)
                        {
                            // mark tardy if after late bell and before early bell of next period
                            if((Convert.ToInt32(DateTime.Now.Hour) > 8) && (Convert.ToInt32(DateTime.Now.Minute) > 4))
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
                }
            }
            _context.SaveChanges(); // update Db
            return RedirectToAction("Index");
        }

        // new day handling
        public IActionResult NewDay()
        {
            var students = from s in _context.Student select s;
            foreach(Student s in students)
            {
                s.AttendaceStatus = "ABSENT";
                s.InClass = false;
                s.TimeLastCheckedIn = null;
                s.TimeLastCheckedOut = null;
            }
            _context.SaveChanges(); // update Db
            return RedirectToAction("Index");
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string search)
        {
            // sorting with hyperlinks - append on top of period
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";

            var students = from s in _context.Student select s;

            // TOFIX: searching won't work immediately after filtering (link still active)
            // search by name or student ID
            if (!String.IsNullOrEmpty(search))
            {
                students = students.Where(s => s.Name.Contains(search)
                                       || s.StudentID.ToString().Contains(search));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;
                case "status":
                    students = students.OrderBy(s => s.AttendaceStatus);
                    break;
                case "status_desc":
                    students = students.OrderByDescending(s => s.AttendaceStatus);
                    break;
                default:  // name ascending 
                    students = students.OrderBy(s => s.Name);
                    break;
            }
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

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
