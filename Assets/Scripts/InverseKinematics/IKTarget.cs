using System;
using System.Collections.Generic;
using UnityEngine;

namespace InverseKinematics
{
    [ExecuteInEditMode]
    public class IKTarget : MonoBehaviour
    {
        [SerializeField] private Transform pole;
        [SerializeField] private Transform root;
        [SerializeField] private Transform effector;
        [SerializeField, Min(1)] private int iterations;
        
        [SerializeField] private List<Transform> joints = new List<Transform>();
        private int steps = 2;
        
        private void Awake()
        {
            joints = new List<Transform>();
            PopulateJoints(effector, joints, root);
        }
        
        private void LateUpdate()
        {
            CalculateIK(transform.position, pole.position);
        }
        
        private static void PopulateJoints(Transform child, List<Transform> jointsList, Transform root = null)
        {
            jointsList.Add(child);
            Transform parent = child.parent;

            if (parent == root)
            {
                jointsList.Add(parent);
                return;
            }
            
            if(parent != null)
                PopulateJoints(parent, jointsList, root);
        }
        
        private void CalculateIK(Vector3 target, Vector3 polePosition)
        {
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int jointIndex = 1; jointIndex < joints.Count; jointIndex++)
                {
                    Transform joint = joints[jointIndex];
                    
                    Vector3 jointPosition = joint.position;
                    Vector3 effectorPosition = effector.position;
                    Vector3 vectorToEffector = effectorPosition - jointPosition;
                    Vector3 directionToTarget = target - jointPosition;
            
                    Quaternion rotationOffset = Quaternion.FromToRotation(vectorToEffector, directionToTarget);
                    Quaternion newRotation = rotationOffset * joint.rotation;

                    if (jointIndex > 1)
                    {
                        Transform followingJoint = joints[jointIndex - 2];
                        
                        Vector3 nextJoint = joints[jointIndex - 1].position;
                        Vector3 followingJointPosition = followingJoint.position;
                    
                        Vector3 normal = (followingJointPosition - jointPosition).normalized;
                        Vector3 projectedJoint = GetProjection(normal, jointPosition, nextJoint);
                        Vector3 projectedPole = GetProjection(normal, jointPosition, polePosition);
                    
                        float angle = GetAngleOnAxis(projectedJoint, projectedPole, normal);
                        Quaternion poleOffset = Quaternion.AngleAxis(angle, normal);
                        Quaternion oppositeOffset = Quaternion.AngleAxis(-angle, normal);
                    
                        newRotation = poleOffset * newRotation;
                        if(jointIndex != 2) followingJoint.rotation = oppositeOffset * followingJoint.rotation;
                    }
                    
                    joint.rotation = newRotation;
                }
            }
        }
        
        private static float GetAngleOnAxis(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {
            Vector3 cross = Vector3.Cross(vectorA, vectorB);
            
            float x = Vector3.Dot(vectorA, vectorB);
            float y = Vector3.Dot(cross, normal);
            
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            return angle;
        }

        private static Vector3 GetProjection(Vector3 normal, Vector3 origin, Vector3 point)
        {
            Vector3 originToPoint = point - origin;
            float distance = Vector3.Dot(originToPoint, normal);
            Vector3 projectedPoint = point - normal * distance;
            return projectedPoint - origin;
        }
    }
}