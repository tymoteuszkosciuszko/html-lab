using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Polozenie_rysunkow_baza
{
    public class Global : System.Web.HttpApplication
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DatabaseKosciuszko.mdf;Integrated Security=True";

        protected void Application_Start(object sender, EventArgs e)
        {
            Application["ActiveUsers"] = new List<UserSession>();
            ResetAllSessionsInDatabase();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                lock (Application["ActiveUsers"])
                {
                    var activeUsers = (List<UserSession>)Application["ActiveUsers"];
                    var userSession = new UserSession
                    {
                        Username = username,
                        StartTime = DateTime.Now,
                        SessionID = Session.SessionID,
                        IsActive = true
                    };
                    activeUsers.Add(userSession);
                    SaveUserSessionToDatabase(userSession);
                    UpdateUserIsActiveInDatabase(username, true);
                    UpdateUserStartTimeInDatabase(username, userSession.StartTime);

                }
                System.Diagnostics.Debug.WriteLine("Nowa sesja dla użytkownika " + username);
            }
        }
        private void UpdateUserIsActiveInDatabase(string username, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [Table] SET IsActive = @IsActive WHERE Username = @Username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsActive", isActive);
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        protected void Session_End(object sender, EventArgs e)
        {
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                lock (Application["ActiveUsers"])
                {
                    var activeUsers = (List<UserSession>)Application["ActiveUsers"];
                    var userSession = activeUsers.FirstOrDefault(u => u.Username == username);
                    if (userSession != null)
                    {
                        userSession.IsActive = false;
                        UpdateUserSessionInDatabase(userSession);
                        activeUsers.Remove(userSession);
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Sesja {username} wygasła.");
            }
        }

        private void SaveUserSessionToDatabase(UserSession userSession)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [Table] (Username, StartTime, MinReactionTime, MaxReactionTime, IleNastapiloZmianRozmiaruZdjecia, IleNastapiloZmianPolozeniaZdjecia, SessionID, IsActive) " +
                               "VALUES (@Username, @StartTime, @MinReactionTime, @MaxReactionTime, @IleNastapiloZmianRozmiaruZdjecia, @IleNastapiloZmianPolozeniaZdjecia, @SessionID, @IsActive)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", userSession.Username);
                command.Parameters.AddWithValue("@StartTime", userSession.StartTime);
                command.Parameters.AddWithValue("@MinReactionTime", userSession.MinReactionTime);
                command.Parameters.AddWithValue("@MaxReactionTime", userSession.MaxReactionTime);
                command.Parameters.AddWithValue("@IleNastapiloZmianRozmiaruZdjecia", userSession.IleNastapiloZmianRozmiaruZdjecia);
                command.Parameters.AddWithValue("@IleNastapiloZmianPolozeniaZdjecia", userSession.IleNastapiloZmianPolozeniaZdjecia);
                command.Parameters.AddWithValue("@SessionID", userSession.SessionID);
                command.Parameters.AddWithValue("@IsActive", userSession.IsActive);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void UpdateUserSessionInDatabase(UserSession userSession)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [Table] SET IsActive = @IsActive WHERE SessionID = @SessionID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsActive", userSession.IsActive);
                command.Parameters.AddWithValue("@SessionID", userSession.SessionID);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void ResetAllSessionsInDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [Table] SET SessionID = NULL, IsActive = 0";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        private void UpdateUserStartTimeInDatabase(string username, DateTime startTime)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [Table] SET StartTime = @StartTime WHERE Username = @Username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StartTime", startTime);
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }


    }
}
