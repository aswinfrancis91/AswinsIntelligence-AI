-- Insert Categories
INSERT INTO Categories (CategoryName, Description, ParentCategoryID, IsActive)
VALUES ('Electronics', 'Electronic devices and accessories', NULL, 1),
       ('Clothing', 'Apparel and fashion items', NULL, 1),
       ('Home & Garden', 'Home improvement and garden supplies', NULL, 1),
       ('Books', 'Books and educational materials', NULL, 1),
       ('Sports & Outdoors', 'Sports equipment and outdoor gear', NULL, 1),
       ('Smartphones', 'Mobile phones and accessories', 1, 1),
       ('Laptops', 'Portable computers', 1, 1),
       ('Mens Clothing', 'Clothing for men', 2, 1),
       ('Womens Clothing', 'Clothing for women', 2, 1),
       ('Furniture', 'Home furniture', 3, 1);

-- Insert Users
INSERT INTO Users (Email, Password, FirstName, LastName, Phone, CreatedAt, IsActive)
VALUES ('john.doe@email.com', 'hashed_password_1', 'John', 'Doe', '555-0101', '2024-01-15', 1),
       ('jane.smith@email.com', 'hashed_password_2', 'Jane', 'Smith', '555-0102', '2024-01-20', 1),
       ('mike.johnson@email.com', 'hashed_password_3', 'Mike', 'Johnson', '555-0103', '2024-02-01', 1),
       ('sarah.williams@email.com', 'hashed_password_4', 'Sarah', 'Williams', '555-0104', '2024-02-10', 1),
       ('david.brown@email.com', 'hashed_password_5', 'David', 'Brown', '555-0105', '2024-02-15', 1),
       ('lisa.davis@email.com', 'hashed_password_6', 'Lisa', 'Davis', '555-0106', '2024-03-01', 1),
       ('tom.wilson@email.com', 'hashed_password_7', 'Tom', 'Wilson', '555-0107', '2024-03-10', 1),
       ('amy.garcia@email.com', 'hashed_password_8', 'Amy', 'Garcia', '555-0108', '2024-03-15', 1),
       ('chris.martinez@email.com', 'hashed_password_9', 'Chris', 'Martinez', '555-0109', '2024-04-01', 1),
       ('emma.anderson@email.com', 'hashed_password_10', 'Emma', 'Anderson', '555-0110', '2024-04-10', 1),
       ('alex.taylor@email.com', 'hashed_password_11', 'Alex', 'Taylor', '555-0111', '2024-04-15', 1),
       ('maria.lopez@email.com', 'hashed_password_12', 'Maria', 'Lopez', '555-0112', '2024-05-01', 1);

-- Insert Addresses
INSERT INTO Addresses (UserID, AddressLine1, AddressLine2, City, State, PostalCode, Country, IsDefault, AddressType)
VALUES (1, '123 Main St', 'Apt 4B', 'New York', 'NY', '10001', 'United States', 1, 'Billing'),
       (1, '123 Main St', 'Apt 4B', 'New York', 'NY', '10001', 'United States', 1, 'Shipping'),
       (2, '456 Oak Ave', NULL, 'Los Angeles', 'CA', '90210', 'United States', 1, 'Billing'),
       (2, '789 Pine St', NULL, 'Los Angeles', 'CA', '90211', 'United States', 0, 'Shipping'),
       (3, '321 Elm Dr', NULL, 'Chicago', 'IL', '60601', 'United States', 1, 'Billing'),
       (3, '321 Elm Dr', NULL, 'Chicago', 'IL', '60601', 'United States', 1, 'Shipping'),
       (4, '654 Maple Ln', NULL, 'Houston', 'TX', '77001', 'United States', 1, 'Billing'),
       (4, '654 Maple Ln', NULL, 'Houston', 'TX', '77001', 'United States', 1, 'Shipping'),
       (5, '987 Cedar Rd', NULL, 'Phoenix', 'AZ', '85001', 'United States', 1, 'Billing'),
       (5, '987 Cedar Rd', NULL, 'Phoenix', 'AZ', '85001', 'United States', 1, 'Shipping'),
       (6, '147 Birch St', NULL, 'Philadelphia', 'PA', '19101', 'United States', 1, 'Billing'),
       (6, '147 Birch St', NULL, 'Philadelphia', 'PA', '19101', 'United States', 1, 'Shipping'),
       (7, '258 Spruce Ave', NULL, 'San Antonio', 'TX', '78201', 'United States', 1, 'Billing'),
       (7, '258 Spruce Ave', NULL, 'San Antonio', 'TX', '78201', 'United States', 1, 'Shipping'),
       (8, '369 Willow Dr', NULL, 'San Diego', 'CA', '92101', 'United States', 1, 'Billing'),
       (8, '369 Willow Dr', NULL, 'San Diego', 'CA', '92101', 'United States', 1, 'Shipping'),
       (9, '741 Poplar St', NULL, 'Dallas', 'TX', '75201', 'United States', 1, 'Billing'),
       (9, '741 Poplar St', NULL, 'Dallas', 'TX', '75201', 'United States', 1, 'Shipping'),
       (10, '852 Hickory Ln', NULL, 'San Jose', 'CA', '95101', 'United States', 1, 'Billing'),
       (10, '852 Hickory Ln', NULL, 'San Jose', 'CA', '95101', 'United States', 1, 'Shipping');

-- Insert Products
INSERT INTO Products (SKU, ProductName, Description, CategoryID, Price, Weight, Dimensions, QuantityInStock, IsActive)
VALUES ('PHONE-001', 'iPhone 15 Pro', 'Latest iPhone with advanced camera system', 6, 999.99, 0.5, '6.1x2.8x0.3', 50,
        1),
       ('PHONE-002', 'Samsung Galaxy S24', 'Premium Android smartphone', 6, 899.99, 0.6, '6.2x2.9x0.3', 45, 1),
       ('PHONE-003', 'Google Pixel 8', 'AI-powered smartphone with pure Android', 6, 699.99, 0.55, '6.0x2.8x0.3', 30,
        1),
       ('LAPTOP-001', 'MacBook Pro 14"', 'Professional laptop with M3 chip', 7, 1999.99, 3.5, '12.3x8.7x0.6', 25, 1),
       ('LAPTOP-002', 'Dell XPS 13', 'Ultra-portable Windows laptop', 7, 1299.99, 2.8, '11.6x7.8x0.6', 20, 1),
       ('LAPTOP-003', 'HP Spectre x360', 'Convertible 2-in-1 laptop', 7, 1149.99, 3.0, '12.1x8.5x0.7', 15, 1),
       ('SHIRT-001', 'Cotton T-Shirt', 'Comfortable cotton t-shirt', 8, 19.99, 0.3, '28x20x1', 100, 1),
       ('SHIRT-002', 'Polo Shirt', 'Classic polo shirt for men', 8, 39.99, 0.4, '28x20x1', 80, 1),
       ('DRESS-001', 'Summer Dress', 'Lightweight summer dress', 9, 49.99, 0.5, '32x24x1', 60, 1),
       ('JEANS-001', 'Denim Jeans', 'Classic blue jeans', 8, 79.99, 1.2, '32x30x2', 70, 1),
       ('CHAIR-001', 'Office Chair', 'Ergonomic office chair', 10, 299.99, 25.0, '26x26x45', 20, 1),
       ('DESK-001', 'Standing Desk', 'Height-adjustable standing desk', 10, 599.99, 85.0, '60x30x50', 10, 1),
       ('BOOK-001', 'Programming Guide', 'Complete guide to modern programming', 4, 29.99, 1.5, '9x6x1', 50, 1),
       ('BOOK-002', 'Data Science Handbook', 'Comprehensive data science reference', 4, 39.99, 2.0, '10x7x1.5', 40, 1),
       ('SPORTS-001', 'Running Shoes', 'Professional running shoes', 5, 129.99, 1.0, '12x5x4', 35, 1);

-- Insert Product Variants
INSERT INTO ProductVariants (ProductID, VariantName, SKU, AdditionalCost, QuantityInStock, IsActive)
VALUES (1, 'iPhone 15 Pro - 128GB', 'PHONE-001-128', 0.00, 25, 1),
       (1, 'iPhone 15 Pro - 256GB', 'PHONE-001-256', 100.00, 20, 1),
       (1, 'iPhone 15 Pro - 512GB', 'PHONE-001-512', 300.00, 5, 1),
       (7, 'Cotton T-Shirt - Small', 'SHIRT-001-S', 0.00, 30, 1),
       (7, 'Cotton T-Shirt - Medium', 'SHIRT-001-M', 0.00, 40, 1),
       (7, 'Cotton T-Shirt - Large', 'SHIRT-001-L', 0.00, 30, 1),
       (9, 'Summer Dress - Small', 'DRESS-001-S', 0.00, 20, 1),
       (9, 'Summer Dress - Medium', 'DRESS-001-M', 0.00, 25, 1),
       (9, 'Summer Dress - Large', 'DRESS-001-L', 0.00, 15, 1),
       (15, 'Running Shoes - Size 8', 'SPORTS-001-8', 0.00, 10, 1),
       (15, 'Running Shoes - Size 9', 'SPORTS-001-9', 0.00, 15, 1),
       (15, 'Running Shoes - Size 10', 'SPORTS-001-10', 0.00, 10, 1);

-- Insert Tax Rates
INSERT INTO TaxRates (State, County, City, ZipCode, TaxRate, IsActive, EffectiveDate)
VALUES ('NY', 'New York', 'New York', '10001', 8.25, 1, '2024-01-01'),
       ('CA', 'Los Angeles', 'Los Angeles', '90210', 9.50, 1, '2024-01-01'),
       ('IL', 'Cook', 'Chicago', '60601', 10.00, 1, '2024-01-01'),
       ('TX', 'Harris', 'Houston', '77001', 8.25, 1, '2024-01-01'),
       ('AZ', 'Maricopa', 'Phoenix', '85001', 8.60, 1, '2024-01-01'),
       ('PA', 'Philadelphia', 'Philadelphia', '19101', 8.00, 1, '2024-01-01');

-- Insert Discounts
INSERT INTO Discounts (DiscountCode, Description, DiscountType, DiscountValue, MinimumPurchase, IsActive, StartDate,
                       EndDate)
VALUES ('WELCOME10', '10% off for new customers', 'Percentage', 10.00, 50.00, 1, '2024-01-01', '2024-12-31'),
       ('SUMMER20', '20% off summer collection', 'Percentage', 20.00, 100.00, 1, '2024-06-01', '2024-08-31'),
       ('TECH50', '$50 off electronics over $500', 'Fixed Amount', 50.00, 500.00, 1, '2024-01-01', '2024-12-31'),
       ('FREESHIP', 'Free shipping on orders over $75', 'Fixed Amount', 0.00, 75.00, 1, '2024-01-01', '2024-12-31'),
       ('BLACKFRIDAY', '25% off everything', 'Percentage', 25.00, 0.00, 0, '2024-11-29', '2024-11-29');

-- Insert Payment Methods
INSERT INTO PaymentMethods (UserID, PaymentType, Provider, AccountNumber, ExpiryDate, IsDefault)
VALUES (1, 'Credit Card', 'Visa', '****1234', '12/26', 1),
       (2, 'Credit Card', 'Mastercard', '****5678', '10/25', 1),
       (3, 'Credit Card', 'Amex', '****9012', '08/27', 1),
       (4, 'PayPal', 'PayPal', 'sarah.williams@email.com', NULL, 1),
       (5, 'Credit Card', 'Visa', '****3456', '06/26', 1),
       (6, 'Credit Card', 'Mastercard', '****7890', '04/25', 1),
       (7, 'Credit Card', 'Visa', '****2345', '02/27', 1),
       (8, 'PayPal', 'PayPal', 'amy.garcia@email.com', NULL, 1),
       (9, 'Credit Card', 'Visa', '****6789', '09/26', 1),
       (10, 'Credit Card', 'Mastercard', '****0123', '11/25', 1);

-- Insert Shipping Methods
INSERT INTO ShippingMethods (MethodName, Description, BasePrice, EstimatedDeliveryDays, IsActive)
VALUES ('Standard Shipping', 'Standard ground shipping', 5.99, 5, 1),
       ('Express Shipping', 'Expedited shipping', 12.99, 2, 1),
       ('Overnight Shipping', 'Next day delivery', 24.99, 1, 1),
       ('Free Economy', 'Free shipping for orders over $75', 0.00, 7, 1),
       ('Premium Delivery', 'White glove delivery service', 49.99, 3, 1);

-- Insert Orders
INSERT INTO Orders (UserID, OrderDate, ShippingAddressID, BillingAddressID, PaymentMethodID, ShippingMethodID,
                    OrderStatus, SubTotal, TaxAmount, ShippingCost, DiscountAmount, TotalAmount, TrackingNumber)
VALUES (1, '2024-02-01', 2, 1, 1, 1, 'Delivered', 999.99, 82.50, 5.99, 0.00, 1088.48, 'TRK001234567'),
       (2, '2024-02-15', 4, 3, 2, 2, 'Delivered', 1299.99, 123.50, 12.99, 0.00, 1436.48, 'TRK001234568'),
       (3, '2024-03-01', 6, 5, 3, 1, 'Delivered', 59.98, 6.00, 5.99, 5.00, 66.97, 'TRK001234569'),
       (4, '2024-03-10', 8, 7, 4, 4, 'Delivered', 149.97, 12.37, 0.00, 29.99, 132.35, 'TRK001234570'),
       (5, '2024-03-15', 10, 9, 5, 1, 'Delivered', 899.99, 77.40, 5.99, 0.00, 983.38, 'TRK001234571'),
       (6, '2024-04-01', 12, 11, 6, 2, 'Shipped', 299.99, 24.00, 12.99, 0.00, 336.98, 'TRK001234572'),
       (7, '2024-04-05', 14, 13, 7, 1, 'Processing', 79.99, 6.60, 5.99, 0.00, 92.58, NULL),
       (8, '2024-04-10', 16, 15, 8, 3, 'Delivered', 49.99, 4.00, 24.99, 0.00, 78.98, 'TRK001234573'),
       (9, '2024-04-15', 18, 17, 9, 1, 'Delivered', 699.99, 57.75, 5.99, 50.00, 713.73, 'TRK001234574'),
       (10, '2024-04-20', 20, 19, 10, 2, 'Shipped', 129.99, 10.40, 12.99, 0.00, 153.38, 'TRK001234575'),
       (1, '2024-05-01', 2, 1, 1, 1, 'Delivered', 39.99, 3.30, 5.99, 0.00, 49.28, 'TRK001234576'),
       (2, '2024-05-05', 4, 3, 2, 4, 'Processing', 599.99, 57.00, 0.00, 0.00, 656.99, NULL),
       (3, '2024-05-10', 6, 5, 3, 1, 'Pending', 19.99, 2.00, 5.99, 0.00, 27.98, NULL),
       (11, '2024-05-15', 1, 1, 1, 2, 'Delivered', 1999.99, 165.00, 12.99, 0.00, 2177.98, 'TRK001234577'),
       (12, '2024-05-20', 3, 3, 2, 1, 'Shipped', 249.97, 20.62, 5.99, 0.00, 276.58, 'TRK001234578');

-- Insert Order Items
INSERT INTO OrderItems (OrderID, ProductID, VariantID, Quantity, UnitPrice, Subtotal, TaxAmount, DiscountAmount,
                        TotalPrice)
VALUES (1, 1, 1, 1, 999.99, 999.99, 82.50, 0.00, 1082.49),
       (2, 5, NULL, 1, 1299.99, 1299.99, 123.50, 0.00, 1423.49),
       (3, 7, 4, 2, 19.99, 39.98, 4.00, 0.00, 43.98),
       (3, 8, NULL, 1, 39.99, 39.98, 2.00, 5.00, 36.98),
       (4, 9, 7, 2, 49.99, 99.98, 8.25, 19.99, 88.24),
       (4, 7, 5, 1, 19.99, 19.99, 1.65, 0.00, 21.64),
       (4, 8, NULL, 1, 39.99, 29.99, 2.47, 10.00, 22.46),
       (5, 2, NULL, 1, 899.99, 899.99, 77.40, 0.00, 977.39),
       (6, 11, NULL, 1, 299.99, 299.99, 24.00, 0.00, 323.99),
       (7, 10, NULL, 1, 79.99, 79.99, 6.60, 0.00, 86.59),
       (8, 9, 8, 1, 49.99, 49.99, 4.00, 0.00, 53.99),
       (9, 3, NULL, 1, 699.99, 699.99, 57.75, 50.00, 707.74),
       (10, 15, 11, 1, 129.99, 129.99, 10.40, 0.00, 140.39),
       (11, 8, NULL, 1, 39.99, 39.99, 3.30, 0.00, 43.29),
       (12, 12, NULL, 1, 599.99, 599.99, 57.00, 0.00, 656.99),
       (13, 7, 4, 1, 19.99, 19.99, 2.00, 0.00, 21.99),
       (14, 4, NULL, 1, 1999.99, 1999.99, 165.00, 0.00, 2164.99),
       (15, 13, NULL, 1, 29.99, 29.99, 2.47, 0.00, 32.46),
       (15, 14, NULL, 1, 39.99, 39.99, 3.30, 0.00, 43.29),
       (15, 15, 10, 1, 129.99, 129.99, 10.72, 0.00, 140.71),
       (15, 7, 5, 2, 19.99, 39.98, 3.30, 0.00, 43.28),
       (15, 8, NULL, 1, 39.99, 39.99, 3.30, 0.00, 43.29);

-- Insert Carts (Active shopping carts)
INSERT INTO Carts (UserID, SessionID, CreatedAt, UpdatedAt)
VALUES (1, 'SESS_001', '2024-05-25', '2024-05-25'),
       (2, 'SESS_002', '2024-05-24', '2024-05-25'),
       (11, 'SESS_011', '2024-05-25', '2024-05-25'),
       (12, 'SESS_012', '2024-05-23', '2024-05-25'),
       (NULL, 'GUEST_001', '2024-05-25', '2024-05-25');

-- Insert Cart Items
INSERT INTO CartItems (CartID, ProductID, VariantID, Quantity, DateAdded)
VALUES (1, 6, NULL, 1, '2024-05-25'),
       (1, 15, 12, 1, '2024-05-25'),
       (2, 7, 6, 2, '2024-05-24'),
       (2, 11, NULL, 1, '2024-05-25'),
       (3, 1, 2, 1, '2024-05-25'),
       (4, 9, 9, 1, '2024-05-23'),
       (4, 13, NULL, 1, '2024-05-25'),
       (5, 2, NULL, 1, '2024-05-25'),
       (5, 14, NULL, 1, '2024-05-25');

-- Insert Product Discounts (Many-to-Many relationships)
INSERT INTO ProductDiscounts (ProductID, DiscountID)
VALUES (1, 3),
       (2, 3),
       (3, 3),
       (4, 3),
       (5, 3),
       (6, 3),  -- Electronics get TECH50 discount
       (7, 2),
       (8, 2),
       (9, 2),
       (10, 2), -- Clothing gets SUMMER20 discount
       (15, 2); -- Sports item gets SUMMER20 discount