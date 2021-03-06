﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyoSharp.Communication;
using MyoSharp.Device;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAzure.MobileServices;
using MyoTestv4.AdductionAbductionFlexion;
using MyoSharp.Poses;
using System.ComponentModel;
using System.Diagnostics;


// <author>Brian Varley</author>
// <summary>MyoDeviceModel</summary>
namespace MyoTestv4.AdductionAbductionFlexion
{
    public class MyoDeviceModel
    {

        public static readonly MyoDeviceModel Instance = new MyoDeviceModel();

        private IChannel channel;
        private IHub hub;

        public event Action<string> StatusUpdated;
        public event Action<string> PoseUpdated;
        public event Action<double> DegreesUpdated;
        public event Action<double> StartDegreeUpdated;
        public event Action<double> EndDegreeUpdated;
        public event Action<double> PainfulArcDegreeUpdated;
        public event Action<double> PitchUpdated;

        private const double PITCH_MAX = 1.46;

        private static double _pitchMin = 1.46;
        private double _calibrationFactor = 61.64;
        private double _startingDegree;
        private double _endDegree;
        private double _degreeOutputDouble;
        private double _degreeOutput;
        private double _painfulArcOutput;
        private double _userMinPitch;

        /// <summary>
        /// Prevents a default instance of the <see cref="MyoDeviceModel"/> class from being created.
        /// </summary>
        private MyoDeviceModel()
        {

        }

        #region Methods

        /// <summary>
        /// Myo Init Method.
        /// </summary>
        public void MyoDeviceStart()
        {
            // create a hub that will manage Myo devices for us
            this.channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create()));
            hub = Hub.Create(channel);
            {
                // listen for when the Myo connects
                this.hub.MyoConnected += (sender, e) =>
                {
                    var handler = StatusUpdated;
                    if (handler != null)
                    {
                        handler("Connected!");
                    }
                    e.Myo.Vibrate(VibrationType.Short);
                    // unlock the Myo so that it doesn't keep locking between poses
                    e.Myo.Unlock(UnlockType.Hold);
                    e.Myo.PoseChanged += Myo_PoseChanged;
                    e.Myo.OrientationDataAcquired += Myo_OrientationDataAcquired;
                };
                // listen for when the Myo disconnects
                hub.MyoDisconnected += (sender, e) =>
                {
                    var handler = StatusUpdated;
                    if (handler != null)
                    {
                        handler("Disconnected!");
                    }
                    e.Myo.Vibrate(VibrationType.Medium);
                    e.Myo.OrientationDataAcquired -= Myo_OrientationDataAcquired;
                    e.Myo.PoseChanged -= Myo_PoseChanged;
                };
                // start listening for Myo data
                channel.StartListening();
            }
        }

        /// <summary>
        /// Handles the PoseChanged event of the Myo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PoseEventArgs"/> instance containing the event data.</param>
        private void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            var handler = PoseUpdated;
            if (handler != null)
            {
                handler(e.Myo.Pose.ToString());
            }
            e.Myo.Vibrate(VibrationType.Short);
        }

        /// <summary>
        /// Handles the OrientationDataAcquired event of the Myo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="OrientationDataEventArgs" /> instance containing the event data.</param>
        private void Myo_OrientationDataAcquired(object sender, OrientationDataEventArgs e)
        {
            _currentPitch = e.Pitch;
            //myo indicator must be facing down or degrees will be inverted.
            _degreeOutputDouble = ((_currentPitch + _pitchMin) * _calibrationFactor);
            _degreeOutputDouble = Math.Round(_degreeOutputDouble, 2);
            _degreeOutput = _degreeOutputDouble;

            double calibCheck = _calibrationFactor;
            //Debug.WriteLine("Calibration value: " + calibCheck);
            //Debug.WriteLine("calibration hash code: " + this.GetHashCode());
            //Debug.WriteLine("degree output: " + _degreeOutputDouble);
            //Debug.WriteLine("degree hash: " + this.GetHashCode());

            //Debug.WriteLine("Current pitch: " + _currentPitch);
            var handler = DegreesUpdated;
            if (handler != null)
            {
                handler(_degreeOutput);
            }

            //painful arc logic
            if (e.Myo.Pose == Pose.Fist)
            {
                //provide haptic feedback, to indicate that 
                //painfull arc is being tracked.
                // e.Myo.Vibrate(VibrationType.Short);//buggy
                _endDegree = 0;
                if (_startingDegree == 0)
                {
                    _startingDegree = _degreeOutput;
                }
                var handlerArcStart = StartDegreeUpdated;
                if (handlerArcStart != null)
                {
                    handlerArcStart(_startingDegree);
                }
            }
            else
            {
                _startingDegree = 0;
                if (_endDegree == 0)
                {
                    _endDegree = _degreeOutput;
                }
                var handlerArcEnd = EndDegreeUpdated;
                if (handlerArcEnd != null)
                {
                    handlerArcEnd(_endDegree);
                    _painfulArcOutput = _endDegree - _startingDegree;
                    var handlerPainfulArc = PainfulArcDegreeUpdated;
                    if (handlerPainfulArc != null)
                    {
                        handlerPainfulArc(_painfulArcOutput);
                    }
                }
            }
        }

        //method to calibrate minimum pitch reading
        /// <summary>
        /// Callibrates the pitch minimum reading.
        /// </summary>
        public void CallibratePitchMinimumReading()
        {

            _userMinPitch = _currentPitch;
            Math.Round(_userMinPitch, 2);

            var handlerPitch = PitchUpdated;
            if (handlerPitch != null)
            {
                handlerPitch(_userMinPitch);
            }           
           
            /*
            _pitchMin = _currentPitch;
            _calibrationFactor = 180 / (Math.Abs(_pitchMin) + PITCH_MAX);
            Math.Round(_calibrationFactor, 2);
            _degreeOutputDouble = ((_currentPitch + Math.Abs(_pitchMin)) * _calibrationFactor);
             * */
        }



        /// <summary>
        /// The _current pitch
        /// </summary>
        private double _currentPitch = 0;
        /// Gets or sets the current pitch.
        /// </summary>
        /// <value>
        /// The current pitch.
        /// </value>
        public double CurrentPitch
        {
            get { return this._currentPitch; }
            set
            {
                this._currentPitch = value;

            }
        }

    }

}

        #endregion


