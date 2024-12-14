using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cumulative_assingment_1.Models;
using System;
using MySql.Data.MySqlClient;



namespace cumulative_assingment_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {

        // This is a dependancy injection
        private readonly SchoolDbContext _studentcontext;
        public StudentAPIController(SchoolDbContext studentcontext)
        {
            _studentcontext = studentcontext;
        }


        /// <summary>
        /// When we click on Students in Navigation bar on Home page, We are directed to a webpage that lists all students in the database student
        /// </summary>
        /// <example>
        /// GET api/Student/ListStudents -> [{"StudentFname":"Mohit", "StudentLName":"Bansal"},{"StudentFname":"Darshan", "StudentLName":"Malhotra"},.............]
        /// GET api/Student/ListStudents -> [{"StudentFname":"Aakriti", "StudentLName":"Garg"},{"StudentFname":"Himanshi", "StudentLName":"Goyal"},.............]
        /// </example>
        /// <returns>
        /// A list all the students in the database student
        /// </returns>


        [HttpGet]
        [Route(template: "ListStudents")]
        public List<Student> ListStudents()
        {
            // Create an empty list of Students
            List<Student> Students = new List<Student>();

            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _studentcontext.AccessDatabase())
            {

                // Opening the connection
                Connection.Open();


                // Establishing a new query for our database 
                MySqlCommand Command = Connection.CreateCommand();


                // Writing the SQL Query we want to give to database to access information
                Command.CommandText = "select * from students";


                // Storing the Result Set query in a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {

                    // While loop is used to loop through each row in the ResultSet 
                    while (ResultSet.Read())
                    {

                        // Accessing the information of Student using the Column name as an index
                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string FirstName = ResultSet["studentfname"].ToString();
                        string LastName = ResultSet["studentlname"].ToString();
                        string S_Number = ResultSet["studentnumber"].ToString();
                        DateTime EnrolmentDate = Convert.ToDateTime(ResultSet["enroldate"]);                       


                        // Assigning short names for properties of the Student
                        Student EachStudent = new Student()
                        {
                            StudentId = Id,
                            StudentFName = FirstName,
                            StudentLName = LastName,
                            StudentEnrolmentDate = EnrolmentDate,
                            StudentNumber = S_Number,
                        };


                        // Adding all the values of properties of EachStudent in Students List
                        Students.Add(EachStudent);

                    }
                }
            }


            //Return the final list of Students 
            return Students;
        }


        /// <summary>
        /// When we select one student , it returns information of the selected Student in the database by their ID 
        /// </summary>
        /// <example>
        /// GET api/Student/FindStudent/3 -> {"StudentId":3,"StudentFname":"Sam","StudentLName":"Cooper", .......}
        /// </example>
        /// <returns>
        /// Information about the Student selected
        /// </returns>



        [HttpGet]
        [Route(template: "FindStudent/{id}")]
        public Student FindStudent(int id)
        {

            // Created an object SelectedStudent using Student definition defined as Class in Models
            Student SelectedStudent = new Student();


            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _studentcontext.AccessDatabase())
            {

                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database 
                MySqlCommand Command = Connection.CreateCommand();


                // @id is replaced with a 'sanitized'(masked) id so that id can be referenced
                // without revealing the actual @id
                Command.CommandText = "select * from students where studentid=@id";
                Command.Parameters.AddWithValue("@id", id);


                // Storing the Result Set query in a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {

                    // While loop is used to loop through each row in the ResultSet 
                    while (ResultSet.Read())
                    {

                        // Accessing the information of Student using the Column name as an index
                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string FirstName = ResultSet["studentfname"].ToString();
                        string LastName = ResultSet["studentlname"].ToString();
                        string S_Number = ResultSet["studentnumber"].ToString();
                        DateTime EnrolmentDate = Convert.ToDateTime(ResultSet["enroldate"]);


                        // Accessing the information of the properties of Student and then assigning it to the short names 
                        // created above for all properties of the Student
                        SelectedStudent.StudentId = Id;
                        SelectedStudent.StudentFName = FirstName;
                        SelectedStudent.StudentLName = LastName;
                        SelectedStudent.StudentEnrolmentDate = EnrolmentDate;
                        SelectedStudent.StudentNumber = S_Number;

                    }
                }
            }


            //Return the Information of the SelectedStudent
            return SelectedStudent;
        }
    /// <summary>
        /// The method adds a new student to the database by inserting a record into the students table and returns the ID of the inserted student
        /// </summary>
        /// <param name="StudentData"> An object containing the details of the student to be added, including first name, last name, employee number, salary, and hire date </param>
        /// <returns>
        /// The ID of the newly inserted student record
        /// </returns>
        /// <example> 
        /// POST: api/StudentAPI/AddStudent -> 11
        /// assuming that 11th record is added
        /// </example>



        [HttpPost(template: "AddStudent")]
        public int AddStudent([FromBody] Student StudentData)
        {
            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _studentcontext.AccessDatabase())
            {
                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database
                MySqlCommand Command = Connection.CreateCommand();

                // It contains the SQL query to insert a new student into the students table            
                Command.CommandText = "INSERT INTO students (studentfname, studentlname, studentnumber, enroldate) VALUES (@studentfname, @studentlname, @studentnumber, @enroldate)";

                Command.Parameters.AddWithValue("@studentfname", StudentData.StudentFName);
                Command.Parameters.AddWithValue("@studentlname", StudentData.StudentLName);
                Command.Parameters.AddWithValue("@studentnumber", StudentData.StudentNumber);
                Command.Parameters.AddWithValue("@enroldate", StudentData.StudentEnrolmentDate);

                // It runs the query against the database and the new record is inserted
                Command.ExecuteNonQuery();

                // It fetches the ID of the newly inserted student record and converts it to an integer to be returned
                return Convert.ToInt32(Command.LastInsertedId);


            }

        }



        /// <summary>
        /// The method deletes a student from the database using the student's ID provided in the request URL. It returns the number of rows affected.
        /// </summary>
        /// <param name="StudentId"> The unique ID of the student to be deleted </param>
        /// <returns>
        /// The number of rows affected by the DELETE operation
        /// </returns>
        /// <example>
        /// DELETE: api/StudentAPI/DeleteStudent/11 -> 1
        /// </example>

        [HttpDelete(template: "DeleteStudent/{StudentId}")]

        public int DeleteStudent(int StudentId)
        {
            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _studentcontext.AccessDatabase())
            {
                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database
                MySqlCommand Command = Connection.CreateCommand();

                // It contains the SQL query to delete a record from the students table based on the student's ID
                Command.CommandText = "DELETE FROM students WHERE studentid=@id";
                Command.Parameters.AddWithValue("@id", StudentId);

                // It runs the DELETE query and the number of affected rows is returned.
                return Command.ExecuteNonQuery();

            }

        }
     /// <summary>
        /// Updates an Student in the database. Data is Student object, request query contains ID
        /// </summary>
        /// <param name="StudentData">Student Object</param>
        /// <param name="StudentId">The Student ID primary key</param>
        /// <example>
        /// PUT: api/StudentAPI/UpdateStudent/14
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// { "StudentFname":"Himani", "StudentLname":"Bansal", "StudentNumber" :1221, StudentEnrolmentDate":2024-12-05} -> 
        /// {"StudentId":14, "StudentFname":"Himani", "StudentLname":"Bansal", "StudentNumber" :1221, StudentEnrolmentDate":2024-12-05}
        /// </example>
        /// <returns>
        /// The updated Student object
        /// </returns>
        [HttpPut(template: "UpdateStudent/{StudentId}")]
        public Student UpdateStudent(int StudentId, [FromBody] Student StudentData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _studentcontext.AccessDatabase())
            {
                Connection.Open();

                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query
                Command.CommandText = "update students set studentfname=@studentfname, studentlname=@studentlname, studentnumber = @studentnumber, enroldate = @enroldate where studentid=@id";
                Command.Parameters.AddWithValue("@studentfname", StudentData.StudentFName);
                Command.Parameters.AddWithValue("@studentlname", StudentData.StudentLName);
                Command.Parameters.AddWithValue("@studentnumber", StudentData.StudentNumber);
                Command.Parameters.AddWithValue("@enroldate", StudentData.StudentEnrolmentDate);

                Command.Parameters.AddWithValue("@id", StudentId);

                Command.ExecuteNonQuery();



            }

            return FindStudent(StudentId);
        }

    }


}


