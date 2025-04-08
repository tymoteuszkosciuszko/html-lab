using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Polozenie_rysunkow
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
            }
            ImageButton1TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[0]));
            ImageButton2TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[1]));
            ImageButton3TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[2]));
            ImageButton4TK.Attributes.Add("onmouseover", Page.ClientScript.GetPostBackEventReference(this, imageButtonsTK[3]));
        }

        private void SetInitialImageTK()
        {
            int randomImageNumberTK = randomTK.Next(1, 12);
            ImageButton1TK.ImageUrl = $"~/pictures/{randomImageNumberTK}.jpg";
            ImageButton1TK.Enabled = true;
            ImageButton1TK.Visible = true;
            imageDisplayedTimeTK = DateTime.Now;
            Debug.WriteLine($"ImageButton1TK włączony z obrazem {randomImageNumberTK}.jpg");

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
                }
            }
        }

        protected void ResolutionRadioTK_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSizeTK = ResolutionRadioTK.SelectedItem.Text;
            SetImageButtonSizeTK(selectedSizeTK);
            ResetReactionTimesTK();
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
        }

        private void ResetReactionTimesTK()
        {
            minReactionTimeTK = TimeSpan.MaxValue;
            maxReactionTimeTK = TimeSpan.MinValue;
            MinReactionTimeLabel.Text = "00:00,000";
            MaxReactionTimeLabel.Text = "00:00,000";
        }
    }
}