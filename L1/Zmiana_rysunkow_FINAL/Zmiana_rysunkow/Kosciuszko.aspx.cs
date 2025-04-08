using System;
using System.Web.UI;

namespace Zmiana_rysunkow
{
    public partial class Kosciuszko : Page, IPostBackEventHandler
    {
        protected int currentImageIndexTK
        {
            get => ViewState["currentImageIndexTK"] != null ? (int)ViewState["currentImageIndexTK"] : 1;
            set => ViewState["currentImageIndexTK"] = value;
        }
        private const int totalImagesTK = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string index = Request.QueryString["imageRedirectTK"];
                if(!string.IsNullOrEmpty(index)) currentImageIndexTK = int.Parse(index);
                if (currentImageIndexTK > totalImagesTK || currentImageIndexTK < 1) currentImageIndexTK = 1;
                imageButtonTK.ImageUrl = $"~/Images/img{currentImageIndexTK}.jpg";
                labelIndexTK.Text = $"{currentImageIndexTK}";
                buttons();
            }
            buttonLeftTK.Attributes.Add("onmouseleave", Page.ClientScript.GetPostBackEventReference(this, "lewo"));
            buttonRightTK.Attributes.Add("onmouseleave", Page.ClientScript.GetPostBackEventReference(this, "prawo"));
        }

        protected void ImageButtonTK_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect($"ImageView.aspx?imageTK={currentImageIndexTK}");
        }
        private void buttons()
        {
            buttonLeftTK.Enabled = currentImageIndexTK > 1;
            buttonRightTK.Enabled = currentImageIndexTK < totalImagesTK;
        }
        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "prawo")
            {
                if (currentImageIndexTK < totalImagesTK)
                {
                    currentImageIndexTK++;
                    imageButtonTK.ImageUrl = $"~/Images/img{currentImageIndexTK}.jpg";
                    labelIndexTK.Text = $"{currentImageIndexTK}";
                    buttons();
                }
            }
            else if (eventArgument == "lewo")
            {
                if (!(currentImageIndexTK < 1))
                {
                    currentImageIndexTK--;
                    if (currentImageIndexTK == 0) currentImageIndexTK = 1 ;
                    imageButtonTK.ImageUrl = $"~/Images/img{currentImageIndexTK}.jpg";
                    labelIndexTK.Text = $"{currentImageIndexTK}";
                    buttons();
                }
            }
        }
    }
}