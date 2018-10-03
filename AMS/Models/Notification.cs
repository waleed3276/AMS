﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class Notification
    {
        [Key]
        public int Notification_Id { get; set; }

        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int Notification_ItemId { get; set; }

        public string Notification_ItemType { get; set; }

        public string Notification_Detail { get; set; }

        public DateTime Notification_Date { get; set; }

        public bool Notification_IsSeen { get; set; }

        public bool Notification_Status { get; set; }
    }
}