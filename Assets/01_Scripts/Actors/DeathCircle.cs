
using DG.Tweening;
using UnityEngine;

public class DeathCircle : MonoBehaviour
{
    [Header("Spread Settings")]
    [SerializeField] private float targetScale = 0.53f;
    [SerializeField] private float targetMoveSpeed = 3f;
    [SerializeField] private float moveTime = 2f;

    [Header("Spin Settings")]
    [SerializeField] private float spinTime = 3f;
    [SerializeField] private float angleSpeed = 120f;

    [Header("Shrink Settings")]
    [SerializeField][Range(0, 1)] private float shrinkRatio = 0.6f;

    private Vector3 pivotPos;
    private float timer = -99f;
    private SpriteRenderer sr;

    private Color lerpColor1;
    private Color lerpColor2;

    private bool didStartShrink;
    
    public void Init(Vector2 moveDir, Color color1, Color color2)
    {
        // spawned at player position
        sr = GetComponent<SpriteRenderer>();
        lerpColor1 = color1;
        lerpColor2 = color2;
        
        pivotPos = transform.position;
        var t = transform;
        
        // play anim and set scale
        var seq = DOTween.Sequence();

        seq.Append(t.DOScale(targetScale, moveTime))
            .Join(t.DOMove(t.position + (Vector3)moveDir * (targetMoveSpeed * moveTime), moveTime))
            .InsertCallback(moveTime * 0.4f,
                () => timer = spinTime);
    }

    private void Update()
    {
        if (timer > 0f)
        {
            var dt = Time.deltaTime;
            timer -= dt;
            transform.RotateAround(pivotPos, new Vector3(0, 0, -1), angleSpeed * dt);
        }  
        if (!didStartShrink && timer > -1f &&  timer <= spinTime * shrinkRatio)
        {
            // shrink scale
            didStartShrink = true;
            transform.DOScale(Vector3.zero, spinTime * shrinkRatio).SetEase(Ease.InCirc).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        
        // lerp color
        sr.color = Color.Lerp(lerpColor1, lerpColor2, 
            Mathf.PingPong(Time.time * 2f, 1));
    }
}


