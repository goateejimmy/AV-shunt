//===========================================================================//
//                                                                           //
//  Copyright (C) 2006 - 2017                                                //
//  IDS Imaging GmbH                                                         //
//  Dimbacherstr. 6-8                                                        //
//  D-74182 Obersulm-Willsbach                                               //
//                                                                           //
//  The information in this document is subject to change without            //
//  notice and should not be construed as a commitment by IDS Imaging GmbH.  //
//  IDS Imaging GmbH does not assume any responsibility for any errors       //
//  that may appear in this document.                                        //
//                                                                           //
//  This document, or source code, is provided solely as an example          //
//  of how to utilize IDS software libraries in a sample application.        //
//  IDS Imaging GmbH does not assume any responsibility for the use or       //
//  reliability of any portion of this document or the described software.   //
//                                                                           //
//  General permission to copy or modify, but not for profit, is hereby      //
//  granted,  provided that the above copyright notice is included and       //
//  reference made to the fact that reproduction privileges were granted     //
//  by IDS Imaging GmbH.                                                     //
//                                                                           //
//  IDS cannot assume any responsibility for the use or misuse of any        //
//  portion of this software for other than its intended diagnostic purpose  //
//  in calibrating and testing IDS manufactured cameras and software.        //
//                                                                           //
//===========================================================================//

/// Developer Note: I tried to keep it as simple as possible.
/// Therefore there are no functions asking for the newest driver software or freeing memory beforehand, etc.
/// This program is solely meant to show one of the simplest ways to interact with an IDS Camera through the uEye API.
/// (XS Cameras are not supported)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Sandkasten_CSharp
{
    public partial class SimpleSingleGrab_C_Sharp : Form
    {

        public delegate void scroll(object sender, EventArgs e);

        //serialport preparetion
        SerialPort Port = new SerialPort();
        string[] PortName;
        Byte[] buffer = new Byte[5];


        //To use the .NET interface you have to create a uEye .NET object
        private uEye.Camera Camera = new uEye.Camera();
        IntPtr DisplayHandle = IntPtr.Zero;

        public SimpleSingleGrab_C_Sharp()
        {
            InitializeComponent();
            DisplayHandle = DisplayWindow.Handle;

            //Initialize the camera
            Camera.Init();

            //Allocate an default image memory
            Camera.Memory.Allocate();

            Camera.EventFrame += onFrameEvent;

            //Capture a single image with
            Camera.Acquisition.Freeze();
        }
        


        private void onFrameEvent(object sender, EventArgs e)
        {
            uEye.Camera Camera = sender as uEye.Camera;

            //Display the image on the screen
            Camera.Display.Render( DisplayHandle, uEye.Defines.DisplayRenderMode.FitToWindow);
        }



        //The following are key definitions
        private void Quit_Click(object sender, EventArgs e)
        {
            //Free the allocated image memory and closes the program
            Camera.Exit();
            Close();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            //Capture a single frame
            Camera.Acquisition.Freeze();
        }

        private void Live_Click(object sender, EventArgs e)
        {
            //Capture a live image
            Camera.Acquisition.Capture();

            //Currently we are working without Ring-Buffer
            //For the purpose of showing this example that's enough
            //But if you want to do some Image data processing you'll need to adjust the settings
        }

        private void DisplayWindow_Click(object sender, EventArgs e)
        {

        }

        private void SimpleSingleGrab_C_Sharp_Load(object sender, EventArgs e)
        {
            PortName = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(PortName);
          

        }

        private void connect_Click(object sender, EventArgs e)
        {
            if (PortName.Length > 0)
            {
                Port.PortName = comboBox1.Text;
                Port.BaudRate = 57600;
                Port.Parity = Parity.None;
                Port.StopBits = StopBits.One;
                //Port.Encoding = Encoding.GetEncoding("utf-32");

                Port.Open();
            }
        }
        public void SetCurrent(int value)
        {


            buffer[0] = 0x02;
            buffer[1] = 0x37;

            buffer[2] = 0x01;

            buffer[3] = 0x01;

            string hexvalue = Convert.ToString(value, 16);
            //   buffer[5] = Convert.ToByte( "0x" + Convert.ToString(value, 16));
            buffer[4] = Convert.ToByte(hexvalue, 16);



            Port.Write(AddCRC(buffer), 0, AddCRC(buffer).Length);
            
        }

        private byte[] AddCRC(byte[] command)//CRC
        {
            int CRC = 0;

            byte[] commandWithCRC = new byte[command.Length + 1];
            for (int i = 0; i < command.Length; i++)
                CRC = CRC + command[i];

            Array.Copy(command, 0, commandWithCRC, 0, command.Length);
            commandWithCRC[commandWithCRC.Length - 1] = (byte)CRC;
            //richTextBox1.Text = String.Format("{0:X}", CRC.ToString());

            return commandWithCRC;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int light_value = trackBar1.Value;

            //  MessageBox.Show(XORstring("$12064"));
            numericUpDown1.Value = light_value;

            SetCurrent(light_value);
            textBox1.Text = "sendind....";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int value =(int)numericUpDown1.Value;
            trackBar1.Value = value;
            //scroll Scroll = new scroll(trackBar1_Scroll);
            trackBar1_Scroll(sender, e);
            //Scroll(sender,e);
        }
    }
}