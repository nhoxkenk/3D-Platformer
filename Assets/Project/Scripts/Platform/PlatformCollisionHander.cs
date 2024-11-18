﻿using UnityEngine;

namespace Platformer
{
    public class PlatformCollisionHander : MonoBehaviour {
        Transform platform; // The platform, if any, we are on top of

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                ContactPoint contact = collision.GetContact(0);
                if(contact.normal.y < 0.5f) return;

                platform = collision.transform;
                transform.SetParent(platform);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform")) {
                transform.SetParent(null);
                platform = null;
            }
        }
    }
}
