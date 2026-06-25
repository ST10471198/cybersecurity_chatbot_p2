-- Create the database
CREATE DATABASE chatbot_tasks;

-- Use the database
USE chatbot_tasks;

-- Create the tasks table
CREATE TABLE tasks (
    task_id INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) NOT NULL,
    task_name VARCHAR(100) NOT NULL,
    task_description VARCHAR(200) NOT NULL,
    task_due_date VARCHAR(20),
    task_status VARCHAR(20) DEFAULT 'Pending',
    reminder_date VARCHAR(20),
    date_created DATETIME DEFAULT GETDATE()
);

-- Verify the table was created
SELECT * FROM tasks;