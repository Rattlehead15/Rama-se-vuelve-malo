using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();

        public ItemAction consumableItem;

        StateManager states;
        
        public void Init(StateManager st)
        {
            states = st;
            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnim = w.actions[i].targetAnim;
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.twoHandedActions.Count; i++)
            {
                Action a = GetAction(w.twoHandedActions[i].input);
                a.targetAnim = w.twoHandedActions[i].targetAnim;
            }
        }
        
        public void EmptyAllSlots()
        {
            for(int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnim = null;
            }
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return GetAction(a_input);
        }

        Action GetAction(ActionInput inp)
        {
            for(int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }

        public ActionInput GetActionInput(StateManager st)
        {
            if (st.R1)
                return ActionInput.R1;
            if (st.R2)
                return ActionInput.R2;
            if (st.L1)
                return ActionInput.L1;
            if (st.L2)
                return ActionInput.L2;

            return ActionInput.R1;
        }
    }

    public enum ActionInput
    {
        R1,L1,R2,L2
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnim;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string itemId;
    }
}