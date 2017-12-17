using IrrigationApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace IrrigationApi.Service
{
    public class DAL
    {
        private readonly string connString = "Server=tcp:irrigationserver.database.windows.net,1433;Initial Catalog=Irrigation;Persist Security Info=False;User ID=aprodaniuc;Password=P@ssw0rd123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<string> GetWeatherInfo()
        {
            List<string> infos = new List<string>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = string.Format("Select * from WeatherInfo");
                    cmd.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string info = reader["info"].ToString();
                        infos.Add(info);
                    }
                    conn.Close();
                    return infos;
                }
            }
        }

        public void CreateUser(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("AddUser", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var userId = Guid.NewGuid();
                    SqlParameter uid = new SqlParameter("Id", userId);
                    cmd.Parameters.Add(uid);

                    SqlParameter uemail = new SqlParameter("Email", email);
                    cmd.Parameters.Add(uemail);

                    var upwd = EncryptPassword(password);
                    SqlParameter pwd = new SqlParameter("Pwd", upwd);
                    cmd.Parameters.Add(pwd);

                    conn.Open();
                    var result = cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public UserSetUpModel CheckPwd(string email, string pwd)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("CheckPwd", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter uemail = new SqlParameter("Email", email);
                    cmd.Parameters.Add(uemail);

                    SqlParameter upwdp = new SqlParameter("Pwd", pwd);
                    cmd.Parameters.Add(upwdp);

                    conn.Open();
                    var result = cmd.ExecuteReader();
                    string id = string.Empty;
                    bool isSetUp = false;
                    while (result.Read())
                    {
                        id = result["Id"].ToString();
                        isSetUp = bool.Parse(result["IsSetUp"].ToString());
                    }

                    conn.Close();
                    return new UserSetUpModel() { Id = id, IsSetUp = isSetUp };
                }
            }
        }

        public List<AreaModel> GetAreas(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAreasForUser", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter uId = new SqlParameter("UserId", userId);
                    cmd.Parameters.Add(uId);

                    conn.Open();
                    var result = cmd.ExecuteReader();
                    List<AreaModel> areas = new List<AreaModel>();
                    while (result.Read())
                    {
                        areas.Add(new AreaModel()
                        {
                            Id = result["Id"].ToString(),
                            UserId = result["UserId"].ToString(),
                            Name = result["Name"].ToString()
                        });
                    }

                    conn.Close();
                    return areas;
                }
            }
        }

        public async Task AddSensor(SensorModel sensor)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("AddSensor", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter Id = new SqlParameter("Id", sensor.Id);
                    cmd.Parameters.Add(Id);

                    SqlParameter AreaId = new SqlParameter("AreaId", sensor.AreaId);
                    cmd.Parameters.Add(AreaId);

                    SqlParameter Lat = new SqlParameter("Lat", sensor.Lat.ToString());
                    cmd.Parameters.Add(Lat);

                    SqlParameter Lng = new SqlParameter("Lng", sensor.Lng.ToString());
                    cmd.Parameters.Add(Lng);

                    conn.Open();

                    var result = cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public async Task AddArea(AreaModel area)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("AddArea", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter Id = new SqlParameter("Id", area.Id);
                    cmd.Parameters.Add(Id);

                    SqlParameter UserId = new SqlParameter("UserId", area.UserId);
                    cmd.Parameters.Add(UserId);

                    SqlParameter Name = new SqlParameter("Name", area.Name);
                    cmd.Parameters.Add(Name);

                    conn.Open();

                    var result = cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public string EncryptPassword(string password)
        {
            SHA1 hash = SHA1.Create();
            var data = System.Text.Encoding.UTF8.GetBytes(password);
            var result = hash.ComputeHash(data);
            return System.Text.Encoding.UTF8.GetString(result);
        }
    }
}