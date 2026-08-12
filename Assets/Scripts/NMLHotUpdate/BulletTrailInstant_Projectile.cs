using UnityEngine;

[RequireComponent(typeof(BulletTrailInstant))]
public class BulletTrailInstant_Projectile : MonoBehaviour
{
	private BulletTrailInstant trail;

	private GameObject projectile;

	private GameObject projectileMesh;

	public float projectileWidth;

	public float projectileLength;

	public Material projectileMaterial;

	public GameObject projectileParticle;

	public GameObject projectileDestroyParticle;

	private float FlightTime;

	private Vector3 start;

	private Vector3 end;

	private float distance;

	private float startTime;

	private void Start()
	{
		trail = GetComponent<BulletTrailInstant>();
		start = trail.start;
		end = trail.end;
		distance = (end - start).magnitude;
		startTime = Time.time;
		FlightTime = distance / trail.flightSpeed;
		CreateProjectile();
	}

	private void CreateProjectile()
	{
		projectile = new GameObject();
		projectile.name = "projectile";
		projectile.transform.parent = base.transform;
		projectile.transform.position = start;
		if (projectileMaterial != null)
		{
			projectileMesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
			projectileMesh.name = "projectileMesh";
			projectileMesh.GetComponent<Renderer>().material = projectileMaterial;
			projectileMesh.transform.parent = projectile.transform;
			projectileMesh.transform.localPosition = Vector3.zero;
			projectileMesh.transform.localRotation = Quaternion.identity;
			projectileMesh.transform.localScale = new Vector3(projectileLength, projectileWidth, projectileWidth);
			projectileMesh.transform.Rotate(90f, 90f, 0f);
		}
		if (projectileParticle != null)
		{
			projectileParticle = Object.Instantiate(projectileParticle);
			projectileParticle.transform.parent = projectile.transform;
			projectileParticle.transform.localPosition = Vector3.zero;
			projectileParticle.transform.localScale = Vector3.one;
			projectileParticle.transform.localRotation = Quaternion.identity;
		}
		projectile.transform.LookAt(end);
	}

	private void Update()
	{
		float num = Time.time - startTime;
		float num2 = Mathf.Clamp01(num / FlightTime);
		if (projectile != null)
		{
			projectile.transform.localPosition = num2 * end + (1f - num2) * start;
		}
		if (num > FlightTime && projectile != null)
		{
			if (projectileParticle != null)
			{
				projectileParticle.transform.parent = null;
				projectileParticle.GetComponent<ParticleSystem>().Stop();
			}
			if (projectileDestroyParticle != null)
			{
				projectileDestroyParticle = Object.Instantiate(projectileDestroyParticle);
				projectileDestroyParticle.transform.position = projectile.transform.position;
				projectileDestroyParticle.transform.LookAt(start);
				projectileDestroyParticle.transform.Rotate(new Vector3(0f, 180f, 0f));
			}
			Object.Destroy(projectile);
		}
	}
}
