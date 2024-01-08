using System;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


// TODO: 레벨에서 돌아오면 상태유지하게 DontDestroy추가.
[RequireComponent(typeof(AudioSource))]
public class MenuManager : MonoBehaviour
{
    // Cam settings 
    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera menuCam;
    [SerializeField] private CinemachineVirtualCamera levelSelectionCam;
    [SerializeField] private CinemachineVirtualCamera level1Cam;
    
    [SerializeField] private Transform mtPivot;
    [SerializeField] private float speed = 5f;
    
    private const float oppositeCenter = 322;
    private const float extent = 130;
    
    private const float leftMax = oppositeCenter + extent - 360;
    private const float rightMax = oppositeCenter - extent;
    private float curYRotation;
    
    private const KeyCode oKey = KeyCode.C;
    private const KeyCode xKey = KeyCode.X;

    private StateMachine sm;
    private const int StMenu = 0;
    private const int StLevelSelect = 1;
    private const int StForceRotation = 2;
    private const int StLevel = 3;
    
    private float selectDelayTimer;
    private const float SelectDelayTime = 0.4f;
    
    // UI settings
    [Header("UI Settings")]
    [SerializeField] private Color blink1 = new Color(1f, 1f, 0.54f);
    [SerializeField] private Color blink2 = new Color(0.75f, 1f, 0.6f);
    [SerializeField] private TMP_Text[] menuTexts;
    [SerializeField] private TMP_Text[] levelTexts;
    
    [SerializeField] private RectTransform menuUI;
    [SerializeField] private RectTransform levelSelectUI;
    [SerializeField] private RectTransform levelUI;
    [SerializeField] private float rectMoveAmount = 800f;
    [SerializeField] private float rectMoveDuration = 1f;
    
    private int activeTextIdx;

    [SerializeField] private Material transitionMat;
    private bool isOnSceneTransition = false;

    private AudioSource clickSource;
    private SoundDataManager Sound => SoundDataManager.I;
    
    private void Start()
    {
        sm = new StateMachine(3); 
        
        sm.SetCallbacks(StMenu, MenuUpdate, MenuBegin, MenuEnd);
        sm.SetCallbacks(StLevelSelect, LevelSelectUpdate, LevelSelectBegin, LevelSelectEnd);
        sm.SetCallbacks(StLevel, LevelUpdate, LevelBegin, LevelEnd);
        sm.SetCallbacks(StForceRotation, RotateUpdate, RotateBegin, null);
        
        var pos = menuUI.position;
        pos.x -= rectMoveAmount;
        menuUI.position = pos;

        pos = levelSelectUI.position;
        pos.y += rectMoveAmount;
        levelSelectUI.position = pos;

        pos = levelUI.position;
        pos.x += rectMoveAmount;
        levelUI.position = pos;

        clickSource = GetComponent<AudioSource>();
        transitionMat.SetFloat("_Progress", 0);
    }

    private void Update()
    {
        sm.Update();
    }

    private void RotateBegin()
    {
        var rotateYVal = curYRotation < oppositeCenter ? rightMax : leftMax;

        DOTween.Sequence()
            .Append(mtPivot.DORotate(Vector3.up * rotateYVal, 0.8f).SetEase(Ease.OutSine))
            .InsertCallback(0.6f, () =>
            {
                sm.State = StLevelSelect;
            });
    }

    private int RotateUpdate() => StForceRotation;

    #region State Menu

    private void MenuBegin()
    {
        menuCam.Priority = 1;

        activeTextIdx = 0;
        
        menuUI.DOMoveX(rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic);
    }

    private int MenuUpdate()
    {
        if (Input.GetKeyDown(oKey))
        {
            if (activeTextIdx == 0)
            {
                Sound.Play(clickSource, Sound.menuClickSndData);
                curYRotation = mtPivot.rotation.eulerAngles.y;
                if (curYRotation <= leftMax || curYRotation >= rightMax) // 항상 0 ~ 360반환하니까. 
                {
                    return StForceRotation;
                }

                return StLevelSelect;
            } 
            if (activeTextIdx == 1)
            {
                // show credit
            }
            else
            {
                Application.Quit();
            }
        }
        mtPivot.Rotate(0, speed * Time.deltaTime, 0);

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menuTexts[activeTextIdx].color = Color.white;
            activeTextIdx = Mathf.Min(++activeTextIdx, menuTexts.Length - 1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            menuTexts[activeTextIdx].color = Color.white;
            activeTextIdx = Mathf.Max(--activeTextIdx, 0);
        }
        
        menuTexts[activeTextIdx].color = Color.Lerp(blink1, blink2, Mathf.PingPong(Time.time * 4, 1));
        
        return StMenu;
    }
    
    private void MenuEnd()
    {
        menuCam.Priority = 0;

        // shake and hide.
        DOTween.Sequence()
            .Append(menuTexts[activeTextIdx].transform.DOShakePosition(0.4f, 40f))
            .Append(menuUI.DOMoveX(-rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic));
    }

    #endregion

    #region State Level Select

    private void LevelSelectBegin()
    {
        levelSelectionCam.Priority = 1;
        selectDelayTimer = SelectDelayTime;
        
        levelSelectUI.DOMoveY(-rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic);
    }
    
    private int LevelSelectUpdate()
    {
        if (selectDelayTimer > 0f) selectDelayTimer -= Time.deltaTime;


        if (Input.GetKeyDown(oKey) && selectDelayTimer < 0f)
        {
            Sound.Play(clickSource, Sound.menuClickSndData);
            return StLevel;
        }
        
        else if (Input.GetKeyDown(xKey))
            return StMenu;
    
        
        return StLevelSelect;
    }

    private void LevelSelectEnd()
    {
        levelSelectionCam.Priority = 0;
        
        levelSelectUI.DOMoveY(rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic);
    }

    #endregion

    #region State Level

    private void LevelBegin()
    {
        level1Cam.Priority = 1;
        
        activeTextIdx = 0;
        
        levelUI.DOMoveX(-rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic);
    }

    private int LevelUpdate()
    {
        if (isOnSceneTransition) return StLevel;
        
        levelTexts[activeTextIdx].color = Color.Lerp(blink1, blink2, Mathf.PingPong(Time.time * 4, 1));
        
        if (Input.GetKeyDown(oKey))
        {
            // start transition
            isOnSceneTransition = true;
            transitionMat.SetVector("_CenterScreenPos", new Vector2(1920 / 2, 1080 / 2));
            
            Sound.Play(clickSource, Sound.menuClickSndData);

            DOTween.Sequence()
                .Append(levelUI.DOMoveX(rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic))
                .Join(transitionMat.DOFloat(1, "_Progress", 1))
                .OnComplete(() => SceneManager.LoadScene(1));
        }
            
        else if (Input.GetKeyDown(xKey))
        {
            return StLevelSelect;
        }

        return StLevel;
    }

    private void LevelEnd()
    {
        level1Cam.Priority = 0;
        
        levelUI.DOMoveX(rectMoveAmount, rectMoveDuration).SetRelative().SetEase(Ease.OutCubic);
    }

    #endregion
} 
