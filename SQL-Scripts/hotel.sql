-- User tabel med PK og CURRENT_TIMESTAMP
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(20),
    HashedPassword VARCHAR(255) NOT NULL,
    PasswordBackdoor VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Rooms tabel med PK og CURRENT_TIMESTAMP
CREATE TABLE Rooms (
    Id SERIAL PRIMARY KEY,
    RoomNumber VARCHAR(10) NOT NULL,
    RoomSize VARCHAR(50) NOT NULL,
    RoomType VARCHAR(50) NOT NULL,
    IsAvailable BOOLEAN NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Bookings tabel med PK og CURRENT_TIMESTAMP
CREATE TABLE Bookings (
    Id SERIAL PRIMARY KEY,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    UserId INTEGER NOT NULL,
    RoomId INTEGER NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RoomId) REFERENCES Rooms(Id)
);