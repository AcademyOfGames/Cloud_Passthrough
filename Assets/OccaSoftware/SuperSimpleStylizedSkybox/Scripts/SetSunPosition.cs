using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OccaSoftware
{
    public class SetSunPosition : MonoBehaviour
    {
        public Text debug;
        [SerializeField] float rotationSpeed = 10f;
        public Transform handTransform;
        public float currentRotationSpeed;
        public float handSpeed;
        private float lowHandPos;
        private float highHandPos;

        private bool morningMusic = false;
        private void Start()
        {
            float startHandYPos = handTransform.localPosition.y;
            lowHandPos = startHandYPos - .1f;
            highHandPos = startHandYPos + .25f;
        }

        void Update()
        {
            //if (transform.eulerAngles.x > 305 || transform.eulerAngles.x < 40)
            {
                handSpeed = Mathf.InverseLerp(lowHandPos,highHandPos, handTransform.localPosition.y);
                currentRotationSpeed = rotationSpeed * handSpeed* handSpeed;
                transform.Rotate(transform.right * currentRotationSpeed * Time.deltaTime, Space.World);
            }

            if (!morningMusic && transform.eulerAngles.x > 355)
            {
                morningMusic = true;
                GetComponent<AudioSource>().Play();
            }
    
        if(debug !=null)           debug.text = "rotation x = " + transform.eulerAngles.x+ "between  " + lowHandPos.ToString() + " and " + highHandPos.ToString() + " at " + handTransform.localPosition.y + " speed is " + handSpeed;
        }
    }
}