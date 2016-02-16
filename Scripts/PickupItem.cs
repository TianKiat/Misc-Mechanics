using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class PickupItem : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other)
    {
        PickItem();
    }

    private void PickItem()
    {
        //do things that you want to do when when the player picks up this item
        audioSource.Play();
        //destroy the gameobject once sound is done playing    
        Destroy(gameObject, .19f);
    }
    
    void OnDestroy()
    {
        audioSource = null;
    }
}
