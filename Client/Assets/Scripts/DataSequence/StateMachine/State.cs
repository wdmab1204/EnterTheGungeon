namespace GameEngine.DataSequence.StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
        void TickUpdate(float time);
    }
}