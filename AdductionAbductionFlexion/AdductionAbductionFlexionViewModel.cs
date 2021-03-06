﻿using System.Windows.Input;
using MyoSharp.Communication;
using MyoSharp.Device;
using System;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyoTestv4.AdductionAbductionFlexion;
using MyoTestv4.Home;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;



// <author>Brian Varley</author>
// <summary>AdductionAbductionViewModel</summary>

namespace MyoTestv4
{
    public class AdductionAbductionFlexionViewModel : ObservableObject, IPageViewModel, INotifyPropertyChanged
    {
        private MyoDeviceModel _myoDevice;
        private DatabaseModel _dataObj;
        public event Action<string> DataChanged;
        private string _itemString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdductionAbductionFlexionViewModel"/> class.
        /// </summary>
        /// <param name="Myodevice">The device.</param>
        /// <param name="progressData">The progress data.</param>
        public AdductionAbductionFlexionViewModel(MyoDeviceModel Myodevice, DatabaseModel progressData)
        {
            DataSubmitCommand = new RelayCommand (this.SaveChangesToPersistence);
            CalibrationSetCommand = new RelayCommand(this.CallibratePitchMinimumCall);
            DatabaseSearchCommand = new RelayCommand(this.QueryDataFromPersistence);

            _myoDevice = Myodevice;
            _myoDevice.MyoDeviceStart();
            _dataObj = progressData;

            _myoDevice.StatusUpdated += (update) =>
            {
                CurrentStatus = update;   
            };

            _myoDevice.PoseUpdated += (update) =>
            {
                PoseStatus = update;   
            };

            _myoDevice.PainfulArcDegreeUpdated += (update) =>
            {
                PainfulArcStatus = update;
            };

            _myoDevice.DegreesUpdated += (update) =>
            {
                DegreeStatus = update;
            };

            _myoDevice.StartDegreeUpdated += (update) =>
            {
                StartDegreeStatus = update;    
            };

            _myoDevice.EndDegreeUpdated += (update) =>
            {
                EndDegreeStatus = update;    
            };  
  
            _myoDevice.PitchUpdated += (update) =>
            {
                PitchStatus = update;    
            };  


        }

        /// <summary>
        /// The commit status
        /// </summary>
        private string commitStatus;
        /// <summary>
        /// Gets or sets the commit status.
        /// </summary>
        /// <value>
        /// The commit status.
        /// </value>
        public string CommitStatus
        {
            get { return this.commitStatus; }
            set
            {
                if (this.commitStatus != value)
                {
                    this.commitStatus = value;
                    this.RaisePropertyChanged("CommitStatus");
                }
            }
        }

        /// <summary>
        /// The current status
        /// </summary>
        private string currentStatus;
        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public string CurrentStatus
        {
            get { return this.currentStatus; }
            set
            {
                if (this.currentStatus != value)
                {
                    this.currentStatus = value;
                    this.RaisePropertyChanged("CurrentStatus");
                }
            }
        }

        /// <summary>
        /// The painful arc status
        /// </summary>
        private double painfulArcStatus;
        /// <summary>
        /// Gets or sets the painful arc status.
        /// </summary>
        /// <value>
        /// The painful arc status.
        /// </value>
        public double PainfulArcStatus
        {
            get { return this.painfulArcStatus; }
            set
            {
                if (this.painfulArcStatus != value)
                {
                    this.painfulArcStatus = value;
                    this.RaisePropertyChanged("PainfulArcStatus");
                }
            }
        }

        /// <summary>
        /// The pose status
        /// </summary>
        private string poseStatus;
        /// <summary>
        /// Gets or sets the pose status.
        /// </summary>
        /// <value>
        /// The pose status.
        /// </value>
        public string PoseStatus
        {
            get { return this.poseStatus; }
            set
            {
                if (this.poseStatus != value)
                {
                    this.poseStatus = value;
                    this.RaisePropertyChanged("PoseStatus");
                }
            }
        }

        /// <summary>
        /// The degree status
        /// </summary>
        private double degreeStatus;
        /// <summary>
        /// Gets or sets the degree status.
        /// </summary>
        /// <value>
        /// The degree status.
        /// </value>
        public double DegreeStatus
        {
            get { return this.degreeStatus; }
            set
            {
                if (this.degreeStatus != value)
                {
                    this.degreeStatus = value;
                    this.RaisePropertyChanged("DegreeStatus");
                }
            }
        }


        private double pitchStatus;
        /// <summary>
        /// Gets or sets the degree status.
        /// </summary>
        /// <value>
        /// The degree status.
        /// </value>
        public double PitchStatus
        {
            get { return this.pitchStatus; }
            set
            {
                if (this.pitchStatus != value)
                {
                    this.pitchStatus = value;
                    this.RaisePropertyChanged("PitchStatus");
                }
            }
        }

        /// <summary>
        /// The end degree status
        /// </summary>
        private double endDegreeStatus;
        /// <summary>
        /// Gets or sets the end degree status.
        /// </summary>
        /// <value>
        /// The end degree status.
        /// </value>
        public double EndDegreeStatus
        {
            get { return this.endDegreeStatus; }
            set
            {
                if (this.endDegreeStatus != value)
                {
                    this.endDegreeStatus = value;
                    this.RaisePropertyChanged("EndDegreeStatus");
                }
            }
        }

        /// <summary>
        /// The start degree status
        /// </summary>
        private double startDegreeStatus;
        /// <summary>
        /// Gets or sets the start degree status.
        /// </summary>
        /// <value>
        /// The start degree status.
        /// </value>
        public double StartDegreeStatus
        {
            get { return this.startDegreeStatus; }
            set
            {
                if (this.startDegreeStatus != value)
                {
                    this.startDegreeStatus = value;
                    this.RaisePropertyChanged("StartDegreeStatus");
                }
            }
        }

        /// <summary>
        /// Gets the data submit command.
        /// </summary>
        /// <value>
        /// The data submit command.
        /// </value>
        public ICommand DataSubmitCommand { get; private set; }

        /// <summary>
        /// Gets the calibration set command.
        /// </summary>
        /// <value>
        /// The calibration set command.
        /// </value>
        public ICommand CalibrationSetCommand { get; private set; }

        /// <summary>
        /// Gets the database search command.
        /// </summary>
        /// <value>
        /// The database search command.
        /// </value>
        public ICommand DatabaseSearchCommand { get; private set; }


        /// <summary>
        /// Saves the changes to persistence.
        /// </summary>
        /// <param name="param">The parameter.</param>
        public void SaveChangesToPersistence(object param)
        {
            DatabaseModel.SubmitChangesAsync(StartDegreeStatus, EndDegreeStatus, HomeViewModel.LoginObject.UserName, HomeViewModel.LoginObject.Gender, DegreeStatus);
            this.CommitStatus = "Data committed successfully!";
            DataChanged(CommitStatus);
        }

        /// <summary>
        /// Callibrates the pitch minimum call.
        /// </summary>
        /// <param name="param">The parameter.</param>
        public void CallibratePitchMinimumCall(object param)
        {
            _myoDevice.CallibratePitchMinimumReading();
                    RaisePropertyChanged("PitchStatus");

        }

        /// <summary>
        /// Queries the data from persistence.
        /// </summary>
        /// <param name="param">The parameter.</param>
        public async void QueryDataFromPersistence(object param)
        {
            List<Item> itemList = await DatabaseModel.QueryTable();
            ExportToCSV(itemList);         
        }

        /// <summary>
        /// Exports to CSV.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        private void ExportToCSV(List<Item> itemList)
        {
            //do something with the queried data here..csv
            _itemString = String.Join(",", itemList.Select(i => String.Format("{0},{1},{2},{3},{4},,{5}" + Environment.NewLine, i.Date, i.User, i.Exercise, i.Painful_Arc_Start, i.Painful_Arc_End, i.Max_Range)));
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PatientRecords.txt");

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(folder))
            {
                try
                {
                    file.WriteLine(_itemString);
                }
                catch (Exception ex)
                {
                    //log export error
                    ex.ToString();
                }
            }
        }
       
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return "Adduction Abduction Flexion";
            }
        }
    }
}