using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SizeInput : MonoBehaviour
{
    public TMP_InputField Input;
    public TextMeshProUGUI DimensionText;

    [Header("Events")]
    public GameEvent OnSetSize;

    private void Awake() 
    {
        SetSize();
    }

    public void SetSize()
    {
        var size = string.IsNullOrEmpty(Input.text) ? 3 : int.Parse(Input.text);
        Input.text = size.ToString();
        DimensionText.text = $"{size} * {size}";
        OnSetSize.Raise(this, size);
    }
}
