using FluentValidation;

namespace Articles.Application.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandValidator : AbstractValidator<DeleteArticleCommand>
{
    public DeleteArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Identyfikator artykułu jest wymagany");
    }
} 