﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ContactAgenda
{
    public partial class LoginForm : Form
    {
        public static string userID;
        public static LoginForm Instance { get; } = new LoginForm();
        private readonly ErrorProvider errorProvider = new ErrorProvider();

        public LoginForm()
        {
            InitializeComponent();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        #region Events

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegisterNewUser();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LoginUser();
        }

        #endregion

        #region Methods

        private void RegisterNewUser()
        {
            RegisterUserForm newRegisterForm = new RegisterUserForm();
            newRegisterForm.Show();
            this.Hide();
        }

        private void LoginUser()
        {
            string username = TxtBxUsername.Text;
            string password = TxtBxPassword.Text;

            errorProvider.Clear();
            if(String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                    if (String.IsNullOrEmpty(username))
                    {
                        errorProvider.SetError(TxtBxUsername, "Username is required");
                    }

                    if (String.IsNullOrEmpty(password))
                    {
                        errorProvider.SetError(TxtBxPassword, "Password is required");
                    }

                   
         
            }
            else
            {
                if (ValidateUserCredentials(username, password))
                {
                    ContactsForm newContactsForm = new ContactsForm();
                    newContactsForm.Show();
                    this.Hide();

                    TxtBxUsername.Clear();
                    TxtBxPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Username or password are incorrect.", "Warning!");
                }
            }
        }


        private bool ValidateUserCredentials(string username, string password)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Id FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@Username", SqlDbType.NVarChar, 30).Value = username;  // Parameterize with type
                        command.Parameters.Add("@Password", SqlDbType.NVarChar, 20).Value = password;  // Parameterize with type

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userID = reader["Id"].ToString();
                                return true;  // Indicate successful authentication
                            }
                        }
                    }
                }
                return false;  // No matching user found
            }
            catch (Exception ex)
            {
                // Handle any exceptions gracefully, e.g., log the error
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}
