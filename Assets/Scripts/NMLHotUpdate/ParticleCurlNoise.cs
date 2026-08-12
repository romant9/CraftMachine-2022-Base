using UnityEngine;

[ExecuteInEditMode]
public class ParticleCurlNoise : MonoBehaviour
{
	public enum curlType
	{
		Position = 0,
		Velocity = 1,
		Acceleration = 2
	}

	public Texture2D curlTexture;

	public curlType method = curlType.Velocity;

	public float curlAmount = 4f;

	public float uvMul = 0.2f;

	public float dampening;

	public Vector2 UvOffsetVelocity = new Vector2(0f, 0f);

	private ParticleSystem psys;

	private ParticleSystem.Particle[] particles;

	private void Start()
	{
		psys = GetComponent<ParticleSystem>();
		particles = new ParticleSystem.Particle[psys.main.maxParticles];
	}

	private void Update()
	{
		int num = psys.GetParticles(particles);
		int i = 0;
		float num2 = 1f - dampening * Time.deltaTime;
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		for (; i < num; i++)
		{
			Color pixelBilinear = curlTexture.GetPixelBilinear(uvMul * particles[i].position.x + UvOffsetVelocity.x * Time.time, uvMul * particles[i].position.y + UvOffsetVelocity.y * Time.time);
			Color pixelBilinear2 = curlTexture.GetPixelBilinear(uvMul * particles[i].position.x + UvOffsetVelocity.x * Time.time, uvMul * particles[i].position.z + UvOffsetVelocity.y * Time.time);
			if (method == curlType.Position)
			{
				vector.x = Time.deltaTime * curlAmount * (pixelBilinear.r * 2f - 1f);
				vector.y = Time.deltaTime * curlAmount * (pixelBilinear2.b * 2f - 1f);
				vector.z = Time.deltaTime * curlAmount * (pixelBilinear.b * 2f - 1f);
				vector2.x = particles[i].position.x + vector.x;
				vector2.y = particles[i].position.y + vector.y;
				vector2.z = particles[i].position.z + vector.z;
				particles[i].position = vector2;
			}
			if (method == curlType.Velocity)
			{
				vector.x = Time.deltaTime * curlAmount * (pixelBilinear.r * 2f - 1f);
				vector.y = Time.deltaTime * curlAmount * (pixelBilinear2.b * 2f - 1f);
				vector.z = Time.deltaTime * curlAmount * (pixelBilinear.b * 2f - 1f);
				vector2.x = (particles[i].velocity.x + vector.x) * num2;
				vector2.y = (particles[i].velocity.y + vector.y) * num2;
				vector2.z = (particles[i].velocity.z + vector.z) * num2;
				particles[i].velocity = vector2;
			}
		}
		psys.SetParticles(particles, num);
	}
}
