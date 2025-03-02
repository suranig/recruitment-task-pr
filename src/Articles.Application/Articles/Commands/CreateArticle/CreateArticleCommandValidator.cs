using FluentValidation;

namespace Articles.Application.Articles.Commands.CreateArticle;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Tytuł jest wymagany")
            .MaximumLength(200).WithMessage("Tytuł nie może przekraczać 200 znaków");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Treść jest wymagana");

        RuleFor(v => v.AuthorId)
            .NotEmpty().WithMessage("Identyfikator autora jest wymagany");
    }
} 