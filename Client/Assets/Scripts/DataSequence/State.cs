namespace GameEngine.DataSequence
{
    public interface IState
    {
        void Enter();
        void Exit();
        void TickUpdate(float time);
    }
}