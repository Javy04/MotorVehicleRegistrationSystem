using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Services;

namespace MotorVehicleRegistrationPortal.Services
{
    public class InsuranceVerificationResponse
    {
        public bool IsValid { get; set; }
        public string Status { get; set; }
        public string PolicyNumber { get; set; }
        public string ProviderName { get; set; }
        public string CoverageType { get; set; }
        public string ExpiryDate { get; set; }
        public string Message { get; set; }
    }

    [WebService(Namespace = "http://taj.gov.jm/mvrs/insurance")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class InsuranceService : WebService
    {
        [WebMethod(Description = "Verifies motor vehicle insurance policy status using a license plate number or chassis number.")]
        public InsuranceVerificationResponse VerifyInsurance(string plateOrChassis)
        {
            string input = (plateOrChassis ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(input))
            {
                return new InsuranceVerificationResponse
                {
                    IsValid = false,
                    Status = "Invalid Request",
                    Message = "License plate or chassis number is required."
                };
            }

            string connStr = ConfigurationManager.ConnectionStrings["MVRSConnectionString"].ConnectionString;
            const string query = @"
                SELECT TOP 1
                    ip.PolicyNumber,
                    ip.ProviderName,
                    ip.CoverageType,
                    ip.ExpiryDate,
                    ip.Status
                FROM dbo.InsurancePolicies ip
                INNER JOIN dbo.Vehicles v ON ip.VehicleId = v.VehicleId
                WHERE UPPER(v.LicensePlate) = @Identifier
                   OR UPPER(v.ChassisNumber) = @Identifier
                ORDER BY ip.ExpiryDate DESC;";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Identifier", input);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return new InsuranceVerificationResponse
                        {
                            IsValid = false,
                            Status = "Not Found",
                            Message = "No insurance policy record found for the specified vehicle."
                        };
                    }

                    DateTime expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                    string policyNumber = reader["PolicyNumber"].ToString();
                    string providerName = reader["ProviderName"].ToString();
                    string coverageType = reader["CoverageType"].ToString();
                    bool isValid = expiryDate.Date >= DateTime.Today;

                    return new InsuranceVerificationResponse
                    {
                        IsValid = isValid,
                        Status = isValid ? "Valid" : "Expired",
                        PolicyNumber = policyNumber,
                        ProviderName = providerName,
                        CoverageType = coverageType,
                        ExpiryDate = expiryDate.ToString("yyyy-MM-dd"),
                        Message = isValid
                            ? $"Active {coverageType} policy ({policyNumber}) with {providerName}, valid through {expiryDate:yyyy-MM-dd}."
                            : $"Insurance policy ({policyNumber}) with {providerName} expired on {expiryDate:yyyy-MM-dd}."
                    };
                }
            }
        }
    }
}
