using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialize
{
    class GpxFileData
    {
      public  List<wpt> GPX_wpt = new List<wpt>();
      public List<rte> GPX_rte = new List<rte>();
      public List<trk> GPX_trk = new List<trk>();




    }


    class wpt {//航点
        //Required Information
      public   double lat = 0 ;//Latitude of the waypoint
      public   double lon = 0;//Longitude of the waypoint

        //Optional Position Information
        public float ele = 0;//Elevation of waypoint.
        public DateTime time = DateTime.Now;//Creation date/time of thewaypoint in Univeral Coordinated Time (UTC), not local time! Conforms to ISO 8601 specification for date/time representation.
        public float mavgvar = 0;//Mnagtic varition of the waypoint
        public float geoidheight = 0;//Height, in meters, of geoid (mean sea level) above WGS-84 earth ellipsoid. (NMEA GGA message);

        //Optinal Description Information
        public string name = "";   //GPS waypoint name of the waypoint
        public string cmt = "";    //GPS comment of the waypoint
        public string desc = "";   //Descriptive description of the waypoint.
        public string src = "";    //source of the waypoint data.
        public string url = "";    //URL associated withe waypoint.
        public string urlname = "";//Text to display on the url hyperlink
        public string type = "";   //Type(category) of waypoint.

        //Optional Accuracy Information
        public string fix = "";//Type of GPS fix;
        public string sat = "";//Number of satellites
        public string hdop = "";//HDOP
        public string vdop = "";//VDOP
        public string pdop = "";//PDOP
        public string ageofdgpsdata = "";//Time since last DGPS fix
        public string dgpsid = "";//DGPS station ID

        //Optional Private Information

        //In here, you can define something else you need.
        public wpt() { }
    }
    class rte { //routes
        //Optional Information
        public string name = "";//GPS route name;
        public string cmt = "";//GPS route comment
        public string desc = "";//Description of the route
        public string src = "";//Source of the route data.
        public string url = "";//URL associated with the route.
        public string urlname = "";//Text to display on the url hyperlink
        public string number = "";//GPS route number.


        public List<rtept> listrtept = new List<rtept>();//list of the routepoints

        //Optional Private Information

        //In here, you can define something else you need.


    }
    class rtept { //routepoint
        //Required Information
        public double lat = 0;//Latitude of the routepoint
        public double lon = 0;//Longitude of the routepoint


        //Optional Position Information
        public float ele = 0;//Elevation of routepoint.
        public DateTime time = DateTime.Now;//Creation date/time of the routepoint in Univeral Coordinated Time (UTC), not local time! Conforms to ISO 8601 specification for date/time representation.
        public float mavgvar = 0;//Mnagtic varition of the routepoint
        public float geoidheight = 0;//Height, in meters, of geoid (mean sea level) above WGS-84 earth ellipsoid. (NMEA GGA message);

        //Optinal Description Information
        public string name = "";   //GPS routepoint name of the routepoint
        public string cmt = "";    //GPS comment of the routepoint
        public string desc = "";   //Descriptive description of the routepoint.
        public string src = "";    //source of the routepoint data.
        public string url = "";    //URL associated withe routepoint.
        public string urlname = "";//Text to display on the url hyperlink
        public string sym = "";    //routepoint symbol
        public string type = "";   //Type(category) of routepoint.

        //Optional Accuracy Information
        public string fix = "";//Type of GPS fix;
        public string sat = "";//Number of satellites
        public string hdop = "";//HDOP
        public string vdop = "";//VDOP
        public string pdop = "";//PDOP
        public string ageofdgpsdata = "";//Time since last DGPS fix
        public string dgpsid = "";//DGPS station ID


        //Optional Private Information

        //In here, you can define something else you need.


    }



    class trk { //
        //Optinal Description Information
        public string name = "";   //GPS track name of the track
        public string cmt = "";    //GPS comment of the track
        public string desc = "";   //Descriptive description of the track.
        public string src = "";    //source of the track data.
        public string url = "";    //URL associated withe track.
        public string urlname = "";//Text to display on the url hyperlink
        public string number = "";//GPS track number.

        public List<trkseg> listtrkseg = new List<trkseg>();


    }


    class trkseg {
        public List<trackpoint> listtrkseg = new List<trackpoint>();
    }
    class trackpoint {
        //Required Information
        public double lat = 0;//Latitude of the trackpoint
        public double lon = 0;//Longitude of the trackpoint


        //Optional Position Information
        public float ele = 0;//Elevation of trackpoint.
        public DateTime time = DateTime.Now;//Creation date/time of the trackpoint in Univeral Coordinated Time (UTC), not local time! Conforms to ISO 8601 specification for date/time representation.
        public float mavgvar = 0;//Mnagtic varition of the trackpoint
        public float geoidheight = 0;//Height, in meters, of geoid (mean sea level) above WGS-84 earth ellipsoid. (NMEA GGA message);

        //Optinal Description Information
        public string name = "";   //GPS trackpoint name of the trackpoint
        public string cmt = "";    //GPS comment of the trackpoint
        public string desc = "";   //Descriptive description of the trackpoint.
        public string src = "";    //source of the trackpoint data.
        public string url = "";    //URL associated withe trackpoint.
        public string urlname = "";//Text to display on the url hyperlink
        public string sym = "";    //trackpoint symbol
        public string type = "";   //Type(category) of trackpoint.

        //Optional Accuracy Information
        public string fix = "";//Type of GPS fix;
        public string sat = "";//Number of satellites
        public string hdop = "";//HDOP
        public string vdop = "";//VDOP
        public string pdop = "";//PDOP
        public string ageofdgpsdata = "";//Time since last DGPS fix
        public string dgpsid = "";//DGPS station ID

        //Optional Private Information

        //In here, you can define something else you need.
    }
}
