<%@ Page Title="My Vehicles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyVehicles.aspx.cs" Inherits="MotorVehicleRegistrationPortal.MyVehicles" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-3">
        <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
            <div>
                <h2 class="fw-bold mb-1">My Linked Vehicles</h2>
                <p class="text-muted mb-0">
                    Welcome, <strong><asp:Label ID="lblUserGreeting" runat="server" /></strong>. Review your linked vehicles and registration expiry dates.
                </p>
            </div>
            <div class="d-flex gap-2">
                <asp:HyperLink ID="lnkLinkVehicle" runat="server" NavigateUrl="~/LinkVehicle.aspx" CssClass="btn btn-taj">
                    Link Existing Vehicle
                </asp:HyperLink>
                <asp:Button ID="btnLogout" runat="server" Text="Log Out" CssClass="btn btn-outline-secondary" OnClick="btnLogout_Click" />
            </div>
        </div>

        <asp:Panel ID="pnlStatus" runat="server" Visible="false" CssClass="alert mb-4" role="alert">
            <asp:Label ID="lblStatus" runat="server" />
        </asp:Panel>

        <div class="taj-card p-3 p-md-4">
            <asp:GridView ID="gvVehicles" runat="server"
                AutoGenerateColumns="False"
                EmptyDataText="No vehicles are linked to your profile yet. Use 'Link Existing Vehicle' to add one."
                CssClass="table table-hover align-middle mb-0"
                GridLines="None">
                <Columns>
                    <asp:BoundField DataField="LicensePlate" HeaderText="License Plate" />
                    <asp:BoundField DataField="ChassisNumber" HeaderText="Chassis Number" />
                    <asp:BoundField DataField="Make" HeaderText="Make" />
                    <asp:BoundField DataField="Model" HeaderText="Model" />
                    <asp:BoundField DataField="ModelYear" HeaderText="Year" />
                    <asp:BoundField DataField="RegistrationExpiryDate" HeaderText="Registration Expiry" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="LinkedDate" HeaderText="Date Linked" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <a href='<%# "RenewVehicle.aspx?vehicleId=" + Eval("VehicleId") %>' class="btn btn-sm btn-outline-primary">
                                Renew
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>