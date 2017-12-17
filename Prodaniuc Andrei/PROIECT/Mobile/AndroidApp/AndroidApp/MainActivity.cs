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

namespace MyApp
{
    [Activity(Label = "AndroidApp",  Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {

        private MyActionBarDrawerToggle mDrawerToggle;
        private SupportToolbar mToolbar;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private ListView mRightDrawer;

        protected override void OnCreate(Bundle bundle)
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
            mLeftDataSet.Add("Left Item 1");
            mLeftDataSet.Add("Left Item 2");
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
            }
            else
            {
                SupportActionBar.SetTitle(Resource.String.closeDrawer);
            }
        }

        private void MLeftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(MapActivity));
            intent.PutExtra("obj", @"{""obj"":""value""}");
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

