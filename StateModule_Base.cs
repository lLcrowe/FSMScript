using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.State
{    
    /// <summary>
    /// ����ó���� ���� ���
    /// </summary>
    /// <typeparam name="T">int, enum:int, enum����</typeparam>
    [System.Serializable]
    public class StateModule_Base<T> where T : struct
    {   
        /// <summary>
        /// ���¾׼�
        /// </summary>
        public class WorkStateAction
        {
            //����� �ڵ������� action<bool>�� ����־ �ڵ� ���º�ȯ�ϴ°͵� �ϳ��� ���
            //������ ���Լ��� Ư�� ��ƾ�� ������ ���� ����̹Ƿ� �ű������ �۾�X
            internal System.Action enterAction;
            internal System.Action updateAction;
            internal System.Action exitAction;

            ~WorkStateAction()
            {
                ClearAction();
            }

            public void SetAction(System.Action enterAction, System.Action updateAction, System.Action exitAction)
            {
                this.enterAction = enterAction;
                this.updateAction = updateAction;
                this.exitAction = exitAction;
            }

            public void SetEnterAction(System.Action action)
            {
                enterAction = action;
            }
            public void SetUpdateAction(System.Action action)
            {
                updateAction = action;
            }
            public void SetExitAction(System.Action action)
            {
                exitAction = action;
            }

            public void ClearAction()
            {
                enterAction = null;
                updateAction = null;
                exitAction = null;
            }
        }

        
        [SerializeField] private T stateType;
        private Dictionary<T, WorkStateAction> stateActionBible = new();


        private static int maxIndex;//�ִ� �۵����� ����
        private static T[] stateTypeArray;

        static StateModule_Base()
        {   
            var tempArray = System.Enum.GetValues(typeof(T));
            maxIndex = tempArray.Length;
            stateTypeArray = new T[maxIndex];
            for (int i = 0; i < maxIndex; i++)
            {
                //�ڽ��� ����//�׷��� �ѹ���
                stateTypeArray[i] = (T)tempArray.GetValue(i);
            }
        }

        public StateModule_Base()
        {
            Init();
        }

        ~StateModule_Base()
        {
            ClearAction();
        }

        private void Init()
        {   
            for (int i = 0; i < stateTypeArray.Length; i++)
            {
                var key = stateTypeArray[i];
                WorkStateAction value = new();
                stateActionBible.Add(key, value);
            }
            stateType = stateTypeArray[0];
        }

        private void ClearAction()
        {
            foreach (var item in stateActionBible)
            {
                item.Value.ClearAction();
            }
        }

        /// <summary>
        /// ���¸� �����ϴ� �Լ�
        /// </summary>
        /// <param name="state">������ ����</param>
        public void SetState(in T state)
        {
            //������ �۵�
            var action = stateActionBible[stateType];
            action.exitAction?.Invoke();

            stateType = state;

            //����������Ʈ enter
            action = stateActionBible[stateType];
            action.enterAction?.Invoke();
        }
        /// <summary>
        /// ���� ���¸� �������� �Լ�
        /// </summary>
        /// <returns>���� ����</returns>
        public T GetState()
        {
            return stateType;
        }

        /// <summary>
        /// ���¿��� �۾������Լ����� �������� �Լ�
        /// </summary>
        /// <param name="key">����</param>
        /// <param name="workStateAction">�۾����� �Լ���</param>
        /// <returns>�����ϴ� ����</returns>
        public bool TryGetWorkStateAction(in T key, out WorkStateAction workStateAction)
        {
            workStateAction = null;
            if (!stateActionBible.TryGetValue(key, out var action))
            {
                return false;
            }
            workStateAction = action;
            return true;
        }

        /// <summary>
        /// ������Ʈ �Լ�
        /// </summary>
        public void UpdateWorkStateModule()
        {
            var action = stateActionBible[stateType];
            action.updateAction?.Invoke();
        }

        /// <summary>
        /// �ʱ�ȭ�� �����Լ�
        /// </summary>
        public void StartAction()
        {
            //ó�� �ε��� �׼��� ����
            var action = stateActionBible[stateType];
            action.enterAction?.Invoke();
        }

        /// <summary>
        /// ���� �׼����� �Ѿ�� �Լ�
        /// </summary>
        public void NextAction()
        {
            if (stateType is not int test)
            {
                return;
            }
            //�����۵��Ǵ°� exit
            var action = stateActionBible[stateType];
            action.exitAction?.Invoke();

            //int index = System.Convert.ToInt32(stateType);
            int index = test;

            //�ε��� �������� �ѱ�
            index++;
            if (maxIndex < index)
            {
                index = 0;
            }


            if (index is T newState)
            {
                stateType = newState;
            }

            //����������Ʈ enter
            action = stateActionBible[stateType];
            action.enterAction?.Invoke();
        }

        /// <summary>
        /// ���� ������ Enum������ �������� �Լ�
        /// </summary>
        /// <returns></returns>
        public T[] GetStateTypeArray()
        {
            return stateTypeArray;
        }
    }
}
