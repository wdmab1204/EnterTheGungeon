using GameEngine.DataSequence.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.DataSequence.StateMachine
{
    public class TeleportState : UnitState
    {
        private List<Vector3Int> teleportableTileList = new();
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D collider;
        private Rigidbody2D rigidbody;
        private Transform player;
        private RoomNode roomNode;
        private float teleportStartTime = 0.1f;
        private float teleportEndTime = 1.5f;
        private float curTime = 0f;


        public TeleportState(Transform myMob, Transform player, RoomNode roomNode)
        {
            this.player = player;
            this.roomNode = roomNode;

            var floorTilemap = roomNode.GetTilemap("Floor");
            var collideableTilemap = roomNode.GetTilemap("Collideable");
            spriteRenderer = myMob.GetComponent<SpriteRenderer>();
            collider = myMob.GetComponent<BoxCollider2D>();
            rigidbody = myMob.GetComponent<Rigidbody2D>();

            foreach(Vector3Int floorTilePos in floorTilemap.cellBounds.allPositionsWithin)
            {
                if(IsValid(floorTilePos))
                    teleportableTileList.Add(floorTilePos);
            }

            bool IsValid(Vector3Int tilePos)
            {
                int minX = (int)(tilePos.x - collider.size.x / 2 + collider.offset.x);
                int maxX = (int)(tilePos.x + collider.size.x / 2 + collider.offset.x);
                int minY = (int)(tilePos.y - collider.size.y / 2 + collider.offset.y);
                int maxY = (int)(tilePos.y + collider.size.y / 2 + collider.offset.y);

                for(int x = minX; x <= maxX; x++)
                {
                    for(int y = minY; y <= maxY; y++)
                    {
                        Vector3Int neighborTilePos = new Vector3Int(x, y);
                        if (floorTilemap.GetTile(neighborTilePos) == null || collideableTilemap.GetTile(neighborTilePos) != null)
                            return false;
                    }
                }

                return true;
            }
        }

        public override void Enter()
        {
            curTime = 0f;
            rigidbody.velocity = Vector2.zero;
        }

        public override void TickUpdate(float time)
        {
            if(curTime >= teleportEndTime)
            {
                spriteRenderer.color = Color.white;
                spriteRenderer.flipX = (transform.position - player.position).x < 0;
                collider.enabled = true;

                var randPos = GetRandomPosition();
                if (randPos == null)
                    return;
                transform.localPosition = randPos.Value;

                changeState(typeof(GroupShootState));
            }
            else if(curTime >= teleportStartTime)
            {
                spriteRenderer.color = new(1, 1, 1, 0);
                collider.enabled = false;
            }
            curTime += time;
        }

        private float minDistance = 5f;

        private Vector3Int? GetRandomPosition()
        {
            int safetyCount = 100;
            while (safetyCount > 0)
            {
                Vector3Int teleportPositionInt = teleportableTileList[UnityEngine.Random.Range(0, teleportableTileList.Count)];
                Vector3 teleportPosition = teleportPositionInt;
                var teleportWorldPosition = GetWorldPositionFromLocal(roomNode.ToVector3(), teleportPosition);
                if ((teleportWorldPosition - player.position).sqrMagnitude < minDistance * minDistance)
                    continue;

                RaycastHit2D hit = Physics2D.Linecast(teleportWorldPosition, player.position, LayerMask.GetMask("Collideable"));
                Debug.DrawRay(teleportWorldPosition, player.position - teleportWorldPosition, Color.red, 1f);
                if (hit.collider == null)
                {
                    return teleportPositionInt;
                }

                safetyCount--;
            }

            UnityEngine.Debug.LogError("Over Count Safety");
            return null;
        }

        private Vector3 GetWorldPositionFromLocal(Vector3 roomWorldPosition, Vector3 localPosition)
        {
            Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(roomWorldPosition, Quaternion.identity, Vector3.one);
            Vector3 worldPosition = localToWorldMatrix.MultiplyPoint3x4(localPosition);
            return worldPosition;
        }
    }
}
