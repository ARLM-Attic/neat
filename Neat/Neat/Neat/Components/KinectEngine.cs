#if KINECT
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using System.IO;
using System.Diagnostics;
using Neat;
using Neat.Components;
using Neat.Components.KinectForWindows;
using Microsoft.Xna.Framework.Graphics;
using System;
using Console = Neat.Components.Console;
#if WINDOWS
using System.Windows.Forms;
using Neat.Mathematics;
using Neat.Graphics;
#endif
public class KinectEngine : GameComponent
{
    /// <summary>
    /// The status to string mapping.
    /// </summary>
    private readonly Dictionary<KinectStatus, string> statusMap = new Dictionary<KinectStatus, string>();

    /// <summary>
    /// The requested color image format.
    /// </summary>
    private readonly ColorImageFormat colorImageFormat;

    /// <summary>
    /// The requested depth image format.
    /// </summary>
    private readonly DepthImageFormat depthImageFormat;

    /// <summary>
        /// Initializes a new instance of the KinectChooser class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="colorFormat">The desired color image format.</param>
        /// <param name="depthFormat">The desired depth image format.</param>
    public KinectEngine(Game game, ColorImageFormat colorFormat, DepthImageFormat depthFormat)
            : base(game)
        {
            this.colorImageFormat = colorFormat;
            this.depthImageFormat = depthFormat;

            Debug.WriteLine("KinectEngine object created.", "Kinect");

            KinectSensor.KinectSensors.StatusChanged += this.KinectSensors_StatusChanged;
            this.DiscoverSensor();

            this.statusMap.Add(KinectStatus.Connected, "Connected");
            this.statusMap.Add(KinectStatus.DeviceNotGenuine, "Device Not Genuine");
            this.statusMap.Add(KinectStatus.DeviceNotSupported, "Device Not Supported");
            this.statusMap.Add(KinectStatus.Disconnected, "Required");
            this.statusMap.Add(KinectStatus.Error, "Error");
            this.statusMap.Add(KinectStatus.Initializing, "Initializing...");
            this.statusMap.Add(KinectStatus.InsufficientBandwidth, "Insufficient Bandwidth");
            this.statusMap.Add(KinectStatus.NotPowered, "Not Powered");
            this.statusMap.Add(KinectStatus.NotReady, "Not Ready");
    }

    /// <summary>
    /// Gets the selected KinectSensor.
    /// </summary>
    public KinectSensor Sensor { get; private set; }
    public bool Draw = false;

    /// <summary>
    /// Gets the last known status of the KinectSensor.
    /// </summary>
    public KinectStatus LastStatus { get; private set; }

    public List<int> TrackedSkeletonsIndices = new List<int>();
    /// <summary>
    /// This method initializes necessary objects.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Console.AddCommand("k_tilt", o => {
            if (o.Count == 1) Console.WriteLine(Sensor.ElevationAngle.ToString());
            else Sensor.ElevationAngle = int.Parse(o[1]);
        });
        Console.AddCommand("k_init", o =>
        {
            if (o.Count == 1) DiscoverSensor();
            else DiscoverSensor(bool.Parse(o[1]), bool.Parse(o[2]));
        });
        Console.AddCommand("k_uninit", o => Uninitialize());
        Console.AddCommand("k_video", o =>
        {
            if (o[1].ToLower() == "open") OpenColorStream();
            else if (o[1].ToLower() == "close") ColorStream = null;
        });
        Console.AddCommand("k_depth", o =>
        {
            if (o[1].ToLower() == "open") OpenDepthStream();
            else if (o[1].ToLower() == "close") DepthStream = null;
        });
        Console.AddCommand("k_status", o => Console.WriteLine(GetStatus()));
        Console.AddCommand("k_draw", o => Draw = bool.Parse(o[1]));
        Console.AddCommand("k_seated", o => SeatedMode = bool.Parse(o[1]));
    }

    public bool SeatedMode
    {
        get
        {
            return Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated;
        }
        set
        {
            if (IsSensorReady)
                Sensor.SkeletonStream.TrackingMode = value ? SkeletonTrackingMode.Seated : SkeletonTrackingMode.Default;
            else
            {
                Console.Run("g_fullscreen false$_g_reinit");
#if WINDOWS //of course it's windows, we don't need this!
                MessageBox.Show("Kinect initialization failed.\n" + GetStatus());
#endif
            }
        }
    }
    public void Uninitialize()
    {
        Debug.WriteLine("KinectEngine.Uninitialize()", "Kinect");
        if (this.Sensor != null)
        {
            Debug.WriteLine("Sensor active, stopping it.", "Kinect");
            this.Sensor.Stop();
        }
    }

    public string GetStatus()
    {
        return statusMap[LastStatus];
    }

    public bool IsSensorReady
    {
        get { return this.Sensor != null && this.LastStatus == KinectStatus.Connected; }
    }

    public bool EnabledWithColor { get; private set; }
    public bool EnabledWithDepth { get; private set; }
    public bool NormalStartup { get; private set; }
    /// <summary>
    /// This method will use basic logic to try to grab a sensor.
    /// Once a sensor is found, it will start the sensor with the
    /// requested options.
    /// </summary>
    private void DiscoverSensor(bool enableColor=true, bool enableDepth=false)
    {
        NormalStartup = false;

        Debug.WriteLine("");
        Debug.WriteLine("KinectEngine.DiscoverSensor(enableColor="+enableColor+", enableDepth="+enableDepth+") Started.", "Kinect");

        // Grab any available sensor
        this.Sensor = KinectSensor.KinectSensors.FirstOrDefault();

        if (this.Sensor != null)
        {
            Debug.WriteLine("Sensor Found.", "Kinect");
            this.LastStatus = this.Sensor.Status;

            // If this sensor is connected, then enable it
            if (this.LastStatus == KinectStatus.Connected)
            {
                Debug.WriteLine("Sensor is connected. Enabling streams.", "Kinect");
                this.Sensor.SkeletonStream.Enable();
                if (enableColor)
                    this.Sensor.ColorStream.Enable(this.colorImageFormat);
                if (enableDepth)
                    this.Sensor.DepthStream.Enable(this.depthImageFormat);

                try
                {
                    this.Sensor.Start();

                    EnabledWithColor = enableColor;
                    EnabledWithDepth = enableDepth;
                    NormalStartup = true;
                }
                catch (IOException e)
                {
                    Debug.WriteLine("IOException caught.", "Kinect");
                    Debug.WriteLine(e, "Kinect");

                    // sensor is in use by another application
                    // will treat as disconnected for display purposes
                    this.Sensor = null;
                }
            }
        }
        else
        {
            Debug.WriteLine("Sensor not found.", "Kinect");
            this.LastStatus = KinectStatus.Disconnected;
        }
    }

    /// <summary>
    /// This wires up the status changed event to monitor for 
    /// Kinect state changes.  It automatically stops the sensor
    /// if the device is no longer available.
    /// </summary>
    /// <param name="sender">The sending object.</param>
    /// <param name="e">The event args.</param>
    private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
    {
        // If the status is not connected, try to stop it
        (Game as Neat.NeatGame).Console.WriteLine("Kinect status changed to: " + statusMap[e.Status]);
        if (e.Status != KinectStatus.Connected)
        {
            e.Sensor.Stop();
            (Game as Neat.NeatGame).Console.WriteLine("Kinect stopped.");
        }

        this.LastStatus = e.Status;
        this.DiscoverSensor();
    }

    protected Console Console { get { return (Game as NeatGame).Console; } }
    protected uint GameFrame { get { return (Game as NeatGame).Frame; } }
    protected uint lastSkeletonFrame = 0;
    public uint LastSkeletonFrame { get { return lastSkeletonFrame; } set { lastSkeletonFrame = value; } }

    public Skeleton[] Skeletons;
    public TimeSpan TrackTime;
    
    public void UpdateSkeletons(GameTime gameTime)
    {
        if (!IsSensorReady || GameFrame == LastSkeletonFrame) return;
        using (var skeletonFrame = Sensor.SkeletonStream.OpenNextFrame(0))
        {
            // Sometimes we get a null frame back if no data is ready
            if (null == skeletonFrame)
            {
                return;
            }

            // Reallocate if necessary
            if (null == Skeletons || Skeletons.Length != skeletonFrame.SkeletonArrayLength)
            {
                Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                TrackTime = gameTime.TotalGameTime;
            }

            skeletonFrame.CopySkeletonDataTo(Skeletons);
            LastSkeletonFrame = GameFrame;

            TrackedSkeletonsIndices.Clear();
            for (int i = 0; i < Skeletons.Length; i++)
            {
                if (Skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    TrackedSkeletonsIndices.Add(i);
            }
        }
    }

    public ColorStreamRenderer ColorStream;
    public DepthStreamRenderer DepthStream;

    public void OpenColorStream()
    {
        ColorStream = new ColorStreamRenderer(Game as NeatGame);
    }

    public void OpenDepthStream()
    {
        DepthStream = new DepthStreamRenderer(Game as NeatGame);
    }

    public override void Update(GameTime gameTime)
    {
        UpdateSkeletons(gameTime);
        if (ColorStream != null) ColorStream.Update(gameTime);
        if (DepthStream != null) DepthStream.Update(gameTime);
        base.Update(gameTime);
    }

    public void Render(GameTime gameTime)
    {
        if (ColorStream != null) ColorStream.Draw(gameTime);
        if (DepthStream != null) DepthStream.Draw(gameTime);
    }

    public Vector3 ToVector3(JointType joint, int skeletonId = 0)
    {
        if (TrackedSkeletonsIndices.Count == 0 || skeletonId >= TrackedSkeletonsIndices.Count) return Vector3.Zero;
        var p = Skeletons[TrackedSkeletonsIndices[skeletonId]].Joints[joint].Position;
        return new Vector3(p.X, p.Y, p.Z);
    }

    public Vector2 ToVector2(JointType joint, int skeletonId = 0)
    {
        var p = ToVector3(joint, skeletonId);
        return new Vector2(p.X, p.Y);
    }

    public Vector2 ToVector2(JointType joint, Vector2 scale, int skeletonId = 0)
    {
        var half = scale * 0.5f;
        return (ToVector2(joint, skeletonId) * half * new Vector2(1, -1)) + half;
    }

    public Vector3 ToVector3(JointType joint, Vector3 scale, int skeletonId = 0)
    {
        var half = scale * 0.5f;
        return (ToVector3(joint, skeletonId) * half * new Vector3(1, -1, 1)) + half;
    }

    public int TrackedSkeletonsCount { get { return TrackedSkeletonsIndices.Count; } }

    public int GetInferredJointsCount(int skeletonId = 0)
    {
        if (!this.IsSensorReady) return int.MaxValue;
        return Skeletons[skeletonId].Joints.Where(o => o.TrackingState != JointTrackingState.Tracked).ToList().Count;
    }

    public bool TryGetJoint(JointType JointType, out Joint joint, int skeletonId = 0)
    {
        if (TrackedSkeletonsIndices.Count == 0 || skeletonId >= TrackedSkeletonsIndices.Count)
        {
            joint = new Joint();
            return false;
        }
        joint = Skeletons[TrackedSkeletonsIndices[skeletonId]].Joints[JointType];
        return true;

    }

    public void DrawSkeleton(SpriteBatch spriteBatch, LineBrush lb, Vector2 position, Vector2 size, Color color, int skeletonId = 0)
    {
        if (Skeletons.Length <= skeletonId || Skeletons[skeletonId] == null)
        {
            //Skeleton not found. Draw an X
            lb.Draw(spriteBatch, position, position + size, color);
            lb.Draw(spriteBatch, new LineSegment(position.X + size.X, position.Y, position.X, position.Y + size.Y), color);
            return;
        }

        //Right Hand
        lb.Draw(spriteBatch, ToVector2(JointType.HandRight, size, skeletonId),
            ToVector2(JointType.WristRight, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.WristRight, size, skeletonId),
            ToVector2(JointType.ElbowRight, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.ElbowRight, size, skeletonId),
            ToVector2(JointType.ShoulderRight, size, skeletonId), color, position);

        //Head & Shoulders
        lb.Draw(spriteBatch, ToVector2(JointType.ShoulderRight, size, skeletonId),
            ToVector2(JointType.ShoulderCenter, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.Head, size, skeletonId),
            ToVector2(JointType.ShoulderCenter, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.ShoulderCenter, size, skeletonId),
            ToVector2(JointType.ShoulderLeft, size, skeletonId), color, position);

        //Left Hand
        lb.Draw(spriteBatch, ToVector2(JointType.HandLeft, size, skeletonId),
            ToVector2(JointType.WristLeft, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.WristLeft, size, skeletonId),
            ToVector2(JointType.ElbowLeft, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.ElbowLeft, size, skeletonId),
            ToVector2(JointType.ShoulderLeft, size, skeletonId), color, position);

        //Hips & Spine
        lb.Draw(spriteBatch, ToVector2(JointType.HipLeft, size, skeletonId),
            ToVector2(JointType.HipCenter, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.HipRight, size, skeletonId),
            ToVector2(JointType.HipCenter, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.Spine, size, skeletonId),
            ToVector2(JointType.HipCenter, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.Spine, size, skeletonId),
            ToVector2(JointType.ShoulderCenter, size, skeletonId), color, position);

        //Left foot
        lb.Draw(spriteBatch, ToVector2(JointType.HipLeft, size, skeletonId),
            ToVector2(JointType.KneeLeft, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.KneeLeft, size, skeletonId),
            ToVector2(JointType.AnkleLeft, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.AnkleLeft, size, skeletonId),
            ToVector2(JointType.FootLeft, size, skeletonId), color, position);

        //Right foot
        lb.Draw(spriteBatch, ToVector2(JointType.HipRight, size, skeletonId),
            ToVector2(JointType.KneeRight, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.KneeRight, size, skeletonId),
            ToVector2(JointType.AnkleRight, size, skeletonId), color, position);
        lb.Draw(spriteBatch, ToVector2(JointType.AnkleRight, size, skeletonId),
            ToVector2(JointType.FootRight, size, skeletonId), color, position);
    }

    protected override void Dispose(bool disposing)
    {
        Uninitialize();
        base.Dispose(disposing);
    }
}
#endif