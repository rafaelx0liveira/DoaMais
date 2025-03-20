using DoaMais.Application.Services.DonorService;
using DoaMais.Domain.Entities.Enums;
using System;
using Xunit;


namespace DoaMais.Tests.Services
{
    public class DonorServiceTests
    {
        [Theory]
        [InlineData("2000-01-01", 25)] // Pessoa com 25 anos
        [InlineData("2010-01-01", 15)] // Pessoa com 15 anos
        [InlineData("1990-12-31", 34)] // Pessoa com 34 anos
        public void CalculateAge_ShouldReturnCorrectAge(string birthDateString, int expectedAge)
        {
            // Arrange
            var birthDate = DateTime.Parse(birthDateString);

            // Act
            int age = DonorService.CalculateAge(birthDate);

            // Assert
            Assert.Equal(expectedAge, age);
        }

        [Theory]
        [InlineData("2015-03-15", "Male", null, false)] // Menor de idade
        [InlineData("2000-01-01", "Male", "2025-03-01", false)] // Menor que 60 dias da última doação (Homem)
        [InlineData("2000-01-01", "Male", "2023-12-01", true)] // Pode doar (mais de 60 dias)
        [InlineData("2000-01-01", "Female", "2025-02-01", false)] // Menor que 90 dias da última doação (Mulher)
        [InlineData("2000-01-01", "Female", "2023-11-01", true)] // Pode doar (mais de 90 dias) (Mulher)
        [InlineData("2000-01-01", "Male", "2023-11-01", true)] // Pode doar (mais de 90 dias) (Homem)
        [InlineData("1990-01-01", "Male", null, true)] // Pode doar (nunca doou antes)
        public void CanDonate_ShouldReturnExpectedResult(string birthDateString, string biologicalSex, string lastDonationString, bool expectedResult)
        {
            var birthDate = DateTime.Parse(birthDateString);
            DateTime? lastDonationDate = string.IsNullOrEmpty(lastDonationString) ? (DateTime?)null : DateTime.Parse(lastDonationString);

            BiologicalSex sex = biologicalSex == "Male" ? BiologicalSex.Male : BiologicalSex.Female;

            bool canDonate = DonorService.CanDonate(birthDate, sex, lastDonationDate);

            Assert.Equal(expectedResult, canDonate);
        }
    }

}
