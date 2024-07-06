using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyQuizProject.Models;
using MyQuizProject.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MyQuizProject.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly QuizDbContext _context;

        public QuizController(QuizDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

        public async Task<IActionResult> Start(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var questions = quiz.Questions.OrderBy(q => q.Id).ToList();
            if (questions.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var questionViewModels = questions.Select(q => new QuestionViewModel
            {
                Id = q.Id,
                Text = q.Text,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                Points = q.Points,    
                CorrectAnswer = q.CorrectAnswer
            }).ToList();

            ViewBag.QuizId = quiz.Id;
            
            return View("Start", questionViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", quiz.CategoryId);
            return View(quiz);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", quiz.CategoryId);
            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", quiz.CategoryId);
            return View(quiz);
        }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.CategoryNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.QuizResults) // QuizResults ilişkisini dahil ediyoruz
                .Include(q => q.Questions)   // Questions ilişkisini dahil ediyoruz
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            _context.QuizResults.RemoveRange(quiz.QuizResults);//toplu olarak kaldırmak için

            _context.Questions.RemoveRange(quiz.Questions);

            _context.Quizzes.Remove(quiz);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public IActionResult Questions(int quizId)
        {
            var questions = _context.Questions.Where(q => q.QuizId == quizId).ToList();
            ViewBag.QuizId = quizId;
            return View(questions);
        }

        public IActionResult Details(int id)
        {
            var quiz = _context.Quizzes
                .Include(q => q.CategoryNavigation)
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }
       

        [HttpPost]
        public async Task<IActionResult> FinishQuiz(QuizViewModel model)
        {
            int score = 0;

            foreach (var question in model.Questions)
            {
                var dbQuestion = await _context.Questions.FindAsync(question.Id);
                if (dbQuestion != null && dbQuestion.CorrectAnswer == question.SelectedAnswer)
                {
                    score += dbQuestion.Points;
                }
            }
            score *= 2;
            HttpContext.Session.SetInt32("CurrentScore", score);

            var userName = HttpContext.Session.GetString("Username");

            if (userName == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var quizResult = new QuizResult
            {
                QuizId = model.QuizId,
                UserId = user.Id,
                Score = score,
                CompletedAt = DateTime.Now
            };

            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();

            return RedirectToAction("Result", new { quizId = model.QuizId });
        }

        public IActionResult Result(int quizId)
        {
            var results = _context.QuizResults
                .Where(r => r.QuizId == quizId)
                .OrderByDescending(r => r.Score)
                .Include(r => r.User)
                .ToList();

            var quizName = _context.Quizzes.Find(quizId)?.Name;

            if (quizName == null)
            {
                return NotFound();
            }

            ViewBag.QuizName = quizName;
            return View(results);
        }
    }
}
