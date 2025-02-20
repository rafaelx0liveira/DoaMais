using DoaMais.Domain.Entities.Enums;

namespace DoaMais.Application.Services.DonorService
{
    public static class DonorService
    {
        public static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public static bool CanDonate(DateTime birthDate, BiologicalSex biologicalSex, DateTime? lastDonationDate)
        {
            int age = CalculateAge(birthDate);

            if (age < 18) return false; // Menor de idade não pode doar

            if (!lastDonationDate.HasValue) return true;

            TimeSpan intervalo = DateTime.Today - lastDonationDate.Value;
            int diasDesdeUltimaDoacao = intervalo.Days;

            if (biologicalSex == BiologicalSex.Female)
                return diasDesdeUltimaDoacao >= 90; // Mulheres: 90 dias de intervalo

            return diasDesdeUltimaDoacao >= 60; // Homens: 60 dias de intervalo
        }
    }
}
