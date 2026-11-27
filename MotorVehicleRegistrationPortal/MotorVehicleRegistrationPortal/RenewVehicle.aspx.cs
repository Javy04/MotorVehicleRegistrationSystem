using MotorVehicleRegistrationPortal.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MotorVehicleRegistrationPortal
{
    public partial class RenewVehicle : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                BindUserVehicles();
            }
        }

        protected void ddlVehicles_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlStatus.Visible = false;
            pnlVerificationResults.Visible = false;
            LoadSelectedVehicleDetails();
        }

        protected void rblDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedVehicleDetails();
        }

        protected void btnRenew_Click(object sender, EventArgs e)
        {
            pnlStatus.Visible = false;

            int vehicleId;
            if (!int.TryParse(ddlVehicles.SelectedValue, out vehicleId))
            {
                ShowStatus("Please select a valid vehicle to renew.", isError: true);
                return;
            }

            int renewalMonths;
            if (!int.TryParse(rblDuration.SelectedValue, out renewalMonths) || (renewalMonths != 6 && renewalMonths != 12))
            {
                ShowStatus("Please select a valid renewal duration (6 months or 1 year).", isError: true);
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            DataRow vehicle = GetLinkedVehicleById(userId, vehicleId);

            if (vehicle == null)
            {
                ShowStatus("The selected vehicle is not linked to your profile.", isError: true);
                return;
            }

            string licensePlate = vehicle["LicensePlate"].ToString();
            string chassisNumber = vehicle["ChassisNumber"].ToString();
            DateTime currentExpiry = Convert.ToDateTime(vehicle["RegistrationExpiryDate"]);

            VerificationResult insuranceCheck;
            VerificationResult fitnessCheck;

            try
            {
                VehicleVerificationService verificationService = new VehicleVerificationService();
                insuranceCheck = verificationService.CheckInsuranceStatus(licensePlate, chassisNumber);
                fitnessCheck = verificationService.CheckFitnessStatus(chassisNumber);
            }
            catch (Exception)
            {
                ShowStatus("Unable to reach the Insurance or Fitness verification service right now. Please try again later.", isError: true);
                return;
            }

            DisplayVerificationResults(insuranceCheck, fitnessCheck);

            if (!insuranceCheck.IsValid || !fitnessCheck.IsValid)
            {
                string reason;
                if (!insuranceCheck.IsValid && !fitnessCheck.IsValid)
                {
                    reason = "Renewal blocked: both Motor Vehicle Insurance and Motor Vehicle Fitness are not up to date.";
                }
                else if (!insuranceCheck.IsValid)
                {
                    reason = "Renewal blocked: Motor Vehicle Insurance is not up to date.";
                }
                else
                {
                    reason = "Renewal blocked: Motor Vehicle Fitness is not up to date.";
                }

                ShowStatus(reason, isError: true);
                return;
            }

            DateTime newExpiry = renewalMonths == 6
                ? currentExpiry.AddMonths(6)
                : currentExpiry.AddYears(1);

            bool saved = SaveVehicleRenewal(userId, vehicleId, currentExpiry, newExpiry, renewalMonths);
            if (saved)
            {
                LoadSelectedVehicleDetails();
                string durationLabel = renewalMonths == 6 ? "6 months" : "1 year";
                ShowStatus(
                    string.Format(
                        "Registration for {0} has been renewed for {1}. New expiry date: {2:yyyy-MM-dd}. (Payment is handled separately.)",
                        licensePlate,
                        durationLabel,
                        newExpiry),
                    isError: false);
            }
        }

        private void BindUserVehicles()
        {
            ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings["MVRSConnectionString"];
            if (connSetting == null || string.IsNullOrWhiteSpace(connSetting.ConnectionString))
            {
                ShowStatus("Database connection is not configured. Please check Web.config.", isError: true);
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connSetting.ConnectionString))
                {
                    string sql = @"
                        SELECT v.VehicleId,
                               v.LicensePlate + ' - ' + v.Make + ' ' + v.Model + ' (' + CAST(v.ModelYear AS VARCHAR(4)) + ')' AS DisplayText
                        FROM dbo.UserVehicles uv
                        INNER JOIN dbo.Vehicles v ON uv.VehicleId = v.VehicleId
                        WHERE uv.UserId = @UserId
                        ORDER BY v.LicensePlate";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                pnlNoVehicles.Visible = true;
                                pnlRenewalForm.Visible = false;
                                return;
                            }

                            pnlNoVehicles.Visible = false;
                            pnlRenewalForm.Visible = true;

                            ddlVehicles.DataSource = dt;
                            ddlVehicles.DataTextField = "DisplayText";
                            ddlVehicles.DataValueField = "VehicleId";
                            ddlVehicles.DataBind();
                        }
                    }
                }

                string requestedId = Request.QueryString["vehicleId"];
                if (!string.IsNullOrWhiteSpace(requestedId))
                {
                    ListItem matchingItem = ddlVehicles.Items.FindByValue(requestedId.Trim());
                    if (matchingItem != null)
                    {
                        ddlVehicles.ClearSelection();
                        matchingItem.Selected = true;
                    }
                    else
                    {
                        ShowStatus("The requested vehicle is not linked to your profile. Please choose one of your linked vehicles below.", isError: true);
                    }
                }

                LoadSelectedVehicleDetails();
            }
            catch (Exception)
            {
                ShowStatus("Unable to load your linked vehicles right now. Please try again later.", isError: true);
            }
        }

        private void LoadSelectedVehicleDetails()
        {
            int vehicleId;
            if (!int.TryParse(ddlVehicles.SelectedValue, out vehicleId))
            {
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            DataRow vehicle = GetLinkedVehicleById(userId, vehicleId);

            if (vehicle == null)
            {
                ShowStatus("Unable to load details for the selected vehicle.", isError: true);
                return;
            }

            DateTime currentExpiry = Convert.ToDateTime(vehicle["RegistrationExpiryDate"]);
            int months = rblDuration.SelectedValue == "12" ? 12 : 6;
            DateTime projectedExpiry = months == 6 ? currentExpiry.AddMonths(6) : currentExpiry.AddYears(1);

            lblLicensePlate.Text = Server.HtmlEncode(vehicle["LicensePlate"].ToString());
            lblChassisNumber.Text = Server.HtmlEncode(vehicle["ChassisNumber"].ToString());
            lblVehicleModel.Text = Server.HtmlEncode(
                string.Format("{0} {1} ({2})", vehicle["Make"], vehicle["Model"], vehicle["ModelYear"]));
            lblCurrentExpiry.Text = currentExpiry.ToString("yyyy-MM-dd");
            lblProjectedExpiry.Text = projectedExpiry.ToString("yyyy-MM-dd");
        }

        private DataRow GetLinkedVehicleById(int userId, int vehicleId)
        {
            ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings["MVRSConnectionString"];
            if (connSetting == null || string.IsNullOrWhiteSpace(connSetting.ConnectionString))
            {
                return null;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connSetting.ConnectionString))
                {
                    string sql = @"
                        SELECT v.VehicleId,
                               v.LicensePlate,
                               v.ChassisNumber,
                               v.Make,
                               v.Model,
                               v.ModelYear,
                               v.RegistrationExpiryDate
                        FROM dbo.UserVehicles uv
                        INNER JOIN dbo.Vehicles v ON uv.VehicleId = v.VehicleId
                        WHERE uv.UserId = @UserId
                          AND v.VehicleId = @VehicleId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@VehicleId", SqlDbType.Int).Value = vehicleId;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool SaveVehicleRenewal(int userId, int vehicleId, DateTime previousExpiry, DateTime newExpiry, int renewalMonths)
        {
            ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings["MVRSConnectionString"];
            if (connSetting == null || string.IsNullOrWhiteSpace(connSetting.ConnectionString))
            {
                ShowStatus("Database connection is not configured. Please check Web.config.", isError: true);
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connSetting.ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            string updateVehicleSql = @"
                                UPDATE dbo.Vehicles
                                SET RegistrationExpiryDate = @NewExpiryDate
                                WHERE VehicleId = @VehicleId";

                            using (SqlCommand updateCmd = new SqlCommand(updateVehicleSql, conn, tx))
                            {
                                updateCmd.Parameters.Add("@NewExpiryDate", SqlDbType.Date).Value = newExpiry.Date;
                                updateCmd.Parameters.Add("@VehicleId", SqlDbType.Int).Value = vehicleId;
                                updateCmd.ExecuteNonQuery();
                            }

                            // Verify these column names match your team's RegistrationRenewals table in the SQL script
                            string insertHistorySql = @"
                                INSERT INTO dbo.RegistrationRenewals
                                    (VehicleId, UserId, PreviousExpiryDate, NewExpiryDate, RenewalPeriodMonths, RenewalDate)
                                VALUES
                                    (@VehicleId, @UserId, @PreviousExpiryDate, @NewExpiryDate, @RenewalPeriodMonths, GETDATE())";

                            using (SqlCommand insertCmd = new SqlCommand(insertHistorySql, conn, tx))
                            {
                                insertCmd.Parameters.Add("@VehicleId", SqlDbType.Int).Value = vehicleId;
                                insertCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                                insertCmd.Parameters.Add("@PreviousExpiryDate", SqlDbType.Date).Value = previousExpiry.Date;
                                insertCmd.Parameters.Add("@NewExpiryDate", SqlDbType.Date).Value = newExpiry.Date;
                                insertCmd.Parameters.Add("@RenewalPeriodMonths", SqlDbType.Int).Value = renewalMonths;
                                insertCmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowStatus("Renewal checks passed, but we could not save the renewal right now. Please try again later.", isError: true);
                return false;
            }
        }

        private void DisplayVerificationResults(VerificationResult insurance, VerificationResult fitness)
        {
            pnlVerificationResults.Visible = true;

            lblInsuranceBadge.Text = Server.HtmlEncode(insurance.StatusLabel);
            lblInsuranceBadge.CssClass = insurance.IsValid ? "badge text-bg-success" : "badge text-bg-danger";
            lblInsuranceMessage.Text = Server.HtmlEncode(insurance.Message);
            lblInsuranceExpiry.Text = insurance.ExpiryDate.HasValue
                ? insurance.ExpiryDate.Value.ToString("yyyy-MM-dd")
                : "N/A";

            lblFitnessBadge.Text = Server.HtmlEncode(fitness.StatusLabel);
            lblFitnessBadge.CssClass = fitness.IsValid ? "badge text-bg-success" : "badge text-bg-danger";
            lblFitnessMessage.Text = Server.HtmlEncode(fitness.Message);
            lblFitnessExpiry.Text = fitness.ExpiryDate.HasValue
                ? fitness.ExpiryDate.Value.ToString("yyyy-MM-dd")
                : "N/A";
        }

        private void ShowStatus(string message, bool isError)
        {
            pnlStatus.Visible = true;
            pnlStatus.CssClass = isError ? "alert alert-danger mb-4" : "alert alert-success mb-4";
            lblStatus.Text = Server.HtmlEncode(message);
        }
    }
}