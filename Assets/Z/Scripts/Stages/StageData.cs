using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
[CreateAssetMenu(fileName = "New Stage Data", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
   [SerializeField] public AudioClip song;
   [SerializeField] public float duration;
   [SerializeField] public int star1Score;
   [SerializeField] public int star2Score;
   [SerializeField] public int star3Score;
   [SerializeField] public AnimatorController stagePattern;
   
   
   
}
