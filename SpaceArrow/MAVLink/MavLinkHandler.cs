using System.Collections.Generic;

namespace MAVLinkWP
{
    class MavLinkHandler
    {
        public int heart_beat_count;
        public bool mission_count_update;
        public int mission_count;
        public Dictionary<ushort, MAVLink.mavlink_mission_item_t> mission_items;

        public MAVLink.mavlink_mission_request_t mavlink_mission_request;
        public bool mission_request;

        public MAVLink.mavlink_mission_ack_t mavlink_mission_ack;
        public bool mission_ack;

        public MAVLink.mavlink_attitude_t mavlink_mission_attitude;
        public bool mission_attitude = false;

    
        public MAVLink.mavlink_global_position_int_t mavlink_mission_global_position_int;
        public bool mission_global_position_int = false;


        public MAVLink.mavlink_param_value_t mavlink_mission_param_value_t;
        public bool mission_param_value_t = false;


        public MAVLink.mavlink_sys_status_t mavlink_mission_sys_status_t;
        public bool mission_sys_status_t;


       //public MAVLink.mavlink_command_ack_t mavlink_mission_command_ack_t;
       //public bool mission_command_ack_t=false;

        private int time_out;
        private System.DateTime start;

        public MavLinkHandler()
        {
            time_out = 0;
            heart_beat_count = 0;
            mission_count_update = false;
            mission_count = 0;
            mission_items = new Dictionary<ushort,MAVLink.mavlink_mission_item_t>();
           // mavlink_mission_request = new MAVLink.mavlink_mission_request_t();
            mission_request = false;
           // mavlink_mission_ack = new MAVLink.mavlink_mission_request_t();
            mission_ack = false;
        }

        public void SetTimeout(int seconds)
        {
            time_out = seconds;
            start = System.DateTime.Now;
        }

        public bool Wait()  //return true if time is up
        {
            return (System.DateTime.Now - start).TotalSeconds > time_out;
        }
    }

}
