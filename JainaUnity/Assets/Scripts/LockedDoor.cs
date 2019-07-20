using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    Animator anim;

    public void OnLockingDoor(bool b)
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Locked", b);
    }
}
