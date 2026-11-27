-- ============================================================================
-- TAJ Motor Vehicle Registration System (MVRS) - Master 3NF Setup & Seed Script
-- ============================================================================

-- 1. Portal Tables
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NULL,
        TRN NVARCHAR(20) NULL
    );
END

IF OBJECT_ID('dbo.Vehicles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vehicles (
        VehicleId INT IDENTITY(1,1) PRIMARY KEY,
        LicensePlate VARCHAR(20) NOT NULL UNIQUE,
        ChassisNumber VARCHAR(50) NOT NULL UNIQUE,
        Make NVARCHAR(50) NOT NULL,
        Model NVARCHAR(50) NOT NULL,
        ModelYear INT NOT NULL,
        RegistrationExpiryDate DATE NOT NULL
    );
END

IF OBJECT_ID('dbo.UserVehicles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserVehicles (
        UserVehicleId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        VehicleId INT NOT NULL UNIQUE,
        LinkedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_UserVehicles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_UserVehicles_Vehicles FOREIGN KEY (VehicleId) REFERENCES dbo.Vehicles(VehicleId)
    );
END

IF OBJECT_ID('dbo.RegistrationRenewals', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RegistrationRenewals (
        RenewalId INT IDENTITY(1,1) PRIMARY KEY,
        VehicleId INT NOT NULL,
        UserId INT NOT NULL,
        PreviousExpiryDate DATE NOT NULL,
        NewExpiryDate DATE NOT NULL,
        RenewalPeriodMonths INT NOT NULL CHECK (RenewalPeriodMonths IN (6, 12)),
        RenewalDate DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_RegistrationRenewals_Vehicles FOREIGN KEY (VehicleId) REFERENCES dbo.Vehicles(VehicleId),
        CONSTRAINT FK_RegistrationRenewals_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
    );
END

-- 2. External Verification Web Service Tables
IF OBJECT_ID('dbo.InsurancePolicies', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InsurancePolicies (
        PolicyId INT IDENTITY(1,1) PRIMARY KEY,
        VehicleId INT NOT NULL,
        PolicyNumber VARCHAR(30) NOT NULL UNIQUE,
        ProviderName NVARCHAR(100) NOT NULL,
        CoverageType NVARCHAR(50) NOT NULL,
        IssueDate DATE NOT NULL,
        ExpiryDate DATE NOT NULL,
        Status VARCHAR(20) NOT NULL,
        CONSTRAINT FK_InsurancePolicies_Vehicles FOREIGN KEY (VehicleId) REFERENCES dbo.Vehicles(VehicleId)
    );
END

IF OBJECT_ID('dbo.FitnessCertificates', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FitnessCertificates (
        CertificateId INT IDENTITY(1,1) PRIMARY KEY,
        VehicleId INT NOT NULL,
        CertificateNumber VARCHAR(30) NOT NULL UNIQUE,
        InspectionDepot NVARCHAR(100) NOT NULL,
        IssueDate DATE NOT NULL,
        ExpiryDate DATE NOT NULL,
        Status VARCHAR(20) NOT NULL,
        CONSTRAINT FK_FitnessCertificates_Vehicles FOREIGN KEY (VehicleId) REFERENCES dbo.Vehicles(VehicleId)
    );
END

-- 3. Seed Users
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'jdoe')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, TRN)
    VALUES ('jdoe', 'Pass123!', 'John Doe', 'jdoe@example.com', '100-200-301');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'kmorgan')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, TRN)
    VALUES ('kmorgan', 'Pass123!', 'Kerry Morgan', 'kmorgan@example.com', '100-200-302');
END

-- 4. Seed Vehicles
IF NOT EXISTS (SELECT 1 FROM dbo.Vehicles WHERE LicensePlate = 'PJ1042')
BEGIN
    INSERT INTO dbo.Vehicles (LicensePlate, ChassisNumber, Make, Model, ModelYear, RegistrationExpiryDate)
    VALUES ('PJ1042', 'CHS9001001', 'Toyota', 'Corolla', 2019, '2026-12-15');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Vehicles WHERE LicensePlate = 'PK2218')
BEGIN
    INSERT INTO dbo.Vehicles (LicensePlate, ChassisNumber, Make, Model, ModelYear, RegistrationExpiryDate)
    VALUES ('PK2218', 'CHS9001002', 'Honda', 'Civic', 2018, '2026-11-30');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Vehicles WHERE LicensePlate = 'PM3390')
BEGIN
    INSERT INTO dbo.Vehicles (LicensePlate, ChassisNumber, Make, Model, ModelYear, RegistrationExpiryDate)
    VALUES ('PM3390', 'CHS9001003', 'Nissan', 'Tiida', 2017, '2026-10-20');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Vehicles WHERE LicensePlate = 'PN4481')
BEGIN
    INSERT INTO dbo.Vehicles (LicensePlate, ChassisNumber, Make, Model, ModelYear, RegistrationExpiryDate)
    VALUES ('PN4481', 'CHS9001004', 'Suzuki', 'Swift', 2020, '2026-09-15');
END

-- 5. Seed User-Vehicle Links
DECLARE @JohnId INT = (SELECT UserId FROM dbo.Users WHERE Username = 'jdoe');
DECLARE @KerryId INT = (SELECT UserId FROM dbo.Users WHERE Username = 'kmorgan');

DECLARE @Veh1 INT = (SELECT VehicleId FROM dbo.Vehicles WHERE LicensePlate = 'PJ1042');
DECLARE @Veh2 INT = (SELECT VehicleId FROM dbo.Vehicles WHERE LicensePlate = 'PK2218');
DECLARE @Veh3 INT = (SELECT VehicleId FROM dbo.Vehicles WHERE LicensePlate = 'PM3390');
DECLARE @Veh4 INT = (SELECT VehicleId FROM dbo.Vehicles WHERE LicensePlate = 'PN4481');

IF NOT EXISTS (SELECT 1 FROM dbo.UserVehicles WHERE VehicleId = @Veh1)
    INSERT INTO dbo.UserVehicles (UserId, VehicleId, LinkedDate) VALUES (@JohnId, @Veh1, '2026-01-15');

IF NOT EXISTS (SELECT 1 FROM dbo.UserVehicles WHERE VehicleId = @Veh2)
    INSERT INTO dbo.UserVehicles (UserId, VehicleId, LinkedDate) VALUES (@JohnId, @Veh2, '2026-02-10');

IF NOT EXISTS (SELECT 1 FROM dbo.UserVehicles WHERE VehicleId = @Veh3)
    INSERT INTO dbo.UserVehicles (UserId, VehicleId, LinkedDate) VALUES (@KerryId, @Veh3, '2026-03-05');

-- 6. Seed Insurance Policies
IF NOT EXISTS (SELECT 1 FROM dbo.InsurancePolicies WHERE PolicyNumber = 'INS-2026-1001')
    INSERT INTO dbo.InsurancePolicies (VehicleId, PolicyNumber, ProviderName, CoverageType, IssueDate, ExpiryDate, Status)
    VALUES (@Veh1, 'INS-2026-1001', 'GK General Insurance', 'Comprehensive', '2026-06-30', '2027-06-30', 'Active');

IF NOT EXISTS (SELECT 1 FROM dbo.InsurancePolicies WHERE PolicyNumber = 'INS-2025-1002')
    INSERT INTO dbo.InsurancePolicies (VehicleId, PolicyNumber, ProviderName, CoverageType, IssueDate, ExpiryDate, Status)
    VALUES (@Veh2, 'INS-2025-1002', 'Advantage General', 'Third Party', '2025-09-15', '2026-09-15', 'Expired');

IF NOT EXISTS (SELECT 1 FROM dbo.InsurancePolicies WHERE PolicyNumber = 'INS-2026-1003')
    INSERT INTO dbo.InsurancePolicies (VehicleId, PolicyNumber, ProviderName, CoverageType, IssueDate, ExpiryDate, Status)
    VALUES (@Veh3, 'INS-2026-1003', 'BCIC', 'Comprehensive', '2026-04-10', '2027-04-10', 'Active');

IF NOT EXISTS (SELECT 1 FROM dbo.InsurancePolicies WHERE PolicyNumber = 'INS-2025-1004')
    INSERT INTO dbo.InsurancePolicies (VehicleId, PolicyNumber, ProviderName, CoverageType, IssueDate, ExpiryDate, Status)
    VALUES (@Veh4, 'INS-2025-1004', 'JN General Insurance', 'Third Party', '2025-07-10', '2026-07-10', 'Expired');

-- 7. Seed Fitness Certificates
IF NOT EXISTS (SELECT 1 FROM dbo.FitnessCertificates WHERE CertificateNumber = 'FIT-2026-5001')
    INSERT INTO dbo.FitnessCertificates (VehicleId, CertificateNumber, InspectionDepot, IssueDate, ExpiryDate, Status)
    VALUES (@Veh1, 'FIT-2026-5001', 'Spanish Town Examination Depot', '2026-08-15', '2027-08-15', 'Valid');

IF NOT EXISTS (SELECT 1 FROM dbo.FitnessCertificates WHERE CertificateNumber = 'FIT-2026-5002')
    INSERT INTO dbo.FitnessCertificates (VehicleId, CertificateNumber, InspectionDepot, IssueDate, ExpiryDate, Status)
    VALUES (@Veh2, 'FIT-2026-5002', 'Swallowfield Examination Depot', '2026-05-20', '2027-05-20', 'Valid');

IF NOT EXISTS (SELECT 1 FROM dbo.FitnessCertificates WHERE CertificateNumber = 'FIT-2025-5003')
    INSERT INTO dbo.FitnessCertificates (VehicleId, CertificateNumber, InspectionDepot, IssueDate, ExpiryDate, Status)
    VALUES (@Veh3, 'FIT-2025-5003', 'Montego Bay Examination Depot', '2025-08-01', '2026-08-01', 'Expired');

IF NOT EXISTS (SELECT 1 FROM dbo.FitnessCertificates WHERE CertificateNumber = 'FIT-2025-5004')
    INSERT INTO dbo.FitnessCertificates (VehicleId, CertificateNumber, InspectionDepot, IssueDate, ExpiryDate, Status)
    VALUES (@Veh4, 'FIT-2025-5004', 'Mandeville Examination Depot', '2025-06-25', '2026-06-25', 'Expired');