using Android.Content;
using Android.Preferences;

namespace PrivateMessenger.Abstractions
{
    public class Settings
    {
        private readonly Context _context;

        public Settings(Context context)
        {
            _context = context;
        }

        private string _firebaseToken;

        public string FirebaseToken
        {
            get => _firebaseToken ?? PreferenceManager.GetDefaultSharedPreferences(_context).GetString("private_messenger_firebase_token", null);
            set
            {
                _firebaseToken = value;
                PreferenceManager.GetDefaultSharedPreferences(_context).Edit().PutString("private_messenger_firebase_token", value).Apply();
            }
        }
    }
}