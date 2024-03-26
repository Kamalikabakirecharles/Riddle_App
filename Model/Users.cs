namespace riddles_WebApp.Model
{
    public class Users
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? userType { get; set; }

        public Users()
        {
        }

        public Users(int? id, string? username, string? password, string? email, string? userType)
        {
            Id = id;
            Username = username;
            Password = password;
            Email = email;
            this.userType = userType;
        }
    }
}
