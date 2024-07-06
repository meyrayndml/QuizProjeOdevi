using System;
using System.Collections.Generic;

namespace MyQuizProject.Models;

public partial class QuizResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int QuizId { get; set; }

    public string? Username { get; set; }

    public int Score { get; set; }

    public DateTime CompletedAt { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
