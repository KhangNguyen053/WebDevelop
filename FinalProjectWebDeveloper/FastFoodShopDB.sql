USE master
GO

IF DB_ID('FastFoodShopDB') IS NOT NULL
BEGIN
    ALTER DATABASE FastFoodShopDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE FastFoodShopDB
END
GO

CREATE DATABASE FastFoodShopDB
GO

USE FastFoodShopDB
GO

/*=====================================================
1. ROLE
=====================================================*/
CREATE TABLE Role
(
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
)

INSERT INTO Role(RoleName)
VALUES
('Admin'),
('User')

/*=====================================================
2. ACCOUNT
=====================================================*/
CREATE TABLE Account
(
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    IsActive BIT DEFAULT 1,

    FOREIGN KEY(RoleID) REFERENCES Role(RoleID)
)

/*=====================================================
3. CUSTOMER
=====================================================*/
CREATE TABLE Customer
(
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100),
    Email VARCHAR(100),
    Phone VARCHAR(20),
    Address NVARCHAR(255),
    AccountID INT UNIQUE,

    FOREIGN KEY(AccountID)
        REFERENCES Account(AccountID)
)

/*=====================================================
4. CATEGORY
=====================================================*/
CREATE TABLE Category
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100)
);

INSERT INTO Category(CategoryName)
VALUES
(N'Gà Rán'),
(N'Hamburger'),
(N'Pizza'),
(N'Đồ Ăn Khác'),
(N'Nước Uống');

/*=====================================================
5. PRODUCT
=====================================================*/
CREATE TABLE Product
(
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(200),
    ImageURL NVARCHAR(255),
    CategoryID INT,
    IsAvailable BIT DEFAULT 1,

    FOREIGN KEY(CategoryID)
        REFERENCES Category(CategoryID)
)

/*=====================================================
6. PRODUCT SIZE
=====================================================*/
CREATE TABLE ProductSize
(
    ProductSizeID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT,
    SizeName VARCHAR(20),
    Price DECIMAL(18,2),

    FOREIGN KEY(ProductID)
        REFERENCES Product(ProductID)
)

/*=====================================================
7. CART
=====================================================*/
CREATE TABLE Cart
(
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT UNIQUE,

    FOREIGN KEY(CustomerID)
        REFERENCES Customer(CustomerID)
)

/*=====================================================
8. CART ITEM
=====================================================*/
CREATE TABLE CartItem
(
    CartItemID INT IDENTITY(1,1) PRIMARY KEY,
    CartID INT,
    ProductSizeID INT,
    Quantity INT,

    FOREIGN KEY(CartID)
        REFERENCES Cart(CartID),

    FOREIGN KEY(ProductSizeID)
        REFERENCES ProductSize(ProductSizeID)
)

/*=====================================================
9. VOUCHER
=====================================================*/
CREATE TABLE Voucher
(
    VoucherID INT IDENTITY(1,1) PRIMARY KEY,
    VoucherCode VARCHAR(50) UNIQUE,
    DiscountPercent INT,
    StartDate DATE,
    EndDate DATE,
    IsActive BIT DEFAULT 1
)

/*=====================================================
10. ORDER HEADER
=====================================================*/
CREATE TABLE OrderHeader
(
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT,
    VoucherID INT NULL,

    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2),
    Status NVARCHAR(50),

    FOREIGN KEY(CustomerID)
        REFERENCES Customer(CustomerID),

    FOREIGN KEY(VoucherID)
        REFERENCES Voucher(VoucherID)
)

/*=====================================================
11. ORDER DETAIL
=====================================================*/
CREATE TABLE OrderDetail
(
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ProductSizeID INT,
    Quantity INT,
    UnitPrice DECIMAL(18,2),

    FOREIGN KEY(OrderID)
        REFERENCES OrderHeader(OrderID),

    FOREIGN KEY(ProductSizeID)
        REFERENCES ProductSize(ProductSizeID)
)

/*=====================================================
12. PAYMENT
=====================================================*/
CREATE TABLE Payment
(
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    PaymentMethod NVARCHAR(50),
    PaymentDate DATETIME,
    Amount DECIMAL(18,2),

    FOREIGN KEY(OrderID)
        REFERENCES OrderHeader(OrderID)
)

/*=====================================================
13. REVIEW
=====================================================*/
CREATE TABLE Review
(
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT,
    ProductID INT,
    Rating INT CHECK(Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(500),
    ReviewDate DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(CustomerID)
        REFERENCES Customer(CustomerID),

    FOREIGN KEY(ProductID)
        REFERENCES Product(ProductID)
)

/*=====================================================
TÀI KHOẢN
=====================================================*/
INSERT INTO Account(Username, PasswordHash, RoleID, IsActive)
VALUES
('admin','123456',1,1),
('user01','123456',2,1),
('user02','123456',2,1)

/*=====================================================
KHÁCH HÀNG
=====================================================*/
INSERT INTO Customer(FullName, Email, Phone, Address, AccountID)
VALUES
(N'Nguyễn Văn Đình An',
'nguyenvanan@gmail.com',
'0901111111',
N'TP Hồ Chí Minh',
2),

(N'Trần Thị Bích',
'tranthibich@gmail.com',
'0902222222',
N'Bình Dương',
3)

/*=====================================================
VOUCHER
=====================================================*/
INSERT INTO Voucher(VoucherCode, DiscountPercent, StartDate, EndDate, IsActive)
VALUES
('WELCOME10',10,'2026-01-01','2026-12-31',1),
('SALE20',20,'2026-01-01','2026-12-31',1),
('VIP30',30,'2026-01-01','2026-12-31',1)

/*=====================================================
30 SẢN PHẨM (Yêu cầu ít nhất 30 sản phẩm)
=====================================================*/
INSERT INTO Product(ProductName,ImageURL,CategoryID)
VALUES
-- Nhóm 1: Gà Rán (ID 1-5)
(N'Gà Rán Chiên Giòn','Id 1.png',1),
(N'Gà Viên Lắc Phô Mai','Id 2.png',1),
(N'Gà Sốt Cay','Id 3.png',1),
(N'Gà Sốt BBQ','Id 4.png',1),
(N'Gà Sốt Chua Ngọt','Id 5.png',1),

-- Nhóm 2: Hambuger (ID 6-11)
(N'Hamburger Bò','Id 6.png',2),
(N'Hamburger Tôm','Id 7.png',2),
(N'Hamburger Gà ','Id 8.png',2),
(N'Hamburger Cá','Id 9.png',2),
(N'Hamburger Chay','Id 10.png',2),
(N'Hamburger Heo','Id 11.png',2),

-- Nhóm 3: Pizza (ID 12-17)
(N'Pizza Phô Mai ','Id 12.png',3),
(N'Pizza Xúc Xích Thịt Xông Khói','Id 13.png',3),
(N'Pizza Hải Sản','Id 14.png',3),
(N'Pizza Xúc Xích Thanh Cua','Id 15.png',3),
(N'Pizza Thập Cẩm','Id 16.png',3),
(N'Pizza Thanh Đạm','Id 17.png',3),

-- Nhóm 4: Đồ Ăn Khác (ID 18-23)
(N'Khoai Tây Chiên','Id 18.png',4),
(N'Khoai Tây Lắc Phô Mai','Id 19.png',4),
(N'Rau củ trộn','Id 20.png',4),
(N'Bánh Táo','Id 21.png',4),
(N'Kem Ốc Quế','Id 22.png',4),
(N'Kem Ly','Id 23.png',4),

-- Nhóm 5: Nước Uống (ID 24-30)
(N'Pepsi','Id 24.png',5),
(N'Coca','Id 25.png',5),
(N'7Up','Id 26.png',5),
(N'Milo','Id 27.png',5),
(N'Fanta Cam','Id 28.png',5),
(N'Mirinda Soda Kem','Id 29.png',5),
(N'Nước Suối','Id 30.png',5)

/*=====================================================
THÊM DỮ LIỆU ProductSize
5 món gà rán  (ID 1-5)
6 món hamburger (ID 6-11)
6 món pizza × 2 size (ID 12-17)
6 đồ ăn khác (ID 18-23)
7 loại nước uống x 2 size (ID 24-30)
Tổng cộng: 5 + 6 + 12 + 6 + 14 = 43 bản ghi ProductSize
=====================================================*/
INSERT INTO ProductSize(ProductID,SizeName,Price)
VALUES
-- =========================
-- GÀ RÁN (1-5)
-- =========================
(1,'1 mieng',25000),
(2,'1 phan',30000),
(3,'1 mieng',30000),
(4,'1 mieng',30000),
(5,'1 mieng',30000),

-- =========================
-- HAMBURGER (6-11)
-- =========================
(6,'1 cái',29000),
(7,'1 cái',32000),
(8,'1 cái',35000),
(9,'1 cái',35000),
(10,'1 cái',25000),
(11,'1 cái',35000),

-- =========================
-- PIZZA (12-17)
-- =========================
(12,'Size M',50000),
(12,'Size L',100000),

(13,'Size M',75000),
(13,'Size L',150000),

(14,'Size M',90000),
(14,'Size L',175000),

(15,'Size M',70000),
(15,'Size L',140000),

(16,'Size M',100000),
(16,'Size L',200000),

(17,'Size M',50000),
(17,'Size L',100000),

-- =========================
-- ĐỒ ĂN KHÁC (18-23)
-- =========================
(18,'1 phan',30000),
(19,'1 phan',35000),
(20,'1 phan',30000),
(21,'1 cái',25000),
(22,'1 cây',15000),
(23,'1 ly',25000),

-- =========================
-- NƯỚC UỐNG (24-30)
-- =========================
(24,'Chai',15000),
(24,'Combo 2 chai',28000),

(25,'Chai',15000),
(25,'Combo 2 chai',28000),

(26,'Chai',15000),
(26,'Combo 2 chai',28000),

(27,'Ly',18000),
(27,'Combo 2 ly',30000),

(28,'Chai',15000),
(28,'Combo 2 chai',28000),

(29,'Chai',15000),
(29,'Combo 2 chai',28000),

(30,'500ml',10000),
(30,'1.5L',18000);


/*=====================================================
REVIEW MẪU
=====================================================*/
INSERT INTO Review(CustomerID,ProductID,Rating,Comment)
VALUES
(1,1,5,N'Gà rất giòn và ngon'),
(1,6,5,N'Hamburger bò rất thơm'),
(2,18,4,N'Khoai tây chiên giòn, giá hợp lý');

/*=====================================================
ĐƠN HÀNG MẪU
=====================================================*/

-- 25/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Gà Chiên Giòn (25.000) + Pepsi (15.000) + Khoai Tây Chiên (30.000)
(1,1,'2026-06-25 10:30:00',70000,N'Đã thanh toán'),

-- Pizza Hải Sản Size M (90.000) + Coca (15.000)
(2,NULL,'2026-06-25 15:45:00',105000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(1,N'MoMo','2026-06-25 10:31:00',70000),
(2,N'Tiền mặt','2026-06-25 15:46:00',105000);


-- 24/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Hamburger Bò (29.000) + Pepsi (15.000) + Kem Ly (25.000)
(1,NULL,'2026-06-24 09:15:00',69000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(3,N'Chuyển khoản','2026-06-24 09:16:00',69000);


-- 23/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Pizza Thập Cẩm Size L (200.000)
(2,2,'2026-06-23 14:20:00',200000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(4,N'MoMo','2026-06-23 14:21:00',200000);


-- 22/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Gà BBQ (30.000) + Pepsi (15.000)
(1,NULL,'2026-06-22 17:00:00',45000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(5,N'Tiền mặt','2026-06-22 17:01:00',45000);


-- 21/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Hamburger Gà (35.000) + Coca (15.000) + Kem Ốc Quế (15.000)
(2,NULL,'2026-06-21 11:10:00',65000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(6,N'MoMo','2026-06-21 11:11:00',65000);


-- 20/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Pizza Hải Sản Size L (175.000)
(1,3,'2026-06-20 16:30:00',175000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(7,N'Tiền mặt','2026-06-20 16:31:00',175000);


-- 19/06/2026
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Hamburger Tôm (32.000) + Khoai Tây Chiên (30.000) + Pepsi (15.000)
(2,NULL,'2026-06-19 13:00:00',77000,N'Đã thanh toán');

INSERT INTO Payment(OrderID,PaymentMethod,PaymentDate,Amount)
VALUES
(8,N'Chuyển khoản','2026-06-19 13:01:00',77000);


-- Đơn hàng mới
INSERT INTO OrderHeader(CustomerID,VoucherID,OrderDate,TotalAmount,Status)
VALUES
-- Hamburger Cá (35.000) + Fanta (15.000)
(1,NULL,'2026-06-25 18:00:00',50000,N'Mới');

GO