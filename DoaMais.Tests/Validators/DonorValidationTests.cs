using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Validators;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Tests.Utils;
using FluentValidation.TestHelper;

namespace DoaMais.Tests.Validators
{
    public class DonorValidationTests
    {
        private readonly DonorValidator _validator;

        public DonorValidationTests()
        {
            _validator = new DonorValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldAreValid_ShouldPass()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
            result.ShouldNotHaveValidationErrorFor(d => d.Email);
            result.ShouldNotHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldNotHaveValidationErrorFor(d => d.BiologicalSex);
            result.ShouldNotHaveValidationErrorFor(d => d.Weight);
            result.ShouldNotHaveValidationErrorFor(d => d.BloodType);
            result.ShouldNotHaveValidationErrorFor(d => d.RhFactor);
        }

        [Fact]
        public void Validate_WhenNameIsEmpty_ShouldFail()
        {
            var donor = new CreateDonorCommand(string.Empty, TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.Name);
        }

        [Fact]
        public void Validate_WhenEmailIsEmpty_ShouldFail()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), string.Empty, DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.Email);
        }

        [Fact]
        public void Validate_WhenDateOfBirthIsLessThan18Years_ShouldFail()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-17), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.DateOfBirth);
        }

        [Fact]
        public void Validate_WhenWeightIsLessThan50_ShouldFail()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 49, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.Weight);
        }

        [Fact]
        public void Validate_WhenBloodTypeIsInvalid_ShouldFail()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, (BloodType)10, TestsUtils.CreateMockedEnum<RHFactor>(), TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.BloodType);
        }

        [Fact]
        public void Validate_WhenRhFactorIsInvalid_ShouldFail()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), (RHFactor)10, TestsUtils.CreateMockedObject<AddressDTO>());

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.RhFactor);
        }

        [Fact]
        public void Validate_WhenAddressIsNull_ShouldFail()
        {
            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), null);

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.Address);
        }

        [Fact]
        public void Validate_WhenAddressIsInvalid_ShouldFail()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            address.StreetAddress = string.Empty;

            var donor = new CreateDonorCommand(TestsUtils.CreateMockedString(), TestsUtils.CreateMockedEmail(), DateTime.Now.AddYears(-20), TestsUtils.CreateMockedEnum<BiologicalSex>(), 70, TestsUtils.CreateMockedEnum<BloodType>(), TestsUtils.CreateMockedEnum<RHFactor>(), address);

            var result = _validator.TestValidate(donor);

            result.ShouldHaveValidationErrorFor(d => d.Address.StreetAddress);
        }
    }
}
