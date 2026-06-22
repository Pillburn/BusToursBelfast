// ToursApp.Application/DTOs/Shared/ParticipantCountDto.cs
namespace ToursApp.Application.DTOs.Shared
{
    public class ParticipantCountDto
    {
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }

        public int Total => Adults + Children + Infants;
    }
}