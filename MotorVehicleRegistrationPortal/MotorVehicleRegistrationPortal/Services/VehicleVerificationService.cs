using System;

namespace MotorVehicleRegistrationPortal.Services
{
    public class VerificationResult
    {
        public bool IsValid { get; set; }
        public string StatusLabel { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Message { get; set; }
    }

    public class VehicleVerificationService
    {
        /// <summary>
        /// Checks whether motor vehicle insurance is up to date using either
        /// the license plate registration number or chassis number.
        /// Replace the mock logic below with Member 3's Insurance Web Service call once ready.
        /// </summary>
        public VerificationResult CheckInsuranceStatus(string licensePlate, string chassisNumber)
        {
            string plate = (licensePlate ?? string.Empty).Trim().ToUpperInvariant();
            string chassis = (chassisNumber ?? string.Empty).Trim().ToUpperInvariant();

            if (plate == "PJ1042" || chassis == "CHS9001001")
            {
                return new VerificationResult
                {
                    IsValid = true,
                    StatusLabel = "Valid",
                    ExpiryDate = DateTime.Today.AddMonths(8),
                    Message = "Insurance policy is active and up to date."
                };
            }

            if (plate == "PK2218" || chassis == "CHS9001002")
            {
                return new VerificationResult
                {
                    IsValid = false,
                    StatusLabel = "Expired",
                    ExpiryDate = DateTime.Today.AddDays(-14),
                    Message = "Insurance policy has expired. Please renew your insurance before renewing registration."
                };
            }

            if (plate == "PM3390" || chassis == "CHS9001003")
            {
                return new VerificationResult
                {
                    IsValid = true,
                    StatusLabel = "Valid",
                    ExpiryDate = DateTime.Today.AddMonths(5),
                    Message = "Insurance policy is active and up to date."
                };
            }

            if (plate == "PN4481" || chassis == "CHS9001004")
            {
                return new VerificationResult
                {
                    IsValid = false,
                    StatusLabel = "Expired",
                    ExpiryDate = DateTime.Today.AddMonths(-2),
                    Message = "Insurance policy has expired. Please renew your insurance before renewing registration."
                };
            }

            return new VerificationResult
            {
                IsValid = false,
                StatusLabel = "Not Found",
                ExpiryDate = null,
                Message = "No valid insurance record was found for this license plate or chassis number."
            };
        }

        /// <summary>
        /// Checks whether motor vehicle fitness is up to date using the chassis number.
        /// Replace the mock logic below with Member 3's Fitness Web Service call once ready.
        /// </summary>
        public VerificationResult CheckFitnessStatus(string chassisNumber)
        {
            string chassis = (chassisNumber ?? string.Empty).Trim().ToUpperInvariant();

            if (chassis == "CHS9001001" || chassis == "CHS9001002")
            {
                return new VerificationResult
                {
                    IsValid = true,
                    StatusLabel = "Valid",
                    ExpiryDate = DateTime.Today.AddMonths(10),
                    Message = "Certificate of fitness is active and up to date."
                };
            }

            if (chassis == "CHS9001003" || chassis == "CHS9001004")
            {
                return new VerificationResult
                {
                    IsValid = false,
                    StatusLabel = "Expired",
                    ExpiryDate = DateTime.Today.AddDays(-30),
                    Message = "Certificate of fitness has expired. Please pass a fitness inspection before renewing registration."
                };
            }

            return new VerificationResult
            {
                IsValid = false,
                StatusLabel = "Not Found",
                ExpiryDate = null,
                Message = "No valid fitness certificate was found for this chassis number."
            };
        }
    }
}