//
// public class Strawberry : Trigger
// {
// // //Triggers
// // if (StateMachine.State != StReflectionFall)
// // {
// //     foreach (Trigger trigger in Scene.Tracker.GetEntities<Trigger>())
// //     {
// //         if (CollideCheck(trigger))
// //         {
// //             if (!trigger.Triggered)
// //             {
// //                 trigger.Triggered = true;
// //                 triggersInside.Add(trigger);
// //                 trigger.OnEnter(this);
// //             }
// //             trigger.OnStay(this);
// //         }
// //         else if (trigger.Triggered)
// //         {
// //             triggersInside.Remove(trigger);
// //             trigger.Triggered = false;
// //             trigger.OnLeave(this);
// //         }
// //     }
// // }
// //             
// // //Strawberry Block
// // StrawberriesBlocked = CollideCheck<BlockField>();
// //    
//     // Entity 종류들. 
//     // Trigger, Killbox, Platform, WallBooster, JumpThru(우주같은 젤리물체), SwapBlock, FlyFeather
//     //  They all have CollideCheck
//     
//     public override void OnTouchingActor(Actor actor)
//     {
//         // onTouchingActor -> follow the actor
//     }
// }
//
//
