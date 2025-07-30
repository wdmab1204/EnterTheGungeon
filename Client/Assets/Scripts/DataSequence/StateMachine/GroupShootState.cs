using UnityEngine;

namespace GameEngine.DataSequence.StateMachine
{
    public class GroupShootState : UnitState
    {
        private GameObject prefab;
        private Transform player;
        private Transform muzzle;

        private GameObject obj;
        private Rigidbody2D rig;
        private float shootTime = 1f;
        private float curTime = 0f;

        public GroupShootState(GameObject prefab, Transform muzzle, Transform player)
        {
            this.prefab = prefab;
            this.player = player;
            this.muzzle = muzzle;
        }

        public override void Enter()
        {
            obj = Object.Instantiate(prefab);
            obj.transform.position = muzzle.position;
            rig = obj.GetComponent<Rigidbody2D>();
            rig.rotation = 45f;

            curTime = 0f;
        }

        public override void TickUpdate(float time)
        {
            if(curTime < shootTime)
            {
                curTime += time;
            }
            else
            {
                rig.velocity = (player.position - muzzle.position).normalized * 2;
                rig.angularVelocity = 100f;
                changeState(typeof(TeleportState));
            }
        }
    }
}
