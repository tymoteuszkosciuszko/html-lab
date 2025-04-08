using System;
using System.Collections.Generic;
using System.Web;

namespace Polozenie_rysunkow_stan
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Application["ActiveUsers"] = new List<UserSession>();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                lock (Application["ActiveUsers"])
                {
                    var activeUsers = (List<UserSession>)Application["ActiveUsers"];
                    activeUsers.Add(new UserSession
                    {
                        Username = username,
                        StartTime = DateTime.Now
                    });
                }
                System.Diagnostics.Debug.WriteLine("Nowa sesja dla użytkownika " + username);
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
                    activeUsers.RemoveAll(u => u.Username == username);
                }
                System.Diagnostics.Debug.WriteLine($"Sesja {username} wygasła.");
            }
        }
    }
}
