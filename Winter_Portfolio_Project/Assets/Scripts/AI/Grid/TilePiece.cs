using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePiece : MonoBehaviour
{
    [SerializeField] bool isBlock;
    [SerializeField] bool isFastPath;

    public bool IsBlock { get { return isBlock; } }
    public bool IsFastPath { get { return isFastPath; } }
}
