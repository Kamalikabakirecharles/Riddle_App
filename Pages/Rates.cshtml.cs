
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using riddles_WebApp.Model;
using System.Data.SqlClient;
using System.Linq; // Add this namespace for LINQ

namespace riddles_WebApp.Pages
{
    public class RatesModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM_DB;Integrated Security=True;";
        public string message = "";
        public Riddles HighestAgreeRiddle { get; set; }
        public Riddles HighestDisagreeRiddle { get; set; }
        public List<Riddles> RiddleList = new List<Riddles>();
        public List<Answers> UserCorrectAnswers { get; set; } = new List<Answers>();
        public List<Answers> userWrongAnswers { get; set; } = new List<Answers>();
        public List<Users> Users { get; set; } = new List<Users>();
        public int TotalCorrectAnswers { get; set; }
        public int TotalIncorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }



        public void OnGet()
        {
            getRiddles();
            FindHighestAgreeAndDisagreeRiddles();
            GetUserCorrectAnswers();
            GetUserWrongAnswers();
            // Set Total Questions
            TotalQuestions = RiddleList.Count;
            // Calculate Overall Totals
            TotalCorrectAnswers = UserCorrectAnswers.Count;
            TotalIncorrectAnswers = userWrongAnswers.Count;
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
                Riddles riddles = new Riddles();
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
                                Riddles currentRiddle = new Riddles();

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

        public void FindHighestAgreeAndDisagreeRiddles()
        {
            HighestAgreeRiddle = RiddleList.OrderByDescending(r => r.agree_count).FirstOrDefault();
            HighestDisagreeRiddle = RiddleList.OrderByDescending(r => r.disagree_count).FirstOrDefault();
        }

        public void GetUserCorrectAnswers()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    using (SqlConnection con = new SqlConnection(stgcon))
                    {
                        string query = "SELECT * FROM Answers WHERE user_id = @userId AND is_correct = 1";
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId.Value);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Answers userCorrectAnswer = new Answers();

                                    userCorrectAnswer.answer_id = reader.GetInt32(0);
                                    userCorrectAnswer.riddle_id = reader.GetInt32(1);
                                    userCorrectAnswer.user_id = reader.GetInt32(2);
                                    userCorrectAnswer.answer_text = reader.GetString(3);
                                    userCorrectAnswer.is_correct = reader.GetBoolean(4);
                                    userCorrectAnswer.created_at = reader.GetDateTime(5);
                                    userCorrectAnswer.AgreeCount = reader.GetInt32(6);
                                    userCorrectAnswer.DisagreeCount = reader.GetInt32(7);

                                    UserCorrectAnswers.Add(userCorrectAnswer);
                                }
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Problem: " + ex.Message;
            }
        }
        public void GetUserWrongAnswers()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    using (SqlConnection con = new SqlConnection(stgcon))
                    {
                        string query = "SELECT * FROM Answers WHERE user_id = @userId AND is_correct = 0";
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId.Value);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Answers userWrongAnswer = new Answers();

                                    userWrongAnswer.answer_id = reader.GetInt32(0);
                                    userWrongAnswer.riddle_id = reader.GetInt32(1);
                                    userWrongAnswer.user_id = reader.GetInt32(2);
                                    userWrongAnswer.answer_text = reader.GetString(3);
                                    userWrongAnswer.is_correct = reader.GetBoolean(4);
                                    userWrongAnswer.created_at = reader.GetDateTime(5);
                                    userWrongAnswer.AgreeCount = reader.GetInt32(6);
                                    userWrongAnswer.DisagreeCount = reader.GetInt32(7);

                                    userWrongAnswers.Add(userWrongAnswer);
                                }
                            }
                        }
                        con.Close();
                    }
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