using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Attack : Attack_Type
{
    public Ball_Explosion myExplosion;
    public Ball_Explosion ballPrefab;
    public GameObject ballPoint;
    public GameObject overchargeVfx;

    public float rotateSpeed; // Qué tan rápido da vueltas en círculos
    public float radius = 3; // Distancia desde el jugador
    float currentAngle = 0;

    public override void CreateResource()
    {
        myExplosion = Instantiate(ballPrefab);
        myExplosion.myAttack = this;
        myExplosion.gameObject.SetActive(false);
    }

    public override void EndAttack()
    {
        myAttack.myCube.gameObject.SetActive(true);
        ballPoint.SetActive(false);
        currentAngle = 0;
    }

    public override void EnteringMode()
    {
        if(myExplosion == null) CreateResource();
        if (myAttack.damageUpgrade) overchargeVfx.SetActive(true);
        myAttack.myCube.gameObject.SetActive(false);
        ballPoint.SetActive(true);
    }

    public override void Interrupt()
    {

    }

    public override void PrimaryAttack()
    {
        if(myExplosion == null) CreateResource();
        myExplosion.transform.position = ballPoint.transform.position;
        myExplosion.gameObject.SetActive(true);
        isAttacking = true;
    }

    public override void SecondaryAttack()
    {

    }

    public override void Setup()
    {

    } 
    void Awake()
    {
        ballPoint.SetActive(false);
    }

    void LateUpdate() // Mantenemos Update para la suavidad con Interpolate
    {
        if (myAttack.currentAttack != this) return;

        // 1. El tiempo corre siempre igual
        currentAngle += rotateSpeed * Time.deltaTime;

        // 2. Calculamos dónde queremos estar en el MUNDO (World Space)
        // Usamos Vector3.right y up globales, ignorando la rotación del padre.
        // Esto crea un círculo perfecto alineado con la pantalla, no con el personaje.
        float x = Mathf.Cos(currentAngle) * radius;
        float y = Mathf.Sin(currentAngle) * radius;

        Vector3 targetWorldPosition = player.transform.position + new Vector3(x, y / 2, 0);
        // 3. LA MAGIA: Convertimos ese punto Mundial a Local
        // Esta función calcula automáticamente si tiene que invertir X, Y, o rotar
        // basándose en cómo está el padre en este exacto frame.
        transform.localPosition = player.transform.InverseTransformPoint(targetWorldPosition);

        // 4. Mantenemos la rotación en cero para que el sprite no gire
        transform.rotation = Quaternion.identity;
    }
}
