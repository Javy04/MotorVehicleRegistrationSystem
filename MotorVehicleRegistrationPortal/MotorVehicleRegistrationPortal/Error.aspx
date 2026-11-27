<%@ Page Title="Something Went Wrong" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="MotorVehicleRegistrationPortal.Error" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center py-5">
        <div class="col-12 col-md-8 col-lg-6">
            <div class="taj-card p-4 p-md-5 text-center">
                <div class="mb-3">
                    <span class="badge text-bg-danger fs-6 px-3 py-2">
                        <asp:Label ID="lblErrorCode" runat="server" Text="Error" />
                    </span>
                </div>

                <h2 class="fw-bold mb-3">
                    <asp:Label ID="lblErrorHeading" runat="server" Text="We ran into an unexpected problem" />
                </h2>

                <p class="text-muted mb-4">
                    <asp:Label ID="lblErrorMessage" runat="server" Text="Your request could not be completed right now. Please try again or return to the portal home page." />
                </p>

                <div class="d-flex flex-wrap justify-content-center gap-2">
                    <asp:HyperLink ID="lnkHome" runat="server" NavigateUrl="~/Default.aspx" CssClass="btn btn-taj px-4">
                        Return to Home
                    </asp:HyperLink>
                    <asp:HyperLink ID="lnkMyVehicles" runat="server" NavigateUrl="~/MyVehicles.aspx" CssClass="btn btn-outline-secondary px-4">
                        Go to My Vehicles
                    </asp:HyperLink>
                </div>
            </div>
        </div>
    </div>
</asp:Content>