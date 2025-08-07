using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      float horizontal = Input.GetAxis("Horizontal"); 
      float vertical = Input.GetAxis("Vertical");     
      Vector3 direccion = new Vector3(horizontal, 0f, vertical);
      transform.Translate(direccion * velocidad * Time.deltaTime);
    }
}
