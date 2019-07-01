using System.Collections.Generic;
using GMap.NET;
using SpaceArrow;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Serialize
{
    public class SerializationClass
    {
        public string ComPortName;
        public string ComBaud;
        public string ComDataBits;
        public string ComStopBit;
        public string ComVerify ;
        public string MapName;

        //public float DebugFlag = 1.0f;
        public string Language;

        public List<PointLatLngAlt> BookMark = null;
        public PointLatLng mapCenter ;
        public PointLatLng HomeLocation;
        public PointLatLng planeLocation;

        public ColorType colorScale;
        public ColorType colorTargetLocation;
        public ColorType colorMouseLocation;
        public ColorType colorZoom;
        public ColorType colorTrack;
        public ColorType colorRoutes;

        public bool isIconHomeUseFile;
        public bool isIconPlaneUseFile;
        public bool isIconWpMarkerUseFile;
        public bool isIconBookMarkerUseFile;

        public iconpara iconhome;
        public iconpara iconplane;
        public iconpara iconwpmarker;
        public iconpara iconbookmarker;

        public string datafilepath;
        public string logfilepath;

        public SerializationClass() {

            iconhome = new iconpara();
            iconplane = new iconpara();
            iconwpmarker = new iconpara();
            iconbookmarker = new iconpara();
        }

        public void SetColorTrack(Color color)
        {
            if (color == null) return;
            if (colorTrack == null) colorTrack = new ColorType();

            colorTrack.a = color.A;
            colorTrack.r = color.R;
            colorTrack.g = color.G;
            colorTrack.b = color.B;
        }
        public void SetColorRoutes(Color color)
        {
            if (color == null) return;
            if (colorRoutes == null) colorRoutes = new ColorType();

            colorRoutes.a = color.A;
            colorRoutes.r = color.R;
            colorRoutes.g = color.G;
            colorRoutes.b = color.B;
        }
        public void SetColorZoom(Color color)
        {
            if (color == null) return;
            if (colorZoom == null) colorZoom = new ColorType();

            colorZoom.a = color.A;
            colorZoom.r = color.R;
            colorZoom.g = color.G;
            colorZoom.b = color.B;
        }
        public void SetColorMouseLocation(Color color)
        {
            if (color == null) return;
            if (colorMouseLocation == null) colorMouseLocation = new ColorType();
            colorMouseLocation.a = color.A;
            colorMouseLocation.r = color.R;
            colorMouseLocation.g = color.G;
            colorMouseLocation.b = color.B;
        }
        public void SetColorTargetLocation(Color color)
        {
            if (color == null) return;
            if (colorTargetLocation == null) colorTargetLocation = new ColorType();

            colorTargetLocation.a = color.A;
            colorTargetLocation.r = color.R;
            colorTargetLocation.g = color.G;
            colorTargetLocation.b = color.B;
        }
        public void SetColorScale(Color color) {
            if (color == null) return;
            if (colorScale == null) colorScale = new ColorType();

            colorScale.a = color.A;
            colorScale.r = color.R;
            colorScale.g = color.G;
            colorScale.b = color.B;
        }

        public Color GetColorTrack() {
            if (colorTrack == null) return Color.Transparent;

            return Color.FromArgb(colorTrack.a, colorTrack.r, colorTrack.g, colorTrack.b);
        }
        public Color GetColorRoutes()
        {
            if (colorRoutes == null) return Color.Transparent;

            return Color.FromArgb(colorRoutes.a, colorRoutes.r, colorRoutes.g, colorRoutes.b);
        }
        public Color GetColorZoom()
        {
            if (colorZoom == null) return Color.Transparent;

            return Color.FromArgb(colorZoom.a, colorZoom.r, colorZoom.g, colorZoom.b);
        }
        public Color GetColorMouseLocation()
        {
            if (colorMouseLocation == null) return Color.Transparent;
            return Color.FromArgb(colorMouseLocation.a, colorMouseLocation.r, colorMouseLocation.g, colorMouseLocation.b);
        }
        public Color GetColorTargetLocation()
        {
            if (colorTargetLocation == null) return Color.Transparent;
            return Color.FromArgb(colorTargetLocation.a, colorTargetLocation.r, colorTargetLocation.g, colorTargetLocation.b);
        }
        public Color GetColorScale()
        {
            if (colorScale == null) return Color.Transparent;

            return Color.FromArgb(colorScale.a, colorScale.r, colorScale.g, colorScale.b);
        }
    }

    public class ColorType {
      public byte a;
      public byte r;
      public byte g;
      public byte b;
    }
    public class iconpara {
        public string filepath = "";
        public int iconwidth;
        public int iconheight;
    }


}
