using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoblinFramework.Gameplay.Phys
{
    public class PhysXU3D : MonoBehaviour
    {
        [HideInInspector]
        public uint actorId;
        [HideInInspector]
        public event Action<PhysXU3D> onCollisionEnter;
        [HideInInspector]
        public event Action<PhysXU3D> onCollisionExit;
        
        private void NotifyCollisionEnter(Collider2D collider)
        {
            if (null == onCollisionEnter) return;
            
            var physxu3d = collider.transform.GetComponent<PhysXU3D>();
            if (null == physxu3d) return;
            
            onCollisionEnter.Invoke(physxu3d);
        }

        private void NotifyCollisionExit(Collider2D collider)
        {
            if (null == onCollisionExit) return;
            
            var physxu3d = collider.transform.GetComponent<PhysXU3D>();
            if (null == physxu3d) return;
            
            onCollisionExit.Invoke(physxu3d);
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