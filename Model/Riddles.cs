namespace riddles_WebApp.Model
{
    public class Riddles
    {
        public int? riddle_id { get; set; }
        public int? user_id { get; set; }
        public string? riddle_text { get; set; }
        public int? agree_count { get; set; }
        public int? disagree_count { get; set;}
        public DateTime? created_at { get; set;}

        public Riddles()
        {
        }

        public Riddles(int? riddle_id, int? user_id, string? riddle_text, int? agree_count, int? disagree_count, DateTime? created_at)
        {
            this.riddle_id = riddle_id;
            this.user_id = user_id;
            this.riddle_text = riddle_text;
            this.agree_count = agree_count;
            this.disagree_count = disagree_count;
            this.created_at = created_at;
        }
    }
}
