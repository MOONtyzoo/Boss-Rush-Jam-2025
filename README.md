# RoomSpin
 
_IMPORTANT NOTE: Originally, this project was coded using Unity VCS. I ported it to GitHub to make the code publically accessable, but as a result, there is not a visible commit history or ability to see the contributions of the other members. In this README I'll explain my contributions._

This was a project that we created for the 2025 Boss Rush Game Jam. My contributions were that I served as the lead gameplay programmer and artist. My biggest achievement was that I created a complex player controller using a state machine and C# events (more details below).

Play it here: https://moontyzoo.itch.io/room-spin
![msedge_8GW3P6GvM9](https://github.com/user-attachments/assets/1a939c62-f4a9-458f-8f11-d695f88f468a)

My Contributions:

**Player Controller**

For the player controller, I followed clean code principles to refactor the player into multiple scripts. The PlayerController is a StateMachine that manages and connects the various
state scripts (such as grounded, aeriel, grappling, wall sliding, etc..). Inside of the player controller are tons of events that fire based on the actions the player performs, such
as jumping, shooting, etc...

There are various other scripts attached to the player, such as PlayerAnimator, PlayerSounds, and PlayerVFX which are responsible for controlling their respective effects by listening
to events fired from the PlayerController script and performing the corresponding actions. The that this is set up makes the PlayerController script non-dependent on the animations, sounds,
or vfx, and all of these scripts can hypothetically be removed and the base functionality will still work.

**Artwork/Animation Integration**

Initially, we were supposed to be working with a dedicated artist, and I would have the role of integrating their art and animations into the game. Due to some complications, this member was
unable to participate in the game jam. In his place, I took the role as the artist since I have some pixel art skills, but unfortunately that meant splitting my effort and being unable to complete
more than one boss.

I drew the art for the background, player character, and mole boss. For the animations, I used a unique combination of frame-based animations combined with squash and stretch effects achieved
by scaling the transform in the Unity Animator. I also integrated these animations in the corresponding PlayerAnimator and MadMoleAnimator scripts.
