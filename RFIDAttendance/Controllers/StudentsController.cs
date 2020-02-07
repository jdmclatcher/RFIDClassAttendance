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

        // Button event handling
        public IActionResult CheckIn()
        {
            long testID = 2000112143;
            // code to run when button pressed
            // TODO optimize this
            var students = from s in _context.Student select s;
            foreach(Student s in students)
            {
                if(s.StudentID == testID/*Convert.ToInt64(search)*/)
                {
                    // TODO: update of tardy, present, absent with bell schedule
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
                            // mark tardy if after late bell
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
            // sorting with hyperlinks
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.IDSortParm = sortOrder == "ID" ? "ID_desc" : "ID";
            ViewBag.StatusSortParm = sortOrder == "present" ? "present_desc" : "present";
            var students = from s in _context.Student
                           select s;
            // search by name or student ID
            if (!String.IsNullOrEmpty(search))
            {
                students = students.Where(s => s.Name.Contains(search)
                                       || s.StudentID.ToString().Contains(search));
            }
            // sort based on input from Index
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;
                case "ID":
                    students = students.OrderBy(s => s.StudentID);
                    break;
                case "ID_desc":
                    students = students.OrderByDescending(s => s.StudentID);
                    break;
                case "present":
                    students = students.OrderBy(s => s.AttendaceStatus);
                    break;
                case "present_desc":
                    students = students.OrderByDescending(s => s.AttendaceStatus);
                    break;
                default:
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
        public async Task<IActionResult> Create([Bind("Id,StudentID,Name,InClass,TimeLastCheckedIn,TimeLastCheckedOut,AttendaceStatus")] Student student)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentID,Name,InClass,TimeLastCheckedIn,TimeLastCheckedOut,AttendaceStatus")] Student student)
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
