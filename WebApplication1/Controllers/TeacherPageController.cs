using Microsoft.AspNetCore.Mvc;
using cumulative_assingment_1.Models;

namespace cumulative_assingment_1.Controllers
{
    public class TeacherPageController : Controller
    {

        // API is responsible for gathering the information from the Database and MVC is responsible for giving an HTTP response
        // as a web page that shows the information written in the View

        // In practice, both the TeacherAPI and TeacherPage controllers
        // should rely on a unified "Service", with an explicit interface

        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }


        /// <summary>
        /// When we click on the Teachers button in Navugation Bar, it returns the web page displaying all the teachers in the Database school
        /// </summary>
        /// <returns>
        /// List of all Teachers in the Database school
        /// </returns>
        /// <example>
        /// GET : api/TeacherPage/List  ->  Gives the list of all Teachers in the Database school
        /// </example>
        
        public IActionResult List()
        {
            List<Teacher> Teachers = _api.ListTeachers();
            return View(Teachers);
        }



        /// <summary>
        /// When we Select one Teacher from the list, it returns the web page displaying the information of the SelectedTeacher from the database school
        /// </summary>
        /// <returns>
        /// Information of the SelectedTeacher from the database school
        /// </returns>
        /// <example>
        /// GET :api/TeacherPage/Show/{id}  ->  Gives the information of the SelectedTeacher
        /// </example>
        /// 
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



        // GET : TeacherPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
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
