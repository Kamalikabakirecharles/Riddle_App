using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using riddles_WebApp.Model;
using System.Data.SqlClient;

namespace riddles_WebApp.Pages
{
    public class riddleModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM_DB;Integrated Security=True;";
        public string message = "";
        Riddles riddles = new Riddles();

        public IActionResult OnGet()
        {
            var redirectToHomeResult = RedirectToHomePageIfNotAuthenticated();
            if (redirectToHomeResult != null)
            {
                // If RedirectToHomePageIfNotAuthenticated returns a result, return that result
                return redirectToHomeResult;
            }
            return Page();
        }
        public void OnPost() {
        
            try
            {

                int? userId = HttpContext.Session.GetInt32("UserId");
                riddles.user_id = userId.Value;
                riddles.riddle_text = Request.Form["text"];

            }
            catch (Exception ex)
            {
                message = "Hello! "+ex.Message;
            }
            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "INSERT INTO Riddles(user_id, riddle_text) VALUES(@user_id, @text)";

                try
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@user_id", riddles.user_id);
                        cmd.Parameters.AddWithValue("@text", riddles.riddle_text);
                        

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Riddle Posted";
                            riddles = new Riddles();
                        }
                        else
                        {
                            message = "Riddle Failed";
                            
                        }
                    }
                    con.Close();

                }
                catch (Exception ex)
                {
                   
                        message = "Problem: " + ex.Message;

                }
            }

        }
        public IActionResult RedirectToHomePageIfNotAuthenticated()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {

                return RedirectToPage("/Index");
            }
            return null;
        }
    }
}
