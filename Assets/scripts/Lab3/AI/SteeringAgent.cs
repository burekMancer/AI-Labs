using System;
using System.Collections.Generic;
using UnityEngine;



    public class SteeringAgent : MonoBehaviour
    {
        [Header("Movement")] public float maxSpeed = 5f;
        public float maxForce = 10f; 

        [Header("Arrive")] public float slowingRadius = 3f;

        [Header("Separation")] public float separationRadius = 1.5f;
        public float separationStrength = 5f;

        [Header("Weights")] public float arriveWeight = 1f;
        public float separationWeight = 1f;

        [Header("Debug")] public bool drawDebug = true;

        private Vector3 velocity = Vector3.zero;

        public Transform target;

        public static List<SteeringAgent> allAgents = new List<SteeringAgent>();

        private void OnEnable()
        {
            allAgents.Add(this);
        }

        private void OnDisable()
        {
            allAgents.Remove(this);
        }

        void Start()
        {
            target = GameObject.Find("Target").transform;
        }

        void Update()
        {
            Vector3 totalSteerng = Vector3.zero;

            if (target != null)
            {
                totalSteerng += Arrive(target.position, slowingRadius) * arriveWeight;
            }

            if (allAgents.Count > 1)
            {
                totalSteerng += Separation(separationRadius, separationStrength) * separationWeight;
            }

            
            totalSteerng = Vector3.ClampMagnitude(totalSteerng, maxSpeed);

          
            velocity += totalSteerng * Time.deltaTime;

           
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

       
            transform.position += velocity * Time.deltaTime;

           
            if (velocity.sqrMagnitude > 0.001f)
            {
                transform.forward = velocity.normalized;
            }
        }
        public Vector3 Seek(Vector3 targetPos)
        {
            Vector3 toTarget = targetPos - transform.position;
            if (toTarget.sqrMagnitude < 0.0001f)
                return Vector3.zero;
            Vector3 desired = toTarget.normalized * maxSpeed;

            return desired - velocity;
        }

        public Vector3 Arrive(Vector3 targetPos, float slowRadius)
        {



            Vector3 toTarget = targetPos - transform.position;
            float distance = toTarget.magnitude;


            if (distance < 0.0001f)
            {
                return Vector3.zero;
            }

            float desiredSpeed = maxSpeed;

            if (distance < slowRadius)
            {
                desiredSpeed = maxSpeed * (distance / slowRadius);
            }

            Vector3 desiredVelocity = toTarget.normalized * desiredSpeed;
            return desiredVelocity - velocity;
        }

        public Vector3 Separation(float radius, float strength)
        {
            Vector3 force = Vector3.zero;
            int neighbourCount = 0;
            foreach (SteeringAgent other in allAgents)
            {
                if (other == this)
                {
                    continue;
                }

                Vector3 toMe = transform.position - other.transform.position;
                float distance = toMe.magnitude;


                if (distance > 0f && distance < radius)
                {

                    force += toMe.normalized / distance;
                    neighbourCount++;
                }
            }

            if (neighbourCount > 0)
            {
                force /= neighbourCount;


                force = force.normalized * maxSpeed;
                force -= velocity;
                force *= strength;
            }

            return force;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawDebug)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + velocity);
        }
    }



