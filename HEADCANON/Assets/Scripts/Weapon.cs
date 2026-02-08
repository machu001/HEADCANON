using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    
    public Transform firepoint;
    public GameObject bulletPrefab;
    public GameObject player;
    private float fireCD = 0.6f;
    public float fireCooldown;

    [SerializeField] private AudioManager au;

    private void Awake()
    {
        au = FindAnyObjectByType<AudioManager>();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && fireCooldown <= 0)
        {
            if(player.GetComponent<PlayerMovement>().inputEnabled)
            {
                Shoot();
                fireCooldown = fireCD;
            }
            
        }
        fireCooldown -= 1 * Time.deltaTime;
        if (fireCooldown < 0) fireCooldown = 0; 

    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
        
        if (au)
        {
            au.Play("Shoot");
        }
    }
}
