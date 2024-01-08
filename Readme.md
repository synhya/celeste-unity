# Portfolio
Platformer based on [Celeste](https://www.celestegame.com/) and [Celeste Origin](https://maddymakesgamesinc.itch.io/celesteclassic)  
Celeste Origin is playable in the linked website.

[Game Play Video](https://www.youtube.com/watch?v=A_KDR0tZdBA)
[한국어버전 상세설명](https://github.com/wkd2314/ForPortfolio/blob/master/Readme_kr.md)

## Features

### Menu

Procedural Terrain Generation was used in menu. [Triangle.net](https://github.com/garykac/triangle.net) library helped a lot.   
The idea basically came from [here](https://github.com/KristinLague/Low-Poly-Terrain-Generator).

### Effects
Hood Color Change on dash is implemented using lookup table. It was inspired by this [devlog](https://www.youtube.com/watch?v=HsOKwUwL1bE&t=1s).  
I first made tools to convert sprite texture to lookup texture and table texture then used it in shader.

Dust(when player jump and land) and Dash Line Effect is implemented   
using compute shader. Using Particle system was  
possible option but wasn't efficient   
as I had to set Every particle's position with cpu each frame.

Death Effect is implemented with pooling because  
I had to create 8 game objects for circles.  
Pooling system is left be more optimized.   
Death Circles would be more pretty if they behave like [metaball](https://www.shadertoy.com/view/wd3SzS).

---
### Physics
Collision is not implemented using unity built-in components.  
It is because I wanted to implement the similar   
collision system as the original game. 

The Physics(collision) system is based on three  
types of entities. Solid, Actor, and Trigger.  
Specifics can be found [here](https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5).

For entities that are collide-able and don't block actor,  
I added Trigger class up on original system.  
(for example, strawberry and dash orb inherit this class)

#### TileCollision
Tiles are solids but they can't inherit solid class because they  
inherit from RuleTile class(In UnityEngine). So I created  
TypeTile class that inherits RuleTile with AABB attached to handle collisions with actor.  


---
### Player
Player moves based on state machine.  
I could implement every single state inherit from IState.  
But I wanted to keep every state in player class because it looked more intuitive.  
For example for [dash state](), I declared DashBegin(), DashUpdate(), DashEnd() in
player script and linked as callback from state machine.

The player class was referenced by [this](https://github.com/NoelFB/Celeste/tree/master/Source/Player).

There are only two states currently but can easily be expended.

---
### User Forgiveness

Got idea mainly from [UserForgiveness](https://maddythorson.medium.com/celeste-forgiveness-31e4a40399f1)  
coyote time, jump buffering, lift momentum storage implemented

dashing wall jump and corner correction is yet to be implemented.


## Asset Credit

- [Red Hood](https://legnops.itch.io/red-hood-character)
- [minifantasy-dungeon-sfx](https://leohpaz.itch.io/minifantasy-dungeon-sfx-pack)

## Articles And Scripts
- [CelestePhysics](https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5)
- [CelestePlayer](https://github.com/NoelFB/Celeste/tree/master/Source/Player)
- [SpriteColorLookUpTable](https://www.youtube.com/watch?v=HsOKwUwL1bE&t=1s)
- [Tiles](https://aran.ink/posts/celeste-tilesets)
- [PixelPerfectLine](https://www.youtube.com/watch?v=nlzvesTsSrI)
- [PlatformerForgiveness](http://kpulv.com/123/Platforming_Ledge_Forgiveness/)
- [CelesteUserForgiveness](https://maddythorson.medium.com/celeste-forgiveness-31e4a40399f1)
- [ProceduralTerrainGeneration](https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3)
- [ShaderTransitions](https://gl-transitions.com/)

[//]: # (- [Scroller]&#40;https://github.com/setchi/FancyScrollView&#41;)
