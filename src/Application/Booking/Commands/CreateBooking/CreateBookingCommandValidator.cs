// Application/Bookings/Commands/CreateBooking/CreateBookingCommandValidator.cs
using FluentValidation;
using ToursApp.Application.DTOs.Shared;
using ToursApp.Domain.Interfaces;

namespace ToursApp.Application.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        private readonly ITourRepository _tourRepository;

        public CreateBookingCommandValidator(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;

            // Call all validation rules
            RuleForTourId();
            RuleForTourName();
            RuleForTourPrice();
            RuleForCustomerName();
            RuleForEmail();
            RuleForPhoneNumber();
            RuleForNumberOfParticipants();
            RuleForPreferredDate();
            RuleForPickupLocation();
            RuleForSpecialRequests();
            RuleForPassportNumber();
            RuleForDateOfBirth();
            RuleForEmergencyContact();
            RuleForTravelInsuranceDetails();
        }

        private void RuleForTourId()
        {
            RuleFor(x => x.TourId)
                .NotEmpty().WithMessage("Tour ID is required")
                .MustAsync(BeValidTour).WithMessage("Tour does not exist");
        }

        private void RuleForTourName()
        {
            RuleFor(x => x.TourName)
                .NotEmpty().WithMessage("Tour name is required")
                .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters");
        }

        private void RuleForTourPrice()
        {
            RuleFor(x => x.TourPrice)
                .GreaterThan(0).WithMessage("Tour price must be greater than 0")
                .LessThan(1000000).WithMessage("Tour price must be less than 1,000,000");
        }

        private void RuleForCustomerName()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Full name can only contain letters, spaces, hyphens, and apostrophes");
        }

        private void RuleForEmail()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");
        }

        private void RuleForPhoneNumber()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
                .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Invalid phone number format");
        }

        private void RuleForNumberOfParticipants()
        {
            // Validate the entire participants object
            RuleFor(x => x.NumberOfParticipants)
                .NotNull().WithMessage("Participants information is required")
                .Must(participants => participants != null && 
                                     (participants.Adults > 0 || 
                                      participants.Children > 0 || 
                                      participants.Infants > 0))
                .WithMessage("At least one participant is required");
            // Validate individual participant counts
            RuleFor(x => x.NumberOfParticipants.Adults)
                .GreaterThanOrEqualTo(0).WithMessage("Adults cannot be negative")
                .LessThanOrEqualTo(20).WithMessage("Maximum 20 adults allowed");

            RuleFor(x => x.NumberOfParticipants.Children)
                .GreaterThanOrEqualTo(0).WithMessage("Children cannot be negative")
                .LessThanOrEqualTo(20).WithMessage("Maximum 20 children allowed");

            RuleFor(x => x.NumberOfParticipants.Infants)
                .GreaterThanOrEqualTo(0).WithMessage("Infants cannot be negative")
                .LessThanOrEqualTo(10).WithMessage("Maximum 10 infants allowed");

            // Validate total participants
            RuleFor(x => x)
                .Must(HaveAtLeastOneParticipant)
                .WithMessage("At least one participant is required");

            RuleFor(x => x)
                .Must(NotExceedTotalCapacity)
                .WithMessage("Total participants cannot exceed 50");
        }

        private void RuleForPreferredDate()
        {
            RuleFor(x => x.PreferredDate)
                .NotEmpty().WithMessage("Preferred date is required")
                .Must(BeValidDate).WithMessage("Invalid date format")
                .Must(BeFutureDate).WithMessage("Preferred date must be in the future")
                .Must(BeWithinOneYear).WithMessage("Preferred date must be within one year from today");
        }

        private void RuleForPickupLocation()
        {
            RuleFor(x => x.PickupLocation)
                .NotEmpty().WithMessage("Pickup location is required")
                .MaximumLength(500).WithMessage("Pickup location must not exceed 500 characters")
                .MinimumLength(3).WithMessage("Pickup location must be at least 3 characters");
        }

        private void RuleForSpecialRequests()
        {
            RuleFor(x => x.SpecialRequests)
                .MaximumLength(1000).WithMessage("Special requests must not exceed 1000 characters");
        }

        private void RuleForPassportNumber()
        {
            RuleFor(x => x.PassportNumber)
                .MaximumLength(50).WithMessage("Passport number must not exceed 50 characters")
                .Matches(@"^[A-Z0-9]+$").WithMessage("Passport number can only contain uppercase letters and numbers")
                .When(x => !string.IsNullOrEmpty(x.PassportNumber));
        }

        private void RuleForDateOfBirth()
        {
            RuleFor(x => x.DateOfBirth)
                .Must(BeValidDateOrEmpty).WithMessage("Invalid date format")
                .Must(BeAtLeast18YearsOld).WithMessage("Must be at least 18 years old")
                .When(x => !string.IsNullOrEmpty(x.DateOfBirth));
        }

        private void RuleForEmergencyContact()
        {
            RuleFor(x => x.EmergencyContact)
                .MaximumLength(200).WithMessage("Emergency contact must not exceed 200 characters")
                .Matches(@"^[a-zA-Z0-9\s\-\(\)]+$").WithMessage("Invalid emergency contact format")
                .When(x => !string.IsNullOrEmpty(x.EmergencyContact));
        }

        private void RuleForTravelInsuranceDetails()
        {
            RuleFor(x => x.TravelInsuranceDetails)
                .MaximumLength(500).WithMessage("Travel insurance details must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.TravelInsuranceDetails));
        }

        // Custom validation methods
        private async Task<bool> BeValidTour(string tourId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(tourId, out var guid))
            {
                return false;
            }
            var tour = await _tourRepository.GetTourByIdAsync(guid);
            return tour != null;
        }

        private bool BeValidParticipantCount(CreateBookingCommand command, ParticipantCountDto participants)
        {
            if (participants == null)
                return false;

            return participants.Adults >= 0 && 
                   participants.Children >= 0 && 
                   participants.Infants >= 0;
        }

        private bool HaveAtLeastOneParticipant(CreateBookingCommand command)
        {
            var participants = command.NumberOfParticipants;
            return participants != null && 
                   (participants.Adults > 0 || participants.Children > 0 || participants.Infants > 0);
        }

        private bool NotExceedTotalCapacity(CreateBookingCommand command)
        {
            var participants = command.NumberOfParticipants;
            if (participants == null)
                return false;

            var total = participants.Adults + participants.Children + participants.Infants;
            return total <= 50; // Maximum 50 participants per booking
        }

        private bool BeValidDate(string date)
        {
            return DateOnly.TryParse(date, out _);
        }

        private bool BeFutureDate(string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            return parsedDate >= today;
        }

        private bool BeWithinOneYear(string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var oneYearFromNow = today.AddYears(1);
            
            return parsedDate <= oneYearFromNow;
        }

        private bool BeValidDateOrEmpty(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return true;

            return DateOnly.TryParse(date, out _);
        }

        private bool BeAtLeast18YearsOld(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return true;

            if (!DateOnly.TryParse(date, out var birthDate))
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Year;
            
            // Check if birthday has occurred this year
            if (birthDate > today.AddYears(-age))
                age--;

            return age >= 18;
        }
    }
}