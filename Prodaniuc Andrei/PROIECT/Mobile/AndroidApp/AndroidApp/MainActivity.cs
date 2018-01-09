using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android;
using System.Collections.Generic;
using MyApp.Services;
using Newtonsoft.Json;

namespace MyApp
{
    [Activity(Label = "AndroidApp", Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {

        private MyActionBarDrawerToggle mDrawerToggle;
        private SupportToolbar mToolbar;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private ListView mRightDrawer;
        private string userId;
        private bool userSetup;
        private Projection projection;

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
           

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);

            SetSupportActionBar(mToolbar);

            var mLeftDataSet = new List<string>();
            mLeftDataSet.Add("Heat map");
            mLeftDataSet.Add("Area");
            var mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
            mLeftDrawer.Adapter = mLeftAdapter;

            mLeftDrawer.ItemClick += MLeftDrawer_ItemClick;

            mDrawerToggle = new MyActionBarDrawerToggle(this,
                mDrawerLayout,
                Resource.String.openDrawer,
                Resource.String.closeDrawer);

            mDrawerLayout.AddDrawerListener(mDrawerToggle);
            //SupportActionBar.SetHomeButtonEnabled(true);
            //SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mDrawerToggle.SyncState();

            if (bundle != null)
            {
                if (bundle.GetString("DrawerState") == "Opened")
                {
                    SupportActionBar.SetTitle(Resource.String.openDrawer);
                }
                else
                {
                    SupportActionBar.SetTitle(Resource.String.closeDrawer);
                }
                userId = bundle.GetString("UserId");
                userSetup = bundle.GetBoolean("IsSetUp");
            }
            else
            {
                SupportActionBar.SetTitle(Resource.String.closeDrawer);
                userId = Intent.Extras.GetString("UserId");
                userSetup = Intent.Extras.GetBoolean("IsSetUp");
            }
            var service = new IrrigationService();
            var areaId = (await service.GetAreasForUser(userId)).Id;
            projection = await service.GetDataForArea(areaId);
            LoadLabels();
        }

        private void LoadLabels()
        {
            TextView areaLabel = FindViewById<TextView>(Resource.Id.areaText);
            TextView nrSensorsLabel = FindViewById<TextView>(Resource.Id.numberOfSensorsText);
            TextView activeLabel = FindViewById<TextView>(Resource.Id.activeSensorsText);
            TextView averageLabel = FindViewById<TextView>(Resource.Id.averageValueText);

            areaLabel.Text = areaLabel.Text + " " + "Timisoara";
            averageLabel.Text = averageLabel.Text + " " + projection.Average;
        }

        private async void MLeftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(MapActivity));
            var srv = new IrrigationService();
            var areaId = (await srv.GetAreasForUser(userId)).Id;
            intent.PutExtra("Projection", JsonConvert.SerializeObject(projection));
            intent.PutExtra("AreaId", areaId);
            var heatMap = e.Position == 0 ? true : false;
            intent.PutExtra("Heatmap", heatMap);
            intent.PutExtra("UserId", userId);
            intent.PutExtra("IsSetUp", userSetup);
            StartActivity(intent);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_refresh:
                    return true;
                case Resource.Id.action_help:
                    if (mDrawerLayout.IsDrawerOpen(mRightDrawer))
                    {
                        mDrawerLayout.CloseDrawer(mRightDrawer);
                    }
                    else
                    {
                        mDrawerLayout.OpenDrawer(mRightDrawer);
                        mDrawerLayout.CloseDrawer(mLeftDrawer);
                    }
                    return true;
                case Android.Resource.Id.Home:
                    mDrawerLayout.CloseDrawer(mRightDrawer);
                    mDrawerToggle.OnOptionsItemSelected(item);

                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (userId != null)
                outState.PutString("UserId", userId);
            outState.PutBoolean("IsSetUp", userSetup);
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }
            else
            {
                outState.PutString("DrawerState", "Closed");
            }
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }

    }
}

