using Blog.DBContext;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Article article)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    article.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                else
                {
                    return Unauthorized();
                }

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(article);
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(int articleId, string content)
        {
            var comment = new Comment
            {
                ArticleId = articleId,
                Content = content,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = articleId });
        }
    }
}
