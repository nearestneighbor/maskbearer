using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlash : MonoBehaviour
{
    private bool _flashing;
    public bool Flashing { get => _flashing; }

    private float _flashTime;
    private float _startFlashAmount;
    private float _endFlashAmount;
    private bool _fadingUp;
    private bool _fading;

    public SpriteRenderer spriteRend;

    private Material _material;

    private void Awake()
    {
        spriteRend ??= GetComponentInChildren<SpriteRenderer>(true);
        _material = spriteRend.material;
    }

    private void Update()
    {
        if (!_fading) return;

        float currentFlashAmount = _material.GetFloat("_FlashAmount");
        _material.SetFloat("_FlashAmount", currentFlashAmount - (_startFlashAmount - _endFlashAmount) * (1f / _flashTime) * Time.deltaTime);
        _flashing = _fading = (_fadingUp ? 
            currentFlashAmount <= _endFlashAmount :
            currentFlashAmount >= _endFlashAmount);
    }

    public void Flash(float flashTime, float flashUpTime = 1, Color? flashColor = null, float startFlashAmount = 1, float endFlashAmount = 0)
    {
        flashColor ??= Color.white;
        _material.SetColor("_FlashColor", flashColor.Value);
        _material.SetFloat("_FlashAmount", startFlashAmount);
        _startFlashAmount = startFlashAmount;
        _endFlashAmount = endFlashAmount;
        _flashTime = flashTime;
        _fadingUp = endFlashAmount > startFlashAmount;
        _flashing = true;
        StartCoroutine(DoFlash());

        IEnumerator DoFlash()
        {
            yield return new WaitForSeconds(flashUpTime);

            _fading = true;
        }
        
        
        
    }
}
