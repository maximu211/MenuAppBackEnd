﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.DAL.Models
{
    public class Subscription
    {
        public ObjectId UserId { get; set; }
        public List<ObjectId> Subscribers { get; set; }

        public Subscription()
        {
            Subscribers = new List<ObjectId>();
        }
    }
}