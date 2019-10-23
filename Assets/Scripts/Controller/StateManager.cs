using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;
        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool R2, R1, L2, L1;
        public bool circulo;
        public bool cuadrado;
        [Header("Stats")]
        public float moveSpeed = 3;
        public float runSpeed = 5;
        public float rotateSpeed = 5;
        public float distanceToGround = 0.5f;
        public float rollSpeed = 1;
        [Header("States")]
        public bool onGround;
        public bool running;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;
        public bool usingItem;
        [Header("Other")]
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;
        public AnimationCurve rollCurve;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody body;
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;
        float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            body = GetComponent<Rigidbody>();
            body.angularDrag = 999;
            body.drag = 4;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init();

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.GetComponent<AnimatorHook>();
            if(a_hook == null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this,null);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);
            anim.SetBool("OnGround", true);

        }

        void SetupAnimator() {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }
            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;
            
            usingItem = anim.GetBool("Interacting");
            
            DetectItemAction();
            DetectAction();

            inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

            if (inAction)
            {
                anim.applyRootMotion = true;
                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else               
                    return;
            }
            
            canMove = anim.GetBool("CanMove");

            if (!canMove)
                return;

            //a_hook.rootMotionMultiplier = 1;
            a_hook.CloseRoll();
            HandleRolls();

            anim.applyRootMotion = false;

            body.drag = (moveAmount > 0 || !onGround) ? 0 : 4;
            float targetSpeed = moveSpeed;

            if (usingItem)
            {
                running = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.35f);
            }

            if (running)
                targetSpeed = runSpeed;

            if (onGround)
            {
                body.velocity = moveDir * (targetSpeed * moveAmount);
            }

            Vector3 targetDir = (lockOn == false) ? moveDir : (lockonTransform!=null) ? lockonTransform.transform.position - transform.position : moveDir;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRot = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRot;

            anim.SetBool("LockOn", lockOn);

            if (lockOn == false)
                HandleMovementAnimations();
            else
                HandleLockonAnimations(moveDir);
        }

        public void DetectItemAction()
        {
            if (!canMove || usingItem)
                return;
            if (!cuadrado)
                return;
            ItemAction Slot = actionManager.consumableItem;
            string targetAnim = Slot.targetAnim;
            if (string.IsNullOrEmpty(targetAnim))
                return;

            inventoryManager.rightHandWeapon.weaponModel.SetActive(false);
            usingItem = true;
            anim.Play(targetAnim);
        }

        public void DetectAction()
        {
            if (!canMove || usingItem)
                return;

            if (R1 == false && R2 == false && L2 == false && L1 == false)
                return;
            string targetAnim = null;

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;
            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            canMove = false;
            inAction = true;
            anim.CrossFade(targetAnim, 0.2f);
            //body.velocity = Vector3.zero;
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool("OnGround", onGround);
        }

        void HandleRolls()
        {
            if (!circulo || usingItem)
                return;
            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;
            /*if (!lockOn)
            {
                v = (moveAmount>0.3f) ? 1 : 0;
                h = 0;
            }
            else
            {
                if (Mathf.Abs(v) < 0.3f)
                    v = 0;
                if (Mathf.Abs(h) < 0.3f)
                    h = 0;
            }*/

            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rootMotionMultiplier = rollSpeed;
            }
            else
            {
                a_hook.rootMotionMultiplier = 1.3f;
            }

            anim.SetFloat("Vertical", v);
            anim.SetFloat("Horizontal", h);

            canMove = false;
            inAction = true;
            anim.CrossFade("Rolls", 0.2f);
        }

        void HandleMovementAnimations()
        {
            anim.SetBool("Running", running);
            anim.SetFloat("Vertical", moveAmount,0.4f,delta);

        }

        void HandleLockonAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;
            anim.SetFloat("Vertical", v, 0.2f, delta);
            anim.SetFloat("Horizontal", h, 0.2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;
            Vector3 origin = transform.position + Vector3.up * distanceToGround;
            Vector3 direction = Vector3.down;
            float dis = distanceToGround + 0.3f;
            RaycastHit Hit;
            if(Physics.Raycast(origin,direction, out Hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = Hit.point;
                transform.position = targetPosition;
            }
            return r;
        }

        public void HandleTwoHanded()
        {
            anim.SetInteger("WeaponType", isTwoHanded?1:0);
            if (isTwoHanded)
                actionManager.UpdateActionsTwoHanded();
            else
                actionManager.UpdateActionsOneHanded();
        }
    }
}
