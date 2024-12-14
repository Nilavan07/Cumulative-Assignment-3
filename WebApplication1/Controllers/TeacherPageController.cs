using Microsoft.AspNetCore.Mvc;
using cumulative_assingment_1.Models;
using cumulative_assingment_1.Controllers;

namespace cumulative_assingment_1.Controllers
{
    public class TeacherPageController : Controller
    {

        // currently relying on the API to retrieve author information
        // this is a simplified example. In practice, both the TeacherAPI and TeacherPage controllers
        // should rely on a unified "Service", with an explicit interface
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        //GET : TeacherPage/List
        public IActionResult List()
        {
            List<Teacher> Teachers = _api.ListTeachers();
            return View(Teachers);
        }

        //GET : TeacherPage/Show/{id}
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }
 // GET : TeacherPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View();
        }



        // POST: TeacherPage/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            int TeacherId = _api.AddTeacher(NewTeacher);

            // redirects to "Show" action on "Teacher" cotroller with id parameter supplied
            return RedirectToAction("Show", new { id = TeacherId });
        }






        // GET : TeacherPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }








        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int TeacherId = _api.DeleteTeacher(id);
            // redirects to list action
            return RedirectToAction("List");
        }


    
        // POST: TeacherPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string TeacherFName, string TeacherLName, decimal TeacherSalary, string EmployeeNumber, DateTime TeacherHireDate)
        {
            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.TeacherFName = TeacherFName;
            UpdatedTeacher.TeacherLName = TeacherLName;
            UpdatedTeacher.TeacherSalary = TeacherSalary;
            UpdatedTeacher.EmployeeNumber = EmployeeNumber;
            UpdatedTeacher.TeacherHireDate = TeacherHireDate;


            // not doing anything with the response
            _api.UpdateTeacher(id, UpdatedTeacher);


            // redirects to show teacher
            return RedirectToAction("Show", new { id });
        }



    }

}

