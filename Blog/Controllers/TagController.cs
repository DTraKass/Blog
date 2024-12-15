using Blog.DBContext;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        // Метод для создания тегов
        [HttpPost("articles/createtag")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTag(Article article, string tagNames)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Ошибка валидации данных." });
            }

            if (string.IsNullOrWhiteSpace(tagNames))
            {
                return Json(new { success = false, message = "Введите хотя бы один тег." });
            }

            var tags = tagNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(t => t.Trim())
                               .Distinct();

            try
            {
                foreach (var tag in tags)
                {
                    var existingTag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));

                    if (existingTag == null)
                    {
                        existingTag = new Tag { Name = tag };
                        _context.Tags.Add(existingTag);
                    }

                    article.ArticleTags.Add(new ArticleTag { Article = article, Tag = existingTag });
                }

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Теги успешно созданы!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ошибка: {ex.Message}" });
            }
        }

        // Метод поиска статей по тегу
        public async Task<IActionResult> Search(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                // Если тег пустой, вернуть все статьи
                var articles = await _context.Articles
                    .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                    .ToListAsync();

                return View("Articles", articles);
            }

            var filteredArticles = await _context.Articles
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Where(a => a.ArticleTags.Any(at => at.Tag.Name == tagName))
                .ToListAsync();

            var model = filteredArticles.Any() ? filteredArticles : new List<Article>();

            return View("Articles", model);
        }
    }
}
