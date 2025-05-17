using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace ACE_MVC.DataService
{
    public class UserService
    {
        private string _connectionString;

        public UserService (string connectionString)
        {
            _connectionString = connectionString;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection test successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection test failed: {ex.Message}");
            }
        }

        public string GetUserByEmail(string email)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Username FROM Users WHERE Email = @Email";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString("Username");
                        }
                    }
                }
            }
            return "";
        }


        public bool CreateUser (string username, string email, string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    Console.WriteLine("Opening database connection...");
                    connection.Open();
                    Console.WriteLine("Database connection opened successfully.");
                    
                    string query = "START TRANSACTION;" + 
                                    "INSERT INTO Users (Username, Email, Passkey) VALUES (@Username, @Email, @Password);" +
                                    "COMMIT;"; // Replace with your table name

                    Console.WriteLine($"Prepared query: {query}");
                    
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password); // Or hashed password here
                        
                        int result = command.ExecuteNonQuery();
                        Console.WriteLine($"Rows affected: {result}");
                        
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        

    public bool ValidateLogin (string email, string password) {
    // Query the database
    using (var connection = new MySqlConnection(_connectionString))
    {
        try
        {
            connection.Open();

            // Query to check if the email and password match
            string query = "SELECT COUNT(*) FROM users WHERE Email = @Email AND Passkey = @Password";
            using (var command = new MySqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count == 1)
                    return true;
                else
                    return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    }

        public User SaveContactToDataBase(User contact, string clientname)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    int clientId;
                    string getClientIdQuery = "SELECT user_id FROM users WHERE Username = @Username;";
                    using (var getClientIdCommand = new MySqlCommand(getClientIdQuery, connection))
                    {
                        getClientIdCommand.Parameters.AddWithValue("@Username", clientname);
                        var result = getClientIdCommand.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("Error: Client user not found.");
                            return null;
                        }
                        clientId = Convert.ToInt32(result);
                    }

                    int contactId;
                    string getContactIdQuery = "SELECT user_id FROM users WHERE Username = @Username;";
                    using (var command = new MySqlCommand(getContactIdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", contact.Username);
                        var result = command.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("Error: Contact user not found.");
                            return null;
                        }
                        contactId = Convert.ToInt32(result);
                    }

                    string insertContactQuery = @"
                        INSERT INTO contacts (user_id, contact_user_id)
                        VALUES (@UserId, @ContactUserId);";
                    using (var insertContactCommand = new MySqlCommand(insertContactQuery, connection))
                    {
                        insertContactCommand.Parameters.AddWithValue("@UserId", clientId);
                        insertContactCommand.Parameters.AddWithValue("@ContactUserId", contactId);
                        int rowsAffected = insertContactCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Contact saved successfully.");
                            return contact;
                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to save contact.");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }
        }


        public bool SaveMessage (string clientname, string contactname, string message)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    int clientId;
                    int contactId;

                    string getClientIdQuery = "SELECT user_id FROM users WHERE Username = @Username;";
                    string getContactIdQuery = "SELECT user_id FROM users WHERE Username = @Username;";
                    


                        var getIdCommand = new MySqlCommand(getClientIdQuery, connection);
                        getIdCommand.Parameters.AddWithValue("@Username", clientname);

                        var result = getIdCommand.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("Error: Client user not found.");
                            return false;
                        }
                        clientId = Convert.ToInt32(result);

                        getIdCommand = new MySqlCommand(getContactIdQuery, connection);

                        getIdCommand.Parameters.AddWithValue("@Username", contactname);
                        result = getIdCommand.ExecuteScalar();
                        if (result == null)
                        {
                            Console.WriteLine("Error: Client user not found.");
                            return false;
                        }
                        contactId = Convert.ToInt32(result);

                    string insertMessageQuery = @"insert into messages (sender_id, receiver_id, message_text) values (@SenderId, @ReceiverId, @Message)";

                    using (var insertContactCommand = new MySqlCommand(insertMessageQuery, connection))
                    {
                        insertContactCommand.Parameters.AddWithValue("@SenderId", clientId);
                        insertContactCommand.Parameters.AddWithValue("@ReceiverId", contactId);
                        insertContactCommand.Parameters.AddWithValue("@Message", message);

                        int rowsAffected = insertContactCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Contact saved successfully.");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to save contact.");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }



    public User GetUserByUsername(string username)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT user_id, Username, Email FROM Users WHERE Username = @Username";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        return new User
                        {
                            Id = reader.GetInt32("user_id"),
                            Username = reader.GetString("Username"),
                            Email = reader.GetString("Email")
                        };
                    }
                }
            }
        }
        return null;
    }


            private async Task<int> GetUserIdAsync(string username)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT user_id FROM users WHERE username = @Username;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var result = await command.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : -1; // Return -1 if user not found
                    }
                }
            }

            private int GetUserId(string username)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id FROM users WHERE username = @Username;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1; // Return -1 if user not found
                    }
                }
            }


            public bool DuplicateContact (string userName, string addedContact) {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM contacts WHERE user_id = @UserId AND contact_user_id = @ContactId;";
                    int userId = GetUserId(userName);
                    int contactId = GetUserId(addedContact);

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@ContactId", contactId);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }

        public async Task <UserContacts> FetchUserContacts (string username)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    int userId = await GetUserIdAsync(username);
                    System.Console.WriteLine("User ID when fetching contacts", userId);
                    
                    var contacts = new List<User>();
                    string getContactsQuery = @"
                        SELECT u.user_id, u.Username, u.Email
                        FROM contacts c
                        JOIN users u ON c.contact_user_id = u.user_id
                        WHERE c.user_id = @UserId;";
                    using (var getContactsCommand = new MySqlCommand(getContactsQuery, connection))
                    {
                        getContactsCommand.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = await getContactsCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                contacts.Add(new User
                                {
                                    Id = reader.GetInt32("user_id"),
                                    Username = reader.GetString("Username"),
                                    Email = reader.GetString("Email")
                                });
                            }
                        }
                    }

                    
                    Console.WriteLine("Contacts data fetched successfully.");

                    Console.WriteLine("Contacts: " + JsonConvert.SerializeObject(contacts));

                    return new UserContacts
                    {
                        Contacts = contacts ?? new List<User>() };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<UserMessages> FetchUserMessages(string username, string contactname)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    int clientId = await GetUserIdAsync(username);
                    int contactId = await GetUserIdAsync(contactname);

                    if (clientId == -1 || contactId == -1)
                    {
                        // Handle user not found scenario
                        Console.WriteLine("One or both users not found.");
                        return new UserMessages { Messages = new List<Message>() };
                    }

                    var messages = new List<Message>();
                    string getMessagesQuery = @"
                        SELECT message_id, sender_id, receiver_id, message_text, sent_at
                        FROM messages
                        WHERE (sender_id = @ClientId AND receiver_id = @ContactId)
                        OR (sender_id = @ContactId AND receiver_id = @ClientId)
                        ORDER BY sent_at;";

                    using (var getMessagesCommand = new MySqlCommand(getMessagesQuery, connection))
                    {
                        getMessagesCommand.Parameters.AddWithValue("@ClientId", clientId);
                        getMessagesCommand.Parameters.AddWithValue("@ContactId", contactId);

                        using (var reader = await getMessagesCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                messages.Add(new Message
                                {
                                    MessageText = reader.GetString("message_text"),
                                    SentAt = reader.GetDateTime("sent_at"),
                                    SentBy = clientId == reader.GetInt32("sender_id") ? username : contactname
                                });
                            }

                        }
                    }

                    return new UserMessages { Messages = messages ?? new List<Message>() };

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching messages: {ex.Message}");
                    return null;
                }
            }
        }






    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }


    public class Message
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public string SentBy { get; set; }
    }

    public class UserContacts
    {
        public List<User> Contacts { get; set; }
    }
    
    public class UserMessages
    {
        public List<Message> Messages { get; set; }
    }

}
}
