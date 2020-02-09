using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        }

        // Check in button event handling
        public IActionResult CheckIn()
        {
            long testID = 2000112143;
            // runs when button pressed
            var students = from s in _context.Student select s;
            foreach(Student s in students)
            {
                if(s.StudentID == testID/*Convert.ToInt64(search)*/)
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
            // period filtering
            //ViewBag.PeriodOneFilter = "1";
            //ViewBag.PeriodTwoFilter = "2";
            //ViewBag.PeriodThreeFilter = "3";
            //ViewBag.PeriodFourFilter = "4";
            //ViewBag.PeriodFiveFilter = "5";
            //ViewBag.PeriodSixFilter = "6";
            //ViewBag.PeriodAllFilter = "";

            // sorting with hyperlinks - append on top of period
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";

            var students = from s in _context.Student
                           select s;

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
            //if (!String.IsNullOrEmpty(sortOrder))
            //{
            //    students = students.Where(s => s.Period.Contains(sortOrder));

            //    //if (sortOrder.Contains("name_desc"))
            //    //{
            //    //    students = students.OrderByDescending(s => s.Name);
            //    //}
            //    //else if (sortOrder.Contains("status"))
            //    //{
            //    //    students = students.OrderBy(s => s.AttendaceStatus);
            //    //}
            //    //else if (sortOrder.Contains("status_desc"))
            //    //{
            //    //    students = students.OrderByDescending(s => s.AttendaceStatus);
            //    //}
            //    //else
            //    //{
            //    //    students = students.OrderBy(s => s.Name);
            //    //}
            //}
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
