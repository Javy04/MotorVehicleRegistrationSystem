using System;
using System.Web.UI;

namespace MotorVehicleRegistrationPortal
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLoggedIn = Session["UserId"] != null;

            lnkLogin.Visible = !isLoggedIn;
            btnLogout.Visible = isLoggedIn;
            lblUserGreeting.Visible = isLoggedIn;

            if (isLoggedIn)
            {
                string displayName = Session["FullName"] != null
                    ? Session["FullName"].ToString()
                    : Session["Username"]?.ToString();

                lblUserGreeting.Text = "Welcome, " + Server.HtmlEncode(displayName);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}