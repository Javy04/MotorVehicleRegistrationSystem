<%@ Page Title="Renew Registration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RenewVehicle.aspx.cs" Inherits="MotorVehicleRegistrationPortal.RenewVehicle" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center py-4">
        <div class="col-12 col-lg-9">
            <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-2 mb-4">
                <div>
                    <h2 class="fw-bold mb-1">Renew Vehicle Registration</h2>
                    <p class="text-muted mb-0">
                        Select a linked vehicle and renewal period. Insurance and fitness must be up to date before renewal is approved.
                    </p>
                </div>
                <asp:HyperLink ID="lnkBackToVehicles" runat="server" NavigateUrl="~/MyVehicles.aspx" CssClass="btn btn-outline-secondary">
                    Back to My Vehicles
                </asp:HyperLink>
            </div>

            <asp:Panel ID="pnlStatus" runat="server" Visible="false" CssClass="alert mb-4" role="alert">
                <asp:Label ID="lblStatus" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlNoVehicles" runat="server" Visible="false" CssClass="taj-card p-4 text-center">
                <h5 class="fw-bold mb-2">No Linked Vehicles Found</h5>
                <p class="text-muted mb-3">
                    You need at least one vehicle linked to your profile before you can renew registration.
                </p>
                <asp:HyperLink ID="lnkGoLinkVehicle" runat="server" NavigateUrl="~/LinkVehicle.aspx" CssClass="btn btn-taj">
                    Link Existing Vehicle
                </asp:HyperLink>
            </asp:Panel>

            <asp:Panel ID="pnlRenewalForm" runat="server" Visible="false">
                <div class="taj-card p-4 mb-4">
                    <div class="mb-4">
                        <label for="<%= ddlVehicles.ClientID %>" class="form-label fw-semibold">Select Linked Vehicle</label>
                        <asp:DropDownList ID="ddlVehicles" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlVehicles_SelectedIndexChanged" />
                    </div>

                    <div class="border rounded-3 p-3 bg-light mb-4">
                        <h6 class="fw-bold mb-3">Selected Vehicle Details</h6>
                        <div class="row g-3 small">
                            <div class="col-6 col-md-4">
                                <span class="text-muted d-block">License Plate</span>
                                <strong><asp:Label ID="lblLicensePlate" runat="server" /></strong>
                            </div>
                            <div class="col-6 col-md-4">
                                <span class="text-muted d-block">Chassis Number</span>
                                <strong><asp:Label ID="lblChassisNumber" runat="server" /></strong>
                            </div>
                            <div class="col-6 col-md-4">
                                <span class="text-muted d-block">Vehicle</span>
                                <strong><asp:Label ID="lblVehicleModel" runat="server" /></strong>
                            </div>
                            <div class="col-6 col-md-4">
                                <span class="text-muted d-block">Current Registration Expiry</span>
                                <strong><asp:Label ID="lblCurrentExpiry" runat="server" /></strong>
                            </div>
                            <div class="col-12 col-md-8">
                                <span class="text-muted d-block">Projected New Expiry Date</span>
                                <strong class="text-primary"><asp:Label ID="lblProjectedExpiry" runat="server" /></strong>
                            </div>
                        </div>
                    </div>

                    <div class="mb-4">
                        <label class="form-label fw-semibold d-block">Renewal Duration</label>
                        <asp:RadioButtonList ID="rblDuration" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblDuration_SelectedIndexChanged" CssClass="d-flex gap-4">
                            <asp:ListItem Text=" 6 Months" Value="6" Selected="True" />
                            <asp:ListItem Text=" 1 Year (12 Months)" Value="12" />
                        </asp:RadioButtonList>
                        <div class="form-text mt-2">
                            The new expiry date is calculated from the vehicle's current registration expiry date. Payment is handled separately.
                        </div>
                    </div>

                    <div class="d-flex flex-wrap gap-2">
                        <asp:Button ID="btnRenew" runat="server" Text="Verify & Renew Registration" CssClass="btn btn-taj px-4" OnClick="btnRenew_Click" />
                        <asp:HyperLink ID="lnkCancel" runat="server" NavigateUrl="~/MyVehicles.aspx" CssClass="btn btn-outline-secondary px-4">
                            Cancel
                        </asp:HyperLink>
                    </div>
                </div>

                <asp:Panel ID="pnlVerificationResults" runat="server" Visible="false" CssClass="taj-card p-4">
                    <h5 class="fw-bold mb-3">Compliance Verification Results</h5>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <div class="border rounded-3 p-3 h-100">
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <span class="fw-semibold">Motor Vehicle Insurance</span>
                                    <asp:Label ID="lblInsuranceBadge" runat="server" CssClass="badge" />
                                </div>
                                <p class="small text-muted mb-1">
                                    <asp:Label ID="lblInsuranceMessage" runat="server" />
                                </p>
                                <p class="small mb-0">
                                    <strong>Expiry:</strong> <asp:Label ID="lblInsuranceExpiry" runat="server" />
                                </p>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="border rounded-3 p-3 h-100">
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <span class="fw-semibold">Motor Vehicle Fitness</span>
                                    <asp:Label ID="lblFitnessBadge" runat="server" CssClass="badge" />
                                </div>
                                <p class="small text-muted mb-1">
                                    <asp:Label ID="lblFitnessMessage" runat="server" />
                                </p>
                                <p class="small mb-0">
                                    <strong>Expiry:</strong> <asp:Label ID="lblFitnessExpiry" runat="server" />
                                </p>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Content>