using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuperDuperFunMovementScript : MonoBehaviour
{
    [SerializeField] CharacterState state;
    // Start is called before the first frame update
    void Start()
    {
        state.SetControlState(CHARACTER_CONTROL_STATE.CUTSCENE, 1); 
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        transform.position += new Vector3(xInput, 0, yInput)*0.1f;
    }
}
