using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using riddles_WebApp.Model;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;

namespace riddles_WebApp.Pages
{
    public class SignupModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM_DB;Integrated Security=True;";
        public string message= "";
        Users users = new Users();

        public void OnGet()
        {
        }

        private string encryptPasswd(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string base64Hash = Convert.ToBase64String(hashBytes);
                return base64Hash;
            }
        }

        public void OnPost() {
            try {

                users.Username = Request.Form["username"];
                users.Email = Request.Form["email"];
                String encrypted = encryptPasswd(Request.Form["password"]);
                users.Password = encrypted;
                users.userType = Request.Form["userType"];

            }catch (Exception ex)
            {
                message = "Hello!: " + ex.Message;
            }
            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "INSERT INTO users(username,email,password,userType)VALUES(@username, @email, @password, @userType)";
                try {

                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", users.Username);
                        cmd.Parameters.AddWithValue("@email", users.Email);
                        cmd.Parameters.AddWithValue("@password", users.Password);
                        cmd.Parameters.AddWithValue("@userType", users.userType);

                        int rowAffected = cmd.ExecuteNonQuery();

                        if (rowAffected > 0)
                        {
                            message = "New User Created";
                            users = new Users();
                        }
                        else
                        {
                            message = "User Not Created";
                        }

                    }

                    con.Close();
                }
                catch (Exception ex)
                {
                    message="Hello!: "+ ex.Message;
                }
            }
        }
    }
}
