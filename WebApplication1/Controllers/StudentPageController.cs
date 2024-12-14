using Microsoft.AspNetCore.Mvc;
using cumulative_assingment_1.Models;

namespace cumulative_assingment_1.Controllers
{
    public class StudentPageController : Controller
    {

        // API is responsible for gathering the information from the Database and MVC is responsible for giving an HTTP response
        // as a web page that shows the information written in the View

        // In practice, both the StudentAPI and StudentPage controllers
        // should rely on a unified "Service", with an explicit interface

        private readonly StudentAPIController _api;

        public StudentPageController(StudentAPIController api)
        {
            _api = api;
        }


        /// <summary>
        /// When we click on the Students button in Navugation Bar, it returns the web page displaying all the teachers in the Database student
        /// </summary>
        /// <returns>
        /// List of all Students in the Database student
        /// </returns>
        /// <example>
        /// GET : api/StudentPage/List  ->  Gives the list of all Students in the Database student
        /// </example>

        public IActionResult ListStudent()
        {
            List<Student> Students = _api.ListStudents();
            return View(Students);
        }



        /// <summary>
        /// When we Select one Student from the list, it returns the web page displaying the information of the SelectedStudent from the database student
        /// </summary>
        /// <returns>
        /// Information of the SelectedStudent from the database student
        /// </returns>
        /// <example>
        /// GET :api/StudentPage/Show/{id}  ->  Gives the information of the SelectedStudent
        /// </example>
        /// 
        public IActionResult ShowStudent(int id)
        {
            Student SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }



        // GET : StudentPage/NewStudent
        [HttpGet]
        public IActionResult NewStudent(int id)
        {
            return View();
        }



        // POST: StudentPage/CreateStudent
        [HttpPost]
        public IActionResult CreateStudent(Student NewStudent)
        {
            int StudentId = _api.AddStudent(NewStudent);

            // redirects to "Show" action on "Student" cotroller with id parameter supplied
            return RedirectToAction("ShowStudent", new { id = StudentId });
        }




        // GET : StudentPage/DeleteConfirmStudent/{id}
        [HttpGet]
        public IActionResult DeleteConfirmStudent(int id)
        {
            Student SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }





        // POST: StudentPage/DeleteStudent/{id}
        [HttpPost]
        public IActionResult DeleteStudent(int id)
        {
            int StudentId = _api.DeleteStudent(id);
            // redirects to list action
            return RedirectToAction("ListStudent");
        }



        // GET : StudentPage/EditStudent/{id}
        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            Student SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }



        // POST: StudentPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string StudentFName, string StudentLName, string StudentNumber, DateTime StudentEnrolmentDate)
        {
            Student UpdatedStudent = new Student();
            UpdatedStudent.StudentFName = StudentFName;
            UpdatedStudent.StudentLName = StudentLName;
            UpdatedStudent.StudentNumber = StudentNumber;
            UpdatedStudent.StudentEnrolmentDate = StudentEnrolmentDate;


            // not doing anything with the response
            _api.UpdateStudent(id, UpdatedStudent);

            // redirects to show teacher
            return RedirectToAction("ShowStudent", new { id });
        }


    }
}
