using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public abstract class StateBase : StateMachineBehaviour, IState
    {
        public abstract void Setup(IStateMachineContext _context);

        /// <summary>
        /// Funzione chiamata all'ingresso dello stato
        /// </summary>
        public virtual void Enter()
        {

        }

        /// <summary>
        /// Funzione chiamata nell'update dello stato
        /// </summary>
        public virtual void Tick()
        {

        }

        /// <summary>
        /// Funzione chiamata all'uscita dallo stato
        /// </summary>
        public virtual void Exit()
        {

        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Enter();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Exit();
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Tick();
        }
    }

    public interface IState
    {
        /// <summary>
        /// Funzione chiamata per eseguire il Setup
        /// </summary>
        /// <param name="_context"></param>
        void Setup(IStateMachineContext _context);

        /// <summary>
        /// Funzione chiamata quando lo stato diventa attivo
        /// </summary>
        void Enter();

        /// <summary>
        /// Funzione chiamata mentre lo stato è attivo ad ogni ciclo di vita dell'applicazione
        /// </summary>
        void Tick();

        /// <summary>
        /// Funzione chiamata quando lo stato viene disattivato
        /// </summary>
        void Exit();
    }

    public interface IStateMachineContext
    {

    }
}
