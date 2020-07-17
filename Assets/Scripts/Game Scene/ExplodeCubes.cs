using UnityEngine;
using Random = UnityEngine.Random;

public class ExplodeCubes : MonoBehaviour
{
    public GameObject restartButton;
    [Range(0f, 10000f)] public float force = 70f;
    public Animator animator;
    public AudioSource breakSource;
    public bool checking;
    public bool collided;
    private bool _collisionSet;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cube") && !_collisionSet && !checking)
        {
            Explode(collision.transform, force);
        }

        if (checking)
        {
            collided = true;
        }
    }

    public void Explode(Transform obj, float needForce)
    {
        float originalMass = obj.GetComponent<Rigidbody2D>().mass;

        int count = obj.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Transform child = obj.transform.GetChild(i);
            Rigidbody2D component = child.gameObject.AddComponent<Rigidbody2D>();
            component.mass = count - i;
            component.AddForce(
                new Vector2(Random.Range(needForce * originalMass * -1, needForce * originalMass),
                    Random.Range(0f, needForce * originalMass)),
                ForceMode2D.Force);
            component.gravityScale = 0.1f;
            child.SetParent(null);
        }

        breakSource.Play();
        Destroy(obj.gameObject);
        _collisionSet = true;
        restartButton.SetActive(true);
        restartButton.GetComponent<Animator>().SetTrigger("restartShow");
        if (animator != null) animator.SetTrigger("restartShow");
    }

    public bool CollisionSet => _collisionSet;
}