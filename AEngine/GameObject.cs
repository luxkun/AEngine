using System;
using System.Collections.Generic;
using AEngine;
using Aiv.Fast2D;
using Aiv.Vorbis;
using OpenTK;

namespace AEngine
{
    public class GameObject
    {
        public delegate void AfterUpdateEventHandler(object sender);

        public delegate void BeforeUpdateEventHandler(object sender);

        /*
		 * 
		 * events management
		 * 
		 */

        public delegate void DestroyEventHandler(object sender);

        public delegate void DisableEventHandler(object sender);

        public delegate void EnableEventHandler(object sender);

        public delegate void StartEventHandler(object sender);

        public delegate void UpdateEventHandler(object sender);

        private bool enabled;

        // rendering order, lower values are rendered before
        private int order;
        private AudioSource audioSource;

        public GameObject()
        {
            Timer = new TimerManager(this);
        }


        public float DeltaTime { get; internal set; }

        public float UnchangedDeltaTime { get; internal set; }

        // not drawn but with hitboxes
        public virtual bool CanDraw { get; set; } = true;

        // not drawn and no hitboxes
        public virtual bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    // call Enable/Disable events
                    if (value)
                    {
                        OnEnable?.Invoke(this);
                    }
                    else
                    {
                        OnDisable?.Invoke(this);
                    }
                }
                enabled = value;
            }
        }

        public AudioSource AudioSource
        {
            get { return audioSource ?? (audioSource = new AudioSource()); }
            set { audioSource = value; }
        }

        public Engine Engine { get; internal set; }

        public Dictionary<string, HitBox> HitBoxes { get; private set; }

        public virtual int Id { get; set; }

        public virtual bool IgnoreCamera { get; set; } = false;

        public virtual string Name { get; set; }

        public virtual int Order
        {
            get { return order; }
            set
            {
                if (Engine != null && order != value) // if the object has been spawned
                    Engine.UpdatedObjectOrder(this);
                order = value;
            }
        }

        public float UnchangedTime { get; internal set; }

        public float Time { get; internal set; }

        public TimerManager Timer { get; }
        public virtual Vector3 Position { get; set; }

        public virtual Vector3 Scale { get; set; } = Vector3.One;

        public event AfterUpdateEventHandler OnAfterUpdate;
        public event BeforeUpdateEventHandler OnBeforeUpdate;
        public event DestroyEventHandler OnDestroy;
        public event DisableEventHandler OnDisable;
        public event EnableEventHandler OnEnable;
        public event StartEventHandler OnStart;
        public event UpdateEventHandler OnUpdate;

        public void AddHitBox(string name, float length, float depth, float height)
        {
            if (HitBoxes == null)
            {
                HitBoxes = new Dictionary<string, HitBox>();
            }
            var hbox = new HitBox(name, length, depth, height, this);
            HitBoxes[name] = hbox;
        }

        // check with all objects
        public List<Collision> CheckCollisions()
        {
            if (HitBoxes == null)
            {
                throw new Exception("GameObject without hitboxes");
            }
            var collisions = new List<Collision>();
            foreach (var obj in Engine.Objects.Values)
            {
                if (!obj.Enabled)
                    continue;
                // ignore myself
                if (obj == this)
                    continue;
                if (obj.HitBoxes == null)
                    continue;
                foreach (var hitBox in HitBoxes.Values)
                {
                    foreach (var otherHitBox in obj.HitBoxes.Values)
                    {
                        if (hitBox.CollideWith(otherHitBox))
                        {
                            var collision = new Collision(hitBox.Name, obj, otherHitBox.Name);
                            collisions.Add(collision);
                        }
                    }
                }
            }
            return collisions;
        }

        public virtual GameObject Clone()
        {
            var go = new GameObject
            {
                Name = Name,

            };
            return go;
        }

        public virtual void Destroy()
        {
            // call event handlers
            OnDestroy?.Invoke(this);
            Enabled = false;

            AudioSource.Dispose();
            Engine?.RemoveObject(this);
        }

        public virtual void Draw(Camera camera)
        {
            OnBeforeUpdate?.Invoke(this);

            Timer.Update();
            Update();
            OnUpdate?.Invoke(this);

            OnAfterUpdate?.Invoke(this);
        }

        public virtual void Initialize()
        {
            Start();
            OnStart?.Invoke(this);
        }

        // this is called when the GameObject is allocated
        public virtual void Start()
        {
        }

        // this is called by the game loop at every cycle
        public virtual void Update()
        {
        }

        public sealed class HitBox : Cuboid
        {
            public GameObject Owner { get; set; }

            public HitBox(string name, float length, float depth, float height, GameObject owner) : base(length, depth, height)
            {
                Name = name;
                Owner = owner;
            }

            public Cuboid Cuboid { get; set; }

            public new HitBox Clone()
            {
                return new HitBox(Name, Cuboid.Length, Cuboid.Depth, Cuboid.Height, Owner);
            }

            public bool CollideWith(HitBox other)
            {
                //var x1 = (int)(Owner.DrawX + X);
                //var y1 = (int)(Owner.DrawY + Y);
                //var x2 = (int)(other.Owner.DrawX + other.X);
                //var y2 = (int)(other.Owner.DrawY + other.Y);
                //// simple rectangle collision check
                //if (x1 + Width >= x2 &&
                //    x1 <= x2 + other.Width &&
                //    y1 + Height >= y2 &&
                //    y1 <= y2 + other.Height)
                //    return true;
                //// no collision
                return false;
            }
        }

        public class Collision
        {
            public Collision(string hitBoxName, GameObject other, string otherHitBoxName)
            {
                HitBox = hitBoxName;
                Other = other;
                OtherHitBox = otherHitBoxName;
            }

            public string HitBox { get; private set; }
            public GameObject Other { get; private set; }
            public string OtherHitBox { get; private set; }
        }
    }

    internal class GameObjectComparer : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            var result = y.Order.CompareTo(x.Order);
            if (result == 0)
                result = y.Id.CompareTo(x.Id);
            return -1 * result;
        }
    }
}