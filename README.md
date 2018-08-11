# My-First-C-Project
This is a C# project which simulate ball collision/bumping.  
How to use:
1. press [F2] to invoke the controller. 
2. press [space] to freeze the ball motion.

Included physics effect：
1. Gravity
    a. linear gravity with controllable strength and angle.
    b. conentric gravity with controllable strength.
    c. Gravity created by each ball. To make this effect visible, you have to increase the mass of the ball, say 500000.
2. Ball collision.
   -To describe ball collison, there are two different algorithms defined in this project.
   a. Hard ball collision. The momentum after collisoin is calculated using momentum/kinetic energy conservation law. Overlapping between       balls are forbidden.
   b. Using impulse-momentum law. Two colliding balls are allowed to go into each other. The repulsive force can be derived from the             amount of overlapping by F = k*l_overlapping, k is hooker constant of the ball, l_overlapping is the amount of overlapping.

    
