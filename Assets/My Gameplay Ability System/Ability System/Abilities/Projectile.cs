using System.Collections;
using AbilitySystem;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour

{

    [SerializeField] private GameObject impactPrefab;

    [SerializeField]
    public AbilitySystemCharacter Source;

    [SerializeField]
    public AbilitySystemCharacter Target;

    public GameplayEffectSpec Spec;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector3 rotationVector;

    [SerializeField]
    private float lifetime;

    private Collider _collider;

    private Rigidbody _rb;

    private bool hasCollided;

    private void Awake()
    {
        this._collider = GetComponent<Collider>();
        this._rb = GetComponent<Rigidbody>();
    }

    public IEnumerator Spawn()
    {
        yield break;
    }

    public IEnumerator TravelForward(Vector3 forward)
    {

        float time = 0;
        while (time < lifetime && !hasCollided)
        {
            time += Time.deltaTime;
            var direction = forward.normalized;
            Quaternion deltaRotation = Quaternion.Euler(rotationVector * Time.fixedDeltaTime);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
            _rb.MovePosition(transform.position + transform.forward.normalized * Time.fixedDeltaTime * this.speed);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
        if (other.tag == "Enemy")
        {
            this.Target = other.GetComponent<AbilitySystemCharacter>();
        }
    }

    public IEnumerator Despawn()
    {
        if (Target != null) Target.ApplyGameplayEffectSpecToSelf(Spec);
        Instantiate(this.impactPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
        yield break;
    }

}
