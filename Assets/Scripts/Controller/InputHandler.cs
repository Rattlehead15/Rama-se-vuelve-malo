using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool equis;
        bool cuadrado;
        bool triangulo;
        bool circulo;
        bool L1;
        bool R1;
        float L2;
        bool L2_b;
        float R2;
        bool R2_b;
        bool L3;
        bool R3;
        
        float R2_timer;
        float L2_timer;

        float delta;
        StateManager states;
        CameraManager camManager;
        // Use this for initialization
        void Start()
        {
            states = GetComponent<StateManager>();
            states.Init();
            camManager = CameraManager.singleton;
            camManager.Init(states);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            updateStates();
            states.FixedTick(delta);
            camManager.Tick(delta);
            
        }

        private void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
        }

        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            triangulo = Input.GetButtonUp("triangulo");
            circulo = Input.GetButton("circulo");
            cuadrado = Input.GetButton("cuadrado");
            equis = Input.GetButton("equis");
            L3 = Input.GetButton("L3");
            R3 = Input.GetButtonUp("R3");
            L1 = Input.GetButton("L1");
            R1 = Input.GetButton("R1");
            R2_b = Input.GetButton("R2");
            R2 = Input.GetAxis("R2");
            if (R2 > 0)
            {
                R2_b = true;
            }
            L2_b = Input.GetButton("L2");
            L2 = Input.GetAxis("L2");
            if (L2 > 0)
            {
                L2_b = true;
            }
        }

        void updateStates()
        {
            states.vertical = vertical;
            states.horizontal = horizontal;

            Vector3 v = states.vertical * camManager.transform.forward;
            Vector3 h = states.horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            if (cuadrado)
                circulo = false;

            if (L3)
            {
                states.running = (states.moveAmount > 0);
                if (states.running)
                {
                    states.lockOn = false;
                    camManager.lockon = false;
                }
            }
            else
            {
                states.running = false;
            }
            states.L1 = L1;
            states.L2 = L2_b;
            states.R1 = R1;
            states.R2 = R2_b;
            states.cuadrado = cuadrado;
            states.circulo = circulo;
            
            if (triangulo)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            if (states.lockonTarget != null)
            {
                if (states.lockonTarget.eStates.isDead)
                {
                    states.lockOn = false;
                    states.lockonTarget = null;
                    states.lockonTransform = null;
                    camManager.lockon = false;
                }
            }

            if (R3)
            {
                states.lockOn = !states.lockOn;
                if (states.lockonTarget == null)
                    states.lockOn = false;
                camManager.lockonTarget = states.lockonTarget;
                states.lockonTransform = camManager.lockonTransform;
                camManager.lockon = states.lockOn;
                
            }
        }
    }
}

