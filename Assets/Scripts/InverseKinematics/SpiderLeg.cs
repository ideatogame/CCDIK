using System.Collections;
using UnityEngine;

namespace InverseKinematics
{
    public class SpiderLeg : MonoBehaviour
    {
        public bool StepIsHappening { get; private set; }
        
        [Header("Transforms")]
        [SerializeField] private Transform body;
        [SerializeField] private Transform target;
        [SerializeField] private Transform targetPoint;
        [SerializeField] private SpiderLeg[] oppositeLegs;
        
        [Header("Settings")]
        [SerializeField] private float distance;
        [SerializeField] private float stepSmoothTime;
        [SerializeField] private float stepSize = 0.5f;

        private bool canWalkAgain = true;

        private void FixedUpdate()
        {
            bool oppositeLegsAreWalking = false;
            
            foreach (SpiderLeg leg in oppositeLegs)
            {
                oppositeLegsAreWalking |= leg.StepIsHappening;
                if(oppositeLegsAreWalking) break;
            }
            
            bool stepIsNotHappening = !StepIsHappening && !oppositeLegsAreWalking;
            bool targetIsTooFar = (target.position - targetPoint.position).sqrMagnitude > distance * distance;
            
            if (stepIsNotHappening && targetIsTooFar && canWalkAgain)
                StartCoroutine(MoveTargetToPoint(targetPoint.position));
        }

        private IEnumerator MoveTargetToPoint(Vector3 point)
        {
            canWalkAgain = false;
            StepIsHappening = true;

            float timer = 0f;
            Vector3 startPoint = target.position;
            float maxHeight = startPoint.y + stepSize;

            if (maxHeight < point.y) 
                maxHeight = point.y + 0.2f;

            while (timer <= stepSmoothTime)
            {
                timer += Time.deltaTime;
                float t = timer / stepSmoothTime;

                Vector3 lerpPosition = Vector3.Lerp(startPoint, point, t);
                float height = t <= 0.5f
                    ? Mathf.Lerp(startPoint.y, maxHeight, t) 
                    : Mathf.Lerp(maxHeight, point.y, t);
                
                lerpPosition.y = height;
                target.position = lerpPosition;
                
                yield return null;
            }

            target.position = point;
            StepIsHappening = false;
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            canWalkAgain = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(targetPoint.position, target.position);
        }
    }
}