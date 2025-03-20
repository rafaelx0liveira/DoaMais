using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.Validators;
using DoaMais.Tests.Utils;
using FluentValidation.TestHelper;

namespace DoaMais.Tests.Validators
{
    public class DonationValidatorTests
    {
        private readonly DonationValidator _validator;

        public DonationValidatorTests()
        {
            _validator = new DonationValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldAreValid_ShouldPass()
        {
            var donation = new CreateDonationCommand(TestsUtils.CreateMockedGuid(), 450);

            var result = _validator.TestValidate(donation);

            result.ShouldNotHaveValidationErrorFor(d => d.DonorId);
            result.ShouldNotHaveValidationErrorFor(d => d.DonationDate);
            result.ShouldNotHaveValidationErrorFor(d => d.QuantityML);
        }

        [Fact]
        public void Validate_WhenQuantityIsLessThan420_ShouldFail()
        {
            var donation = new CreateDonationCommand(TestsUtils.CreateMockedGuid(), 419);

            var result = _validator.TestValidate(donation);

            result.ShouldHaveValidationErrorFor(d => d.QuantityML);
        }

        [Fact]
        public void Validate_WhenQuantityIsGreaterThan470_ShouldFail()
        {
            var donation = new CreateDonationCommand(TestsUtils.CreateMockedGuid(), 471);

            var result = _validator.TestValidate(donation);

            result.ShouldHaveValidationErrorFor(d => d.QuantityML);
        }
    }
}
