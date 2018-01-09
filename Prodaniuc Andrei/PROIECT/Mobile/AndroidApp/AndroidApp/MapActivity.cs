using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using System;
using Android.Gms.Maps.Model;
using static Android.Gms.Maps.GoogleMap;
using Android.Views;
using Android.Graphics;
using Com.Google.Maps.Android.Heatmaps;
using System.Collections.Generic;
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using Android.Content;
using MyApp.Services;
using System.Linq;
using Newtonsoft.Json;

namespace MyApp
{
    [Activity(Label = "MapActivity", ParentActivity = typeof(MainActivity))]
    public class MapActivity : Activity, IOnMapReadyCallback, IInfoWindowAdapter, IOnInfoWindowClickListener, IOnMapLongClickListener
    {
        private GoogleMap gmap;

        private Button submitBtn;
        private Button btnNormal;
        private Button btnHybrid;
        private Button btnSatellite;
        private Button btnTerrain;
        private List<LatLng> Sensors;
        private string userId;
        private string areaId;
        private EditText et;
        private bool heatMap;
        private List<LatLng> data = new List<LatLng>();

        public async void OnMapReady(GoogleMap googleMap)
        {
            IrrigationService srv = new IrrigationService();
            //if (!string.IsNullOrEmpty(areaId))
            //    data = await srv.GetDataForArea(areaId);
            int[] colors = {
                Color.Rgb(102, 225, 0), //green
                Color.Rgb(255, 0, 0)    // red
            };
            float[] startPoints = { 0.2f, 1f };
            var gradient = new Gradient(colors, startPoints);
            gmap = googleMap;
            if (data.Any())
            {
                var mProvider = new HeatmapTileProvider.Builder().Data(data).Gradient(gradient).Build();
                mProvider.SetOpacity(0.7);
                gmap.AddTileOverlay(new TileOverlayOptions().InvokeTileProvider(mProvider));
                gmap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(data.FirstOrDefault(), 9));
            }
            else
            {
                gmap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(45.780153, 21.172579), 6));
            }
           
            gmap.MapLongClick += Gmap_MapLongClick;
            gmap.UiSettings.MyLocationButtonEnabled = true;
            gmap.UiSettings.MapToolbarEnabled = true;
            gmap.UiSettings.CompassEnabled = true;
            gmap.UiSettings.RotateGesturesEnabled = true;
            
            gmap.SetInfoWindowAdapter(this);
            gmap.SetOnInfoWindowClickListener(this);

            /*MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(41, 23));
            markerOptions.SetTitle("My position");
            markerOptions.SetSnippet("Info");
            gmap.AddMarker(markerOptions);
            PolygonOptions rectangle = new PolygonOptions();
            rectangle
                .Add(new LatLng(41.05, 23))
                .Add(new LatLng(41.25, 23))
                .Add(new LatLng(41.23, 23.10));

            CircleOptions circle = new CircleOptions();
            circle.InvokeRadius(50000);
            circle.InvokeCenter(new LatLng(41.05, 23));
            gmap.AddCircle(circle);

            rectangle.InvokeFillColor(Color.Rgb(184, 26, 141));
            Polygon polyline = gmap.AddPolygon(rectangle);*/


        }

        private void Gmap_MapLongClick(object sender, MapLongClickEventArgs e)
        {
            var point = e.Point;
            Sensors.Add(point);
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(point);
            markerOptions.SetTitle("Sensor");
            markerOptions.SetSnippet("Info");
            gmap.AddMarker(markerOptions);
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Sensors = new List<LatLng>();
            // Create your application here
            userId = Intent.Extras.GetString("UserId");
            var userSetup = Intent.Extras.GetBoolean("IsSetUp");
            if (!userSetup)
            {
                RequestWindowFeature(WindowFeatures.NoTitle);
            }

            SetContentView(Resource.Layout.Map);
            SetupButtons();
            if (!userSetup)
                Toast.MakeText(this, "Please select your pins on the map", ToastLength.Long).Show();
            if (userSetup)
            {
                var proj = JsonConvert.DeserializeObject<Projection>(Intent.Extras.GetString("Projection"));
                var points = JsonConvert.DeserializeObject<List<Coord>>(proj.Data);
                foreach(var p in points)
                {
                    data.Add(new LatLng(p.Latitude, p.Longitude));
                }
                areaId = Intent.Extras.GetString("AreaId");
                heatMap= Intent.Extras.GetBoolean("Heatmap");
                submitBtn.Visibility = ViewStates.Invisible;
            }

         
            SetUpMap();
        }

        private void SetupButtons()
        {
            btnNormal = FindViewById<Button>(Resource.Id.btnNormal);
            btnHybrid = FindViewById<Button>(Resource.Id.btnHybrid);
            btnSatellite = FindViewById<Button>(Resource.Id.btnSatellite);
            btnTerrain = FindViewById<Button>(Resource.Id.btnTerrain);
            submitBtn = FindViewById<Button>(Resource.Id.SubmitBtn);
            btnNormal.Click += BtnNormal_Click;
            btnHybrid.Click += BtnHybrid_Click;
            btnSatellite.Click += BtnSatellite_Click;
            btnTerrain.Click += BtnTerrain_Click;
            submitBtn.Click += SubmitBtn_Click;
        }


        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            var service = new IrrigationService();
            //create area
            var _areaId = await service.CreateArea(userId, "AreaName");
            //add sensors to area
            if (!string.IsNullOrEmpty(_areaId))
            {
                await service.PostSensorsForArea(Sensors, _areaId);
                //setup user
                await service.SetupUser(userId);

                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("UserId", userId);
                intent.PutExtra("IsSetUp", true);
                StartActivity(intent);
            }
        }

        #region maptype
        private void BtnTerrain_Click(object sender, EventArgs e)
        {
            gmap.MapType = GoogleMap.MapTypeTerrain;
        }

        private void BtnSatellite_Click(object sender, EventArgs e)
        {
            gmap.MapType = GoogleMap.MapTypeSatellite;
        }

        private void BtnHybrid_Click(object sender, EventArgs e)
        {
            gmap.MapType = GoogleMap.MapTypeHybrid;
        }

        private void BtnNormal_Click(object sender, EventArgs e)
        {
            gmap.MapType = GoogleMap.MapTypeNormal;
        }
        #endregion
        private void SetUpMap()
        {
            if (gmap == null)
            {
                MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
                mapFragment.GetMapAsync(this);
            }
        }

        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        public View GetInfoWindow(Marker marker)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.info_window, null, false);
            view.FindViewById<TextView>(Resource.Id.txtName).Text = "Xamarin";
            view.FindViewById<TextView>(Resource.Id.txtAddress).Text = "Xamarinlal";
            view.FindViewById<TextView>(Resource.Id.txtHours).Text = "Xamarinsddd";
            return view;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            Console.WriteLine("Info windows clicked");
        }

        public void OnMapLongClick(LatLng point)
        {
            Sensors.Add(point);
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(point);
            markerOptions.SetTitle("Sensor");
            markerOptions.SetSnippet("Info");
            gmap.AddMarker(markerOptions);
        }
    }
}