-- Database schema for MyBackendApi
-- MySQL Database

CREATE DATABASE IF NOT EXISTS MyBackendApi;
USE MyBackendApi;

-- Users table
CREATE TABLE IF NOT EXISTS Users (
    Id CHAR(36) PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NULL DEFAULT NULL,
    INDEX idx_users_email (Email),
    INDEX idx_users_active (IsActive)
);

-- Sample data (optional)
INSERT INTO Users (Id, Email, FirstName, LastName, IsActive, CreatedAt) VALUES
('550e8400-e29b-41d4-a716-446655440000', 'john.doe@example.com', 'John', 'Doe', TRUE, NOW()),
('550e8400-e29b-41d4-a716-446655440001', 'jane.smith@example.com', 'Jane', 'Smith', TRUE, NOW()),
('550e8400-e29b-41d4-a716-446655440002', 'mike.wilson@example.com', 'Mike', 'Wilson', TRUE, NOW());
