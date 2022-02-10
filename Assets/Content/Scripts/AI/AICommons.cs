using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

namespace ArchTitan.AI
{
    [IncludeInSettings(true)]
    public class AICommons : MonoBehaviour
    {
        //public Variables Variables;

        public NavMeshAgent Agent;
        public float WanderMaxDistance = 15;
        public bool UseYAxis;

        public Transform Look;
        public float Angle = 180;
        public string EnemyTag = "Player";

#if UNITY_EDITOR
        public bool Debug;
#endif

        public void Attack(Weapon weapon, GameObject target, string onShoot)
        {
            var dir = (target.transform.position - Look.position).normalized;

            weapon.Shoot(dir);

            void finished() => CustomEvent.Trigger(gameObject, onShoot);

            weapon.OnFinishedShooting -= finished;
            weapon.OnFinishedShooting += finished;
        }

        public void MoveToPosition(Vector3 position, string onReach)
        {
            Agent.SetDestination(position);

            StartCoroutine(waitForAgent(() =>
            {
                if (onReach != null)
                    CustomEvent.Trigger(gameObject, onReach);
            }));
        }

        public void Idle(string wanderState, string enemyFindState)
        {
            var result = look();

            if(result)
            {
                //Variables.declarations.Set("Target", result);
                CustomEvent.Trigger(gameObject, enemyFindState, result);
            }
            else
            {
                var random = UnityEngine.Random.Range(0, 100);
                if (random <= 30)
                    CustomEvent.Trigger(gameObject, wanderState);
            }
        }

        static Collider[] visionHits = new Collider[320];
        GameObject look()
        {
            /*            int count = Angle * 2;

                        NativeArray<RaycastCommand> vision = new(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                        NativeArray<RaycastHit> hits = new NativeArray<RaycastHit>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                        GameObject target = null;

                        try
                        {
                            var position = Look.position;
                            var forward = Look.forward;

                            int index = 0;
                            for (int x = -(Angle / 2); x < Angle / 2; x++)
                                for (int y = -(Angle / 2); y < Angle / 2; y++)
                                {
                                    vision[index] = new RaycastCommand(position, forward + new Vector3(x, y, 0), 300f);
                                    index++;
                                }

                            var handle = RaycastCommand.ScheduleBatch(vision, hits, count / 10);
                            handle.Complete();

                            for (int i = 0; i < count; i++)
                            {
                                var hit = hits[i];

                                if (hit.collider)
                                {
                                    if (hit.collider.CompareTag(EnemyTag))
                                    {
                                        target = hit.collider.gameObject;
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            vision.Dispose();
                            hits.Dispose();
                        }*/

            int results = Physics.OverlapSphereNonAlloc(Look.position, 300f, visionHits);

            for (int i = 0; i < results; i++)
            {
                var hit = visionHits[i];

                Vector3 targetDir = hit.transform.position - Look.position;
                float angle = Vector3.Angle(targetDir, Look.forward);

                if (angle < Angle)
                {
                    if (hit.CompareTag(EnemyTag))
                        return hit.gameObject;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if(Debug)
            {
                /*                var position = Look.position;
                                var forward = Look.forward;

                                for (int x = -(Angle / 2); x < Angle / 2; x++)
                                    for (int y = -(Angle / 2); y < Angle / 2; y++)
                                        Gizmos.DrawLine(position, Vector3.Scale(forward, new Vector3(x, y, 0)));*/
                var col = Gizmos.color;

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(Look.position, 300f);
                Gizmos.color = col;
            }
        }
#endif

        public void Wander(string triggerOnComplete)
        {
            var agent = Agent;
            var maxDistance = WanderMaxDistance;
            var useYAxis = UseYAxis;

            var random = UnityEngine.Random.insideUnitSphere * maxDistance;
            if (!useYAxis)
                random.y = 0;
            else
                random.y = Mathf.Clamp(random.y, agent.transform.position.y, 30);

            agent.SetDestination(agent.transform.position + new Vector3(random.x, random.y, random.y));

            StartCoroutine(waitForAgent(() => CustomEvent.Trigger(gameObject, triggerOnComplete)));
        }

        IEnumerator waitForAgent(Action onReach)
        {
            while (!Agent.isOnNavMesh) yield return null;

            while (Agent.pathPending) yield return null;

            while (!Agent.isStopped)
                yield return null;

            onReach();
        }
    }
}