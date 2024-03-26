using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using riddles_WebApp.Model;
using System.Data.SqlClient;

namespace riddles_WebApp.Pages
{
    public class PlayersModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM_DB;Integrated Security=True;";
        public string message = "";
        Riddles riddles = new Riddles();
        Answers answers = new Answers();
        public List<Riddles> RiddleList = new List<Riddles>();
        public List<Answers> AnswersList = new List<Answers>();

        public IActionResult OnGet()
        {
            getRiddles();
            getAnswers();
            var redirectToHomeResult = RedirectToHomePageIfNotAuthenticated();
            if (redirectToHomeResult != null)
            {
                // If RedirectToHomePageIfNotAuthenticated returns a result, return that result
                return redirectToHomeResult;
            }
            return Page();
        }

        public void OnPost()
        {
            try
            {

                int? userId = HttpContext.Session.GetInt32("UserId");
                answers.user_id = userId.Value;
                answers.answer_text = Request.Form["answer"];
                answers.riddle_id = int.Parse(Request.Form["riddle_id"]);


            }
            catch (Exception ex)
            {
                message = "Hello! " + ex.Message;
            }
            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string query = "INSERT INTO Answers(riddle_id, user_id, answer_text) VALUES(@riddle_id, @user_id, @answer)";

                try
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@riddle_id", answers.riddle_id);
                        cmd.Parameters.AddWithValue("@user_id", answers.user_id);
                        cmd.Parameters.AddWithValue("@answer", answers.answer_text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Answer Posted";
                            riddles = new Riddles();
                        }
                        else
                        {
                            message = "Answer Failed";

                        }
                    }
                    con.Close();

                }
                catch (Exception ex)
                {

                    message = "Problem: " + ex.Message;

                }
            }
            getRiddles();
            getAnswers();
        }

        public void getRiddles()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    // Redirect to login or handle unauthenticated user
                    message = "no id:";
                }
                riddles.user_id = userId.Value;

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM Riddles";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Riddles currentRiddle = new Riddles();  // Use a different variable name to avoid re-declaration

                                currentRiddle.riddle_id = reader.GetInt32(0);
                                currentRiddle.user_id = reader.GetInt32(1);
                                currentRiddle.riddle_text = reader.GetString(2);
                                currentRiddle.agree_count = reader.GetInt32(3);
                                currentRiddle.disagree_count = reader.GetInt32(4);
                                currentRiddle.created_at = reader.GetDateTime(5);

                                RiddleList.Add(currentRiddle);
                            }
                        }
                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                message = "Problem: " + ex.Message;
            }
        }
        public void getAnswers()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                answers.user_id = userId;

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM Answers WHERE user_id=@userId";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", answers.user_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Answers answers = new Answers();

                                answers.answer_id = reader.GetInt32(0);
                                answers.riddle_id = reader.GetInt32(1);
                                answers.user_id = reader.GetInt32(2);
                                answers.answer_text = reader.GetString(3);
                                answers.is_correct = reader.GetBoolean(4);
                                answers.created_at = reader.GetDateTime(5);

                                AnswersList.Add(answers);
                            }
                        }
                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                message = "Problem: " + ex.Message;
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
