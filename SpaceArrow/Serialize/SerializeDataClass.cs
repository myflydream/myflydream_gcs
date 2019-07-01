using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceArrow
{
    public enum ActionType
    {
        ACTION_SET_HOME=0,

    }


    public class SerializeDataClass
    {
        public List<posture> listposture = new List<posture>();        //姿态列表
        public List<sysstatus> listsysstatus = new List<sysstatus>();  //系统消息列表
        public List<position> listposition = new List<position>();     //位置列表
        public List<connect> listconnect = new List<connect>();        //连接列表

        public SerializeDataClass() { }

        public void Clear()
        {
            listposture.Clear();
            listsysstatus.Clear();
            listposition.Clear();
            listconnect.Clear();
        }
         
    }

    public class customeraction {
        public ActionType actiontype;
        public DateTime time;
        public customeraction() { }

        public customeraction(ActionType type) {
            actiontype = type;
            time = DateTime.Now;
        }
    }


    public class posture {
        public DateTime time;
        public MAVLink.mavlink_attitude_t mission_attitude;
        public posture() { }

        public  posture(MAVLink.mavlink_attitude_t attitude)
        {
            mission_attitude = attitude;
            time = DateTime.Now;
        }

        public void SetAttitude(MAVLink.mavlink_attitude_t attitude) {
            mission_attitude = attitude;
            time = DateTime.Now;
        }
    }

    public class sysstatus
    {
        public DateTime time;
        public MAVLink.mavlink_sys_status_t mission_sysstatus;
        public sysstatus() { }

        public sysstatus(MAVLink.mavlink_sys_status_t sysstatus)
        {
            mission_sysstatus = sysstatus;
            time = DateTime.Now;
        }

        public void SetAttitude(MAVLink.mavlink_sys_status_t sysstatus)
        {
            mission_sysstatus = sysstatus;
            time = DateTime.Now;
        }
    }

    public class position
    {
        public DateTime time;
        public MAVLink.mavlink_global_position_int_t mission_position;
        public position() { }

        public position(MAVLink.mavlink_global_position_int_t globalposition)
        {
            mission_position.alt = globalposition.alt;
            mission_position.hdg = globalposition.hdg;
            mission_position.lat = globalposition.lat;
            mission_position.lon = globalposition.lon;

            mission_position.relative_alt = globalposition.relative_alt;
            mission_position.time_boot_ms = globalposition.time_boot_ms;
            mission_position.vx = globalposition.vx;
            mission_position.vy = globalposition.vy;
            mission_position.vz = globalposition.vz;

            time = DateTime.Now;
        }

        public void SetAttitude(MAVLink.mavlink_global_position_int_t globalposition)
        {
            mission_position = globalposition;
            time = DateTime.Now;
        }
    }

    public class connect {
        public DateTime time;
        public bool conn;

        public connect() { }

        public connect(bool conn1)
        {
            conn = conn1;
            time = DateTime.Now;
        }

        public void SetAttitude(bool conn1)
        {
            conn = conn1;
            time = DateTime.Now;
        }
    }


}
