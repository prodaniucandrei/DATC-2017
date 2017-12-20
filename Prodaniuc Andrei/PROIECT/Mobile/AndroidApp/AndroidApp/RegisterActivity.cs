using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyApp.Services;

namespace MyApp
{
    [Activity(Label = "RegisterActivity", ParentActivity = typeof(LoginActivity))]
    public class RegisterActivity : Activity
    {
        #region uicontrols
        private EditText email;
        private EditText password;
        private EditText passwordRetype;
        private Button register;
        #endregion

        private IrrigationService _service;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            SetupControls();
        }

        private void SetupControls()
        {
            email = FindViewById<EditText>(Resource.Id.registerEmail);
            password = FindViewById<EditText>(Resource.Id.registerPassword);
            passwordRetype = FindViewById<EditText>(Resource.Id.registerPasswordRetype);
            register = FindViewById<Button>(Resource.Id.register);

            register.Click += Register_Click;
        }

        private async void Register_Click(object sender, EventArgs e)
        {
            if (CheckEmailBox())
            {
                Toast.MakeText(this, "Please insert your email", ToastLength.Long).Show();
                return;
            }
            if (CheckPasswordBoxes())
            {
                Toast.MakeText(this, "Please insert your password and then retype it.", ToastLength.Long).Show();
                return;
            }
            _service = new IrrigationService();
            
            var result = await _service.CreateUser(email.Text, password.Text);
            if (!result)
            {
                Toast.MakeText(this, "An error occured. The user was not created", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "User created successfully", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
            }
        }

        private bool CheckEmailBox()
        {
            return string.IsNullOrEmpty(email.Text);
        }

        private bool CheckPasswordBoxes()
        {
            return string.IsNullOrEmpty(password.Text) &&
                string.IsNullOrEmpty(passwordRetype.Text) &&
                password.Text.Equals(passwordRetype.Text);
        }
    }
}