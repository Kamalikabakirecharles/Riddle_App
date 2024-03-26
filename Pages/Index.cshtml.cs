using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace riddles_WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        private string stgcon = "Data Source=DESKTOP-8UTAP68\\SQLEXPRESS;Initial Catalog=riddlesDB;Integrated Security=True;";
        public string message = "";

        public void OnGet()
        {
            ResetSession();
        }
        public void ExecuteUpdateRiddleCountsProcedure()
        {
            using (SqlConnection con = new SqlConnection(stgcon))
            {
                string storedProcedure = "UpdateRiddleCounts";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    message = "Problem: " + ex.Message;
                }
            }
        }
        public IActionResult ResetSession()
        {
            HttpContext.Session.Clear(); 
            return RedirectToPage("/Index"); 
        }

    }
}
