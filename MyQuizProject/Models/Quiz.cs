using System;
using System.Collections.Generic;

namespace MyQuizProject.Models;

public partial class Quiz
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public int? TimeLimitSeconds { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();

    public virtual User? User { get; set; }
}
