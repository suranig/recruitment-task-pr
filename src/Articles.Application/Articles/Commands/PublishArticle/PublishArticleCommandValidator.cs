using FluentValidation;
using Articles.Application.Commons.Constants;

namespace Articles.Application.Articles.Commands.PublishArticle;

public class PublishArticleCommandValidator : AbstractValidator<PublishArticleCommand>
{
    public PublishArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(ValidationErrorMessages.ArticleIdRequired);
    }
} 