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

        public void OnMapReady(GoogleMap googleMap)
        {
            var data = new List<LatLng>()
            {
                new LatLng(41.12345, 23.12345),
                 new LatLng(41.123456, 23.123455),
                  new LatLng(41.123457, 23.123456),
                   new LatLng(41.123458, 23.123457),
                new LatLng(41.12355, 23.12355),
                new LatLng(41.12375, 23.12365),
                new LatLng(41.12385, 23.12385),
                new LatLng(41.12395, 23.12395)
            };

            int[] colors = {
                Color.Rgb(102, 225, 0), //green
                Color.Rgb(255, 0, 0)    // red
            };
            float[] startPoints = { 0.2f, 1f };
            var gradient = new Gradient(colors, startPoints);

            var mProvider = new HeatmapTileProvider.Builder().Data(data).Gradient(gradient).Build();

            mProvider.SetOpacity(0.7);


            gmap = googleMap;
            gmap.MapLongClick += Gmap_MapLongClick;

            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(41, 23));
            markerOptions.SetTitle("My position");
            markerOptions.SetSnippet("Info");
            gmap.AddMarker(markerOptions);

            gmap.UiSettings.MyLocationButtonEnabled = true;
            gmap.UiSettings.MapToolbarEnabled = true;
            gmap.UiSettings.CompassEnabled = true;
            gmap.UiSettings.RotateGesturesEnabled = true;
            gmap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(41, 23), 6));

            gmap.SetInfoWindowAdapter(this);
            gmap.SetOnInfoWindowClickListener(this);

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
            Polygon polyline = gmap.AddPolygon(rectangle);

            gmap.AddTileOverlay(new TileOverlayOptions().InvokeTileProvider(mProvider));
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            userId = Intent.Extras.GetString("UserId");
            var data = Intent.Extras.GetBoolean("IsSetUp");
            if (!data)
            {
                RequestWindowFeature(WindowFeatures.NoTitle);
            }
            Sensors = new List<LatLng>();
            SetContentView(Resource.Layout.Map);
            submitBtn = FindViewById<Button>(Resource.Id.SubmitBtn);
            if (data)
            {
                areaId = Intent.Extras.GetString("AreaId");
                submitBtn.Visibility = ViewStates.Invisible;

            }
            btnNormal = FindViewById<Button>(Resource.Id.btnNormal);
            btnHybrid = FindViewById<Button>(Resource.Id.btnHybrid);
            btnSatellite = FindViewById<Button>(Resource.Id.btnSatellite);
            btnTerrain = FindViewById<Button>(Resource.Id.btnTerrain);

            btnNormal.Click += BtnNormal_Click;
            btnHybrid.Click += BtnHybrid_Click;
            btnSatellite.Click += BtnSatellite_Click;
            btnTerrain.Click += BtnTerrain_Click;
            submitBtn.Click += SubmitBtn_Click;
            SetUpMap();
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