using Android.Util;
using Firebase.Database;
using PrivateMessenger.Extensions;
using System.Linq;
using JavaObject = Java.Lang.Object;

namespace PrivateMessenger.Database
{
    public class TokenValidator : JavaObject, IValueEventListener
    {
        private readonly string _token;
        private readonly DatabaseReference _reference;

        public TokenValidator(DatabaseReference reference, string token)
        {
            _reference = reference;
            _token = token;
        }

        public async void OnDataChange(DataSnapshot snapshot)
        {
            _reference.RemoveEventListener(this);

            if (!snapshot.MapChildren(child => child.GetChildValueString("Token")).Any(x => x == _token))
            {
                await _reference.PutAsync(new { Token = _token });
            }
        }

        public void OnCancelled(DatabaseError error)
        {
            _reference.RemoveEventListener(this);
            Log.Error(error.Details, error.Message);
        }
    }
}