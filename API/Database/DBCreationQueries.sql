﻿-- Create Database
CREATE DATABASE eShop;
GO

USE eShop;
GO

-- Create Users Table
CREATE TABLE Users
(
    UserID    INT PRIMARY KEY IDENTITY (1,1),
    Email     VARCHAR(100) NOT NULL UNIQUE,
    Password  VARCHAR(255) NOT NULL, -- Store hashed passwords in production
    FirstName VARCHAR(50)  NOT NULL,
    LastName  VARCHAR(50)  NOT NULL,
    Phone     VARCHAR(20),
    CreatedAt DATETIME     NOT NULL DEFAULT GETDATE(),
    IsActive  BIT          NOT NULL DEFAULT 1
);
GO

-- Create Addresses Table
CREATE TABLE Addresses
(
    AddressID    INT PRIMARY KEY IDENTITY (1,1),
    UserID       INT          NOT NULL,
    AddressLine1 VARCHAR(100) NOT NULL,
    AddressLine2 VARCHAR(100),
    City         VARCHAR(50)  NOT NULL,
    State        VARCHAR(50)  NOT NULL,
    PostalCode   VARCHAR(20)  NOT NULL,
    Country      VARCHAR(50)  NOT NULL DEFAULT 'United States',
    IsDefault    BIT          NOT NULL DEFAULT 0,
    AddressType  VARCHAR(20)  NOT NULL, -- 'Billing' or 'Shipping'
    FOREIGN KEY (UserID) REFERENCES Users (UserID)
);
GO

-- Create Categories Table
CREATE TABLE Categories
(
    CategoryID       INT PRIMARY KEY IDENTITY (1,1),
    CategoryName     VARCHAR(50) NOT NULL,
    Description      VARCHAR(500),
    ParentCategoryID INT,
    IsActive         BIT         NOT NULL DEFAULT 1,
    FOREIGN KEY (ParentCategoryID) REFERENCES Categories (CategoryID)
);
GO

-- Create Products Table
CREATE TABLE Products
(
    ProductID       INT PRIMARY KEY IDENTITY (1,1),
    SKU             VARCHAR(50)    NOT NULL UNIQUE,
    ProductName     VARCHAR(100)   NOT NULL,
    Description     TEXT,
    CategoryID      INT,
    Price           DECIMAL(10, 2) NOT NULL,
    Weight          DECIMAL(10, 2),
    Dimensions      VARCHAR(50), -- Format: LxWxH
    QuantityInStock INT            NOT NULL DEFAULT 0,
    IsActive        BIT            NOT NULL DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Categories (CategoryID)
);
GO

-- Create Product Variants Table (for products with options like size, color)
CREATE TABLE ProductVariants
(
    VariantID       INT PRIMARY KEY IDENTITY (1,1),
    ProductID       INT          NOT NULL,
    VariantName     VARCHAR(100) NOT NULL,
    SKU             VARCHAR(50)  NOT NULL UNIQUE,
    AdditionalCost  DECIMAL(10, 2)        DEFAULT 0,
    QuantityInStock INT          NOT NULL DEFAULT 0,
    IsActive        BIT          NOT NULL DEFAULT 1,
    FOREIGN KEY (ProductID) REFERENCES Products (ProductID)
);
GO

-- Create Tax Rates Table
CREATE TABLE TaxRates
(
    TaxRateID     INT PRIMARY KEY IDENTITY (1,1),
    State         VARCHAR(50)   NOT NULL,
    County        VARCHAR(50),
    City          VARCHAR(50),
    ZipCode       VARCHAR(20),
    TaxRate       DECIMAL(5, 2) NOT NULL, -- Percentage
    IsActive      BIT           NOT NULL DEFAULT 1,
    EffectiveDate DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- Create Discounts Table
CREATE TABLE Discounts
(
    DiscountID      INT PRIMARY KEY IDENTITY (1,1),
    DiscountCode    VARCHAR(50) UNIQUE,
    Description     VARCHAR(255),
    DiscountType    VARCHAR(20)    NOT NULL, -- 'Percentage', 'Fixed Amount'
    DiscountValue   DECIMAL(10, 2) NOT NULL,
    MinimumPurchase DECIMAL(10, 2)          DEFAULT 0,
    IsActive        BIT            NOT NULL DEFAULT 1,
    StartDate       DATETIME       NOT NULL DEFAULT GETDATE(),
    EndDate         DATETIME
);
GO

-- Create Product Discounts Table (Many-to-Many)
CREATE TABLE ProductDiscounts
(
    ProductID  INT NOT NULL,
    DiscountID INT NOT NULL,
    PRIMARY KEY (ProductID, DiscountID),
    FOREIGN KEY (ProductID) REFERENCES Products (ProductID),
    FOREIGN KEY (DiscountID) REFERENCES Discounts (DiscountID)
);
GO

-- Create Payment Methods Table
CREATE TABLE PaymentMethods
(
    PaymentMethodID INT PRIMARY KEY IDENTITY (1,1),
    UserID          INT         NOT NULL,
    PaymentType     VARCHAR(50) NOT NULL, -- 'Credit Card', 'PayPal', etc.
    Provider        VARCHAR(50),          -- 'Visa', 'Mastercard', 'PayPal', etc.
    AccountNumber   VARCHAR(255),         -- Encrypted in production
    ExpiryDate      VARCHAR(10),          -- MM/YY format - Encrypted in production
    IsDefault       BIT         NOT NULL DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users (UserID)
);
GO

-- Create Shipping Methods Table
CREATE TABLE ShippingMethods
(
    ShippingMethodID      INT PRIMARY KEY IDENTITY (1,1),
    MethodName            VARCHAR(50)    NOT NULL,
    Description           VARCHAR(255),
    BasePrice             DECIMAL(10, 2) NOT NULL,
    EstimatedDeliveryDays INT,
    IsActive              BIT            NOT NULL DEFAULT 1
);
GO

-- Create Orders Table
CREATE TABLE Orders
(
    OrderID           INT PRIMARY KEY IDENTITY (1,1),
    UserID            INT            NOT NULL,
    OrderDate         DATETIME       NOT NULL DEFAULT GETDATE(),
    ShippingAddressID INT            NOT NULL,
    BillingAddressID  INT            NOT NULL,
    PaymentMethodID   INT            NOT NULL,
    ShippingMethodID  INT            NOT NULL,
    OrderStatus       VARCHAR(50)    NOT NULL DEFAULT 'Pending', -- 'Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'
    SubTotal          DECIMAL(10, 2) NOT NULL,
    TaxAmount         DECIMAL(10, 2) NOT NULL,
    ShippingCost      DECIMAL(10, 2) NOT NULL,
    DiscountAmount    DECIMAL(10, 2) NOT NULL DEFAULT 0,
    TotalAmount       DECIMAL(10, 2) NOT NULL,
    TrackingNumber    VARCHAR(100),
    Notes             VARCHAR(500),
    FOREIGN KEY (UserID) REFERENCES Users (UserID),
    FOREIGN KEY (ShippingAddressID) REFERENCES Addresses (AddressID),
    FOREIGN KEY (BillingAddressID) REFERENCES Addresses (AddressID),
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods (PaymentMethodID),
    FOREIGN KEY (ShippingMethodID) REFERENCES ShippingMethods (ShippingMethodID)
);
GO

-- Create Order Items Table
CREATE TABLE OrderItems
(
    OrderItemID    INT PRIMARY KEY IDENTITY (1,1),
    OrderID        INT            NOT NULL,
    ProductID      INT            NOT NULL,
    VariantID      INT,
    Quantity       INT            NOT NULL,
    UnitPrice      DECIMAL(10, 2) NOT NULL,
    Subtotal       DECIMAL(10, 2) NOT NULL,
    TaxAmount      DECIMAL(10, 2) NOT NULL,
    DiscountAmount DECIMAL(10, 2) NOT NULL DEFAULT 0,
    TotalPrice     DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders (OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products (ProductID),
    FOREIGN KEY (VariantID) REFERENCES ProductVariants (VariantID)
);
GO

-- Create Carts Table
CREATE TABLE Carts
(
    CartID    INT PRIMARY KEY IDENTITY (1,1),
    UserID    INT,
    SessionID VARCHAR(100) NOT NULL, -- For guest users
    CreatedAt DATETIME     NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME     NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users (UserID)
);
GO

-- Create Cart Items Table
CREATE TABLE CartItems
(
    CartItemID INT PRIMARY KEY IDENTITY (1,1),
    CartID     INT      NOT NULL,
    ProductID  INT      NOT NULL,
    VariantID  INT,
    Quantity   INT      NOT NULL DEFAULT 1,
    DateAdded  DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CartID) REFERENCES Carts (CartID),
    FOREIGN KEY (ProductID) REFERENCES Products (ProductID),
    FOREIGN KEY (VariantID) REFERENCES ProductVariants (VariantID)
);
GO

-- Create index for common queries
CREATE INDEX IX_Orders_UserID ON Orders (UserID);
CREATE INDEX IX_Orders_OrderStatus ON Orders (OrderStatus);
CREATE INDEX IX_OrderItems_ProductID ON OrderItems (ProductID);
CREATE INDEX IX_Products_CategoryID ON Products (CategoryID);
CREATE INDEX IX_CartItems_CartID ON CartItems (CartID);
CREATE INDEX IX_Addresses_UserID ON Addresses (UserID);
GO