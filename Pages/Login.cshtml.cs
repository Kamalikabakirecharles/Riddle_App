using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using riddles_WebApp.Model;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;


namespace riddles_WebApp.Pages
{
    public class LoginModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM_DB;Integrated Security=True;";
        public string message = "";
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

        public IActionResult OnPost()
        {
            try
            {
                users.Email = Request.Form["email"];
                string encrypted = encryptPasswd(Request.Form["password"]);
                users.Password = encrypted;
            }
            catch (Exception ex)
            {
                message = "Hello!: " + ex.Message;
            }

            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "SELECT user_id, userType FROM Users WHERE email=@email AND password=@password";

                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@email", users.Email);
                        cmd.Parameters.AddWithValue("@password", users.Password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string userType = reader.GetString(1);

                                HttpContext.Session.SetInt32("UserId", userId);

                                if (userType == "Guests")
                                {
                                    return RedirectToPage("/Guest");
                                }
                                else if (userType == "Players")
                                {
                                    return RedirectToPage("/Players");

                                }else if(userType == "Riddlers")
                                {
                                    return RedirectToPage("/Dash");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "Error: " + ex.Message;
                }
            }

            return Page();
        }

    }
}
