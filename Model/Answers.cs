namespace riddles_WebApp.Model
{
    public class Answers
    {
        public int? answer_id { get; set; }
        public int? riddle_id { get; set; }
        public int? user_id { get; set; }
        public string? answer_text { get; set; }
        public int? AgreeCount { get; set; }
        public int? DisagreeCount { get; set; }
        public bool? is_correct { get; set; }
        public DateTime? created_at { get; set; }

        public Answers()
        {
        }

        public Answers(int? answer_id, int? riddle_id, int user_id, string? answer_text, bool? is_correct, DateTime? created_at)
        {
            this.answer_id = answer_id;
            this.riddle_id = riddle_id;
            this.user_id = user_id;
            this.answer_text = answer_text;
            this.is_correct = is_correct;
            this.created_at = created_at;
        }
    }
}
