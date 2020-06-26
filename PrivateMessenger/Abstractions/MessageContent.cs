using Firebase.Database;
using Java.Util;
using PrivateMessenger.JavaHelpers;
using System;
using System.Collections.Generic;
using JavaObject = Java.Lang.Object;

namespace PrivateMessenger.Abstractions
{
    public class MessageContent : IJavaConvertible
    {
        public string Email { get; set; }

        public string Message { get; set; }

        public DateTime Time { get; set; }

        public string SenderToken { get; set; }

        public MessageContent()
        {

        }

        public MessageContent(string email, string message, string senderToken)
        {
            Email = email;
            Message = message;
            SenderToken = senderToken;
            Time = DateTime.Now;
        }

        public MessageContent(DataSnapshot snapshot)
        {
            if (snapshot.HasChildren)
            {
                Email = snapshot.Child("Email")?.GetValue(true)?.ToString();
                Message = snapshot.Child("Message")?.GetValue(true)?.ToString();
                SenderToken = snapshot.Child("SenderToken")?.GetValue(true)?.ToString();
                Time = DateTime.Parse(snapshot.Child("Time")?.GetValue(true)?.ToString());
            }
        }

        public JavaObject Convert()
        {
            return new HashMap(new Dictionary<string, string>
            {
                { "Email", Email },
                { "Message", Message },
                { "SenderToken", SenderToken },
                { "Time", Time.ToString("o") }
            });
        }
    }
}