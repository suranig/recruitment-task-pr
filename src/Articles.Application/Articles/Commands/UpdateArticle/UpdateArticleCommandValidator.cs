using FluentValidation;
using Articles.Application.Commons.Constants;

namespace Articles.Application.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(ValidationErrorMessages.ArticleIdRequired);

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(ValidationErrorMessages.TitleRequired)
            .MaximumLength(200).WithMessage(string.Format(ValidationErrorMessages.TitleMaxLength, 200));

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage(ValidationErrorMessages.ContentRequired);
    }
} 