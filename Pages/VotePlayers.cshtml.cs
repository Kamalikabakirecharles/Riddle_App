using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using riddles_WebApp.Model;
using System.Data.SqlClient;

namespace riddles_WebApp.Pages
{
    public class AnswerPlayersModel : PageModel
    {
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=FINAL_EXAM_DB;Integrated Security=True;";
        public string message = "";
        Riddles riddles = new Riddles();
        Answers answers = new Answers();
        public List<Riddles> RiddleList = new List<Riddles>();
        public List<Answers> AnswersList = new List<Answers>();
        public List<Answers> VotesList = new List<Answers>();

        public IActionResult OnGet()
        {
            var redirectToHomeResult = RedirectToHomePageIfNotAuthenticated();
            if (redirectToHomeResult != null)
            {
                // If RedirectToHomePageIfNotAuthenticated returns a result, return that result
                return redirectToHomeResult;
            }
            getRiddles();
            getAnswers();
            getVotes();

            return Page();
        }

        public void OnPost()
        {
            try
            {
                string type = Request.Form["vote_type"];
                int? userId = HttpContext.Session.GetInt32("UserId");
                int answerId = int.Parse(Request.Form["answer_id"]);

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    con.Open();

                    // Update AgreeCount or DisagreeCount in the database directly
                    string updateQuery = "";
                    if (type.Equals("Agree"))
                    {
                        updateQuery = "UPDATE c SET AgreeCount = AgreeCount + 1 WHERE answer_id = @answerId";
                    }
                    else if (type.Equals("Disagree"))
                    {
                        updateQuery = "UPDATE Answers SET DisagreeCount = DisagreeCount + 1 WHERE answer_id = @answerId";
                    }

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@answerId", answerId);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Vote Posted";
                        }
                        else
                        {
                            message = "Vote Failed";
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                message = "Hello! " + ex.Message;
            }

            // Refresh data after voting
            getRiddles();
            getAnswers();
            getVotes();
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

                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT * FROM Answers";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

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
                                answers.AgreeCount = reader.GetInt32(6);
                                answers.DisagreeCount = reader.GetInt32(7);

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
        public void getVotes()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(stgcon))
                {
                    string query = "SELECT answer_id, AgreeCount, DisagreeCount FROM Answers";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Answers vote = new Answers();

                                vote.answer_id = reader.GetInt32(0);
                                vote.AgreeCount = reader.GetInt32(1);
                                vote.DisagreeCount = reader.GetInt32(2);

                                // Find the existing vote in VotesList and update it
                                var existingVote = VotesList.FirstOrDefault(v => v.answer_id == vote.answer_id);

                                if (existingVote != null)
                                {
                                    existingVote.AgreeCount = vote.AgreeCount;
                                    existingVote.DisagreeCount = vote.DisagreeCount;
                                }
                                else
                                {
                                    VotesList.Add(vote);
                                }
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
