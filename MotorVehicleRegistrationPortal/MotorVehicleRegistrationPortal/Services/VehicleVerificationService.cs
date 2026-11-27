using System;
using MotorVehicleRegistrationPortal.InsuranceServiceRef;
using MotorVehicleRegistrationPortal.FitnessServiceRef;

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
        /// Verifies motor vehicle insurance status via InsuranceService.asmx web service.
        /// </summary>
        public VerificationResult CheckInsuranceStatus(string licensePlate, string chassisNumber)
        {
            string plate = (licensePlate ?? string.Empty).Trim().ToUpperInvariant();
            string chassis = (chassisNumber ?? string.Empty).Trim().ToUpperInvariant();
            string identifier = !string.IsNullOrEmpty(plate) ? plate : chassis;

            try
            {
                using (var client = new InsuranceService())
                {
                    var response = client.VerifyInsurance(identifier);

                    // Fallback to chassis number if plate lookup returned Not Found and chassis is available
                    if (!response.IsValid && response.Status == "Not Found" &&
                        !string.IsNullOrEmpty(plate) && !string.IsNullOrEmpty(chassis))
                    {
                        response = client.VerifyInsurance(chassis);
                    }

                    DateTime parsedExpiry;
                    DateTime? expiryDate = DateTime.TryParse(response.ExpiryDate, out parsedExpiry)
                        ? (DateTime?)parsedExpiry
                        : null;

                    return new VerificationResult
                    {
                        IsValid = response.IsValid,
                        StatusLabel = response.Status,
                        ExpiryDate = expiryDate,
                        Message = response.Message
                    };
                }
            }
            catch (Exception ex)
            {
                return new VerificationResult
                {
                    IsValid = false,
                    StatusLabel = "Service Error",
                    ExpiryDate = null,
                    Message = "Unable to reach the Insurance Verification Service: " + ex.Message
                };
            }
        }

        /// <summary>
        /// Verifies motor vehicle certificate of fitness status via live FitnessService.asmx web service.
        /// </summary>
        public VerificationResult CheckFitnessStatus(string chassisNumber)
        {
            string chassis = (chassisNumber ?? string.Empty).Trim().ToUpperInvariant();

            try
            {
                using (var client = new FitnessService())
                {
                    var response = client.VerifyFitness(chassis);

                    DateTime parsedExpiry;
                    DateTime? expiryDate = DateTime.TryParse(response.ExpiryDate, out parsedExpiry)
                        ? (DateTime?)parsedExpiry
                        : null;

                    return new VerificationResult
                    {
                        IsValid = response.IsValid,
                        StatusLabel = response.Status,
                        ExpiryDate = expiryDate,
                        Message = response.Message
                    };
                }
            }
            catch (Exception ex)
            {
                return new VerificationResult
                {
                    IsValid = false,
                    StatusLabel = "Service Error",
                    ExpiryDate = null,
                    Message = "Unable to reach the Fitness Verification Service: " + ex.Message
                };
            }
        }
    }
}