using Blog.DBContext;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return RedirectToAction("Errors", "Error");
            }
            return View(article);
        }

        [HttpGet("articles/createarticle")]
        public IActionResult CreateArticle() => View();

        [HttpPost("articles/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            try
            {
                // Проверка аутентификации
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                // Устанавливаем UserId для статьи
                article.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Валидация модели
                if (!ModelState.IsValid)
                {
                    // Журналирование возможных ошибок
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _loggerService.LogError(error.ErrorMessage);
                    }
                    return View("CreateArticle", article);
                }

                // Добавление статьи в контекст и сохранение
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                _loggerService.LogUserAction($"Пользователь {article.UserId} написал статью {article.Title}");
                return RedirectToAction("Articles", "Articles");
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex.Message);
                ModelState.AddModelError("", "Произошла ошибка при создании статьи.");
            }
            return View("CreateArticle", article);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost("articles/addcomment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int articleId, string content)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Чтобы добавить комментарий к статье нужно авторизироваться.");
                return RedirectToAction("Loginview", "Account");

            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Создание нового комментария
            var comment = new Comment
            {
                ArticleId = articleId,
                UserId = userId,
                Content = content
            };

            try
            {
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                // Логирование действия
                _loggerService.LogUserAction($"Пользователь {userId} добавил комментарий к статье {articleId}");

                return RedirectToAction("Article", "Articles", new { id = articleId });
            }
            catch(Exception ex)
            {
                _loggerService.LogError(ex.Message);
                ModelState.AddModelError("", "Произошла ошибка при добавлении комментария.");
            }
            return RedirectToAction("Article", "Articles", new { id = articleId });
        }
    }
}
