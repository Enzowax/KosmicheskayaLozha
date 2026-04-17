-- ============================================
-- БД: Космическая ложа
-- Скрипт создания и заполнения базы данных
-- ============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'KosmicheskayaLozha')
BEGIN
    ALTER DATABASE KosmicheskayaLozha SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE KosmicheskayaLozha;
END
GO

CREATE DATABASE KosmicheskayaLozha;
GO

USE KosmicheskayaLozha;
GO

-- ===================== ТАБЛИЦЫ =====================

CREATE TABLE Roles (
    Id   INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    Id         INT PRIMARY KEY IDENTITY(1,1),
    Login      NVARCHAR(100) NOT NULL UNIQUE,
    Password   NVARCHAR(100) NOT NULL,
    FirstName  NVARCHAR(100) NOT NULL,
    LastName   NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    Phone      NVARCHAR(20)  NULL,
    RoleId     INT NOT NULL REFERENCES Roles(Id),
    IsFrozen   BIT NOT NULL DEFAULT 0
);

CREATE TABLE ServiceTypes (
    Id    INT PRIMARY KEY IDENTITY(1,1),
    Name  NVARCHAR(100)  NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);

CREATE TABLE MasterServices (
    MasterId      INT NOT NULL REFERENCES Users(Id),
    ServiceTypeId INT NOT NULL REFERENCES ServiceTypes(Id),
    PRIMARY KEY (MasterId, ServiceTypeId)
);

CREATE TABLE Appointments (
    Id                  INT PRIMARY KEY IDENTITY(1,1),
    ClientId            INT NULL REFERENCES Users(Id),
    MasterId            INT NOT NULL REFERENCES Users(Id),
    ServiceTypeId       INT NOT NULL REFERENCES ServiceTypes(Id),
    AppointmentDateTime DATETIME NOT NULL,
    PaymentMethod       NVARCHAR(50) NULL,
    Comment             NVARCHAR(500) NULL,
    IsCompleted         BIT NOT NULL DEFAULT 0,
    IsCancelled         BIT NOT NULL DEFAULT 0
);

CREATE TABLE Manufacturers (
    Id   INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE ProductTypes (
    Id   INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    Id             INT PRIMARY KEY IDENTITY(1,1),
    Name           NVARCHAR(200)  NOT NULL,
    Price          DECIMAL(10,2)  NOT NULL,
    Description    NVARCHAR(1000) NULL,
    Discount       INT NOT NULL DEFAULT 0,
    ManufacturerId INT NOT NULL REFERENCES Manufacturers(Id),
    ProductTypeId  INT NOT NULL REFERENCES ProductTypes(Id),
    Rating         DECIMAL(3,2) NOT NULL DEFAULT 0,
    IsFrozen       BIT NOT NULL DEFAULT 0
);

CREATE TABLE Orders (
    Id            INT PRIMARY KEY IDENTITY(1,1),
    ClientId      INT NOT NULL REFERENCES Users(Id),
    OrderDate     DATETIME NOT NULL DEFAULT GETDATE(),
    DeliveryDate  DATE NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    IsClosed      BIT NOT NULL DEFAULT 0
);

CREATE TABLE OrderItems (
    Id        INT PRIMARY KEY IDENTITY(1,1),
    OrderId   INT NOT NULL REFERENCES Orders(Id),
    ProductId INT NOT NULL REFERENCES Products(Id),
    Quantity  INT NOT NULL DEFAULT 1,
    Price     DECIMAL(10,2) NOT NULL
);

CREATE TABLE CartItems (
    Id        INT PRIMARY KEY IDENTITY(1,1),
    UserId    INT NOT NULL REFERENCES Users(Id),
    ProductId INT NOT NULL REFERENCES Products(Id),
    Quantity  INT NOT NULL DEFAULT 1,
    UNIQUE (UserId, ProductId)
);

-- ===================== ДАННЫЕ =====================

INSERT INTO Roles (Name) VALUES
    (N'Клиент'),
    (N'Мастер'),
    (N'Менеджер'),
    (N'Администратор');

-- Пароли: admin123, manager123, master123, client123
INSERT INTO Users (Login, Password, FirstName, LastName, MiddleName, Phone, RoleId, IsFrozen) VALUES
    ('admin',    'admin123',   N'Иван',     N'Администраторов', N'Иванович',   N'+7 900 000-0001', 4, 0),
    ('manager1', 'manager123', N'Мария',    N'Менеджерова',     N'Сергеевна',  N'+7 900 000-0002', 3, 0),
    ('master1',  'master123',  N'Анна',     N'Мастерова',       N'Петровна',   N'+7 900 000-0003', 2, 0),
    ('master2',  'master123',  N'Елена',    N'Красивая',        N'Николаевна', N'+7 900 000-0004', 2, 0),
    ('client1',  'client123',  N'Пётр',     N'Клиентов',        N'Алексеевич', N'+7 900 000-0005', 1, 0),
    ('client2',  'client123',  N'Светлана', N'Покупателева',    N'Игоревна',   N'+7 900 000-0006', 1, 0);

INSERT INTO ServiceTypes (Name, Price) VALUES
    (N'Маникюр',              1500.00),
    (N'Педикюр',              2000.00),
    (N'Стрижка',              1000.00),
    (N'Окрашивание',          3000.00),
    (N'Наращивание ресниц',   2500.00);

-- master1 (Id=3): маникюр, педикюр, наращивание
-- master2 (Id=4): стрижка, окрашивание
INSERT INTO MasterServices (MasterId, ServiceTypeId) VALUES
    (3, 1), (3, 2), (3, 5),
    (4, 3), (4, 4);

INSERT INTO Manufacturers (Name) VALUES
    (N'L''Oreal'),
    (N'Maybelline'),
    (N'Chanel'),
    (N'MAC'),
    (N'NYX Professional Makeup');

INSERT INTO ProductTypes (Name) VALUES
    (N'Помада'),
    (N'Тушь'),
    (N'Тональный крем'),
    (N'Тени'),
    (N'Уход за кожей');

INSERT INTO Products (Name, Price, Description, Discount, ManufacturerId, ProductTypeId, Rating, IsFrozen) VALUES
    (N'Rouge Pur Couture',        2500.00, N'Роскошная губная помада с насыщенным пигментом и сатиновым финишем. Долгостойкая формула.',                    20, 3, 1, 4.5, 0),
    (N'Lash Sensational',          800.00, N'Объёмная тушь для ресниц с веерным эффектом. Захватывает каждую ресничку.',                                      0, 2, 2, 4.2, 0),
    (N'True Match Тональный',     1200.00, N'Лёгкий тональный крем с SPF 17. Идеально подходит к тону кожи.',                                                10, 1, 3, 4.0, 0),
    (N'Палетка теней Nude',       1800.00, N'12 матовых и шиммерных нюдовых оттенков. Стойкость до 12 часов.',                                               25, 4, 4, 4.7, 0),
    (N'Крем-уход SPF 50',         3500.00, N'Интенсивно увлажняющий дневной крем с защитой от солнца. Для всех типов кожи.',                                   0, 1, 5, 4.8, 0),
    (N'Matte Lip Color',           600.00, N'Матовая помада стойкого действия. Не сушит губы.',                                                                5, 5, 1, 3.9, 0),
    (N'Hydra Genius',             2200.00, N'Жидкое увлажняющее средство с гиалуроновой кислотой.',                                                           18, 1, 5, 4.3, 0),
    (N'Studio Fix Fluid',         3200.00, N'Тональное средство полного перекрытия с матовым финишем, SPF 15.',                                                0, 4, 3, 4.6, 0),
    (N'Fit Me Loose Powder',       950.00, N'Рассыпчатая пудра для матирования. Минимизирует поры.',                                                          30, 2, 3, 4.1, 0),
    (N'Can''t Stop Won''t Stop',  1100.00, N'Жидкий тональный крем полного перекрытия. 24-часовая стойкость.',                                                 0, 5, 3, 4.4, 0);

-- Генерируем слоты записей (свободные и занятые)
DECLARE @base DATETIME = CAST(CAST(GETDATE() AS DATE) AS DATETIME);

-- Слоты для master1 (Id=3)
INSERT INTO Appointments (ClientId, MasterId, ServiceTypeId, AppointmentDateTime) VALUES
    (NULL, 3, 1, DATEADD(hour,  9, DATEADD(day, 1, @base))),
    (NULL, 3, 1, DATEADD(hour, 10, DATEADD(day, 1, @base))),
    (NULL, 3, 1, DATEADD(hour, 11, DATEADD(day, 1, @base))),
    (NULL, 3, 2, DATEADD(hour, 13, DATEADD(day, 1, @base))),
    (NULL, 3, 2, DATEADD(hour, 14, DATEADD(day, 1, @base))),
    (NULL, 3, 5, DATEADD(hour, 16, DATEADD(day, 1, @base))),
    (NULL, 3, 1, DATEADD(hour,  9, DATEADD(day, 2, @base))),
    (NULL, 3, 1, DATEADD(hour, 10, DATEADD(day, 2, @base))),
    (NULL, 3, 5, DATEADD(hour, 12, DATEADD(day, 2, @base))),
    (5,    3, 1, DATEADD(hour, 14, DATEADD(day, 2, @base))),  -- занято client1
    (NULL, 3, 2, DATEADD(hour,  9, DATEADD(day, 3, @base))),
    (NULL, 3, 2, DATEADD(hour, 11, DATEADD(day, 3, @base)));

-- Слоты для master2 (Id=4)
INSERT INTO Appointments (ClientId, MasterId, ServiceTypeId, AppointmentDateTime) VALUES
    (NULL, 4, 3, DATEADD(hour,  9, DATEADD(day, 1, @base))),
    (NULL, 4, 3, DATEADD(hour, 10, DATEADD(day, 1, @base))),
    (NULL, 4, 4, DATEADD(hour, 14, DATEADD(day, 1, @base))),
    (NULL, 4, 4, DATEADD(hour, 15, DATEADD(day, 1, @base))),
    (6,    4, 3, DATEADD(hour, 11, DATEADD(day, 2, @base))),  -- занято client2
    (NULL, 4, 4, DATEADD(hour, 13, DATEADD(day, 2, @base))),
    (NULL, 4, 3, DATEADD(hour,  9, DATEADD(day, 3, @base)));

-- Обновляем оплату/комментарий занятых записей
UPDATE Appointments SET PaymentMethod = N'Наличные', Comment = N'Жду с нетерпением!'
WHERE ClientId = 5;

UPDATE Appointments SET PaymentMethod = N'Карта'
WHERE ClientId = 6;

-- Демо-заказ
INSERT INTO Orders (ClientId, OrderDate, DeliveryDate, PaymentMethod, IsClosed) VALUES
    (5, GETDATE(), DATEADD(day, 3, CAST(GETDATE() AS DATE)), N'Карта', 0);

INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) VALUES
    (1, 1, 1, 2500.00 * (1 - 20.0/100)),
    (1, 2, 2, 800.00);

PRINT N'База данных KosmicheskayaLozha успешно создана и заполнена!';
GO
