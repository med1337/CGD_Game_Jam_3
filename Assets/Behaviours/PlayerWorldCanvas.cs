using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWorldCanvas : MonoBehaviour
{
    [SerializeField] GameObject interact_prompt;


    public void SetInteractPromptActive(bool _active)
    {
        interact_prompt.SetActive(_active);
    }


}
