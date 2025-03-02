using FluentValidation;
using Articles.Application.Commons.Constants;

namespace Articles.Application.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandValidator : AbstractValidator<DeleteArticleCommand>
{
    public DeleteArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(ValidationErrorMessages.ArticleIdRequired);
    }
} 