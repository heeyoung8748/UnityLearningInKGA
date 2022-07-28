using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class PlayerController : MonoBehaviour, ITargetable
{
    public void Damaged()
    {
        Debug.Log("공격을 받았다");
    }
    
}
