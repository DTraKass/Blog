using Blog.DBContext;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly LoggerService _loggerService;
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context, LoggerService loggerService)
        {
            _context = context;
            _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
        }

        public IActionResult Articles()
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        public IActionResult Article(int id)
        {
            var article = _context.Articles.Find(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        public IActionResult CreateArticle() => View();

        public async Task<IActionResult> Create(Article article)
        {
            try
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

                    _loggerService.LogUserAction($"Пользователь {article.UserId} написал статью {article.Title}"); 

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex.Message);
                ModelState.AddModelError("", "Произошла ошибка при создании статьи."); 
            }
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int articleId, string content)
        {
            try
            {
                var comment = new Comment
                {
                    ArticleId = articleId,
                    Content = content,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                _loggerService.LogUserAction($"Пользователь {comment.UserId} написал комментарий к статье {articleId}");

            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex.Message);
            }

            return RedirectToAction("Details", new { id = articleId });
        }
    }
}