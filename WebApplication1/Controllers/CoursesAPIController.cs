using cumulative_assingment_1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace cumulative_assingment_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        public class CoursesAPIController : ControllerBase
        {

            // This is dependancy injection
            private readonly SchoolDbContext _schoolcontext;
            public CoursesAPIController(SchoolDbContext schoolcontext)
            {
                _schoolcontext = schoolcontext;
            }


            /// <summary>
            /// When we click on Courses in Navigation bar on Home page, We are directed to a webpage that lists all courses in the database school
            /// </summary>
            /// <example>
            /// GET api/Course/ListCourses -> [{"Coursename":"Digital Design","Coursename":"Database Development",.............]
            /// <returns>
            /// A list all the courses in the database school
            /// </returns>


            [HttpGet]
            [Route(template: "ListCourses")]
            public List<Course> ListCourses()
            {
                // Create an empty list of Courses
                List<Course> Courses = new List<Course>();

                // 'using' keyword is used that will close the connection by itself after executing the code given inside
                using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
                {

                    // Opening the connection
                    Connection.Open();


                    // Establishing a new query for our database 
                    MySqlCommand Command = Connection.CreateCommand();


                    // Writing the SQL Query we want to give to database to access information
                    Command.CommandText = "select * from courses";


                    // Storing the Result Set query in a variable
                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {

                        // While loop is used to loop through each row in the ResultSet 
                        while (ResultSet.Read())
                        {

                            // Accessing the information of Course using the Column name as an index
                            int C_Id = Convert.ToInt32(ResultSet["courseid"]);
                            string Code = ResultSet["coursecode"].ToString();
                            int Id = Convert.ToInt32(ResultSet["courseid"]);
                            DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]).Date;
                            DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]).Date;
                            string Name = ResultSet["coursename"].ToString();


                        // Assigning short names for properties of the Course
                        Course EachCourse = new Course()
                        {
                            CourseId = C_Id,
                            CourseCode = Code,
                            CourseStartDate = StartDate,
                            CourseFinishDate = FinishDate,
                            CourseName = Name
                        };


                            // Adding all the values of properties of EachCourse in Courses List
                            Courses.Add(EachCourse);

                        }
                    }
                }


                //Return the final list of Courses 
                return Courses;
            }


            /// <summary>
            /// When we select one course , it returns information of the selected Course in the database by their ID 
            /// </summary>
            /// <example>
            /// GET api/Course/FindCourse/3 -> {"CourseId":3,"Coursename":Web Development, .......}
            /// </example>
            /// <returns>
            /// Information about the Course selected
            /// </returns>



            [HttpGet]
            [Route(template: "FindCourse/{id}")]
            public Course FindCourse(int id)
            {

                // Created an object SelectedCourse using Course definition defined as Class in Models
                Course SelectedCourse = new Course();


                // 'using' keyword is used that will close the connection by itself after executing the code given inside
                using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
                {

                    // Opening the Connection
                    Connection.Open();

                    // Establishing a new query for our database 
                    MySqlCommand Command = Connection.CreateCommand();


                    // @id is replaced with a 'sanitized'(masked) id so that id can be referenced
                    // without revealing the actual @id
                    Command.CommandText = "select * from courses where courseid = @id;";
                    Command.Parameters.AddWithValue("@id", id);


                    // Storing the Result Set query in a variable
                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {

                        // While loop is used to loop through each row in the ResultSet 
                        while (ResultSet.Read())
                        {

                        // Accessing the information of Course using the Column name as an index
                        int C_Id = Convert.ToInt32(ResultSet["courseid"]);
                        string Code = ResultSet["coursecode"].ToString();
                        int Id = Convert.ToInt32(ResultSet["courseid"]);
                        DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]).Date;
                        DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]).Date;
                        string Name = ResultSet["coursename"].ToString();


                        // Accessing the information of the properties of Course and then assigning it to the short names 
                        // created above for all properties of the Course
                            SelectedCourse.CourseId = C_Id;
                            SelectedCourse.CourseCode = Code;
                            SelectedCourse.CourseId = Id;
                            SelectedCourse.CourseStartDate = StartDate;
                            SelectedCourse.CourseFinishDate= FinishDate;
                            SelectedCourse.CourseName = Name;

                        };
                    }
                }


                //Return the Information of the SelectedCourse
                return SelectedCourse;
            }



        /// <summary>
        /// The method adds a new course to the database by inserting a record into the courses table and returns the ID of the inserted course
        /// </summary>
        /// <param name="CourseData"> An object containing the details of the course to be added, including first name, last name, employee number, salary, and hire date </param>
        /// <returns>
        /// The ID of the newly inserted course record
        /// </returns>
        /// <example> 
        /// POST: api/CourseAPI/AddCourse -> 11
        /// assuming that 11th record is added
        /// </example>


[HttpPost(template: "AddCourse")]
        public int AddCourse([FromBody] Course CourseData)
        {
            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
            {
                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database
                MySqlCommand Command = Connection.CreateCommand();

                // It contains the SQL query to insert a new course into the courses table            
                Command.CommandText = "INSERT INTO courses (coursecode, teacherid, startdate, finishdate, coursename) VALUES (@coursecode, @teacherid, @startdate, @finishdate, @coursename)";

                Command.Parameters.AddWithValue("@coursecode", CourseData.CourseCode);
                Command.Parameters.AddWithValue("@teacherid", CourseData.TeacherId);
                Command.Parameters.AddWithValue("@startdate", CourseData.CourseStartDate);
                Command.Parameters.AddWithValue("@finishdate", CourseData.CourseFinishDate);
                Command.Parameters.AddWithValue("@coursename", CourseData.CourseName);

                // It runs the query against the database and the new record is inserted
                Command.ExecuteNonQuery();

                // It fetches the ID of the newly inserted course record and converts it to an integer to be returned
                return Convert.ToInt32(Command.LastInsertedId);


            }

        }



        /// <summary>
        /// The method deletes a course from the database using the course's ID provided in the request URL. It returns the number of rows affected.
        /// </summary>
        /// <param name="CourseId"> The unique ID of the course to be deleted </param>
        /// <returns>
        /// The number of rows affected by the DELETE operation
        /// </returns>
        /// <example>
        /// DELETE: api/CourseAPI/DeleteCourse/11 -> 1
        /// </example>

        [HttpDelete(template: "DeleteCourse/{CourseId}")]

        public int DeleteCourse(int CourseId)
        {
            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
            {
                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database
                MySqlCommand Command = Connection.CreateCommand();

                // It contains the SQL query to delete a record from the courses table based on the course's ID
                Command.CommandText = "DELETE FROM courses WHERE courseid=@id";
                Command.Parameters.AddWithValue("@id", CourseId);

                // It runs the DELETE query and the number of affected rows is returned.
                return Command.ExecuteNonQuery();

            }

        }




    
        /// <summary>
        /// Updates an Course in the database. Data is Course object, request query contains ID
        /// </summary>
        /// <param name="CourseData">Course Object</param>
        /// <param name="CourseId">The Course ID primary key</param>
        /// <example>
        /// PUT: api/CoursesAPI/UpdateCourse/4
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// { "CourseCode":"http5121", "CourseStartDate":2024-12-05,"CourseFinishDate":2024-12-04,"CourseName":"WebDev"} -> 
        /// {"CourseId":4,"CourseCode":"http5121", "CourseStartDate":2024-12-05 12:00:00 AM,"CourseFinishDate":2024-12-04 12:00:00 AM,"CourseName":"WebDev" }
        /// </example>
        /// <returns>
        /// The updated Course object
        /// </returns>
        
        
        [HttpPut(template: "UpdateCourse/{CourseId}")]
        public Course UpdateCourse(int CourseId, [FromBody] Course CourseData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
            {
                Connection.Open();

                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query
                Command.CommandText = "update courses set coursecode=@coursecode, startdate = @startdate, finishdate = @finishdate, coursename = @coursename, teacherid = @teacherid where courseid=@id";
                Command.Parameters.AddWithValue("@coursecode", CourseData.CourseCode);
                Command.Parameters.AddWithValue("@startdate", CourseData.CourseStartDate);
                Command.Parameters.AddWithValue("@finishdate", CourseData.CourseFinishDate);
                Command.Parameters.AddWithValue("@coursename", CourseData.CourseName);
                Command.Parameters.AddWithValue("@teacherid", CourseData.TeacherId);


                Command.Parameters.AddWithValue("@id", CourseId);

                Command.ExecuteNonQuery();



            }

            return FindCourse(CourseId);
        }





    }


}



