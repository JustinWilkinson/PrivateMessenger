using Android.App;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using PrivateMessenger.Abstractions;
using PrivateMessenger.Database;

namespace PrivateMessenger.Activities
{
    [Activity(Label = "Private Messenger", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        internal const int ResultCode = 1;

        private static FirebaseDatabase _database;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (PlayServicesAvailable())
            {
                if (_database == null)
                {
                    FirebaseApp.InitializeApp(this);
                    _database = FirebaseDatabase.Instance;
                    _database.SetPersistenceEnabled(true);
                }

                if (FirebaseAuth.Instance.CurrentUser == null)
                {
                    StartActivityForResult(typeof(SignIn), ResultCode);
                }
                else
                {
                    var reference = _database.GetReference("tokens");
                    reference.AddValueEventListener(new TokenValidator(reference, new Settings(this).FirebaseToken));
                    StartActivityForResult(typeof(Chat), ResultCode);
                }
            }
        }

        public bool PlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Toast.MakeText(this, GoogleApiAvailability.Instance.GetErrorString(resultCode), ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "This device is not supported", ToastLength.Short).Show();
                    Finish();
                }

                return false;
            }

            return true;
        }
    }
}