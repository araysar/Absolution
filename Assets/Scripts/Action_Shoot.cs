using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Shoot : MonoBehaviour
{
    [SerializeField] private GameObject myChakram;
    public bool canShoot = true;
    [HideInInspector] public bool isShooting = false;

    private void Start()
    {
        if (myChakram == null) myChakram = GameObject.Find("ChakramPrefab");
        myChakram.transform.parent = null;
        myChakram.GetComponent<Chakram>().myShooter = this;
        myChakram.SetActive(false);
    }


    public void Shoot(Transform shootingPoint)
    {
        if (!canShoot ) return;
        
        if(!isShooting)
        {
            isShooting = true;
            myChakram.gameObject.SetActive(true);
        }
    }
}
