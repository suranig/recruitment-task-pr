using FluentValidation;

namespace Articles.Application.Articles.Commands.UnpublishArticle;

public class UnpublishArticleCommandValidator : AbstractValidator<UnpublishArticleCommand>
{
    public UnpublishArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Identyfikator artykułu jest wymagany");
    }
} 