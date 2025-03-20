
using DoaMais.Application.Commands.HospitalCommands.CreateHospitalCommand;
using DoaMais.Application.Validators;
using DoaMais.Tests.Utils;
using FluentValidation.TestHelper;

namespace DoaMais.Tests.Validators
{
    public class HospitalValidatorTests
    {
        private readonly HospitalValidator _validator;

        public HospitalValidatorTests()
        {
            _validator = new HospitalValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldAreValid_ShouldPass()
        {
            var hospital = new CreateHospitalCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedStringWithLenght(14), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());

            var result = _validator.TestValidate(hospital);

            result.ShouldNotHaveValidationErrorFor(e => e.Name);
            result.ShouldNotHaveValidationErrorFor(e => e.CNPJ);
            result.ShouldNotHaveValidationErrorFor(e => e.Email);
            result.ShouldNotHaveValidationErrorFor(e => e.Phone);
            result.ShouldNotHaveValidationErrorFor(e => e.IsActive);
        }

        [Fact]
        public void Validate_WhenNameIsEmpty_ShouldFail()
        {
            var hospital = new CreateHospitalCommand(string.Empty, TestsUtils.CreateMockedStringWithLenght(14), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());

            var result = _validator.TestValidate(hospital);

            result.ShouldHaveValidationErrorFor(e => e.Name);
        }

        [Fact]
        public void Validate_WhenCNPJIsEmpty_ShouldFail()
        {
            var hospital = new CreateHospitalCommand(TestsUtils.CreateMockedString(), string.Empty, TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());

            var result = _validator.TestValidate(hospital);

            result.ShouldHaveValidationErrorFor(e => e.CNPJ);
        }

        [Fact]
        public void Validate_WhenEmailIsEmpty_ShouldFail()
        {
            var hospital = new CreateHospitalCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedStringWithLenght(14), string.Empty, TestsUtils.CreateMockedString());
            var result = _validator.TestValidate(hospital);
            result.ShouldHaveValidationErrorFor(e => e.Email);
        }

        [Fact]
        public void Validate_WhenPhoneIsEmpty_ShouldFail()
        {
            var hospital = new CreateHospitalCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedStringWithLenght(14), TestsUtils.CreateMockedEmail(), string.Empty);

            var result = _validator.TestValidate(hospital);

            result.ShouldHaveValidationErrorFor(e => e.Phone);
        }

        [Fact]
        public void Validate_WhenPhoneIsLessThan8_ShouldFail()
        {
            var hospital = new CreateHospitalCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedStringWithLenght(14), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedStringWithLenght(7));

            var result = _validator.TestValidate(hospital);

            result.ShouldHaveValidationErrorFor(e => e.Phone);
        }

        [Fact]
        public void Validate_WhenCNPJIsLessThan14_ShouldFail()
        {
            var hospital = new CreateHospitalCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedStringWithLenght(13), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());

            var result = _validator.TestValidate(hospital);

            result.ShouldHaveValidationErrorFor(e => e.CNPJ);
        }
    }
}
