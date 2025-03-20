using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Validators;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Tests.Utils;
using FluentValidation.TestHelper;

namespace DoaMais.Tests.Validators
{
    public class EmployeeValidatorTests
    {
        private readonly EmployeeValidator _validator;

        public EmployeeValidatorTests()
        {
            _validator = new EmployeeValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldAreValid_ShouldPass()
        {
            var employee = new CreateEmployeeCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEnum<EmployeeRole>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(employee);

            result.ShouldNotHaveValidationErrorFor(e => e.Name);
            result.ShouldNotHaveValidationErrorFor(e => e.Email);
            result.ShouldNotHaveValidationErrorFor(e => e.Password);
            result.ShouldNotHaveValidationErrorFor(e => e.Role);
            result.ShouldNotHaveValidationErrorFor(e => e.Address);
        }

        [Fact]
        public void Validate_WhenNameIsEmpty_ShouldFail()
        {
            var employee = new CreateEmployeeCommand(string.Empty, TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEnum<EmployeeRole>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(employee);

            result.ShouldHaveValidationErrorFor(e => e.Name);
        }

        [Fact]
        public void Validate_WhenEmailIsEmpty_ShouldFail()
        {
            var employee = new CreateEmployeeCommand(TestsUtils.CreateMockedString(), string.Empty, TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEnum<EmployeeRole>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(employee);

            result.ShouldHaveValidationErrorFor(e => e.Email);
        }

        [Fact]
        public void Validate_WhenPasswordIsEmpty_ShouldFail()
        {
            var employee = new CreateEmployeeCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), string.Empty, TestsUtils.CreateMockedEnum<EmployeeRole>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(employee);

            result.ShouldHaveValidationErrorFor(e => e.Password);
        }

        [Fact]
        public void Validate_WhenRoleIsEmpty_ShouldFail()
        {
            var employee = new CreateEmployeeCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString(), (EmployeeRole)50, TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(employee);

            result.ShouldHaveValidationErrorFor(e => e.Role);
        }

        [Fact]
        public void Validate_WhenAddressIsNull_ShouldFail()
        {
            var employee = new CreateEmployeeCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEnum<EmployeeRole>(), null);

            var result = _validator.TestValidate(employee);

            result.ShouldHaveValidationErrorFor(e => e.Address);
        }

    }
}
