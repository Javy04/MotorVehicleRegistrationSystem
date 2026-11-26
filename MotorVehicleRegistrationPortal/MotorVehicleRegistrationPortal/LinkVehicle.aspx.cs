using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace MotorVehicleRegistrationPortal
{
    public partial class LinkVehicle : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        protected void btnLinkVehicle_Click(object sender, EventArgs e)
        {
            string licensePlate = txtLicensePlate.Text.Trim().ToUpperInvariant();
            string chassisNumber = txtChassisNumber.Text.Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(licensePlate) || string.IsNullOrEmpty(chassisNumber))
            {
                ShowMessage("Please enter both the license plate number and chassis number.", isError: true);
                return;
            }

            ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings["MVRSConnectionString"];
            if (connSetting == null || string.IsNullOrWhiteSpace(connSetting.ConnectionString))
            {
                ShowMessage("Database connection is not configured. Please check web.config.", isError: true);
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connSetting.ConnectionString))
                {
                    conn.Open();

                    int vehicleId = 0;
                    string findSql = @"
                        SELECT VehicleId
                        FROM dbo.Vehicles
                        WHERE UPPER(LicensePlate) = @LicensePlate
                          AND UPPER(ChassisNumber) = @ChassisNumber";

                    using (SqlCommand findCmd = new SqlCommand(findSql, conn))
                    {
                        findCmd.Parameters.Add("@LicensePlate", SqlDbType.VarChar, 20).Value = licensePlate;
                        findCmd.Parameters.Add("@ChassisNumber", SqlDbType.VarChar, 50).Value = chassisNumber;

                        object result = findCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            ShowMessage("No existing vehicle matched that license plate and chassis number.", isError: true);
                            return;
                        }
                        vehicleId = Convert.ToInt32(result);
                    }

                    string existingOwnerSql = @"
                        SELECT UserId
                        FROM dbo.UserVehicles
                        WHERE VehicleId = @VehicleId";

                    using (SqlCommand ownerCmd = new SqlCommand(existingOwnerSql, conn))
                    {
                        ownerCmd.Parameters.Add("@VehicleId", SqlDbType.Int).Value = vehicleId;
                        object ownerResult = ownerCmd.ExecuteScalar();

                        if (ownerResult != null && ownerResult != DBNull.Value)
                        {
                            int existingUserId = Convert.ToInt32(ownerResult);
                            if (existingUserId == userId)
                            {
                                ShowMessage("This vehicle is already linked to your profile.", isError: true);
                            }
                            else
                            {
                                ShowMessage("This vehicle is already linked to another user profile.", isError: true);
                            }
                            return;
                        }
                    }

                    string insertSql = @"
                        INSERT INTO dbo.UserVehicles (UserId, VehicleId, LinkedDate)
                        VALUES (@UserId, @VehicleId, GETDATE())";

                    using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                    {
                        insertCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        insertCmd.Parameters.Add("@VehicleId", SqlDbType.Int).Value = vehicleId;
                        insertCmd.ExecuteNonQuery();
                    }

                    txtLicensePlate.Text = string.Empty;
                    txtChassisNumber.Text = string.Empty;
                    ShowMessage("Vehicle linked to your profile successfully.", isError: false);
                }
            }
            catch (Exception)
            {
                ShowMessage("Unable to link the vehicle right now. Please try again later.", isError: true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isError ? "alert alert-danger mt-4 mb-0" : "alert alert-success mt-4 mb-0";
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}