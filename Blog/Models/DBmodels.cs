namespace Blog.Models
{
    public class DBmodels
    {
    }

    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Article
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public int ViewCount { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }

    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }

        public Article Article { get; set; }
        public ApplicationUser User { get; set; }
    }
}
