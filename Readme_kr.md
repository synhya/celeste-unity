# 구현 세부사항

Celeste라는 게임을 직접 플레이하고 최대한 비슷하게 구현하는데 초점을 맞추었습니다.

## 메뉴

기존 게임에서는 메뉴화면에 로우폴리로 만들어진 산과 주변 풍경들이 배치되는데     
블랜더로 모델을 직접 만들기에는 시간이 걸릴 것 같아서      
절차적 생성방식을 활용해 랜덤으로 산지형을 생성했습니다.     

우선 xy너비값을 받아서 해당 지역내부에 우선 골고루 분포된 점들을 생성합니다.    
단순히 랜덤값을 받아오면 점들이 밀집된 지역들이 생성되 지형이 자연스럽지     
않았기 떄문에 푸아송 디스크 샘플링으로 점들의 xz위치를 정했습니다.   

이후 점들을 자연스럽게 연결하기 위해 들로네 삼각분할 기법을 이용해     
매쉬의 삼각형 인덱스 정보들을 채우고 각 점들의 높이는 페럴린 노이즈를 샘플링해 구했습니다.     

[Triangle.Net](https://github.com/garykac/triangle.net)라이브러리와 해당 [영상](https://www.youtube.com/watch?v=sRn8TL3EKDU)의 도움을 받았습니다.    
화면전환은 쉐이더와 풀스크린 패스를 활용해서 구현했습니다. 전환 쉐이더는 [GLTransition 사이트](https://gl-transitions.com/)를 참고했습니다.

## 이펙트
### 대쉬시에 후드 색상 변경
원작 게임에서 플레이어의 남은 대시 수에 따라서 머리의 색이 바뀝니다.   
해당 기능을 구현하기에 가장 단순한 방법은 옷 색상을 파랗게 칠한 캐릭터를 새로 하나 더  
만드는 것인데, 플레이어 애니메이션 스프라이트가 100개가까이 되므로 현실적인 어려움이 있었습니다.

픽셀 캐릭터들은 사용하는 색상의 수가 많지 않기 때문에     
룩업 테이블을 사용하기로 했습니다.  
원리는 1차 텍스쳐의 rgb중 r과 g의 값을     
2차 텍스쳐(룩업 테이블)의 uv값으로 사용하는 것입니다.    
해당 방식을 적용하기 위해서 우선은 기존의 애니메이션 스프라이트를      
1차와 2차 텍스쳐로 나눠주는 툴을 제작하고 알맞게 쉐이더를 수정해 준뒤      
런타임에서 2차 텍스쳐에서 옷부분과 머리부분의 색상을 변경하여 구현했습니다.     

<img src="https://github.com/wkd2314/ForPortfolio/assets/25860861/85437d03-6eba-46d7-8720-24e822ba078c.png"  width="300" height="400"/>
<img src="https://github.com/wkd2314/ForPortfolio/assets/25860861/a64fcda2-115a-4f84-9b48-23b2a1196b44.png"  width="300" height="300"/>
<img src="https://github.com/wkd2314/ForPortfolio/assets/25860861/3a52b387-de50-4357-82f2-2b9a453c0d8f.png"  width="300" height="300"/>



### 더스트, 대시라인
플레이어가 점프를 하거나 앉을때 하얀 먼지 이펙트가 있는데          
원작 게임에서는 픽셀 그리드에 딱 맞춰서 해당 효과가 연출됩니다.     
파티클 시스템을 이용하면 비슷하게는 나오지만     
완전히 그리드에 맞춰서 생성하려면 모든 입자를 매프레임마다      
위치조정하는 비효율적인 방식말고는 좋은 방법을 찾지 못했습니다.      
따라서 Compute Shader와 인스턴싱을 이용해 구현하였습니다.      
대시 라인은 플레이어가 대시시에 뒤에 나오는 1픽셀 실선인데 해당효과도 이와 비슷하게        
구현했습니다.     

    // DustCompute
    uint2 to2D(float id)
    {
        return uint2(id % _Rect.z, id / _Rect.z);
    }
    
    [numthreads(256,1,1)]
    void CSDust (uint3 id : SV_DispatchThreadID)
    {
        float2 posOS = float2(to2D(id.x));
        float extentX = _Rect.z * 0.5;
        float passedTime = _TotalTime - _LeftTime;
        
        float clip = _RandBuffer[id.x]
            - smoothstep(0, _TotalTime, passedTime)
            - smoothstep(0, _Rect.w , posOS.y * (1.0 - _BoxierValue.y))
            - smoothstep(0, extentX, abs(posOS.x - extentX) * (1.0 - _BoxierValue.x));
        
        float2 dirOffset = round(_Dir * passedTime);
        
        float2 posWS = posOS + _Rect.xy + 0.5f + dirOffset;
        
        _Instances[id.x] = float3(posWS, clip); // 그리드 맞추기위해 +0.5f 
    }

    // Dust 클래스 
    private void Update()
    {
        if (aliveTimer > 0f)
        {
            dustCompute.SetFloat("_LeftTime", aliveTimer);
            dustCompute.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);
            
            Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, bounds, argsBuffer);
            aliveTimer -= Time.deltaTime;
        } 
        else if (didBurst)
        {
            didBurst = false;
            pool?.Release(this);
        }
    }

### 사망시 서클
플레이어가 죽으면 8개 원으로 흩어져 돌다 사라지는데 이러한 이펙트나 앞서 소개한     
이펙트들은 상당히 자주 생성되고 사라지니 풀링 시스템을 활용했습니다.       
게임 오브젝트 단위로 풀링하면 사용할 때마다 GetComponent를 매번 해줘야 하기 때문에        
컴포넌트 단위로 작성했습니다.      

    public class ComponentPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        ...
    }

## 물리
유니티 내장 콜라이더2D를 사용할 수도 있었지만 셀레스테 개발자가 친절하게      
작성한 자체 물리시스템에 대한 [설명](https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5)이 있어서 해당 방식을 그대로 사용하기로 했습니다.    
기본적인 로직은 다음과 같습니다.        
- 모든 씬의 객체들은 xy축과 평행한 직사각형의 히트박스를 갖고 있으며 정수단위로만 움직입니다.     
- 객체들은 Solid와 Actor의 두가지 클래스로 나누어져 있으며 Actor는 Solid와 절대 곂치지 않습니다.    
  - Actor가 한칸씩 움직일 때마다 Solid와의 박스가 곂치는지 확인하고 곂치면 이동하지 않습니다.     
  - 이동을 하는 Solid의 경우 Actor를 밀쳐내며 이때 Actor가 두 Solid사이에 낑기면 파괴합니다.    
  - Actor가 움직이는 Solid를 타고있는경우 해당 Solid를 따라 이동합니다.      

Actor가 이동할 때마다 레벨에 존재하는 모든 Solid와    
충돌을 확인하면 비효율적이므로 방별로 Solid의 리스트를 담아서      
해당 방의 객체들간에만 충돌을 체크하도록 했습니다.    

### Unity Tilemap
타일의 경우 RuleTile을 상속받기 때문에 MonoBehavior를      
상속받는 Solid를 그대로 상속시킬수 없었습니다.       
따라서 Hitbox(AABB)와 enum flag를 추가하고       
플레이어 Hitbox 근처에 있는 타일들만 좌표를 가져와서    
충돌체크를 할 수 있도록 작성했습니다.  
원웨이 플랫폼의 경우 올라갈때는 충돌을 무시하도록 했습니다.  
<img src="https://github.com/wkd2314/ForPortfolio/assets/25860861/c42dbdf5-face-4534-973c-c9d6bcee9799.png"  width="300" height="300"/>

    protected bool OverlapTileFlagCheckOS(TileType flag, Vector2Int offset, out TileType type)
    {
        var was = PositionWS;
        PositionWS += offset;
        
        var min = GridPosMin;
        var max = GridPosMax;
        
        var tileRect = new RectInt();
        var pos = new Vector3Int();

        var ret = false;
        type = TileType.None;
        
        for(int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                pos.x = i;
                pos.y = j;
                var tile = tileMap.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type.HasFlag(flag))
                {
                    tileRect = tile.AABB;
                    var tilePosWS = tileMap.GetCellCenterWorld(pos) 
                                    - Vector3.one * TileSize / 2;
                    tileRect.position += Vector2Int.RoundToInt(tilePosWS);

                    if (HitBoxWS.Overlaps(tileRect) && 
                        (flag != TileType.Ground || !tile.Type.HasFlag(TileType.HalfGround) ||
                         PositionWS.y - offset.y >= tilePosWS.y + TileSize))
                    {
                        // special case, if entity position.y + offset.y < tile center.y + Tilesize/2
                        // player's previous position is below tile so pass
                        
                        type = tile.Type;
                        ret = true;
                    }
                }
            }
        }
        PositionWS = was;

        return ret;
    }

## 플레이어
플레이어는 State Machine에 따라 움직입니다. 상태머신은 플레이어의 상태에 맞추어
알맞은 Action을 실행합니다.

    private void OnStateChange(int newState) 
    {
        if(currentStateIdx == newState) return;

        if (stateDict.TryGetValue(newState, out var stateActions))
        {
            if (stateDict.TryGetValue(currentStateIdx, out var prevActions))
            {
                prevActions.end?.Invoke();
            }
            
            stateActions.begin?.Invoke();
            currentUpdateAction = stateActions.update;
        }
    }

각각의 State마다 클래스를 생성할 수도 있었지만 플레이어 스크립트 내부에 Callback형식을   
넣는것이 보기가 깔끔해서 해당 방식을 채택 했습니다.

    // in player class
    public const int StateNormal = 0;
    public const int StateDash = 1;
    
    protected override void Start()
    {
        base.Start();
        
        sm = new StateMachine(3);
        sm.SetCallbacks(StateNormal, NormalUpdate, NormalBegin, null);
        sm.SetCallbacks(StateDash, DashUpdate, DashBegin, DashEnd);
        ...
        sm.State = StateNormal;
    }

## 조작감 개선
플랫포머 게임에서는 많이 채택하고 있는 부분인 Jump buffering, Jump GraceTime을 포함하여
Celeste 개발자가 소개한 몇가지 [특징](https://maddythorson.medium.com/celeste-forgiveness-31e4a40399f1)들을 그대로 구현하였습니다.       

Corner Correction은 구현 예정에 있습니다. 현재 충돌 체크 함수는 bool값만 반환하지만  
해당 기능을 구현하기 위해서는 충돌한 범위와 센터값을 알아야 하기 때문에   
추가적인 정보를 out Attribute로 가져오게 할 예정입니다.
