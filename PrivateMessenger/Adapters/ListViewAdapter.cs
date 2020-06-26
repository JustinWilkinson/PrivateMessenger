using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PrivateMessenger.Abstractions;
using PrivateMessenger.Activities;
using System.Collections.Generic;
using System.Linq;

namespace PrivateMessenger.Adapters
{
    public class ListViewAdapter : BaseAdapter
    {
        private readonly Chat _chatActivity;
        private readonly List<MessageContent> _messages;

        public ListViewAdapter(Chat chatActivity, IEnumerable<MessageContent> messages)
        {
            _chatActivity = chatActivity;
            _messages = messages.ToList();
        }

        public override int Count => _messages.Count;

        public override Object GetItem(int position) => position;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater)_chatActivity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            var itemView = inflater.Inflate(Resource.Layout.ListItem, null);

            itemView.FindViewById<TextView>(Resource.Id.message_user).Text = _messages[position].Email;
            itemView.FindViewById<TextView>(Resource.Id.message_time).Text = _messages[position].Time.ToString("yyyy-MM-dd HH:mm:ss");
            itemView.FindViewById<TextView>(Resource.Id.message_text).Text = _messages[position].Message;

            return itemView;
        }
    }
}