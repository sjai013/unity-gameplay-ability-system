using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Gamekit3D
{
    public class InteractOnButton : InteractOnTrigger
    {

        public string buttonName = "X";
        public UnityEvent OnButtonPress;

        bool canExecuteButtons = false;

        protected override void ExecuteOnEnter(Collider other)
        {
            canExecuteButtons = true;
        }

        protected override void ExecuteOnExit(Collider other)
        {
            canExecuteButtons = false;
        }

        void Update()
        {
            if (canExecuteButtons && Input.GetButtonDown(buttonName))
            {
                OnButtonPress.Invoke();
            }
        }

    } 
}
