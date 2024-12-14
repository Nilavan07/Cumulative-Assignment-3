using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cumulative_assingment_1.Models;
using System;
using MySql.Data.MySqlClient;



namespace cumulative_assingment_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Teachers in the system
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"TeacherId":1,"TeacherFname":"Brian", "TeacherLName":"Smith"},{"TeacherId":2,"TeacherFname":"Jillian", "TeacherLName":"Montgomery"},..]
        /// </example>
        /// <returns>
        /// A list of author objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            // Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from teachers";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string EmployeeNumber = ResultSet["employeenumber"].ToString();
                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                        //short form for setting all properties while creating the object
                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = Id,
                            TeacherFName = FirstName,
                            TeacherLName = LastName,
                            TeacherHireDate = TeacherHireDate,
                            EmployeeNumber = EmployeeNumber,
                            TeacherSalary = Salary
                         };

                        Teachers.Add(CurrentTeacher);

                    }
                }
            }


            //Return the final list of authors
            return Teachers;
        }


        /// <summary>
        /// Returns an author in the database by their ID
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/3 -> {"TeacherId":3,"TeacherFname":"Sam","TeacherLName":"Cooper"}
        /// </example>
        /// <returns>
        /// A matching author object by its ID. Empty object if Teacher not found
        /// </returns>
        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {

            //Empty Teacher
            Teacher SelectedTeacher = new Teacher();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a 'sanitized' id
                Command.CommandText = "select * from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string EmployeeNumber = ResultSet["employeenumber"].ToString();                       
                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                        SelectedTeacher.TeacherId = Id;
                        SelectedTeacher.TeacherFName = FirstName;
                        SelectedTeacher.TeacherLName = LastName;
                        SelectedTeacher.TeacherHireDate = TeacherHireDate;
                        SelectedTeacher.EmployeeNumber = EmployeeNumber;
                        SelectedTeacher.TeacherSalary = Salary;

                    }
                }
            }

            //Return the final list of author names
            return SelectedTeacher;
        }
        /// <summary>
        /// The method adds a new teacher to the database by inserting a record into the teachers table and returns the ID of the inserted teacher
        /// </summary>
        /// <param name="TeacherData"> An object containing the details of the teacher to be added, including first name, last name, employee number, salary, and hire date </param>
        /// <returns>
        /// The ID of the newly inserted teacher record
        /// </returns>
        /// <example> 
        /// POST: api/TeacherAPI/AddTeacher -> 11
        /// assuming that 11th record is added
        /// </example>



        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database
                MySqlCommand Command = Connection.CreateCommand();

                // It contains the SQL query to insert a new teacher into the teachers table            
                Command.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, salary, hiredate) VALUES (@teacherfname, @teacherlname, @employeenumber, @salary, @hiredate)";
                
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.TeacherHireDate);
                Command.Parameters.AddWithValue("@salary", TeacherData.TeacherSalary);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);

                // It runs the query against the database and the new record is inserted
                Command.ExecuteNonQuery();

                // It fetches the ID of the newly inserted teacher record and converts it to an integer to be returned
                return Convert.ToInt32(Command.LastInsertedId);


            }

        }



        /// <summary>
        /// The method deletes a teacher from the database using the teacher's ID provided in the request URL. It returns the number of rows affected.
        /// </summary>
        /// <param name="TeacherId"> The unique ID of the teacher to be deleted </param>
        /// <returns>
        /// The number of rows affected by the DELETE operation
        /// </returns>
        /// <example>
        /// DELETE: api/TeacherAPI/DeleteTeacher/11 -> 1
        /// </example>

        [HttpDelete(template: "DeleteTeacher/{TeacherId}")]

        public int DeleteTeacher(int TeacherId)
        {
            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database
                MySqlCommand Command = Connection.CreateCommand();

                // It contains the SQL query to delete a record from the teachers table based on the teacher's ID
                Command.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
                Command.Parameters.AddWithValue("@id", TeacherId);

                // It runs the DELETE query and the number of affected rows is returned.
                return Command.ExecuteNonQuery();

            }

        }
    
   
        /// <summary>
        /// Updates an Teacher in the database. Data is Teacher object, request query contains ID
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <param name="TeacherId">The Teacher ID primary key</param>
        /// <example>
        /// PUT: api/TeacherAPI/UpdateTeacher/4
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// { "TeacherFname":"Himani", "TeacherLname":"Bansal", "TeacherSalary":12000, "TeacherHireDate":2024-12-05 12:00:00 AM,"EmployeeNumber":1002} -> 
        /// {"TeacherId":15, "TeacherFname":"Himani", "TeacherLname":"Bansal", "TeacherSalary":12000, "TeacherHireDate":2024-12-05 12:00:00 AM,"EmployeeNumber":1002}
        /// </example>
        /// <returns>
        /// The updated Teacher object
        /// </returns>
        [HttpPut(template: "UpdateTeacher/{TeacherId}")]
        public Teacher UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query
                Command.CommandText = "update teachers set teacherfname=@teacherfname, teacherlname=@teacherlname, salary=@salary, employeenumber=@employeenumber, hiredate=@hiredate where teacherid=@id";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.TeacherHireDate);
                Command.Parameters.AddWithValue("@salary", TeacherData.TeacherSalary);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);

                Command.Parameters.AddWithValue("@id", TeacherId);

                Command.ExecuteNonQuery();


            }

            return FindTeacher(TeacherId);
        }


    }
}
