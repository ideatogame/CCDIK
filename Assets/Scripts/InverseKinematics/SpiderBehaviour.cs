using UnityEngine;

namespace InverseKinematics
{
    public class SpiderBehaviour : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform body;
        [SerializeField] private float speed;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float spiderHeight;
        
        [Header("Targets")]
        [SerializeField] private Transform leftFrontTarget;
        [SerializeField] private Transform leftBackTarget;
        [SerializeField] private Transform rightFrontTarget;
        [SerializeField] private Transform rightBackTarget;

        [Header("Target Points")]
        [SerializeField] private Transform leftFrontPoint;
        [SerializeField] private Transform leftBackPoint;
        [SerializeField] private Transform rightFrontPoint;
        [SerializeField] private Transform rightBackPoint;

        private Transform[] targetPoints;

        private void Start()
        {
            targetPoints = new[] {leftFrontPoint, rightFrontPoint, leftBackPoint, rightBackPoint};
        }

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            
            foreach (Transform target in targetPoints)
            {
                Vector3 targetPosition = target.position;
                targetPosition += direction * (speed * Time.deltaTime);
                targetPosition.y = body.position.y;
                
                bool result = Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer);
                if (result) target.position = hit.point;
            }

            Vector3 mediumFront = Vector3.Lerp(leftFrontTarget.position, rightFrontTarget.position, 0.5f);
            Vector3 mediumBack = Vector3.Lerp(leftBackTarget.position, rightBackTarget.position, 0.5f);
            Vector3 mediumPoint = Vector3.Lerp(mediumFront, mediumBack, 0.5f);
            
            mediumPoint.y += spiderHeight;
            body.position = mediumPoint;

            //float frontDifference = leftFrontTarget.position.y - rightFrontTarget.position.y;
            //float backDifference = leftBackTarget.position.y - rightBackTarget.position.y;
        }
    }
}