using UnityEngine;

public class Hades_Transition : MonoBehaviour
{
    [SerializeField] private Material material;
    public float targetValue = 1f;

    private float maskAmount;

    private void Update()
    {
        var maskAmountChange = targetValue > maskAmount ? +.1f : -.1f;
        maskAmount += maskAmountChange * Time.deltaTime * 6f;
        maskAmount = Mathf.Clamp01(maskAmount);
        material.SetFloat("_MaskAmount", maskAmount);
    }
}