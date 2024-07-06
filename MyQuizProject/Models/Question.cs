using System;
using System.Collections.Generic;

namespace MyQuizProject.Models;

public partial class Question
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public int? QuizId { get; set; }

    public int Points { get; set; }

    public string? OptionA { get; set; }

    public string? OptionB { get; set; }

    public string? OptionC { get; set; }

    public string? OptionD { get; set; }

    public string? CorrectAnswer { get; set; }

    public virtual Quiz? Quiz { get; set; }
}
