USE RAHHAL;
GO
-- Create Country table with Unicode support
CREATE TABLE Country (
    CountryID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName NVARCHAR(255) NOT NULL,
    Continent NVARCHAR(255) NOT NULL,
    CONSTRAINT country_countryname_unique UNIQUE (CountryName)
);

ALTER TABLE Country
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;



-- Create User table with Unicode support (quoted as it's often a reserved word)
CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL, -- Note: Store only hashed passwords in production
    JoinDate DATE NOT NULL,
    CONSTRAINT user_email_unique UNIQUE (Email)
);

ALTER TABLE [User]
ADD Role NVARCHAR(50) NOT NULL 
    DEFAULT 'Regular' 
    CHECK (Role IN ('Admin', 'Regular'));

ALTER TABLE [User]
ADD 
    failed_attempts INT NOT NULL DEFAULT 0,
    last_attempt DATETIME;

ALTER TABLE [User]
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;


INSERT INTO [User] (UserName, Email, Password, JoinDate, Role, failed_attempts, last_attempt, IsDeleted, UpdatedAt)
VALUES 
    ('admin', 'admin@example.com', 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7', GETDATE(), 'Admin', 0, GETDATE(), 0, NULL),
    ('user1', 'user1@example.com', '7ce5244ba2ba7e5db8babdfbe89af1e9852e5f7697c695a0cd08188c9ddc2cc8', GETDATE(), 'Regular', 0, GETDATE(), 0, NULL);



-- Create City table with Unicode support
CREATE TABLE City (
    CityID INT IDENTITY(1,1) PRIMARY KEY,
    CountryID INTEGER NOT NULL,
    CityName NVARCHAR(255) NOT NULL,
    CONSTRAINT city_unique_combination UNIQUE (CountryID, CityName),
    CONSTRAINT city_countryid_foreign FOREIGN KEY (CountryID) 
        REFERENCES Country(CountryID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

ALTER TABLE City
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;


-- Create Trip table with Unicode support
CREATE TABLE Trip (
    TripID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INTEGER NOT NULL,
    TripName NVARCHAR(255) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TravelMethod NVARCHAR(255) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    CONSTRAINT trip_userid_foreign FOREIGN KEY (UserID) 
        REFERENCES "User"(UserID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

ALTER TABLE Trip
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;


-- Create CityVisit table with Unicode support
CREATE TABLE CityVisit (
    VisitID INT IDENTITY(1,1) PRIMARY KEY,
    TripID INTEGER NOT NULL,
    CityID INTEGER NOT NULL,
    VisitDate DATE NOT NULL,
    Rating NVARCHAR(1) NOT NULL CHECK (Rating IN ('1', '2', '3', '4', '5')),
    Notes NVARCHAR(MAX) NOT NULL,
    CONSTRAINT cityvisit_tripid_foreign FOREIGN KEY (TripID) 
        REFERENCES Trip(TripID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT cityvisit_cityid_foreign FOREIGN KEY (CityID) 
        REFERENCES City(CityID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

ALTER TABLE CityVisit
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;


-- Create Phrase table with Unicode support
CREATE TABLE Phrase (
    PhraseID INT IDENTITY(1,1) PRIMARY KEY,
    VisitID INTEGER NOT NULL,
    OriginalText NVARCHAR(MAX) NOT NULL,
    Translation NVARCHAR(MAX) NOT NULL,
    Language NVARCHAR(255) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    CONSTRAINT phrase_visitid_foreign FOREIGN KEY (VisitID) 
        REFERENCES CityVisit(VisitID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

ALTER TABLE Phrase
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;


-- Create UserActivityLog table to track user actions
CREATE TABLE UserActivityLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    UserName NVARCHAR(100) NOT NULL,
    ActionType NVARCHAR(50) NOT NULL,
    ActionDetails NVARCHAR(MAX) NULL,
    Timestamp DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_useractivity_user FOREIGN KEY (UserID)
        REFERENCES [User](UserID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

ALTER TABLE UserActivityLog
ADD 
    IsDeleted BIT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NULL;

-- Entering country names into the countries table
INSERT INTO Country (CountryName, Continent)
VALUES
    ('Libya', 'Africa'),              -- 1
    ('Egypt', 'Africa'),              -- 2
    ('Italy', 'Europe'),              -- 3
    ('France', 'Europe'),             -- 4
    ('United States', 'North America'),--5
    ('Japan', 'Asia'),                -- 6
    ('Australia', 'Oceania'),         -- 7
    ('Brazil', 'South America'),      -- 8
    ('Canada', 'North America'),      -- 9
    ('Germany', 'Europe'),            --10
    ('Spain', 'Europe'),              --11
    ('Mexico', 'North America'),      --12
    ('New Zealand', 'Oceania'),       --13
    ('South Africa', 'Africa'),       --14
    ('Thailand', 'Asia'),             --15
    ('Iceland', 'Europe'),            --16
    ('Maldives', 'Asia'),             --17
    ('Bahamas', 'North America'),     --18
    ('Jamaica', 'North America'),     --19
    ('Fiji', 'Oceania'),              --20
    ('South Korea', 'Asia'),          --21   
    ('China', 'Asia'),                --22  
    ('Greece', 'Europe'),             --23  
    ('Switzerland', 'Europe'),        --24  
    ('Austria', 'Europe'),            --25  
    ('Kazakhstan', 'Asia');           --26  


--Entering city names into the cities table
-- Libya (CountryID = 1)
INSERT INTO City (CountryID, CityName) VALUES
    (1, 'Tripoli'),
    (1, 'Benghazi'),
    (1, 'Misrata');

-- Egypt (CountryID = 2)
INSERT INTO City (CountryID, CityName) VALUES
    (2, 'Cairo'),
    (2, 'Alexandria'),
    (2, 'Luxor');

-- Italy (CountryID = 3)
INSERT INTO City (CountryID, CityName) VALUES
    (3, 'Rome'),
    (3, 'Milan'),
    (3, 'Naples');

-- France (CountryID = 4)
INSERT INTO City (CountryID, CityName) VALUES
    (4, 'Paris'),
    (4, 'Lyon'),
    (4, 'Nice');

-- United States (CountryID = 5)
INSERT INTO City (CountryID, CityName) VALUES
    (5, 'New York'),
    (5, 'Los Angeles'),
    (5, 'Chicago');

-- Japan (CountryID = 6)
INSERT INTO City (CountryID, CityName) VALUES
    (6, 'Tokyo'),
    (6, 'Kyoto'),
    (6, 'Osaka');

-- Australia (CountryID = 7)
INSERT INTO City (CountryID, CityName) VALUES
    (7, 'Sydney'),
    (7, 'Melbourne'),
    (7, 'Brisbane');

-- Brazil (CountryID = 8)
INSERT INTO City (CountryID, CityName) VALUES
    (8, 'Rio de Janeiro'),
    (8, 'São Paulo'),
    (8, 'Brasília');

-- Canada (CountryID = 9)
INSERT INTO City (CountryID, CityName) VALUES
    (9, 'Toronto'),
    (9, 'Vancouver'),
    (9, 'Montreal');

-- Germany (CountryID = 10)
INSERT INTO City (CountryID, CityName) VALUES
    (10, 'Berlin'),
    (10, 'Munich'),
    (10, 'Hamburg');

-- Spain (CountryID = 11)
INSERT INTO City (CountryID, CityName) VALUES
    (11, 'Madrid'),
    (11, 'Barcelona'),
    (11, 'Valencia');

-- Mexico (CountryID = 12)
INSERT INTO City (CountryID, CityName) VALUES
    (12, 'Mexico City'),
    (12, 'Cancún'),
    (12, 'Guadalajara');

-- New Zealand (CountryID = 13)
INSERT INTO City (CountryID, CityName) VALUES
    (13, 'Auckland'),
    (13, 'Wellington'),
    (13, 'Christchurch');

-- South Africa (CountryID = 14)
INSERT INTO City (CountryID, CityName) VALUES
    (14, 'Johannesburg'),
    (14, 'Cape Town'),
    (14, 'Durban');

-- Thailand (CountryID = 15)
INSERT INTO City (CountryID, CityName) VALUES
    (15, 'Bangkok'),
    (15, 'Chiang Mai'),
    (15, 'Phuket');

-- Iceland (CountryID = 16)
INSERT INTO City (CountryID, CityName) VALUES
    (16, 'Reykjavik'),
    (16, 'Akureyri'),
    (16, 'Keflavik');

-- Maldives (CountryID = 17)
INSERT INTO City (CountryID, CityName) VALUES
    (17, 'Malé'),
    (17, 'Addu City'),
    (17, 'Fuvahmulah');

-- Bahamas (CountryID = 18)
INSERT INTO City (CountryID, CityName) VALUES
    (18, 'Nassau'),
    (18, 'Freeport'),
    (18, 'Marsh Harbour');

-- Jamaica (CountryID = 19)
INSERT INTO City (CountryID, CityName) VALUES
    (19, 'Kingston'),
    (19, 'Montego Bay'),
    (19, 'Ocho Rios');

-- Fiji (CountryID = 20)
INSERT INTO City (CountryID, CityName) VALUES
    (20, 'Suva'),
    (20, 'Nadi'),
    (20, 'Lautoka');

-- South Korea (CountryID = 21)
INSERT INTO City (CountryID, CityName) VALUES
    (21, 'Seoul'),
    (21, 'Busan'),
    (21, 'Incheon');

-- China (CountryID = 22)
INSERT INTO City (CountryID, CityName) VALUES
    (22, 'Beijing'),
    (22, 'Shanghai'),
    (22, 'Guangzhou');

-- Greece (CountryID = 23)
INSERT INTO City (CountryID, CityName) VALUES
    (23, 'Athens'),
    (23, 'Thessaloniki'),
    (23, 'Patras');

-- Switzerland (CountryID = 24)
INSERT INTO City (CountryID, CityName) VALUES
    (24, 'Zurich'),
    (24, 'Geneva'),
    (24, 'Bern');

-- Austria (CountryID = 25)
INSERT INTO City (CountryID, CityName) VALUES
    (25, 'Vienna'),
    (25, 'Salzburg'),
    (25, 'Graz');

-- Kazakhstan (CountryID = 26)
INSERT INTO City (CountryID, CityName) VALUES
    (26, 'Almaty'),
    (26, 'Nur-Sultan'),
    (26, 'Shymkent');



