using Android.App;
using Android.Content.Res;
using Android.Runtime;
using PrivateMessenger.Abstractions;
using System;

namespace PrivateMessenger
{    
    #if DEBUG
    [Application(Debuggable = true)]
    #else
    [Application(Debuggable = false)]
    #endif
    public class App : Application
    {
        public static Resources AppResources { get; set; }

        public static Settings Settings { get; set; }

        public App() : base()
        {

        }

        public App(IntPtr javaReference, JniHandleOwnership handleOwnership) : base(javaReference, handleOwnership)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            AppResources = Resources;
            Settings = new Settings(this);
        }
    }
}