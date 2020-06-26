using Android.App;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Firebase.Auth;

namespace PrivateMessenger.Activities
{
    [Activity(Label = "Private Messenger - Sign In")]
    public class SignIn : AppCompatActivity, IOnCompleteListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignIn);
            var edtEmail = FindViewById<EditText>(Resource.Id.edtEmail);
            var edtPassword = FindViewById<EditText>(Resource.Id.edtPassword);
            var btnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            btnSignIn.Click += (sender, args) => FirebaseAuth.Instance.SignInWithEmailAndPassword(edtEmail.Text, edtPassword.Text).AddOnCompleteListener(this);
        }

        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                if (task.IsSuccessful)
                {
                    Toast.MakeText(this, "Sign In Successful!", ToastLength.Short).Show();
                    StartActivityForResult(typeof(Chat), MainActivity.ResultCode);
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, task.Exception.Message, ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Sign In Field!", ToastLength.Short).Show();
                Finish();
            }
        }
    }
}