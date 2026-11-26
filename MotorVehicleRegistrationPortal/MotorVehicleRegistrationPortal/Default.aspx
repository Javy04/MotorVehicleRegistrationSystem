<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MotorVehicleRegistrationPortal.Default" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-4">
        <div class="taj-card p-4 p-md-5 mb-4">
            <div class="row align-items-center g-4">
                <div class="col-lg-8">
                    <span class="badge text-bg-primary mb-2">TAJ Portal</span>
                    <h1 class="fw-bold mb-2">Motor Vehicle Registration Portal</h1>
                    <p class="text-muted mb-4">
                        Manage vehicles linked to your profile, link existing registered vehicles, and renew vehicle registration for 6 months or 1 year after verifying insurance and fitness status. Payment is handled separately.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <asp:HyperLink ID="lnkPrimaryAction" runat="server" NavigateUrl="~/Login.aspx" CssClass="btn btn-taj px-4">
                            Sign In to Profile
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkSecondaryAction" runat="server" NavigateUrl="~/LinkVehicle.aspx" CssClass="btn btn-outline-primary px-4">
                            Link Existing Vehicle
                        </asp:HyperLink>
                    </div>
                </div>
                <div class="col-lg-4">
                    <div class="border rounded-3 p-3 bg-light">
                        <h6 class="fw-bold mb-2">Portal Workflow</h6>
                        <ol class="small text-muted mb-0 ps-3">
                            <li class="mb-1">Sign in to your user profile.</li>
                            <li class="mb-1">View or link an existing vehicle.</li>
                            <li class="mb-1">Verify insurance and fitness status.</li>
                            <li>Renew for 6 or 12 months.</li>
                        </ol>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3">
            <div class="col-md-4">
                <div class="taj-card p-4 h-100">
                    <h5 class="fw-bold mb-2">My Linked Vehicles</h5>
                    <p class="text-muted small mb-0">
                        View vehicles associated with your account along with their license plate, chassis number, and current registration expiry date.
                    </p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="taj-card p-4 h-100">
                    <h5 class="fw-bold mb-2">Link Existing Vehicle</h5>
                    <p class="text-muted small mb-0">
                        Associate an existing vehicle record to your profile using its registered license plate and chassis number.
                    </p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="taj-card p-4 h-100">
                    <h5 class="fw-bold mb-2">Registration Renewal</h5>
                    <p class="text-muted small mb-0">
                        Renew registration for 6 months or 1 year once insurance and fitness checks pass through integrated services.
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>