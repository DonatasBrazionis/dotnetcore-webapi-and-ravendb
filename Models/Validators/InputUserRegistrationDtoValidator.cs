using System;
using dotnetcore_webapi_and_ravendb.Models.Dtos;
using FluentValidation;

namespace dotnetcore_webapi_and_ravendb.Models.Validators
{
    public class InputUserRegistrationDtoValidator : AbstractValidator<InputUserRegistrationDto>
    {
        public InputUserRegistrationDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().Length(0, 255);
            RuleFor(x => x.LastName).NotEmpty().Length(0, 255);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            // TODO: password must have letter, number, special symbol
            RuleFor(x => x.Password).NotEmpty().Length(8, 255);
            RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password);
        }
    }
}
