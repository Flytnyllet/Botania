using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShaderVariableInitiator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Shader.SetGlobalFloat("gWindSpeed", 1);
    }
}
