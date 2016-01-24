using System;
using System.Collections.Generic;
using System.Linq;
using Aiv.Fast2D;
using Aiv.Vorbis;

namespace AEngine
{
    public class Engine
    {
        public delegate void AfterUpdateEventHandler(object sender);

        public delegate void BeforeUpdateEventHandler(object sender);

        public const float MaxDeltaTime = 0.33f;

        public bool debugCollisions;
        private Dictionary<GameObject.HitBox, Cuboid> debugCollisionsCuboids;

        // objects that need to be added (1) or removed (0) from sortedObjects
        // objects that have changed order (2)
        private Dictionary<GameObject, int> dirtyObjects;

        private int totalObjCount;

        private readonly Window window;
        private float wantedDeltaTime;

        protected Engine()
        {
        }

        public Engine(string windowName, int width, int height, int fps = 60, bool fullscreen = false, bool vsync = false)
        {
            // FPS?
            window = new Window(width, height, windowName, fullscreen, vsync);

            Initialize();

            wantedDeltaTime = fps > 0 ? 1f / fps : 0;
        }

        public TimerManager Timer { get; private set; }

        public Dictionary<string, Asset> Assets { get; private set; }

        public Camera Camera { get; set; }

        public bool ClearEveryFrame { get; set; } = true;

        public int Height => window.Height;

        public bool IsGameRunning { get; set; }

        public Joystick[] Joysticks { get; private set; }

        public int MouseX => window.mouseX;

        public int MouseY => window.mouseY;

        public bool MouseLeft => window.mouseLeft;

        public bool MouseRight => window.mouseRight;

        public bool MouseMiddle => window.mouseMiddle;

        public Dictionary<string, GameObject> Objects { get; private set; }

        public SortedSet<GameObject> SortedObjects { get; private set; }
        public float UnchangedTime { get; private set; }
        public float Time { get; private set; }

        // time modifier for deltaTime and deltaTicks
        public float TimeModifier { get; set; } = 1f;

        public int Width => window.Width;

        public float DeltaTime { get; private set; }
        public float UnchangedDeltaTime { get; private set; }

        public event AfterUpdateEventHandler OnAfterUpdate;

        public event BeforeUpdateEventHandler OnBeforeUpdate;

        public Texture WorkingTexture { get; private set; }
        public Sprite WorkingSprite { get; private set; }

        protected void GameUpdate()
        {
            window.Update();

            UnchangedDeltaTime = window.deltaTime;
            // having a deltatime higher than 0.33 means either that the game is having only 3fps or that something is loading
            // in both cases having a fixed maximum deltatime would make things smoother
            if (UnchangedDeltaTime > MaxDeltaTime)
                UnchangedDeltaTime = MaxDeltaTime;
            DeltaTime = UnchangedDeltaTime * TimeModifier;
            UnchangedTime += UnchangedDeltaTime;
            Time += DeltaTime;

            Timer.Update();

            OnBeforeUpdate?.Invoke(this);

            foreach (var obj in SortedObjects)
            {
                obj.UnchangedTime = UnchangedTime;
                obj.Time = Time;
                obj.UnchangedDeltaTime = UnchangedDeltaTime;
                obj.DeltaTime = DeltaTime;
                if (!obj.Enabled)
                    continue;
                obj.Draw(Camera);
                if (debugCollisions && obj.HitBoxes != null)
                {
                    foreach (var hitBox in obj.HitBoxes.Values)
                    {
                        hitBox.Cuboid.Draw(Camera);
                    }
                }
            }
            WorkingTexture.Update();
            WorkingSprite.DrawTexture(WorkingTexture);

            OnAfterUpdate?.Invoke(this);

            if (dirtyObjects.Count > 0)
            {
                var newObjects = dirtyObjects.ToArray();
                dirtyObjects.Clear();
                foreach (var pair in newObjects)
                {
                    switch (pair.Value)
                    {
                        case 1:
                            pair.Key.Time = Time;
                            SortedObjects.Add(pair.Key);
                            break;
                        case 0:
                            SortedObjects.Remove(pair.Key);
                            break;
                        case 2:
                            SortedObjects.Remove(pair.Key);
                            SortedObjects.Add(pair.Key);
                            break;
                    }
                }
            }
        }


        protected void Initialize()
        {
            // crappy
            new AudioSource();

            Camera = new Camera();

            // create dictionaries
            Objects = new Dictionary<string, GameObject>();
            SortedObjects = new SortedSet<GameObject>(new GameObjectComparer());
            Assets = new Dictionary<string, Asset>();

            dirtyObjects = new Dictionary<GameObject, int>();

            Joysticks = new Joystick[8];

            debugCollisionsCuboids = new Dictionary<GameObject.HitBox, Cuboid>();

            Timer = new TimerManager(this);

            WorkingTexture = new Texture(Width, Height);
            WorkingSprite = new Sprite(Width, Height);
        }


        public void DestroyAllObjects()
        {
            foreach (var obj in Objects.Values)
            {
                obj.Destroy();
            }
            // redundant now, could be useful in the future
            Objects.Clear();
            SortedObjects.Clear();
        }

        public Asset GetAsset(string name)
        {
            return Assets[name];
        }

        // keyboard management
        public bool IsKeyDown(KeyCode key)
        {
            return window.GetKey(key);
        }

        /*
		 * 
		 * Asset's management
		 * 
		 */

        public void LoadAsset(string name, Asset asset)
        {
            asset.Engine = this;
            Assets[name] = asset;
        }

        public void RemoveObject(GameObject obj)
        {
            if (debugCollisions && obj.HitBoxes != null)
                foreach (var hitBox in obj.HitBoxes.Values)
                    if (debugCollisionsCuboids.ContainsKey(hitBox))
                        debugCollisionsCuboids.Remove(hitBox);

            Objects.Remove(obj.Name);
            dirtyObjects[obj] = 0;
        }

        public void Run()
        {

            IsGameRunning = true;

            // compute update frequency
            //int freq = 1000 / this.fps;

            while (IsGameRunning && window.opened)
            {
                GameUpdate();
                if (!window.opened)
                    IsGameRunning = false;
                // maybe calculate average DeltaTime
                //if (Window.deltaTime < wantedDeltaTime)
                //{
                //    Thread.Sleep((int)((wantedDeltaTime - Window.deltaTime) * 111000));
                //}
            }
            IsGameRunning = false;
        }

        public void SpawnObject(GameObject obj)
        {
            SpawnObject(obj.Name, obj);
        }

        /* 
		 * 
		 * GameObject's management
		 * 
		 */

        public void SpawnObject(string name, GameObject obj)
        {
            obj.Name = name;
            obj.Engine = this;
            obj.Enabled = true;
            obj.Id = totalObjCount++;
            Objects[name] = obj;
            dirtyObjects[obj] = 1;
            obj.Initialize();
        }

        public void UpdatedObjectOrder(GameObject gameObject)
        {
            dirtyObjects[gameObject] = 2;
        }

        public class Joystick
        {
            public bool[] buttons;
            public long id;
            public int index;
            public string name;
            public int x;
            public int y;

            public Joystick()
            {
                // max 20 buttons
                buttons = new bool[20];
            }

            public bool anyButton()
            {
                foreach (var pressed in buttons)
                {
                    if (pressed)
                        return true;
                }
                return false;
            }

            public bool AnyButton()
            {
                return anyButton();
            }

            public int GetAxis(int axisIndex)
            {
                if (axisIndex < 0 || axisIndex > 1)
                    throw new ArgumentOutOfRangeException($"Wrong axis index '{axisIndex}', only 0 and 1 are supported.");
                return axisIndex == 0 ? x : y;
            }

            public bool GetButton(int buttonIndex)
            {
                return buttons[buttonIndex];
            }
        }
    }
}