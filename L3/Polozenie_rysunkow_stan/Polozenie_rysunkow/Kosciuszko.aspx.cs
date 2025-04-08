using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Polozenie_rysunkow_stan
{
    public partial class Kosciuszko : System.Web.UI.Page, IPostBackEventHandler
    {
        private static Random randomTK = new Random();
        private static string[] imageButtonsTK = { "ImageButton1TK", "ImageButton2TK", "ImageButton3TK", "ImageButton4TK" };
        private static DateTime imageDisplayedTimeTK;
        private static TimeSpan minReactionTimeTK = TimeSpan.MaxValue;
        private static TimeSpan maxReactionTimeTK = TimeSpan.MinValue;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetInitialImageTK();
                UpdateUsersDropDownTK();
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

                activeUsers.Add(new UserSession
                {
                    Username = username,
                    StartTime = DateTime.Now
                });
                Session["Username"] = username;
            }

            UsernameTK.Text = username;
            RegistrationPanel.Visible = false;
            MainPanel.Visible = true;
            UserPanel.Visible = true;
            UpdateUsersDropDownTK();
            LoadUserDetails(username);
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
                        var userSession = GetUserSession(currentUsername);
                        if (userSession != null)
                        {
                            userSession.IleNastapiloZmianPolozeniaZdjecia++;
                        }
                    }
                    LoadUserDetails(selectedUsername); // Natychmiastowa aktualizacja danych w DetailsViewTK
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
                var userSession = GetUserSession(currentUsername);
                if (userSession != null)
                {
                    userSession.IleNastapiloZmianRozmiaruZdjecia++;
                }
            }
            LoadUserDetails(selectedUsername); // Natychmiastowa aktualizacja danych w DetailsViewTK
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
                    var userSession = GetUserSession(currentUsername);
                    if (userSession != null)
                    {
                        userSession.IleNastapiloZmianPolozeniaZdjecia++;
                    }
                }
                LoadUserDetails(selectedUsername); // Natychmiastowa aktualizacja danych w DetailsViewTK
            }
            ResetReactionTimesTK();
        }

        private void UpdateReactionTimesTK(TimeSpan reactionTimeTK)
        {
            string formattedTimeTK = reactionTimeTK.ToString(@"mm\:ss\,fff");

            if (reactionTimeTK < minReactionTimeTK)
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
                var userSession = GetUserSession(currentUsername);
                if (userSession != null)
                {
                    if (reactionTimeTK < userSession.MinReactionTime)
                    {
                        userSession.MinReactionTime = reactionTimeTK;
                    }

                    if (reactionTimeTK > userSession.MaxReactionTime)
                    {
                        userSession.MaxReactionTime = reactionTimeTK;
                    }

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
                    UsersDropDownTK.Items.Add(new ListItem(user.Username));
                }
            }

            string currentUsername = Session["Username"] as string;
            if (!string.IsNullOrEmpty(currentUsername))
            {
                UsersDropDownTK.SelectedValue = currentUsername;
            }
            else if (!string.IsNullOrEmpty(selectedUsername))
            {
                UsersDropDownTK.SelectedValue = selectedUsername;
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
            var userSession = ((List<UserSession>)Application["ActiveUsers"]).FirstOrDefault(u => u.Username == username);

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
            }
            else
            {
                DetailsViewTK.DataSource = null;
                DetailsViewTK.DataBind();
            }
        }

        private UserSession GetUserSession(string username)
        {
            lock (Application)
            {
                if (Application["ActiveUsers"] == null)
                {
                    Application["ActiveUsers"] = new List<UserSession>();
                }

                return ((List<UserSession>)Application["ActiveUsers"]).FirstOrDefault(u => u.Username == username);
            }
        }
    }
}

