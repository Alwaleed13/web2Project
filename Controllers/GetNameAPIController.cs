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

        [HttpGet]
        public IEnumerable<UserAccount> Get()
        {
            List<UserAccount> li = new List<UserAccount>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("web2projectdbserverConnection");
            SqlConnection conn = new SqlConnection(conStr);
            string sql;
            sql = "select * from customer";
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
