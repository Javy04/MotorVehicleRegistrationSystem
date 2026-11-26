<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MotorVehicleRegistrationPortal.Login" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center py-4">
        <div class="col-12 col-md-6 col-lg-5">
            <div class="taj-card p-4 p-md-5">
                <asp:PlaceHolder ID="phLoginForm" runat="server">
                    <div class="text-center mb-4">
                        <span class="badge bg-primary-subtle text-primary mb-2 px-3 py-2">TAJ Portal Access</span>
                        <h2 class="fw-bold mb-1">Sign In</h2>
                        <p class="text-muted small mb-0">Log in with your account to view linked vehicles and renew registration.</p>
                    </div>

                    <div class="mb-3">
                        <label for="<%= txtUsername.ClientID %>" class="form-label fw-semibold">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username" />
                    </div>

                    <div class="mb-3">
                        <label for="<%= txtPassword.ClientID %>" class="form-label fw-semibold">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password" />
                    </div>

                    <div class="d-grid mt-4">
                        <asp:Button ID="btnLogin" runat="server" Text="Log In" CssClass="btn btn-taj py-2" OnClick="btnLogin_Click" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phAlreadyLoggedIn" runat="server" Visible="false">
                    <div class="text-center">
                        <h3 class="fw-bold mb-2">You are already logged in</h3>
                        <p class="text-muted mb-4">
                            Signed in as <strong><asp:Label ID="lblActiveUser" runat="server" /></strong>.
                        </p>
                        <div class="d-flex justify-content-center gap-2">
                            <asp:HyperLink ID="lnkGoVehicles" runat="server" NavigateUrl="~/MyVehicles.aspx" CssClass="btn btn-taj px-4">My Vehicles</asp:HyperLink>
                            <asp:Button ID="btnSignOut" runat="server" Text="Log Out" CssClass="btn btn-outline-danger px-4" OnClick="btnSignOut_Click" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-danger mt-4 mb-0 small" role="alert">
                    <asp:Label ID="lblMessage" runat="server" />
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>