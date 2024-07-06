namespace MyQuizProject.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
        public int Points { get; set; }
        

        public string SelectedAnswer { get; set; }

        public virtual Question? Question { get; set; }

        public Quiz Quiz { get; set; } // Quiz modeline bağlı özellik
        public string UserName { get; set; } // Kullanıcı adı özelliği
        public List<Question> Questions { get; set; } // List olarak değiştirildis
    }
}
 