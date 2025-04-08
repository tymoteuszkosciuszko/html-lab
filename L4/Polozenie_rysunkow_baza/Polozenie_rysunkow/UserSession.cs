using System;
using System.Collections.Generic;

namespace Polozenie_rysunkow_baza
{
    public class UserSession
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan MinReactionTime { get; set; } = TimeSpan.MaxValue;
        public TimeSpan MaxReactionTime { get; set; } = TimeSpan.MinValue;
        public int IleNastapiloZmianRozmiaruZdjecia { get; set; } = 0;
        public int IleNastapiloZmianPolozeniaZdjecia { get; set; } = 0;
        public string SessionID { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
