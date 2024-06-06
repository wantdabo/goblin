// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2023 Kybernetik //

using UnityEngine;

namespace Animancer.FSM
{
    /// <summary>Base class for <see cref="MonoBehaviour"/> states to be used in a <see cref="StateMachine{TState}"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/fsm/state-types">State Types</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.FSM/StateBehaviour
    /// 
    [HelpURL(StateExtensions.APIDocumentationURL + nameof(StateBehaviour))]
    public abstract class StateBehaviour : MonoBehaviour, IState
    {
        /************************************************************************************************************************/

        /// <summary>[<see cref="IState.CanEnterState"/>]
        /// Determines whether the <see cref="StateMachine{TState}"/> can enter this state.
        /// Always returns true unless overridden.
        /// </summary>
        public virtual bool CanEnterState => true;

        /// <summary>[<see cref="IState.CanExitState"/>]
        /// Determines whether the <see cref="StateMachine{TState}"/> can exit this state.
        /// Always returns true unless overridden.
        /// </summary>
        public virtual bool CanExitState => true;

        /************************************************************************************************************************/

        /// <summary>[<see cref="IState.OnEnterState"/>]
        /// Asserts that this component isn't already enabled, then enables it.
        /// </summary>
        public virtual void OnEnterState()
        {
#if UNITY_ASSERTIONS
            if (enabled)
                Debug.LogError($"{nameof(StateBehaviour)} was already enabled before {nameof(OnEnterState)}: {this}", this);
#endif
#if UNITY_EDITOR
            // Unity doesn't constantly repaint the Inspector if all the components are collapsed.
            // So we can simply force it here to ensure that it shows the correct state being enabled.
            else
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif

            enabled = true;
        }

        /************************************************************************************************************************/

        /// <summary>[<see cref="IState.OnExitState"/>]
        /// Asserts that this component isn't already disabled, then disables it.
        /// </summary>
        public virtual void OnExitState()
        {
            if (this == null)
                return;

#if UNITY_ASSERTIONS
            if (!enabled)
                Debug.LogError($"{nameof(StateBehaviour)} was already disabled before {nameof(OnExitState)}: {this}", this);
#endif

            enabled = false;
        }

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only] States start disabled and only the current state gets enabled at runtime.</summary>
        /// <remarks>Called in Edit Mode whenever this script is loaded or a value is changed in the Inspector.</remarks>
        protected virtual void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            enabled = false;
        }
#endif

        /************************************************************************************************************************/
    }
}
