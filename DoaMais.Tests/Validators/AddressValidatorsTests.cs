using DoaMais.Application.DTOs;
using DoaMais.Application.Validators;
using DoaMais.Tests.Utils;
using FluentValidation.TestHelper;

namespace DoaMais.Tests.Validators
{
    public class AddressValidatorsTests
    {
        private readonly AddressValidator _validator;

        public AddressValidatorsTests()
        {
            _validator = new AddressValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldAreValid_ShouldPass()
        {
            var address =  TestsUtils.CreateMockedObject<AddressDTO>();

            var result = _validator.TestValidate(address);

            result.ShouldNotHaveValidationErrorFor(a => a.StreetAddress);
            result.ShouldNotHaveValidationErrorFor(a => a.City);
            result.ShouldNotHaveValidationErrorFor(a => a.State);
            result.ShouldNotHaveValidationErrorFor(a => a.PostalCode);
        }

        [Fact]
        public void Validate_WhenStreetAddressIsEmpty_ShouldFail()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            address.StreetAddress = string.Empty;

            var result = _validator.TestValidate(address);

            result.ShouldHaveValidationErrorFor(a => a.StreetAddress);
        }

        [Fact]
        public void Validate_WhenCityIsEmpty_ShouldFail()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            address.City = string.Empty;

            var result = _validator.TestValidate(address);

            result.ShouldHaveValidationErrorFor(a => a.City);
        }

        [Fact]
        public void Validate_WhenStateIsEmpty_ShouldFail()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            address.State = string.Empty;

            var result = _validator.TestValidate(address);

            result.ShouldHaveValidationErrorFor(a => a.State);
        }

        [Fact]
        public void Validate_WhenPostalCodeIsEmpty_ShouldFail()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            address.PostalCode = string.Empty;

            var result = _validator.TestValidate(address);

            result.ShouldHaveValidationErrorFor(a => a.PostalCode);
        }
    }
}
