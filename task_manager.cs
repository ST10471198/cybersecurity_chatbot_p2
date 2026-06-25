using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace cybersecurity_chatbot_p2
{//start of namespace
    public class task_manager
    {//start of class

        private string connectionString;
        private string currentUsername;

        public task_manager(string username)
        {//start of constructor
            currentUsername = username;

            // Connection string for local SQL Server
            connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=chatbot_tasks;Integrated Security=True;";

            create_database_and_table_if_not_exists();
        }//end of constructor

        private void create_database_and_table_if_not_exists()
        {//start of method
            try
            {//start of try
                // First, check if database exists
                if (!database_exists())
                {//start of if
                    create_database();
                }//end of if

                // Now create the table
                create_table_if_not_exists();
            }//end of try
            catch (Exception ex)
            {//start of catch
                MessageBox.Show("Error setting up database: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }//end of catch
        }//end of method

        private bool database_exists()
        {//start of method
            try
            {//start of try
                string masterConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
                using (SqlConnection conn = new SqlConnection(masterConnection))
                {//start of using
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM sys.databases WHERE name = 'chatbot_tasks'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }//end of using
                }//end of using
            }//end of try
            catch
            {//start of catch
                return false;
            }//end of catch
        }//end of method

        private void create_database()
        {//start of method
            try
            {//start of try
                string masterConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
                using (SqlConnection conn = new SqlConnection(masterConnection))
                {//start of using
                    conn.Open();
                    string query = "CREATE DATABASE chatbot_tasks";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.ExecuteNonQuery();
                    }//end of using
                }//end of using

                // Update connection string to use the new database
                connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=chatbot_tasks;Integrated Security=True;";
            }//end of try
            catch (Exception ex)
            {//start of catch
                MessageBox.Show("Failed to create database: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }//end of catch
        }//end of method

        private void create_table_if_not_exists()
        {//start of method
            try
            {//start of try
                // Make sure we're using the correct database
                if (connectionString == @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;")
                {//start of if
                    connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=chatbot_tasks;Integrated Security=True;";
                }//end of if

                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tasks' AND xtype='U')
                        CREATE TABLE tasks (
                            task_id INT PRIMARY KEY IDENTITY(1,1),
                            username VARCHAR(50) NOT NULL,
                            task_name VARCHAR(100) NOT NULL,
                            task_description VARCHAR(200) NOT NULL,
                            task_due_date VARCHAR(20),
                            task_status VARCHAR(20) DEFAULT 'Pending',
                            reminder_date VARCHAR(20),
                            date_created DATETIME DEFAULT GETDATE()
                        )";

                    using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
                    {//start of using
                        cmd.ExecuteNonQuery();
                    }//end of using
                }//end of using
            }//end of try
            catch (Exception ex)
            {//start of catch
                MessageBox.Show("Error creating table: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }//end of catch
        }//end of method

        // ============================================================
        // TASK CRUD METHODS
        // ============================================================

        public string add_task(string taskName, string taskDescription, string reminderDate)
        {//start of method
            try
            {//start of try
                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string query = @"INSERT INTO tasks (username, task_name, task_description, task_due_date, task_status, reminder_date) 
                                    VALUES (@username, @taskName, @taskDescription, @taskDueDate, 'Pending', @reminderDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.Parameters.AddWithValue("@username", currentUsername);
                        cmd.Parameters.AddWithValue("@taskName", taskName);
                        cmd.Parameters.AddWithValue("@taskDescription", taskDescription);
                        cmd.Parameters.AddWithValue("@taskDueDate", string.IsNullOrEmpty(reminderDate) ? "" : reminderDate);
                        cmd.Parameters.AddWithValue("@reminderDate", string.IsNullOrEmpty(reminderDate) ? "" : reminderDate);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {//start of if
                            if (!string.IsNullOrEmpty(reminderDate))
                            {//start of if
                                return "Task added successfully! I'll remind you on " + reminderDate + ".";
                            }//end of if
                            else
                            {//start of else
                                return "Task added successfully! No reminder set for this task.";
                            }//end of else
                        }//end of if
                        else
                        {//start of else
                            return "Failed to add task. Please try again.";
                        }//end of else
                    }//end of using
                }//end of using
            }//end of try
            catch (Exception ex)
            {//start of catch
                return "Database error: " + ex.Message;
            }//end of catch
        }//end of method

        public string view_tasks()
        {//start of method
            try
            {//start of try
                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string query = @"SELECT task_id, task_name, task_description, task_due_date, task_status, reminder_date 
                                    FROM tasks WHERE username = @username ORDER BY date_created DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.Parameters.AddWithValue("@username", currentUsername);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using
                            if (!reader.HasRows)
                            {//start of if
                                return "You have no tasks. Add a task to get started!";
                            }//end of if

                            string result = "Here are your tasks:\n\n";
                            int count = 1;

                            while (reader.Read())
                            {//start of while
                                int taskId = reader.GetInt32(0);
                                string taskName = reader.GetString(1);
                                string taskDescription = reader.GetString(2);
                                string dueDate = reader.IsDBNull(3) ? "No due date" : reader.GetString(3);
                                string status = reader.GetString(4);
                                string reminder = reader.IsDBNull(5) ? "No reminder" : reader.GetString(5);

                                string statusIcon = status.ToLower() == "completed" ? "✓" : "○";

                                result += count + ". " + statusIcon + " " + taskName + "\n";
                                result += "   Description: " + taskDescription + "\n";
                                result += "   Status: " + status + "\n";
                                result += "   Due Date: " + dueDate + "\n";
                                result += "   Reminder: " + reminder + "\n\n";
                                count++;
                            }//end of while

                            return result;
                        }//end of using
                    }//end of using
                }//end of using
            }//end of try
            catch (Exception ex)
            {//start of catch
                return "Database error: " + ex.Message;
            }//end of catch
        }//end of method

        public string complete_task(int taskId)
        {//start of method
            try
            {//start of try
                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string query = @"UPDATE tasks SET task_status = 'Completed' 
                                    WHERE task_id = @taskId AND username = @username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        cmd.Parameters.AddWithValue("@username", currentUsername);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {//start of if
                            return "Task marked as completed! Great job!";
                        }//end of if
                        else
                        {//start of else
                            return "Task not found or already completed.";
                        }//end of else
                    }//end of using
                }//end of using
            }//end of try
            catch (Exception ex)
            {//start of catch
                return "Database error: " + ex.Message;
            }//end of catch
        }//end of method

        public string delete_task(int taskId)
        {//start of method
            try
            {//start of try
                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string query = @"DELETE FROM tasks WHERE task_id = @taskId AND username = @username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        cmd.Parameters.AddWithValue("@username", currentUsername);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {//start of if
                            return "Task deleted successfully!";
                        }//end of if
                        else
                        {//start of else
                            return "Task not found. Please check the task ID.";
                        }//end of else
                    }//end of using
                }//end of using
            }//end of try
            catch (Exception ex)
            {//start of catch
                return "Database error: " + ex.Message;
            }//end of catch
        }//end of method

        public int get_task_id_from_name(string taskName)
        {//start of method
            try
            {//start of try
                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string query = @"SELECT TOP 1 task_id FROM tasks 
                                    WHERE username = @username AND task_name LIKE @taskName 
                                    AND task_status != 'Completed' ORDER BY date_created DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.Parameters.AddWithValue("@username", currentUsername);
                        cmd.Parameters.AddWithValue("@taskName", "%" + taskName + "%");

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {//start of if
                            return Convert.ToInt32(result);
                        }//end of if
                        else
                        {//start of else
                            return -1;
                        }//end of else
                    }//end of using
                }//end of using
            }//end of try
            catch
            {//start of catch
                return -1;
            }//end of catch
        }//end of method

        public string get_pending_task_names()
        {//start of method
            try
            {//start of try
                using (SqlConnection conn = new SqlConnection(connectionString))
                {//start of using
                    conn.Open();
                    string query = @"SELECT task_name FROM tasks 
                                    WHERE username = @username AND task_status = 'Pending'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {//start of using
                        cmd.Parameters.AddWithValue("@username", currentUsername);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using
                            if (!reader.HasRows)
                            {//start of if
                                return "No pending tasks.";
                            }//end of if

                            string result = "Your pending tasks:\n";
                            int count = 1;

                            while (reader.Read())
                            {//start of while
                                result += count + ". " + reader.GetString(0) + "\n";
                                count++;
                            }//end of while

                            return result;
                        }//end of using
                    }//end of using
                }//end of using
            }//end of try
            catch (Exception ex)
            {//start of catch
                return "Database error: " + ex.Message;
            }//end of catch
        }//end of method

    }//end of class
}//end of namespace