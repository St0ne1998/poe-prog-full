CREATE DATABASE IF NOT EXISTS cybersecurity_db;
USE cybersecurity_db;

DROP TABLE IF EXISTS tasks;

CREATE TABLE tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    reminder VARCHAR(100) NOT NULL,
    is_completed TINYINT(1) DEFAULT 0
);

-- Optional: Insert sample task to verify database connection
INSERT INTO tasks (title, description, reminder, is_completed) 
VALUES ('Set up Two-Factor Authentication', 'Secured via NLP assistant prompt.', 'in 3 days', 0);