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
using System.Threading.Tasks;

namespace MyApp
{
    [Activity(Label = "LoginActivity", MainLauncher = true, Theme = "@style/MyTheme")]
    public class LoginActivity : Activity
    {
        private EditText email;
        private EditText password;
        private Button loginBtn;
        private TextView registerText;
        private IrrigationService _service;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);
            loginBtn = FindViewById<Button>(Resource.Id.login);
            email = FindViewById<EditText>(Resource.Id.loginEmail);
            password = FindViewById<EditText>(Resource.Id.loginPassword);
            registerText = FindViewById<TextView>(Resource.Id.loginRegister);
            email.Text = "aprodaniuc@mail.com";
            password.Text = "123456";

            registerText.Click += RegisterText_Click;
            loginBtn.Click += LoginBtn_Click;
        }

        private void RegisterText_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(email.Text) && !string.IsNullOrEmpty(password.Text))
            {
                _service = new IrrigationService();
                var result = await _service.GetLogin(email.Text, password.Text);
                if (result!=null && !result.IsSetUp)
                {
                    Intent intent = new Intent(this, typeof(MapActivity));
                    intent.PutExtra("UserId", result.Id);
                    intent.PutExtra("IsSetUp", result.IsSetUp);
                    StartActivity(intent);
                }
                else if(result == null)
                {
                    Toast.MakeText(this, "Incorrect credentials!", ToastLength.Long).Show();
                }
                else if (result.IsSetUp)
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra("UserId", result.Id);
                    intent.PutExtra("IsSetUp", result.IsSetUp);
                    StartActivity(intent);
                }
            }
            else
                Toast.MakeText(this, "Email and password cannot be empty!", ToastLength.Long).Show();
        }
    }
}