-- Create Country table with Unicode support
CREATE TABLE Country (
    CountryID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName NVARCHAR(255) NOT NULL,
    Continent NVARCHAR(255) NOT NULL,
    CONSTRAINT country_countryname_unique UNIQUE (CountryName)
);


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