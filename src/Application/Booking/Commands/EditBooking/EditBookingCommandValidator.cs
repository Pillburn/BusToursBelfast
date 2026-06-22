// Application/Bookings/Commands/EditBooking/EditBookingCommandValidator.cs
using FluentValidation;
using ToursApp.Domain.Interfaces;
using ToursApp.Domain.Enums;
using ToursApp.Application.DTOs.Shared;

namespace ToursApp.Application.Bookings.Commands.EditBooking
{
    public class EditBookingCommandValidator : AbstractValidator<EditBookingCommand>
    {
        private readonly IBookingRepository _bookingRepository;

        public EditBookingCommandValidator(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;

            // Call all validation rules
            RuleForBookingId();
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
            RuleForBookingStatus();
        }

        private void RuleForBookingId()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("Booking ID is required")
                .MustAsync(BeValidBooking).WithMessage("Booking does not exist");
        }

        private void RuleForCustomerName()
        {
            RuleFor(x => x.CustomerName)
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Full name can only contain letters, spaces, hyphens, and apostrophes")
                .When(x => !string.IsNullOrEmpty(x.CustomerName));
        }

        private void RuleForEmail()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }

        private void RuleForPhoneNumber()
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
                .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }

        private void RuleForNumberOfParticipants()
        {
            // Validate the entire participants object if provided
            RuleFor(x => x.NumberOfParticipants)
                .Must(BeValidParticipantCount).WithMessage("Invalid participant count")
                .When(x => x.NumberOfParticipants != null);

            // Individual participant counts
            When(x => x.NumberOfParticipants != null, () =>
            {
                RuleFor(x => x.NumberOfParticipants!.Adults)
                    .GreaterThanOrEqualTo(0).WithMessage("Adults cannot be negative")
                    .LessThanOrEqualTo(20).WithMessage("Maximum 20 adults allowed");

                RuleFor(x => x.NumberOfParticipants!.Children)
                    .GreaterThanOrEqualTo(0).WithMessage("Children cannot be negative")
                    .LessThanOrEqualTo(20).WithMessage("Maximum 20 children allowed");

                RuleFor(x => x.NumberOfParticipants!.Infants)
                    .GreaterThanOrEqualTo(0).WithMessage("Infants cannot be negative")
                    .LessThanOrEqualTo(10).WithMessage("Maximum 10 infants allowed");

                // Total participants validation
                RuleFor(x => x)
                    .Must(NotExceedTotalCapacity)
                    .WithMessage("Total participants cannot exceed 50")
                    .When(x => x.NumberOfParticipants != null);

                RuleFor(x => x)
                    .Must(HaveAtLeastOneParticipant)
                    .WithMessage("At least one participant is required")
                    .When(x => x.NumberOfParticipants != null);
            });
        }

        private void RuleForPreferredDate()
        {
            RuleFor(x => x.PreferredDate)
                .Must(BeValidDateOrEmpty).WithMessage("Invalid date format")
                .Must(BeFutureDate).WithMessage("Preferred date must be in the future")
                .Must(BeWithinOneYear).WithMessage("Preferred date must be within one year from today")
                .When(x => !string.IsNullOrEmpty(x.PreferredDate));
        }

        private void RuleForPickupLocation()
        {
            RuleFor(x => x.PickupLocation)
                .MaximumLength(500).WithMessage("Pickup location must not exceed 500 characters")
                .MinimumLength(3).WithMessage("Pickup location must be at least 3 characters")
                .When(x => !string.IsNullOrEmpty(x.PickupLocation));
        }

        private void RuleForSpecialRequests()
        {
            RuleFor(x => x.SpecialRequests)
                .MaximumLength(1000).WithMessage("Special requests must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.SpecialRequests));
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

        private void RuleForBookingStatus()
        {
            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid booking status")
                .When(x => x.NewStatus.HasValue);
        }

        // Custom validation methods
        private async Task<bool> BeValidBooking(Guid bookingId, CancellationToken cancellationToken)
        {
            if (bookingId == Guid.Empty)
                return false;

            return await _bookingRepository.BookingExistsAsync(bookingId);
        }

        private bool BeValidParticipantCount(EditBookingCommand command, ParticipantCountDto? participants)
        {
            if (participants == null)
                return true; // It's optional in edit

            return participants.Adults >= 0 && 
                   participants.Children >= 0 && 
                   participants.Infants >= 0;
        }

        private bool HaveAtLeastOneParticipant(EditBookingCommand command)
        {
            var participants = command.NumberOfParticipants;
            if (participants == null)
                return true; // It's optional in edit

            return participants.Adults > 0 || 
                   participants.Children > 0 || 
                   participants.Infants > 0;
        }

        private bool NotExceedTotalCapacity(EditBookingCommand command)
        {
            var participants = command.NumberOfParticipants;
            if (participants == null)
                return true; // It's optional in edit

            var total = participants.Adults + participants.Children + participants.Infants;
            return total <= 50; // Maximum 50 participants per booking
        }

        private bool BeValidDateOrEmpty(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return true;

            return DateOnly.TryParse(date, out _);
        }

        private bool BeFutureDate(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return true;

            if (!DateOnly.TryParse(date, out var parsedDate))
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            return parsedDate >= today;
        }

        private bool BeWithinOneYear(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return true;

            if (!DateOnly.TryParse(date, out var parsedDate))
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var oneYearFromNow = today.AddYears(1);
            
            return parsedDate <= oneYearFromNow;
        }

        private bool BeAtLeast18YearsOld(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return true;

            if (!DateOnly.TryParse(date, out var birthDate))
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Year;
            
            if (birthDate > today.AddYears(-age))
                age--;

            return age >= 18;
        }
    }
}