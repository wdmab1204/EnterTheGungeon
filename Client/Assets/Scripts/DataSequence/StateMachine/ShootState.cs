using System;

namespace GameEngine.DataSequence.StateMachine
{
    public class ShootState : UnitState
    {
        private float interval = .3f;
        private int count = 3;

        private int curCount;
        private float curTime;

        private Action shootAction;

        public ShootState(Action shootAction)
        {
            this.shootAction = shootAction;
        }
        
        public override void Enter()
        {
            curCount = 0;
            curTime = 0f;
        }

        public override void Exit()
        {
        }

        public override void TickUpdate(float time)
        {
            if (curTime >= interval && curCount < count)
            {
                curTime = 0f;
                curCount++;
                shootAction();
            }
            else if (curCount >= count)
                changeState(typeof(WalkState));
            else
                curTime += time;
        }
    }
}