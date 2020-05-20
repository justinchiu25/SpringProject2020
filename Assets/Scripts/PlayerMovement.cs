using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Transform movePoint;
    public GameObject exit;
    public LayerMask obstacle;
    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f),
                    .2f, obstacle))
                {
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                }
            }
            if (Mathf.Abs(Input.GetAxis("Vertical")) == 1f)
            {
                movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }
        }
    }
    //reset map
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Exit") || (col.gameObject.CompareTag("Enemy")))
        {
            transform.position = new Vector3(0, 0, 0f);
            movePoint.transform.position = new Vector3(0, 0, 0f);
            Debug.Log("trigger");

            SceneManager.LoadScene(0);

        }
        
    }
    
    
}
