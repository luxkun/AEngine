using System;
using System.Collections.Generic;
using System.Linq;
using AEngine;
using Aiv.Fast2D;
using Aiv.Vorbis;
using System.Numerics;
using AEngine.Shape;
using OpenTK.Graphics;

namespace AEngine
{
    public class GameObject
    {
        public delegate void AfterUpdateEventHandler(object sender);

        public delegate void BeforeUpdateEventHandler(object sender);

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

        public virtual Engine Engine { get; internal set; }

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
        public virtual Vector3 Rotation { get; set; } = Vector3.Zero;

        public event AfterUpdateEventHandler OnAfterUpdate;
        public event BeforeUpdateEventHandler OnBeforeUpdate;
        public event DestroyEventHandler OnDestroy;
        public event DisableEventHandler OnDisable;
        public event EnableEventHandler OnEnable;
        public event StartEventHandler OnStart;
        public event UpdateEventHandler OnUpdate;

        public void AddHitBox(string name, Vector3 min, Vector3 max)
        {
            if (HitBoxes == null)
            {
                HitBoxes = new Dictionary<string, HitBox>();
            }
            var hbox = new HitBox(name, min, max, this);
            HitBoxes[name] = hbox;
        }

        public bool HasCollisions(Func<GameObject, HitBox, bool> predicate = null)
        {
            if (HitBoxes == null) return false;
            foreach (var obj in Engine.Objects.Values)
            {
                if (!obj.Enabled || obj == this || obj.HitBoxes == null)
                    continue;
                foreach (var hitBox in HitBoxes.Values)
                {
                    foreach (var otherHitBox in obj.HitBoxes.Values)
                    {
                        if ((predicate == null || predicate(obj, hitBox)) && hitBox.CollideWith(otherHitBox as Cuboid))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
                            var collision = new Collision(hitBox, obj, otherHitBox);
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

            RemoveAllHitBoxes();
            audioSource?.Dispose();
            Engine?.RemoveObject(this);
        }

        public void RemoveAllHitBoxes()
        {
            if (HitBoxes != null)
                foreach (var hitBox in HitBoxes.Keys.ToArray())
                    RemoveHitBox(hitBox);
        }

        public void RemoveHitBox(string hitBoxName)
        {
            HitBoxes[hitBoxName].Destroy();
            HitBoxes.Remove(hitBoxName);
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

            public override Engine Engine => Owner.Engine;

            public override Vector3 Position
            {
                get { return base.Position + Owner.Position; }
                set { base.Position = value; }
            }

            public override Vector3 Rotation
            {
                get { return base.Rotation + Owner.Rotation; }
                set { base.Rotation = value; }
            }

            public HitBox(string name, Vector3 min, Vector3 max, GameObject owner) : base(min, max)
            {
                Name = name;
                Owner = owner;
                Color = Color4.Green;
            }

            public new HitBox Clone()
            {
                return new HitBox(Name, Min, Max, Owner);
            }
        }

        public class Collision
        {
            private Vector2 minimumTranslation2D;
            private bool calculatedMT2D;

            public Collision(HitBox hitBox, GameObject other, HitBox otherHitBox)
            {
                HitBox = hitBox;
                Other = other;
                OtherHitBox = otherHitBox;
            }

            public HitBox HitBox { get; }
            public GameObject Other { get; private set; }
            public HitBox OtherHitBox { get; }

            public Vector2 MinimumTranslation2D
            {
                get
                {
                    if (!calculatedMT2D)
                    {
                        calculatedMT2D = true;
                        minimumTranslation2D = HitBox.MinimumTranslation2D(OtherHitBox);
                    }
                    return minimumTranslation2D;
                }
            }
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