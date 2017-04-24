using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panda_Player.Models.PandaPlayer
{
    public class UserAccessControl
    {
        public UserAccessControl()
        {
            LastLogin = DateTime.Now;
            LockoutEndTime = DateTime.Now;
            BanEndTime = DateTime.Now;
        }

        [Key, ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public DateTime UserRegisterDate { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LockoutEndTime { get; set; }

        public DateTime BanEndTime { get; set; }

        public bool IsBanned()
        {
            if (DateTime.Now  < BanEndTime)
            {
                return true;
            }

            return false;
        }

        public string RemainingTimeBan()
        {
            if (this.IsBanned())
            {
                TimeSpan span = BanEndTime.Subtract(DateTime.Now);
                TimeSpan remainingBanTime = new TimeSpan(span.Days, span.Hours, span.Minutes, span.Seconds);

                string formatedRemainingBanTime = $"{remainingBanTime:ddd\\.hh\\:mm\\:ss}";

                return formatedRemainingBanTime;
            }

            return "user is not banned";
        }
    }
}