using MissionPlanner;
using System;
using System.Collections;
// hashs
using System.Collections.Generic;
using System.Windows.Forms;
// hashs
using System.Threading;
using System.IO.Ports;

using SpaceArrow;


namespace MAVLinkWP
{
    class MAVLink_wps
    {
        const int TARGET_SYSTEM_ID = 1;
        const int TARGET_SYS_COMPID=200;
        const int SYS_ID = 255;

        private byte packet_count = 0;

        public  SerialPort serial;

      

        public delegate void MessageShow(string message);
        public MessageShow ListMessage = null;

        Form1 form;

        public MAVLink_wps(SerialPort se,Form1 f)
        {
            this.serial = se;
            form = f;
        }
        public Hashtable getParamListBG() {
            //  giveComport = true;
            List<int> indexsreceived = new List<int>();

            // create new list so if canceled we use the old list
            MAVLink.MAVLinkParamList newparamlist = new MAVLink.MAVLinkParamList();
            MAVLink.mavlink_param_request_list_t req = new MAVLink.mavlink_param_request_list_t();
            req.target_system = TARGET_SYSTEM_ID;
            req.target_component = TARGET_SYS_COMPID;
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST, req);
            return null;
        }
        //PC--->>>plane  发送数据
        private void generatePacket(byte messageType, object indata)
        {
            byte[] data = MavlinkUtil.StructureToByteArray(indata);
            //Console.WriteLine(DateTime.Now + " PC Doing req "+ messageType + " " + this.BytesToRead);
            byte[] packet = new byte[data.Length + 6 + 2];
            packet[0] = 254;
            packet[1] = (byte)data.Length;
            packet[2] = (byte)packet_count;
            packet_count++;
            packet[3] = SYS_ID; // this is always 255 - MYGCS
            packet[4] = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
            packet[5] = messageType;
            int i = 6;
            foreach (byte b in data)
            {
                packet[i] = b;
                i++;
            }
            ushort checksum = MAVLink.MavlinkCRC.crc_calculate(packet, packet[1] + 6);
            checksum = MAVLink.MavlinkCRC.crc_accumulate(MAVLink.MAVLINK_MESSAGE_CRCS[messageType], checksum);
            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte
            packet[i] = ck_a;
            packet[i + 1] = ck_b;

            //if (this.serial.isComOpen)

            if(this.serial.IsOpen)
                this.serial.Write(packet, 0, packet.Length);
        }

        #region  发送航点相关

        public void sendWPS(List<PointLatLngAlt> Points)
        {
            MAVLink.MAV_FRAME frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
            Locationwp home = new Locationwp();
            try
            {
                home.id = (byte)MAVLink.MAV_CMD.WAYPOINT;
                home.lat = 23.1647061918178;
                home.lng = 113.455638885498;
                home.alt = 20; // use saved home
            }
            catch { throw new Exception("Your home location is invalid"); }

            ushort count = (ushort)(Points.Count+1);//含家地址

           UpLoadForm     upform = new UpLoadForm();
           upform.StartPosition = FormStartPosition.Manual;
           upform.Location = new System.Drawing.Point((this.form.Width - upform.Width) / 2, (this.form.Height - upform.Height) / 2);

         //   this.upform.Visible = true;

           // Application.DoEvents();
            
            upform.Show();

            

        //    Application.DoEvents();
           upform.SetMessage(0, count);
          // this.upform.ShowDialog();
        //  Thread.Sleep(100);

          setWPTotal(count);
          this.ListMessage("已发送航点数量...");

           upform.SetMessage(1, 0);

           bool mission_uploading = true;

           Locationwp[] wp_location = new Locationwp[count - 1];
            int i = 0;
            foreach (PointLatLngAlt point in Points)
            {
                wp_location[i].id = (byte)MAVLink.MAV_CMD.WAYPOINT;
                wp_location[i].lat = point.Lat;
                wp_location[i].lng = point.Lng;
                wp_location[i].alt = point.Alt;
                i++;
            }
            int seq = 0;

            bool isSendTotalSuccess = false;
            int cnt = 0;

            while (mission_uploading)
           {
               Program.mav_msg_handler.mission_request = false;
               Program.mav_msg_handler.mission_ack = false;
               Program.mav_msg_handler.SetTimeout(5);
               while (true)
               {
                   if (Program.mav_msg_handler.Wait())
                   {
                       if (isSendTotalSuccess)
                       {
                           ListMessage("发送航点超时出错");
                           mission_uploading = false;
                           break;
                       }
                       else {
                           cnt++;
                           if (cnt >= 3)
                           {
                               ListMessage("发送航点数量超时出错");
                               mission_uploading = false;
                               break;
                           }
                           else { 
                           
                           ListMessage("发送航点数量超时出错，准备重发...");
                           setWPTotal(count);
                           break;                           
                           }

                       }
                   }

                   if (upform.isCancel) {
                       this.ListMessage("您已经取消航点发送...");
                       upform.Close();
                       return;
                   }

                   if (Program.mav_msg_handler.mission_ack)
                   {
                     //  this.ListMessage("收到ACK...");
                       if (Program.mav_msg_handler.mavlink_mission_ack.type != 0)
                       {
                           ListMessage("发送航点出错");
                       }
                       else
                       {
                           ListMessage("航点上传完毕");
                       }
                       mission_uploading = false;
                       break;
                   }

                   if (Program.mav_msg_handler.mission_request == true)
                   {
                       isSendTotalSuccess = true;
                    //   this.ListMessage("收到REQUEST...");
                       seq = Program.mav_msg_handler.mavlink_mission_request.seq;
                       if (seq == 0)
                       {
                           setWP(home, 0, MAVLink.MAV_FRAME.GLOBAL, 0);
                           ListMessage("正在发送HOME...");
                       }
                       else
                       {
                           setWP(wp_location[seq-1], (ushort)(seq), frame, 0);
                           ListMessage("正在发送航点" + seq.ToString() + "...");
                           upform.SetMessage(2, seq);
                           upform.SetCount(seq, count);
                           
                       }
                       break;
                   }
                   Application.DoEvents();
               }
           }


            Thread.Sleep(500);
            upform.SetMessage(3,0);
            Thread.Sleep(500);
            setWPACK();
            upform.Close();
        }
        private void setWPACK()
        {
            MAVLink.mavlink_mission_ack_t req = new MAVLink.mavlink_mission_ack_t();
            req.target_system = TARGET_SYSTEM_ID;
            req.target_component = TARGET_SYS_COMPID;
            req.type = 0;
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.MISSION_ACK, req);
        }
        private MAVLink.MAV_MISSION_RESULT setWP(Locationwp loc, ushort index, MAVLink.MAV_FRAME frame, byte current = 0, byte autocontinue = 1)
        {
            MAVLink.mavlink_mission_item_t req = new MAVLink.mavlink_mission_item_t();
            req.target_system = TARGET_SYSTEM_ID;
            req.target_component = TARGET_SYS_COMPID; // MSG_NAMES.MISSION_ITEM
            req.command = loc.id;
            req.current = current;
            req.autocontinue = autocontinue;
            req.frame = (byte)frame;
            req.y = (float)(loc.lng);
            req.x = (float)(loc.lat);
            req.z = (float)(loc.alt);
            req.param1 = loc.p1;
            req.param2 = loc.p2;
            req.param3 = loc.p3;
            req.param4 = loc.p4;
            req.seq = index;
            return setWP(req);
        }

        private MAVLink.MAV_MISSION_RESULT setWP(MAVLink.mavlink_mission_item_t req)
        {
            ushort index = req.seq;


                DateTime start = DateTime.Now;      
                generatePacket((byte)MAVLink.MAVLINK_MSG_ID.MISSION_ITEM, req);

                return MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED;
       
        }
        private bool  setWPTotal(ushort wp_total)
        {
            MAVLink.MAVLinkParamList param = new MAVLink.MAVLinkParamList();
            MAVLink.mavlink_mission_count_t req = new MAVLink.mavlink_mission_count_t();
            req.target_system = TARGET_SYSTEM_ID;
            req.target_component = TARGET_SYS_COMPID;
            req.count = wp_total;//航点数量
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.MISSION_COUNT, req);
            return true;
        }

        #endregion
        #region 读取航点相关

        private ushort getWPCount() { //获取飞行器航点数量
            MAVLink.mavlink_mission_request_list_t req = new MAVLink.mavlink_mission_request_list_t();

            req.target_system = TARGET_SYSTEM_ID;
            req.target_component = TARGET_SYS_COMPID;

            bool success = false;

            for (int i = 0; i < 3; i++) { 
                Program.mav_msg_handler.mission_count_update = false;
                Program.mav_msg_handler.SetTimeout(3);
                generatePacket((byte)MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST, req);

                while (Program.mav_msg_handler.mission_count_update == false)
                {
                    if (Program.mav_msg_handler.Wait())
                    {
                        success = true;
                        this.ListMessage("Read WayPoint count timeout...."+(i+1).ToString());
                        break;
                    }

                    Application.DoEvents();
                }            
            }

            if (success)
            {
                this.ListMessage("读取航点数量失败...");
                return 0;
            }
            else {
                this.ListMessage("读取航点数量成功...");
            }



            ushort mis_count = (ushort)Program.mav_msg_handler.mission_count;

            return mis_count;
        }

        private Locationwp getWP(ushort index, ref bool succeed) {
            Locationwp loc = new Locationwp();
            MAVLink.mavlink_mission_request_t req = new MAVLink.mavlink_mission_request_t();
            req.target_system = TARGET_SYSTEM_ID;
            req.target_component = TARGET_SYS_COMPID;
            req.seq = index;
            MAVLink.mavlink_mission_item_t wp = new MAVLink.mavlink_mission_item_t ();
            bool found_ret = false;


            for (int i = 0; i < 3; i++) { 
                generatePacket((byte)MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST, req);

                Program.mav_msg_handler.SetTimeout(5);
                while (found_ret == false)
                {
                    if (Program.mav_msg_handler.Wait()) {
                        this.ListMessage("获取航点坐标" + index.ToString() + "超时失败[" + (i + 1).ToString() + "]");
                        break;
                    }

                    Application.DoEvents();

                    if(Program.mav_msg_handler.mission_items.ContainsKey(req.seq))
                    {
                        found_ret = true;
                        wp = Program.mav_msg_handler.mission_items[req.seq];
                        break;
                    }
                }


                if (found_ret == false)
                {
                    succeed = false;
                }
                else
                {
                    succeed = true;
                    loc.options = (byte)(wp.frame);
                    loc.id = (byte)(wp.command);
                    loc.p1 = (wp.param1);
                    loc.p2 = (wp.param2);
                    loc.p3 = (wp.param3);
                    loc.p4 = (wp.param4);

                    loc.alt = ((wp.z));
                    loc.lat = ((wp.x));
                    loc.lng = ((wp.y));

                    this.ListMessage("获取航点坐标" + index.ToString() + "成功...");
                    break;
                }                
            }



            return loc;
        }
        public List<PointLatLngAlt> drawPoints = new List<PointLatLngAlt>();  //点集
        public void getWPs() {
            bool succeed = false;
            List<Locationwp> cmds = new List<Locationwp>();



            UpLoadForm up = new UpLoadForm();

            up.SetMessage(4, 0);
            up.Show();
            int cmdcount = getWPCount();

            up.SetMessage(5, cmdcount);



            Program.mav_msg_handler.mission_items.Clear();
            ushort a = 0;
            for ( a = 0; a < cmdcount; a++)
            {
                up.SetCount1(a, cmdcount);
                Locationwp wp = getWP(a, ref succeed);
               
                if (succeed)
                {
                    cmds.Add(wp);
                  
                }
                else
                {
                    
                    break;
                }

                if (up.isCancel) {
                    ListMessage("已取消下载航点...");
                    up.Close();
                    return;
                }


                Application.DoEvents();
            }

            if (succeed)
            {
                up.SetMessage(7, 0);
                drawPoints.Clear();
                foreach (Locationwp loc in cmds)
                {
                    PointLatLngAlt PL = new PointLatLngAlt();
                    PL.Lat = loc.lat;
                    PL.Lng = loc.lng;
                    PL.Alt = loc.alt;
                    drawPoints.Add(PL);
                }
            }
            else
            {
                up.SetMessage(8, 0);
            }
            Thread.Sleep(500);
            up.Close();
        }
        #endregion


        public void GoToTargetPoint(PointLatLngAlt target)
        {
            Locationwp gotohere = new Locationwp();

            gotohere.id = (byte)MAVLink.MAV_CMD.WAYPOINT;
            gotohere.alt = target.Alt; // back to m
            gotohere.lat = target.Lat;
            gotohere.lng = target.Lng;
            MAVLink.MAV_MISSION_RESULT ans=this.setWP(gotohere, 0, MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, (byte)2);
                this.ListMessage("正在发送直达航点...");
        }

        public void SendLotiter(MAVLink.mavlink_command_long_t cmd) {
            cmd.command = (ushort)MAVLink.MAV_CMD.LOITER_TIME;
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendLotiter(float yaw, PointLatLngAlt point, int type,float seconds)
        {
            MAVLink.mavlink_command_long_t dd = new MAVLink.mavlink_command_long_t();

            dd.command = (ushort)MAVLink.MAV_CMD.LOITER_TIME;
            dd.param1 = seconds;
            dd.param2 = type;
            dd.param4 = yaw;
            dd.param5 = (float)point.Lat;
            dd.param6 = (float)point.Lng;
            dd.param7 = (float)point.Alt;
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG,dd);
        }

        public void SendMissionStart(int height, PointLatLngAlt point, int type, float seconds)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.MISSION_START;

             unsafe
            {
                *(int*)&cmd.param1 = height;
            }   
            cmd.param2 = 0;
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }
        public void SendTakeOff()
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.TAKEOFF;

            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendToLand() {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH;

            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendSignleMode(){
            
        }

        public void SendToMode(int mode)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.DO_SET_MODE;
            unsafe
            {
                *(int*)&cmd.param1 = mode;
             //   *(int*)&cmd.param2 = 
            }
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendHome()
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.DO_SET_HOME;
              unsafe
            {
                *(int*)&cmd.param1 = 1;
            }            
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendToCamera(int mode)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.DO_DIGICAM_CONTROL;
            unsafe
            {
                *(int*)&cmd.param1 = 0;  //REC ON/OFF
                *(int*)&cmd.param5 = mode;
            }
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendCamZoom(int mode)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.DO_DIGICAM_CONTROL;
            unsafe
            {
                *(int*)&cmd.param1 = 1;
                *(int*)&cmd.param5 = mode;  //Zoom in/out (0/1)
            }
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendGimbal(float pitch)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.DO_MOUNT_CONTROL;
            unsafe
            {
                *(int*)&cmd.param1 = 1; //pitch
                //cmd.param3 is useless here
            }
            cmd.param2 = pitch;
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendYaw(int left_or_right)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)MAVLink.MAV_CMD.DO_MOUNT_CONTROL;
            unsafe
            {
                *(int*)&cmd.param1 = 2; //yaw
                //cmd.param2 is useless here
                *(int*)&cmd.param3 = left_or_right;
            }
            
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendHeartBeat()
        {
            MAVLink.mavlink_heartbeat_t hb = new MAVLink.mavlink_heartbeat_t();
            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb);
        }

        public void SendCalCompass1()
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)40000;    //cal command
            unsafe {
            *(int*)&cmd.param1 = 1; //cal compass1
            }

            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public void SendCalCompass2()
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t();

            cmd.command = (ushort)40000;    //cal command
            unsafe {
            *(int*)&cmd.param1 = 2; //cal compass1
            }

            generatePacket((byte)MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }
    }
}
