using FluentValidation;

namespace Articles.Application.Articles.Commands.PublishArticle;

public class PublishArticleCommandValidator : AbstractValidator<PublishArticleCommand>
{
    public PublishArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Identyfikator artykułu jest wymagany");
    }
} 