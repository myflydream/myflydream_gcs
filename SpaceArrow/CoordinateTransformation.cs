using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;


namespace SpaceArrow
{
    class CoordinateTransformation
    {

        const double pi = 3.14159265358979324;
        const double a = 6378245.0;
        const double ee = 0.00669342162296594323;
        const double x_pi = 3.14159265358979324 * 3000.0 / 180.0;

        public CoordinateTransformation() { }

        bool outOfChina(double lat, double lon){
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }
        double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * System.Math.Sqrt(System.Math.Abs(x));
            ret += (20.0 * System.Math.Sin(6.0 * x * pi) + 20.0 * System.Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * System.Math.Sin(y * pi) + 40.0 * System.Math.Sin(y /3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * System.Math.Sin(y / 12.0 * pi) + 320 * System.Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        double transformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * System.Math.Sqrt(System.Math.Abs(x));
            ret += (20.0 * System.Math.Sin(6.0 * x * pi) + 20.0 * System.Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * System.Math.Sin(x * pi) + 40.0 * System.Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * System.Math.Sin(x / 12.0 * pi) + 300.0 * System.Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
            return ret;
        }  

    /** 
     * 地球坐标转换为火星坐标 
     * World Geodetic System ==> Mars Geodetic System 
     * 
     * @param wgLat  地球坐标 
     * @param wgLon 
     * 
     * mglat,mglon 火星坐标 
     */
    void transform2Mars(double wgLat, double wgLon, out double mgLat, out double mgLon)  
    {
        if (outOfChina(wgLat, wgLon) || !Program.isMarsCoordinatesInChina)  
        {  
            mgLat  = wgLat;  
            mgLon = wgLon;  
            return ;  
        }  
        double dLat = transformLat(wgLon - 105.0, wgLat - 35.0);  
        double dLon = transformLon(wgLon - 105.0, wgLat - 35.0);  
        double radLat = wgLat / 180.0 * pi;  
        double magic = System.Math.Sin(radLat);  
        magic = 1 - ee * magic * magic;  
        double sqrtMagic = System.Math.Sqrt(magic);  
        dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);  
        dLon = (dLon * 180.0) / (a / sqrtMagic * System.Math.Cos(radLat) * pi);  
        mgLat = wgLat + dLat;  
        mgLon = wgLon + dLon;  
    }
    void transform2Mars1(double wgLat, double wgLon, out double mgLat, out double mgLon)
    {
        if (outOfChina(wgLat, wgLon) || !Program.isMarsCoordinatesInChina)
        {
            mgLat = wgLat;
            mgLon = wgLon;
            return;
        }
        double dLat = transformLat(wgLon - 105.0, wgLat - 35.0);
        double dLon = transformLon(wgLon - 105.0, wgLat - 35.0);
        double radLat = wgLat / 180.0 * pi;
        double magic = System.Math.Sin(radLat);
        magic = 1 - ee * magic * magic;
        double sqrtMagic = System.Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
        dLon = (dLon * 180.0) / (a / sqrtMagic * System.Math.Cos(radLat) * pi);
        mgLat = wgLat + dLat;
        mgLon = wgLon + dLon;
    }  
    /** 
     * 火星坐标转换为百度坐标 
     * @param gg_lat 
     * @param gg_lon 
     */  
     void bd_encrypt(double gg_lat, double gg_lon,ref double bd_lat,ref double bd_lon)  
    {  
        double x = gg_lon, y = gg_lat;  
        double z = System.Math.Sqrt(x * x + y * y) + 0.00002 * System.Math.Sin(y * x_pi);  
        double theta = System.Math.Atan2(y, x) + 0.000003 * System.Math.Cos(x * x_pi);  
        bd_lon = z * System.Math.Cos(theta) + 0.0065;  
        bd_lat = z * System.Math.Sin(theta) + 0.006;  
  
    }  
  
    /** 
     * 百度转火星 
     * @param bd_lat 
     * @param bd_lon 
     */  
     void bd_decrypt(double bd_lat, double bd_lon,ref double gg_lat,ref double gg_lon)  
    {  
        double x = bd_lon - 0.0065, y = bd_lat - 0.006;
        double z = System.Math.Sqrt(x * x + y * y) - 0.00002 * System.Math.Sin(y * x_pi);
        double theta = System.Math.Atan2(y, x) - 0.000003 * System.Math.Cos(x * x_pi);
        gg_lon = z * System.Math.Cos(theta);
        gg_lat = z * System.Math.Sin(theta);  
  
    }
        
     public PointLatLng Mar2Earth(PointLatLng Mar) {
         PointLatLng Earth = new PointLatLng();
         if (outOfChina(Mar.Lat, Mar.Lng) || !Program.isMarsCoordinatesInChina)
         {
            return Mar;
         }

        PointLatLng tmp;
        double initDelta = 0.1;
        //double threshold = 0.00000001;//0.00000002097630869002387
        double threshold = 0.000001;
        double dLat = initDelta, dLon = initDelta;
        double mLat = Mar.Lat - dLat, mLon = Mar.Lng - dLon;
        double pLat = Mar.Lat + dLat, pLon = Mar.Lng + dLon;
        double wgsLat, wgsLon, i = 0;
        double lastDlat = initDelta, lastDlng = initDelta;
        PointLatLng lasttmp = new PointLatLng((mLat + pLat) / 2, (mLon + pLon) / 2);
        PointLatLng minitemp=new PointLatLng();

        while (true) {
            wgsLat = (mLat + pLat) / 2;
            wgsLon = (mLon + pLon) / 2;

            tmp = GetEarth2Mars(new PointLatLng(wgsLat, wgsLon));


            dLat = tmp.Lat - Mar.Lat;
            dLon = tmp.Lng - Mar.Lng;

            if ((System.Math.Abs(dLat) < threshold) && (System.Math.Abs(dLon) < threshold))
                break;

            if (dLat + dLon >= lastDlat + lastDlng)
            {
            }
            else {
                lastDlat = dLat;
                lastDlng = dLon;

                minitemp.Lat = tmp.Lat;
                minitemp.Lng = tmp.Lng;
            }

            if (lasttmp.Lat == tmp.Lat && lasttmp.Lng == tmp.Lng) {
                wgsLat = minitemp.Lat;
                wgsLon = minitemp.Lng;

                break;
            }
            lasttmp = tmp;

            if (dLat > 0) pLat = wgsLat; else mLat = wgsLat;
            if (dLon > 0) pLon = wgsLon; else mLon = wgsLon;

         //   System.Diagnostics.Debug.WriteLine("i:" + i + "\twgsLat:" + wgsLat.ToString() + "\twgsLng:" + wgsLon + "\ttemLat:" + tmp.Lat + "\ttmpLng:" + tmp.Lng + "\tdLat:" + dLat + "\tdlng:" + dLon);

           if (++i > 10000) break;
        }

        if(i>=10000){
            return new PointLatLng();
        }
        Earth.Lat = wgsLat;
        Earth.Lng = wgsLon;
        return Earth;
     }
  /*
     public Map<String, Double> transform(double lon, double lat)
     {
         HashMap<String, Double> localHashMap = new HashMap<String, Double>();
         if (outofChina(lat, lon))
         {
             localHashMap.put("lon", Double.valueOf(lon));
             localHashMap.put("lat", Double.valueOf(lat));
             return localHashMap;
         }
         double dLat = transformLat(lon - 105.0, lat - 35.0);
         double dLon = transformLon(lon - 105.0, lat - 35.0);
         double radLat = lat / 180.0 * pi;
         double magic = Math.sin(radLat);
         magic = 1 - ee * magic * magic;
         double sqrtMagic = Math.sqrt(magic);
         dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
         dLon = (dLon * 180.0) / (a / sqrtMagic * Math.cos(radLat) * pi);
         double mgLat = lat + dLat;
         double mgLon = lon + dLon;
         localHashMap.put("lon", mgLon);
         localHashMap.put("lat", mgLat);
         return localHashMap;
     }
      
     // gcj02-84
     public Map<String, Double> gcj2wgs(double lon, double lat)
     {
         Map<String, Double> localHashMap = new HashMap<String, Double>();
         double lontitude = lon
                 - (((Double)transform(lon, lat).get("lon")).doubleValue() - lon);
         double latitude = (lat - (((Double)(transform(lon, lat))
                 .get("lat")).doubleValue() - lat));
         localHashMap.put("lon", lontitude);
         localHashMap.put("lat", latitude);
         return localHashMap;
     }
         * */

     public PointLatLng GetEarth2Mars(PointLatLng p) {
         PointLatLng point = new PointLatLng();
         double resultLat = 0;
         double resultLon = 0;
         transform2Mars(p.Lat, p.Lng, out resultLat, out resultLon);
         point.Lat = resultLat;
         point.Lng = resultLon;

         return point;
     }

     public PointLatLng GetEarth2Mars1(PointLatLng p)
     {
         PointLatLng point = new PointLatLng();
         double resultLat = 0;
         double resultLon = 0;
         transform2Mars1(p.Lat, p.Lng, out resultLat, out resultLon);
         point.Lat = resultLat;
         point.Lng = resultLon;

         return point;
     }





     public PointLatLng GetEarth2bd(PointLatLng p)
     {
         return GetMars2bd(GetEarth2Mars(p));
     }

     public PointLatLng GetMars2bd(PointLatLng p)
     {
         PointLatLng point = new PointLatLng();
         double resultLat = 0;
         double resultLon = 0;
         bd_encrypt(p.Lat, p.Lng, ref resultLat, ref resultLon);
         point.Lat = resultLat;
         point.Lng = resultLon;

         return point;
     }

    }

}



