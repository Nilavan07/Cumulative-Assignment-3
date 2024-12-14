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

        // This is dependancy injection
        private readonly SchoolDbContext _schoolcontext;
        public TeacherAPIController(SchoolDbContext schoolcontext)
        {
            _schoolcontext = schoolcontext;
        }


        /// <summary>
        /// When we click on Teachers in Navigation bar on Home page, We are directed to a webpage that lists all teachers in the database school
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"TeacherFname":"Manik", "TeacherLName":"Bansal"},{"TeacherFname":"Asha", "TeacherLName":"Bansal"},.............]
        /// GET api/Teacher/ListTeachers -> [{"TeacherFname":"Apurva", "TeacherLName":"Gupta"},{"TeacherFname":"Himani", "TeacherLName":"Garg"},.............]
        /// </example>
        /// <returns>
        /// A list all the teachers in the database school
        /// </returns>


        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            // Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher>();

            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
            {

                // Opening the connection
                Connection.Open();


                // Establishing a new query for our database 
                MySqlCommand Command = Connection.CreateCommand();


                // Writing the SQL Query we want to give to database to access information
                Command.CommandText = "select * from teachers";


                // Storing the Result Set query in a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {

                    // While loop is used to loop through each row in the ResultSet 
                    while (ResultSet.Read())
                    {

                        // Accessing the information of Teacher using the Column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string EmployeeNumber = ResultSet["employeenumber"].ToString();
                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);


                        // Assigning short names for properties of the Teacher
                        Teacher EachTeacher = new Teacher()
                        {
                            TeacherId = Id,
                            TeacherFName = FirstName,
                            TeacherLName = LastName,
                            TeacherHireDate = TeacherHireDate,
                            EmployeeNumber = EmployeeNumber,
                            TeacherSalary = Salary
                        };


                        // Adding all the values of properties of EachTeacher in Teachers List
                        Teachers.Add(EachTeacher);

                    }
                }
            }


            //Return the final list of Teachers 
            return Teachers;
        }


        /// <summary>
        /// When we select one teacher , it returns information of the selected Teacher in the database by their ID 
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/3 -> {"TeacherId":3,"TeacherFname":"Sam","TeacherLName":"Cooper"}
        /// </example>
        /// <returns>
        /// Information about the Teacher selected
        /// </returns>



        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {

            // Created an object SelectedTeacher using Teacher definition defined as Class in Models
            Teacher SelectedTeacher = new Teacher();


            // 'using' keyword is used that will close the connection by itself after executing the code given inside
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
            {

                // Opening the Connection
                Connection.Open();

                // Establishing a new query for our database 
                MySqlCommand Command = Connection.CreateCommand();


                // @id is replaced with a 'sanitized'(masked) id so that id can be referenced
                // without revealing the actual @id
                Command.CommandText = "select * from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);


                // Storing the Result Set query in a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {

                    // While loop is used to loop through each row in the ResultSet 
                    while (ResultSet.Read())
                    {

                        // Accessing the information of Teacher using the Column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string EmployeeNumber = ResultSet["employeenumber"].ToString();
                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);


                        // Accessing the information of the properties of Teacher and then assigning it to the short names 
                        // created above for all properties of the Teacher
                        SelectedTeacher.TeacherId = Id;
                        SelectedTeacher.TeacherFName = FirstName;
                        SelectedTeacher.TeacherLName = LastName;
                        SelectedTeacher.TeacherHireDate = TeacherHireDate;
                        SelectedTeacher.EmployeeNumber = EmployeeNumber;
                        SelectedTeacher.TeacherSalary = Salary;

                    }
                }
            }


            //Return the Information of the SelectedTeacher
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
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
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
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
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
            using (MySqlConnection Connection = _schoolcontext.AccessDatabase())
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