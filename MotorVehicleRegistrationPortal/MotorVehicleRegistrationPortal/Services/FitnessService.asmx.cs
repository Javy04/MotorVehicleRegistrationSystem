using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Services;

namespace MotorVehicleRegistrationPortal.Services
{
    public class FitnessVerificationResponse
    {
        public bool IsValid { get; set; }
        public string Status { get; set; }
        public string CertificateNumber { get; set; }
        public string InspectionDepot { get; set; }
        public string ExpiryDate { get; set; }
        public string Message { get; set; }
    }

    [WebService(Namespace = "http://taj.gov.jm/mvrs/fitness")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class FitnessService : WebService
    {
        [WebMethod(Description = "Verifies motor vehicle certificate of fitness status using the vehicle chassis number.")]
        public FitnessVerificationResponse VerifyFitness(string chassisNumber)
        {
            string input = (chassisNumber ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(input))
            {
                return new FitnessVerificationResponse
                {
                    IsValid = false,
                    Status = "Invalid Request",
                    Message = "Vehicle chassis number is required."
                };
            }

            string connStr = ConfigurationManager.ConnectionStrings["MVRSConnectionString"].ConnectionString;
            const string query = @"
                SELECT TOP 1
                    fc.CertificateNumber,
                    fc.InspectionDepot,
                    fc.ExpiryDate,
                    fc.Status
                FROM dbo.FitnessCertificates fc
                INNER JOIN dbo.Vehicles v ON fc.VehicleId = v.VehicleId
                WHERE UPPER(v.ChassisNumber) = @ChassisNumber
                ORDER BY fc.ExpiryDate DESC;";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ChassisNumber", input);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return new FitnessVerificationResponse
                        {
                            IsValid = false,
                            Status = "Not Found",
                            Message = "No Certificate of Fitness record found for the specified chassis number."
                        };
                    }

                    DateTime expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                    string certNumber = reader["CertificateNumber"].ToString();
                    string depot = reader["InspectionDepot"].ToString();
                    bool isValid = expiryDate.Date >= DateTime.Today;

                    return new FitnessVerificationResponse
                    {
                        IsValid = isValid,
                        Status = isValid ? "Valid" : "Expired",
                        CertificateNumber = certNumber,
                        InspectionDepot = depot,
                        ExpiryDate = expiryDate.ToString("yyyy-MM-dd"),
                        Message = isValid
                            ? $"Valid Certificate of Fitness ({certNumber}) issued by {depot}, valid through {expiryDate:yyyy-MM-dd}."
                            : $"Certificate of Fitness ({certNumber}) expired on {expiryDate:yyyy-MM-dd}."
                    };
                }
            }
        }
    }
}
