using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace MotorVehicleRegistrationPortal
{
    public partial class MyVehicles : Page
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
                string displayName = Session["FullName"] != null
                    ? Session["FullName"].ToString()
                    : (Session["Username"] != null ? Session["Username"].ToString() : "User");
                lblUserGreeting.Text = Server.HtmlEncode(displayName);

                LoadLinkedVehicles();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LoadLinkedVehicles()
        {
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
                    string sql = @"
                        SELECT v.VehicleId,
                               v.LicensePlate,
                               v.ChassisNumber,
                               v.Make,
                               v.Model,
                               v.ModelYear,
                               v.RegistrationExpiryDate,
                               uv.LinkedDate
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
                            gvVehicles.DataSource = dt;
                            gvVehicles.DataBind();
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowMessage("Unable to load your linked vehicles right now. Please try again later.", isError: true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            pnlStatus.Visible = true;
            pnlStatus.CssClass = isError ? "alert alert-danger mb-4" : "alert alert-success mb-4";
            lblStatus.Text = Server.HtmlEncode(message);
        }
    }
}