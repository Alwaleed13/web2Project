using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using web2Project.Models;

namespace web2Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetNameAPIController : ControllerBase
    {

        [HttpGet("{ro}")]
        public IEnumerable<UserAccount> Get(string ro)
        {
            List<UserAccount> li = new List<UserAccount>();
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Project;Integrated Security=True;Trust Server Certificate=True");

            string sql;
            sql = "select * from users_account where Role = '" + ro + "' ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                li.Add(new UserAccount
                {
                    Name = (string)reader["Name"],
                    Role = (string)reader["Role"],
                });
            }
            reader.Close();
            conn.Close();

            return li;

        }

    }
}
