using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using PrivateMessenger.Abstractions;
using PrivateMessenger.Adapters;
using PrivateMessenger.Extensions;

namespace PrivateMessenger.Activities
{
    [Activity(Label = "Private Messenger - Chat")]
    public class Chat : AppCompatActivity, IValueEventListener
    {
        private FirebaseUser _user;

        private ListView _chat;
        private EditText _editText;
        private FloatingActionButton _button;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _user = FirebaseAuth.Instance.CurrentUser;

            SetContentView(Resource.Layout.Chat);
            Toast.MakeText(this, $"Welcome {_user.Email}", ToastLength.Short).Show();

            var chats = FirebaseDatabase.Instance.GetReference("chats");
            chats.KeepSynced(true);
            chats.AddValueEventListener(this);

            _button = FindViewById<FloatingActionButton>(Resource.Id.fab);
            _editText = FindViewById<EditText>(Resource.Id.input);
            _chat = FindViewById<ListView>(Resource.Id.list_of_messages);
            _button.Click += (sender, args) => PostMessageAsync();
        }


        private async void PostMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(_editText.Text))
            {
                Toast.MakeText(this, "Please enter some text!", ToastLength.Short).Show();
            }
            else
            {
                await FirebaseDatabase.Instance.GetReference("chats").PutAsync(new MessageContent(_user.Email, _editText.Text, App.Settings.FirebaseToken));
                _editText.Text = "";
            }
        }

        public void OnCancelled(DatabaseError error)
        {
            Log.Error(error.Details, error.Message);
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            _chat.Adapter = new ListViewAdapter(this, snapshot.MapChildren(child => new MessageContent(child)));
        }
    }
}