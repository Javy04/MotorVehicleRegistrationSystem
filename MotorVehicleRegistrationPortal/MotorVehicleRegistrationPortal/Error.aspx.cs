using System;
using System.Web.UI;

namespace MotorVehicleRegistrationPortal
{
    public partial class Error : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string code = (Request.QueryString["code"] ?? string.Empty).Trim();
                Response.TrySkipIisCustomErrors = true;

                switch (code)
                {
                    case "404":
                        lblErrorCode.Text = "Error 404";
                        lblErrorHeading.Text = "Page Not Found";
                        lblErrorMessage.Text = "The page or resource you requested could not be found. Check the web address or use the navigation links below.";
                        break;

                    case "403":
                        lblErrorCode.Text = "Error 403";
                        lblErrorHeading.Text = "Access Denied";
                        lblErrorMessage.Text = "You do not have permission to view this resource. Please sign in with an authorized account.";
                        break;

                    case "500":
                        lblErrorCode.Text = "Error 500";
                        lblErrorHeading.Text = "Service Temporarily Unavailable";
                        lblErrorMessage.Text = "An unexpected server error occurred while processing your request. No technical details were exposed for your security. Please try again shortly.";
                        break;

                    default:
                        lblErrorCode.Text = "System Notice";
                        lblErrorHeading.Text = "Unable to Complete Request";
                        lblErrorMessage.Text = "Something went wrong while handling your request. Please return to the portal and try again.";
                        break;
                }
            }
        }
    }
}