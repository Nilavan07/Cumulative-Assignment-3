using Microsoft.AspNetCore.Mvc;
using cumulative_assingment_1.Models;

namespace cumulative_assingment_1.Controllers
{
    public class CoursesPageController : Controller
    {

        // API is responsible for gathering the information from the Database and MVC is responsible for giving an HTTP response
        // as a web page that shows the information written in the View

        // In practice, both the CoursesAPI and CoursesPage controllers
        // should rely on a unified "Service", with an explicit interface

        private readonly CoursesAPIController _api;

        public CoursesPageController(CoursesAPIController api)
        {
            _api = api;
        }


        /// <summary>
        /// When we click on the Courses button in Navugation Bar, it returns the web page displaying all the teachers in the Database school
        /// </summary>
        /// <returns>
        /// List of all Courses in the Database school
        /// </returns>
        /// <example>
        /// GET : api/CoursesPage/List  ->  Gives the list of all Courses in the Database school
        /// </example>

        public IActionResult ListCourses()
        {
            List<Course> Courses = _api.ListCourses();
            return View(Courses);
        }



        /// <summary>
        /// When we Select one Course from the list, it returns the web page displaying the information of the SelectedCourse from the database school
        /// </summary>
        /// <returns>
        /// Information of the SelectedCourse from the database school
        /// </returns>
        /// <example>
        /// GET :api/CoursesPage/Show/{id}  ->  Gives the information of the SelectedCourse
        /// </example>
        /// 
        public IActionResult ShowCourses(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);
            return View(SelectedCourse);
        }



        // GET : CoursePage/NewCourse
        [HttpGet]
        public IActionResult NewCourse(int id)
        {
            return View();
        }



        // POST: CoursePage/CreateCourse
        [HttpPost]
        public IActionResult CreateCourse(Course NewCourse)
        {
            int CourseId = _api.AddCourse(NewCourse);

            // redirects to "Show" action on "Course" cotroller with id parameter supplied
            return RedirectToAction("ShowCourses", new { id = CourseId });
        }






        // GET : CoursePage/DeleteConfirmCourse/{id}
        [HttpGet]
        public IActionResult DeleteConfirmCourse(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);
            return View(SelectedCourse);
        }








        // POST: CoursePage/DeleteCourse/{id}
        [HttpPost]
        public IActionResult DeleteCourse(int id)
        {
            int CourseId = _api.DeleteCourse(id);
            // redirects to list action
            return RedirectToAction("ListCourses");
        }


     // POST: CoursesPage/Update/{id}
     
        [HttpPost]
        public IActionResult Update(int id, string CourseCode, DateTime CourseStartDate, DateTime CourseFinishDate, string CourseName, int TeacherId)
        {
            Course UpdatedCourse = new Course();
            UpdatedCourse.CourseCode = CourseCode;
            UpdatedCourse.CourseStartDate = CourseStartDate;
            UpdatedCourse.CourseFinishDate = CourseFinishDate;
            UpdatedCourse.CourseName = CourseName;
            UpdatedCourse.TeacherId = TeacherId;


            // not doing anything with the response
            _api.UpdateCourse(id, UpdatedCourse);

            // redirects to show teacher
            return RedirectToAction("ShowCourses", new { id });
        }


    }
}

