using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;

namespace DiscussionFleet.Application.MembershipFeatures.Validators;

using FluentValidation;

public class MemberRegistrationRequestValidator : AbstractValidator<MemberRegistrationRequest>
{
    public MemberRegistrationRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(1, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 100);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);
    }
}