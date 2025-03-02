using FluentValidation;
using Articles.Application.Commons.Constants;

namespace Articles.Application.Articles.Commands.CreateArticle;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(ValidationErrorMessages.TitleRequired)
            .MaximumLength(200).WithMessage(string.Format(ValidationErrorMessages.TitleMaxLength, 200));

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage(ValidationErrorMessages.ContentRequired);

        RuleFor(v => v.AuthorId)
            .NotEmpty().WithMessage(ValidationErrorMessages.AuthorIdRequired);
    }
} 