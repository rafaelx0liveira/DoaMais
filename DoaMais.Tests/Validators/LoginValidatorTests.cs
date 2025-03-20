using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using DoaMais.Application.Validators;
using DoaMais.Tests.Utils;
using FluentValidation.TestHelper;

namespace DoaMais.Tests.Validators
{
    public class LoginValidatorTests
    {
        private readonly LoginValidator _validator;

        public LoginValidatorTests()
        {
            _validator = new LoginValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldAreValid_ShouldPass()
        {
            var login = new LoginCommand(TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());

            var result = _validator.TestValidate(login);

            result.ShouldNotHaveValidationErrorFor(e => e.Email);
            result.ShouldNotHaveValidationErrorFor(e => e.Password);
        }

        [Fact]
        public void Validate_WhenEmailIsEmpty_ShouldFail()
        {
            var login = new LoginCommand(string.Empty, TestsUtils.CreateMockedString());

            var result = _validator.TestValidate(login);

            result.ShouldHaveValidationErrorFor(e => e.Email);
        }

        [Fact]
        public void Validate_WhenPasswordIsEmpty_ShouldFail()
        {
            var login = new LoginCommand(TestsUtils.CreateMockedEmail(), string.Empty);

            var result = _validator.TestValidate(login);

            result.ShouldHaveValidationErrorFor(e => e.Password);
        }
    }
}
