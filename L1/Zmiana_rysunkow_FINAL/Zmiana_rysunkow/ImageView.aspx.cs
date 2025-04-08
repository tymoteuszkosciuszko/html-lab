// ImageView.aspx.cs
using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Zmiana_rysunkow
{
    public partial class ImageView : Page
    {
        protected string imageTK;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string currentImageTK = Request.QueryString["imageTK"];
                imageViewTK.Attributes.Add("imgindex", currentImageTK);
                if (!string.IsNullOrEmpty(currentImageTK))
                {
                    imageViewTK.ImageUrl = $"~/Images/img{currentImageTK}.jpg";
                    textTK.Text = $"Wybrałeś zdjęcie {currentImageTK}!";
                }
                else
                {
                    imageViewTK.Visible = false ;
                    textTK.Text = "Nie podano zdjęcia!";
                }
            }
            imageTK = Request.QueryString["imageTK"];
        }

        protected void ImageViewTK_Click(object sender, ImageClickEventArgs e)
        {
            //imageTK = imageViewTK.Attributes["imgindex"];
            Response.Redirect($"Kosciuszko.aspx?imageRedirectTK={imageTK}");
        }
    }
}