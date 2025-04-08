using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Polozenie_rysunkow_baza
{
    public partial class Kosciuszko : System.Web.UI.Page, IPostBackEventHandler
    {
        private static Random randomTK = new Random();
        private static string[] imageButtonsTK = { "ImageButton1TK", "ImageButton2TK", "ImageButton3TK", "ImageButton4TK" };
        private static DateTime imageDisplayedTimeTK;
        private static TimeSpan minReactionTimeTK = TimeSpan.MaxValue;
        private static TimeSpan maxReactionTimeTK = TimeSpan.MinValue;
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DatabaseKosciuszko.mdf;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetInitialImageTK();
                UpdateUsersDropDownTK();
                LoadCurrentUserDetails();
            }
            ImageButton1TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[0]));
            ImageButton2TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[1]));
            ImageButton3TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[2]));
            ImageButton4TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[3]));
            Session.Timeout = 1;
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ErrorLabelTK.Text = "Nazwa użytkownika nie może być pusta.";
                return;
            }

            lock (Application)
            {
                if (Application["ActiveUsers"] == null)
                {
                    Application["ActiveUsers"] = new List<UserSession>();
                }

                var activeUsers = (List<UserSession>)Application["ActiveUsers"];
                if (activeUsers.Any(u => u.Username == username))
                {
                    ErrorLabelTK.Text = "Użytkownik o tej nazwie jest już zalogowany.";
                    return;
                }

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
                Session["Username"] = username;
            }

            UsernameTK.Text = username;
            RegistrationPanel.Visible = false;
            MainPanel.Visible = true;
            UserPanel.Visible = true;
            UpdateUsersDropDownTK();
            LoadCurrentUserDetails();
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


        private void SetInitialImageTK()
        {
            int randomImageNumberTK = randomTK.Next(1, 12);
            ImageButton1TK.ImageUrl = $"~/pictures/{randomImageNumberTK}.jpg";
            ImageButton1TK.Enabled = true;
            ImageButton1TK.Visible = true;
            imageDisplayedTimeTK = DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"ImageButton1TK włączony z obrazem {randomImageNumberTK}.jpg");

            // Ustawienie pozostałych przycisków jako niewidoczne
            for (int i = 1; i < imageButtonsTK.Length; i++)
            {
                ImageButton buttonTK = (ImageButton)FindControl(imageButtonsTK[i]);
                if (buttonTK != null)
                {
                    buttonTK.Enabled = false;
                    buttonTK.Visible = false;
                }
            }
        }

        public void RaisePostBackEvent(string eventArgumentTK)
        {
            int indexTK = Array.IndexOf(imageButtonsTK, eventArgumentTK);
            if (indexTK >= 0)
            {
                ImageButton currentButtonTK = (ImageButton)FindControl(imageButtonsTK[indexTK]);
                ImageButton nextButtonTK = (ImageButton)FindControl(imageButtonsTK[(indexTK + 1) % imageButtonsTK.Length]);

                if (currentButtonTK != null && nextButtonTK != null)
                {
                    TimeSpan reactionTimeTK = DateTime.Now - imageDisplayedTimeTK;
                    UpdateReactionTimesTK(reactionTimeTK);

                    nextButtonTK.ImageUrl = currentButtonTK.ImageUrl;
                    currentButtonTK.Enabled = false;
                    currentButtonTK.Visible = false;
                    nextButtonTK.Enabled = true;
                    nextButtonTK.Visible = true;
                    imageDisplayedTimeTK = DateTime.Now;

                    string selectedUsername = UsersDropDownTK.SelectedValue;
                    string currentUsername = Session["Username"] as string;
                    if (!string.IsNullOrEmpty(currentUsername))
                    {
                        var userSession = GetUserSessionFromDatabase(currentUsername);
                        if (userSession != null)
                        {
                            userSession.IleNastapiloZmianPolozeniaZdjecia++;
                            UpdateUserSessionInDatabase(userSession);
                        }
                    }
                    LoadUserDetails(selectedUsername); // Natychmiastowa aktualizacja danych w DetailsViewTK
                    LoadCurrentUserDetails(); // Natychmiastowa aktualizacja danych w DetailsLocalViewTK
                }
            }
        }

        protected void ResolutionRadioTK_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSizeTK = ResolutionRadioTK.SelectedItem.Text;
            SetImageButtonSizeTK(selectedSizeTK);
            ResetReactionTimesTK();

            string selectedUsername = UsersDropDownTK.SelectedValue;
            string currentUsername = Session["Username"] as string;
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var userSession = GetUserSessionFromDatabase(currentUsername);
                if (userSession != null)
                {
                    userSession.IleNastapiloZmianRozmiaruZdjecia++;
                    UpdateUserSessionInDatabase(userSession);
                }
            }
            LoadUserDetails(selectedUsername); // Natychmiastowa aktualizacja danych w DetailsViewTK
            LoadCurrentUserDetails(); // Natychmiastowa aktualizacja danych w DetailsLocalViewTK
        }

        private void SetImageButtonSizeTK(string sizeTK)
        {
            int widthTK, heightTK;

            switch (sizeTK)
            {
                case "Mały rozmiar":
                    widthTK = 50;
                    heightTK = 50;
                    break;
                case "Średni rozmiar":
                    widthTK = 75;
                    heightTK = 75;
                    break;
                case "Duży rozmiar":
                default:
                    widthTK = 100;
                    heightTK = 100;
                    break;
            }

            ImageButton1TK.Width = widthTK;
            ImageButton1TK.Height = heightTK;
            ImageButton2TK.Width = widthTK;
            ImageButton2TK.Height = heightTK;
            ImageButton3TK.Width = widthTK;
            ImageButton3TK.Height = heightTK;
            ImageButton4TK.Width = widthTK;
            ImageButton4TK.Height = heightTK;
        }

        protected void ButtonSwitchTK_Click(object sender, EventArgs e)
        {
            // Znajdź aktualnie widoczny przycisk
            ImageButton currentButtonTK = imageButtonsTK.Select(idTK => (ImageButton)FindControl(idTK)).FirstOrDefault(btnTK => btnTK.Visible);

            if (currentButtonTK != null)
            {
                // Wylosuj nowe zdjęcie, które nie jest aktualnym
                string currentImageUrlTK = currentButtonTK.ImageUrl;
                string newImageUrlTK;
                do
                {
                    int randomImageNumberTK = randomTK.Next(1, 12);
                    newImageUrlTK = $"~/pictures/{randomImageNumberTK}.jpg";
                } while (newImageUrlTK == currentImageUrlTK);

                // Wylosuj nowy przycisk, który nie jest aktualnym
                ImageButton newButtonTK;
                do
                {
                    int randomButtonIndexTK = randomTK.Next(imageButtonsTK.Length);
                    newButtonTK = (ImageButton)FindControl(imageButtonsTK[randomButtonIndexTK]);
                } while (newButtonTK == currentButtonTK);
                newButtonTK.ImageUrl = newImageUrlTK;
                newButtonTK.Enabled = true;
                newButtonTK.Visible = true;
                currentButtonTK.Enabled = false;
                currentButtonTK.Visible = false;
                imageDisplayedTimeTK = DateTime.Now;

                string selectedUsername = UsersDropDownTK.SelectedValue;
                string currentUsername = Session["Username"] as string;
                if (!string.IsNullOrEmpty(currentUsername))
                {
                    var userSession = GetUserSessionFromDatabase(currentUsername);
                    if (userSession != null)
                    {
                        userSession.IleNastapiloZmianPolozeniaZdjecia++;
                        UpdateUserSessionInDatabase(userSession);
                    }
                }
                LoadUserDetails(selectedUsername); // Natychmiastowa aktualizacja danych w DetailsViewTK
                LoadCurrentUserDetails(); // Natychmiastowa aktualizacja danych w DetailsLocalViewTK
            }
            ResetReactionTimesTK();
        }

        private void UpdateReactionTimesTK(TimeSpan reactionTimeTK)
        {
            string formattedTimeTK = reactionTimeTK.ToString(@"mm\:ss\,fff");

            if (reactionTimeTK < minReactionTimeTK || minReactionTimeTK == TimeSpan.Zero)
            {
                minReactionTimeTK = reactionTimeTK;
                MinReactionTimeLabel.Text = formattedTimeTK;
            }

            if (reactionTimeTK > maxReactionTimeTK)
            {
                maxReactionTimeTK = reactionTimeTK;
                MaxReactionTimeLabel.Text = formattedTimeTK;
            }

            string currentUsername = Session["Username"] as string;
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var userSession = GetUserSessionFromDatabase(currentUsername);
                if (userSession != null)
                {
                    if (reactionTimeTK < userSession.MinReactionTime || userSession.MinReactionTime == TimeSpan.Zero)
                    {
                        userSession.MinReactionTime = reactionTimeTK;
                    }

                    if (reactionTimeTK > userSession.MaxReactionTime)
                    {
                        userSession.MaxReactionTime = reactionTimeTK;
                    }

                    UpdateUserSessionInDatabase(userSession);

                    // Natychmiastowa aktualizacja danych w DetailsViewTK dla aktualnie zalogowanego użytkownika
                    if (UsersDropDownTK.SelectedValue == currentUsername)
                    {
                        LoadUserDetails(currentUsername);
                    }
                }
            }
        }



        private void ResetReactionTimesTK()
        {
            minReactionTimeTK = TimeSpan.MaxValue;
            maxReactionTimeTK = TimeSpan.MinValue;
            MinReactionTimeLabel.Text = "00:00,000";
            MaxReactionTimeLabel.Text = "00:00,000";
        }

        private void UpdateUsersDropDownTK()
        {
            string selectedUsername = UsersDropDownTK.SelectedValue;
            UsersDropDownTK.Items.Clear();
            lock (Application)
            {
                if (Application["ActiveUsers"] == null)
                {
                    Application["ActiveUsers"] = new List<UserSession>();
                }

                var activeUsers = (List<UserSession>)Application["ActiveUsers"];
                foreach (var user in activeUsers)
                {
                    if (user.Username != Session["Username"] as string)
                    {
                        UsersDropDownTK.Items.Add(new ListItem(user.Username));
                    }
                }
            }

            if (UsersDropDownTK.Items.Count == 0)
            {
                UsersDropDownTK.Items.Add(new ListItem("Brak użytkowników", ""));
                DetailsViewTK.Visible = false;
            }
            else
            {
                DetailsViewTK.Visible = true;
                if (!string.IsNullOrEmpty(selectedUsername))
                {
                    UsersDropDownTK.SelectedValue = selectedUsername;
                }
            }
        }

        private void RemoveInactiveUsers()
        {
            var activeUsernames = ((List<UserSession>)Application["ActiveUsers"]).Select(u => u.Username).ToList();
            var itemsToRemove = UsersDropDownTK.Items.Cast<ListItem>().Where(item => !activeUsernames.Contains(item.Value)).ToList();

            foreach (var item in itemsToRemove)
            {
                UsersDropDownTK.Items.Remove(item);
            }

            if (!activeUsernames.Contains(UsersDropDownTK.SelectedValue))
            {
                DetailsViewTK.DataSource = null;
                DetailsViewTK.DataBind();
                DetailsViewTK.Visible = false;
            }
        }

        protected void TimerUsers_Tick(object sender, EventArgs e)
        {
            string selectedUsername = UsersDropDownTK.SelectedValue;
            UpdateUsersDropDownTK();
            RemoveInactiveUsers();

            // Synchronizacja danych co kilkanaście sekund dla innych użytkowników
            if (!string.IsNullOrEmpty(selectedUsername) && selectedUsername != Session["Username"] as string)
            {
                LoadUserDetails(selectedUsername);
            }

            // Zachowanie wybranego użytkownika w UsersDropDownTK
            if (!string.IsNullOrEmpty(selectedUsername))
            {
                UsersDropDownTK.SelectedValue = selectedUsername;
            }
        }

        protected void UsersDropDownTK_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUsername = UsersDropDownTK.SelectedValue;
            LoadUserDetails(selectedUsername);
        }

        private void LoadUserDetails(string username)
        {
            var userSession = GetUserSessionFromDatabase(username);

            if (userSession != null)
            {
                var userDetails = new
                {
                    DataUruchomienia = userSession.StartTime.ToString("yyyy-MM-dd"),
                    GodzinaUruchomienia = userSession.StartTime.ToString("HH:mm:ss"),
                    MinimalnyCzasReakcji = userSession.MinReactionTime == TimeSpan.MaxValue ? "00:00,000" : userSession.MinReactionTime.ToString(@"mm\:ss\,fff"),
                    MaksymalnyCzasReakcji = userSession.MaxReactionTime == TimeSpan.MinValue ? "00:00,000" : userSession.MaxReactionTime.ToString(@"mm\:ss\,fff"),
                    IleNastapiloZmianRozmiaruZdjecia = userSession.IleNastapiloZmianRozmiaruZdjecia,
                    IleNastapiloZmianPolozeniaZdjecia = userSession.IleNastapiloZmianPolozeniaZdjecia
                };

                DetailsViewTK.DataSource = new List<object> { userDetails };
                DetailsViewTK.DataBind();
                DetailsViewTK.Visible = true;
            }
            else
            {
                DetailsViewTK.DataSource = null;
                DetailsViewTK.DataBind();
                DetailsViewTK.Visible = false;
            }
        }

        private void LoadCurrentUserDetails()
        {
            string currentUsername = Session["Username"] as string;
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var userSession = GetUserSessionFromDatabase(currentUsername);
                if (userSession != null)
                {
                    var userDetails = new
                    {
                        DataUruchomienia = userSession.StartTime.ToString("yyyy-MM-dd"),
                        GodzinaUruchomienia = userSession.StartTime.ToString("HH:mm:ss"),
                        MinimalnyCzasReakcji = userSession.MinReactionTime == TimeSpan.MaxValue ? "00:00,000" : userSession.MinReactionTime.ToString(@"mm\:ss\,fff"),
                        MaksymalnyCzasReakcji = userSession.MaxReactionTime == TimeSpan.MinValue ? "00:00,000" : userSession.MaxReactionTime.ToString(@"mm\:ss\,fff"),
                        IleNastapiloZmianRozmiaruZdjecia = userSession.IleNastapiloZmianRozmiaruZdjecia,
                        IleNastapiloZmianPolozeniaZdjecia = userSession.IleNastapiloZmianPolozeniaZdjecia
                    };

                    DetailsLocalViewTK.DataSource = new List<object> { userDetails };
                    DetailsLocalViewTK.DataBind();
                }
            }
        }

        protected void ButtonResetTK_Click(object sender, EventArgs e)
        {
            string currentUsername = Session["Username"] as string;
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var userSession = GetUserSessionFromDatabase(currentUsername);
                if (userSession != null)
                {
                    userSession.MinReactionTime = TimeSpan.Zero;
                    userSession.MaxReactionTime = TimeSpan.Zero;
                    userSession.IleNastapiloZmianRozmiaruZdjecia = 0;
                    userSession.IleNastapiloZmianPolozeniaZdjecia = 0;

                    UpdateUserSessionInDatabase(userSession);
                    LoadCurrentUserDetails(); // Natychmiastowa aktualizacja danych w DetailsLocalViewTK
                }
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
                command.Parameters.AddWithValue("@MinReactionTime", ValidateTime(userSession.MinReactionTime));
                command.Parameters.AddWithValue("@MaxReactionTime", ValidateTime(userSession.MaxReactionTime));
                command.Parameters.AddWithValue("@IleNastapiloZmianRozmiaruZdjecia", userSession.IleNastapiloZmianRozmiaruZdjecia);
                command.Parameters.AddWithValue("@IleNastapiloZmianPolozeniaZdjecia", userSession.IleNastapiloZmianPolozeniaZdjecia);
                command.Parameters.AddWithValue("@SessionID", userSession.SessionID);
                command.Parameters.AddWithValue("@IsActive", userSession.IsActive);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private TimeSpan ValidateTime(TimeSpan time)
        {
            if (time < TimeSpan.Zero)
                return TimeSpan.Zero;
            if (time > TimeSpan.FromHours(24) - TimeSpan.FromTicks(1))
                return TimeSpan.FromHours(24) - TimeSpan.FromTicks(1);
            return time;
        }

        private void UpdateUserSessionInDatabase(UserSession userSession)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [Table] SET MinReactionTime = @MinReactionTime, MaxReactionTime = @MaxReactionTime, IleNastapiloZmianRozmiaruZdjecia = @IleNastapiloZmianRozmiaruZdjecia, IleNastapiloZmianPolozeniaZdjecia = @IleNastapiloZmianPolozeniaZdjecia, IsActive = @IsActive WHERE SessionID = @SessionID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MinReactionTime", userSession.MinReactionTime);
                command.Parameters.AddWithValue("@MaxReactionTime", userSession.MaxReactionTime);
                command.Parameters.AddWithValue("@IleNastapiloZmianRozmiaruZdjecia", userSession.IleNastapiloZmianRozmiaruZdjecia);
                command.Parameters.AddWithValue("@IleNastapiloZmianPolozeniaZdjecia", userSession.IleNastapiloZmianPolozeniaZdjecia);
                command.Parameters.AddWithValue("@IsActive", userSession.IsActive);
                command.Parameters.AddWithValue("@SessionID", userSession.SessionID);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private UserSession GetUserSessionFromDatabase(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM [Table] WHERE Username = @Username AND IsActive = 1";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserSession
                        {
                            Username = reader["Username"].ToString(),
                            StartTime = Convert.ToDateTime(reader["StartTime"]),
                            MinReactionTime = (TimeSpan)reader["MinReactionTime"],
                            MaxReactionTime = (TimeSpan)reader["MaxReactionTime"],
                            IleNastapiloZmianRozmiaruZdjecia = Convert.ToInt32(reader["IleNastapiloZmianRozmiaruZdjecia"]),
                            IleNastapiloZmianPolozeniaZdjecia = Convert.ToInt32(reader["IleNastapiloZmianPolozeniaZdjecia"]),
                            SessionID = reader["SessionID"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return null;
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

