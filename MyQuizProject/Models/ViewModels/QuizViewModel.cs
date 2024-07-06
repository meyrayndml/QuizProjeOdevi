using System.Collections.Generic;

namespace MyQuizProject.Models.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string Name { get; set; }
        public int TimeLimitSeconds { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

    }
}
