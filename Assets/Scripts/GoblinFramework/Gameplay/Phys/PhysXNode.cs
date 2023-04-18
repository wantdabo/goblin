using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoblinFramework.Gameplay.Phys
{
    public class PhysXNode : MonoBehaviour
    {
        [HideInInspector]
        public uint actorId;
        [HideInInspector]
        public event Action<uint, int> onCollisionEnter;
        [HideInInspector]
        public event Action<uint, int> onCollisionExit;

        private Rigidbody2D mRigibody2d;
        public Rigidbody2D rigibody2d
        {
            get
            {
                if (null == mRigibody2d)
                {
                    mRigibody2d = gameObject.AddComponent<Rigidbody2D>();
                    mRigibody2d.bodyType = RigidbodyType2D.Static;
                    mRigibody2d.simulated = false;
                    mRigibody2d.gravityScale = 0f;
                }

                return mRigibody2d;
            }
            private set { mRigibody2d = value; }
        }

        private BoxCollider2D mBoxcollider2d;
        public BoxCollider2D boxcollider2d
        {
            get
            {
                if (null == mBoxcollider2d)
                {
                    mBoxcollider2d = gameObject.AddComponent<BoxCollider2D>();
                    // TODO 设置物理材质，摩檫力不要，弹力也不要
                }

                return mBoxcollider2d;
            }
            private set { mBoxcollider2d = value; }
        }

        private CircleCollider2D mCirclecollider2d;
        public CircleCollider2D circlecollider2d
        {
            get
            {
                if (null == mCirclecollider2d)
                {
                    mCirclecollider2d = gameObject.AddComponent<CircleCollider2D>();
                    // TODO 设置物理材质，摩檫力不要，弹力也不要
                }

                return mCirclecollider2d;
            }
            private set { mCirclecollider2d = value; }
        }

        public void SetIdle()
        {
            if (null != mRigibody2d) mRigibody2d.simulated = false;
            if (null != mBoxcollider2d && mBoxcollider2d.enabled) mBoxcollider2d.enabled = false;
            if (null != mCirclecollider2d && mCirclecollider2d.enabled) mCirclecollider2d.enabled = false;
        }

        private void NotifyCollisionEnter(Collider2D collider)
        {
            if (null == onCollisionEnter) return;

            var node = collider.transform.GetComponent<PhysXNode>();
            if (null == node) return;

            onCollisionEnter.Invoke(node.actorId, collider.gameObject.layer);
        }

        private void NotifyCollisionExit(Collider2D collider)
        {
            if (null == onCollisionExit) return;

            var node = collider.transform.GetComponent<PhysXNode>();
            if (null == node) return;

            onCollisionExit.Invoke(node.actorId, collider.gameObject.layer);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            NotifyCollisionEnter(col.collider);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            NotifyCollisionExit(other.collider);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            NotifyCollisionEnter(col);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            NotifyCollisionExit(other);
        }
    }
}