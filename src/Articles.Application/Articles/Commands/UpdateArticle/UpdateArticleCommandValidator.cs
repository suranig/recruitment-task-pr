using FluentValidation;

namespace Articles.Application.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Identyfikator artykułu jest wymagany");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Tytuł jest wymagany")
            .MaximumLength(200).WithMessage("Tytuł nie może przekraczać 200 znaków");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Treść jest wymagana");
    }
} 