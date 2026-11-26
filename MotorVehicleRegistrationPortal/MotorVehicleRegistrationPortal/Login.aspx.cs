using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace MotorVehicleRegistrationPortal
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateViewState();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both your username and password.");
                return;
            }

            ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings["MVRSConnectionString"];
            if (connSetting == null || string.IsNullOrWhiteSpace(connSetting.ConnectionString))
            {
                ShowError("Database connection is not configured yet. Please check web.config.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connSetting.ConnectionString))
                {
                    string sql = @"
                        SELECT UserId, Username, FullName, PasswordHash
                        FROM Users
                        WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["PasswordHash"].ToString();

                                // Replace with a proper password hash check once your team sets hashing
                                if (storedPassword == password)
                                {
                                    Session["UserId"] = Convert.ToInt32(reader["UserId"]);
                                    Session["Username"] = reader["Username"].ToString();
                                    Session["FullName"] = reader["FullName"].ToString();

                                    Response.Redirect("~/MyVehicles.aspx", false);
                                    Context.ApplicationInstance.CompleteRequest();
                                    return;
                                }
                            }
                        }
                    }
                }

                ShowError("Invalid username or password.");
            }
            catch (Exception)
            {
                ShowError("Unable to complete sign-in right now. Please try again later.");
            }
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void UpdateViewState()
        {
            bool isLoggedIn = Session["UserId"] != null;
            phLoginForm.Visible = !isLoggedIn;
            phAlreadyLoggedIn.Visible = isLoggedIn;

            if (isLoggedIn)
            {
                string name = Session["FullName"] != null
                    ? Session["FullName"].ToString()
                    : Session["Username"]?.ToString();
                lblActiveUser.Text = Server.HtmlEncode(name);
            }
        }

        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}