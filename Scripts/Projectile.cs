using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public Color trailColor;
    public LayerMask collisionMask;

    public bool isPenerating = false;

    public float damage = 1f;
    public float speed = 10f;

    float lifeTime = 3f;
    public float skinWidth = 0.01f;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0) // If collides with anything...
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

	void Update () 
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        if (c.GetComponent<IDamageable>() != null)
        {
            IDamageable damagableObject = c.GetComponent<IDamageable>();
            if (damagableObject != null)
            {
                damagableObject.TakeHit(damage, hitPoint, transform.forward);
            }
        }
        if (!isPenerating)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
