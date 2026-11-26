<%@ Page Title="Link Existing Vehicle" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LinkVehicle.aspx.cs" Inherits="MotorVehicleRegistrationPortal.LinkVehicle" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center py-4">
        <div class="col-12 col-md-7 col-lg-6">
            <div class="taj-card p-4 p-md-5">
                <div class="mb-4">
                    <h2 class="fw-bold mb-1">Link Existing Vehicle</h2>
                    <p class="text-muted small mb-0">
                        Enter the license plate and chassis number of an existing registered vehicle to link it to your profile.
                    </p>
                </div>

                <div class="mb-3">
                    <label for="<%= txtLicensePlate.ClientID %>" class="form-label fw-semibold">License Plate Number</label>
                    <asp:TextBox ID="txtLicensePlate" runat="server" CssClass="form-control text-uppercase" MaxLength="20" placeholder="e.g., PM3390" />
                </div>

                <div class="mb-3">
                    <label for="<%= txtChassisNumber.ClientID %>" class="form-label fw-semibold">Chassis Number</label>
                    <asp:TextBox ID="txtChassisNumber" runat="server" CssClass="form-control text-uppercase" MaxLength="50" placeholder="e.g., CHS9001003" />
                </div>

                <div class="d-flex flex-wrap gap-2 mt-4">
                    <asp:Button ID="btnLinkVehicle" runat="server" Text="Link Vehicle" CssClass="btn btn-taj px-4" OnClick="btnLinkVehicle_Click" />
                    <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/MyVehicles.aspx" CssClass="btn btn-outline-secondary px-4">
                        Back to My Vehicles
                    </asp:HyperLink>
                </div>

                <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert mt-4 mb-0" role="alert">
                    <asp:Label ID="lblMessage" runat="server" />
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>