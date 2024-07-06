using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyQuizProject.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyQuizProject.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly QuizDbContext _context;

        public QuestionController(QuizDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var questions = await _context.Questions
                .Include(q => q.Quiz)
                .ToListAsync();
            return View(questions);
        }

        public IActionResult Create(int quizId)
        {
            ViewBag.Quizzes = new SelectList(_context.Quizzes.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create( Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Quizzes = new SelectList(_context.Quizzes.ToList(), "Id", "Name", question.QuizId);
            return View(question);
        }


        public IActionResult Edit(int id)
        {
            var question = _context.Questions
                           .Include(q => q.Quiz) // İlişkili quiz bilgisini dahil ediyoruz
                           .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.Quizzes = new SelectList(_context.Quizzes.ToList(), "Id", "Name", question.QuizId);
            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Questions.Update(question);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); 
            }

            ViewBag.Quizzes = new SelectList(_context.Quizzes.ToList(), "Id", "Name", question.QuizId);
            return View(question);
        }

        public IActionResult Delete(int id)
        {
            var question = _context.Questions
                                  .Include(q => q.Quiz) // İlişkili quiz bilgisini dahil ediyoruz
                                  .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions
                                        .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); 
        }


        public IActionResult Details(int id)
        {
            var question = _context.Questions
                .Include(q => q.Quiz) 
                .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

    }
}
