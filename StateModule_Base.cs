using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.State
{    
    /// <summary>
    /// 상태처리를 위한 모듈
    /// </summary>
    /// <typeparam name="T">int, enum:int, enum형식</typeparam>
    [System.Serializable]
    public class StateModule_Base<T> where T : struct
    {   
        /// <summary>
        /// 상태액션
        /// </summary>
        public class WorkStateAction
        {
            //여기다 자동적으로 action<bool>을 집어넣어서 자동 상태변환하는것도 하나의 방법
            //하지만 현함수는 특정 루틴을 돌리기 위한 기능이므로 거기까지는 작업X
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


        private static int maxIndex;//최대 작동상태 수량
        private static T[] stateTypeArray;

        static StateModule_Base()
        {   
            var tempArray = System.Enum.GetValues(typeof(T));
            maxIndex = tempArray.Length;
            stateTypeArray = new T[maxIndex];
            for (int i = 0; i < maxIndex; i++)
            {
                //박싱이 예상//그러나 한번뿐
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
        /// 상태를 설정하는 함수
        /// </summary>
        /// <param name="state">설정할 상태</param>
        public void SetState(in T state)
        {
            //이전거 작동
            var action = stateActionBible[stateType];
            action.exitAction?.Invoke();

            stateType = state;

            //다음스테이트 enter
            action = stateActionBible[stateType];
            action.enterAction?.Invoke();
        }
        /// <summary>
        /// 현재 상태를 가져오는 함수
        /// </summary>
        /// <returns>현재 상태</returns>
        public T GetState()
        {
            return stateType;
        }

        /// <summary>
        /// 상태에서 작업상태함수들을 가져오는 함수
        /// </summary>
        /// <param name="key">상태</param>
        /// <param name="workStateAction">작업상태 함수들</param>
        /// <returns>존재하는 여부</returns>
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
        /// 업데이트 함수
        /// </summary>
        public void UpdateWorkStateModule()
        {
            var action = stateActionBible[stateType];
            action.updateAction?.Invoke();
        }

        /// <summary>
        /// 초기화시 시작함수
        /// </summary>
        public void StartAction()
        {
            //처음 인덱스 액션을 시작
            var action = stateActionBible[stateType];
            action.enterAction?.Invoke();
        }

        /// <summary>
        /// 다음 액션으로 넘어가는 함수
        /// </summary>
        public void NextAction()
        {
            if (stateType is not int test)
            {
                return;
            }
            //현재작동되는걸 exit
            var action = stateActionBible[stateType];
            action.exitAction?.Invoke();

            //int index = System.Convert.ToInt32(stateType);
            int index = test;

            //인덱스 다음으로 넘김
            index++;
            if (maxIndex < index)
            {
                index = 0;
            }


            if (index is T newState)
            {
                stateType = newState;
            }

            //다음스테이트 enter
            action = stateActionBible[stateType];
            action.enterAction?.Invoke();
        }

        /// <summary>
        /// 현재 지정된 Enum값들을 가져오는 함수
        /// </summary>
        /// <returns></returns>
        public T[] GetStateTypeArray()
        {
            return stateTypeArray;
        }
    }
}
