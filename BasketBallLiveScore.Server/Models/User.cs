﻿namespace BasketBallLiveScore.Server.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Mot de passe haché
    }
}