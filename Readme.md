# Portfolio
Platformer based on [Celeste](https://www.celestegame.com/) and [Celeste Origin](https://maddymakesgamesinc.itch.io/celesteclassic)  
Celeste Origin is playable in the linked website.

## Features

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
TypeTile class with AABB attached to handle collisions with actor.

---
### Player
Player moves based on state machine.  
I could implement every single state inherit from IState.  
But I wanted to keep every state in player class because   
it looked more intuitive.  
The player class was referenced by [this](https://github.com/NoelFB/Celeste/tree/master/Source/Player).

---
### Effects
Dust and Dash Line Effect is implemented   
using compute shader. Using Particle system was  
possible option but wasn't efficient   
as I had to set Every particle's position.

Death Effect is implemented with pooling.  
Pooling system is left be more optimized.

Lighting..

---
## Referenced Assets 

- [Red Hood](https://legnops.itch.io/red-hood-character)

[//]: # (- [Pixel Art GUI Elements]&#40;https://mounirtohami.itch.io/pixel-art-gui-elements&#41;)

[//]: # (- [fantasy icons pack]&#40;https://shikashipx.itch.io/shikashis-fantasy-icons-pack&#41;)

## Articles And Scripts
- [CelestePhysics](https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5)
- [CelestePlayer](https://github.com/NoelFB/Celeste/tree/master/Source/Player)
- [SpriteColorLUT](https://www.youtube.com/watch?v=HsOKwUwL1bE&t=1s)
- [Tiles](https://aran.ink/posts/celeste-tilesets)
- [PixelPerfectLine](https://www.youtube.com/watch?v=nlzvesTsSrI)
- [JumpGraceTime](http://kpulv.com/123/Platforming_Ledge_Forgiveness/)

[//]: # (- [Scroller]&#40;https://github.com/setchi/FancyScrollView&#41;)