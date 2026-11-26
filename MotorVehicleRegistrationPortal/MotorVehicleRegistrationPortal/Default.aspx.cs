using System;
using System.Web.UI;

namespace MotorVehicleRegistrationPortal
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool isLoggedIn = Session["UserId"] != null;

                if (isLoggedIn)
                {
                    lnkPrimaryAction.Text = "Go to My Vehicles";
                    lnkPrimaryAction.NavigateUrl = "~/MyVehicles.aspx";
                    lnkSecondaryAction.Visible = true;
                    lnkSecondaryAction.NavigateUrl = "~/LinkVehicle.aspx";
                }
                else
                {
                    lnkPrimaryAction.Text = "Sign In to Profile";
                    lnkPrimaryAction.NavigateUrl = "~/Login.aspx";
                    lnkSecondaryAction.Visible = false;
                }
            }
        }
    }
}