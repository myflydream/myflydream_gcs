using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SpaceArrow
{
    public class PointLatLngAlt :IDisposable
    {

        public float Alt=30;//高度
        public double Lng=0;//经度
        public double Lat=0;//纬度

        double threshold = 0.000002;

        public DateTime date;

        public PointLatLngAlt() {
           
        }
        public override bool Equals(object obj){
            bool b = true;
            PointLatLngAlt p=(PointLatLngAlt)obj;

            if (System.Math.Abs(this.Alt - p.Alt) > threshold)
            {
                b = false;
            }
            if (System.Math.Abs(this.Lat - p.Lat) > threshold)
            {
                b = false;
            }
            if (System.Math.Abs(this.Lng - p.Lng) > threshold)
            {
                b = false;
            }
            return b;
            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PointLatLngAlt(PointLatLngAlt p) {
            this.Alt = p.Alt;
            this.Lng = p.Lng;
            this.Lat = p.Lat;
        }
        public PointLatLngAlt(PointLatLng p)
        {
            this.Lng = p.Lng;
            this.Lat = p.Lat;
        }
        public PointLatLngAlt(double lat,double lng)
        {
            this.Lng = lng;
            this.Lat = lat;

            date = DateTime.Now;
        }
        public PointLatLngAlt(double lat, double lng,float alt)
        {
            this.Lng = lng;
            this.Lat = lat;
            this.Alt = alt;
            date = DateTime.Now;

        }

        /// <summary>
        /// 比较当前点是否和对应点在同一个纬度上
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CompareLatEquals(PointLatLngAlt p) {
            if (System.Math.Abs(this.Lat - p.Lat) > threshold)
            {
                return  false;
            }
            return true;
        }
        /// <summary>
        /// 比较当前点是否和对应点在同一个经度上
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CompareLngEquals(PointLatLngAlt p)
        {
            if (System.Math.Abs(this.Lng - p.Lng) > threshold)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 比较当前点纬度是否比对应点的纬度值高
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CompareLatGreater(PointLatLngAlt p) {
            if (this.Lat - p.Lat > threshold)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 比较当前点经度是否比对应点的经度值高
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CompareLngGreater(PointLatLngAlt p)
        {
            if (this.Lng - p.Lng> threshold)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 比较当前点纬度是否比对应点纬度值低
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CompareLatLess(PointLatLngAlt p)
        {
            if (p.Lat -this.Lat  > threshold)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 比较当前点经度是否比对应点经度低
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CompareLngLess(PointLatLngAlt p)
        {
            if (p.Lng - this.Lng > threshold)
            {
                return true;
            }
            return false;
        } 

        public override string ToString() {
            return this.Lng.ToString() + " " + this.Lat.ToString() + " " + this.Alt.ToString();
        }

        public void Dispose()
        {
         //   throw new NotImplementedException();
        }             
    
    
    }
}
