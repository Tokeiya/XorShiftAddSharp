# XorShiftAddSharp
This is a pure C# implementation of a XORSHIFT-ADD (XSadd) pseudo-random number generator.



## Concept

- Small internal vector size (128bit)

- Very long period(2^128-1)

- Save and restore internal state anytime.

- Can Retrieve the arbitrary n-step jumped state.

  



## Overview

- There is the implementation of almost same semantics of "[xsadd.c](https://github.com/MersenneTwister-Lab/XSadd/blob/master/xsadd.c)" and "[xsadd.h](https://github.com/MersenneTwister-Lab/XSadd/blob/master/xsadd.h)". ([XorShiftAddCore](https://github.com/Tokeiya/XorShiftAddSharp/blob/master/XorShiftAddSharp/XorShiftAddCore.cs))
- There is the implementation of Inherited by System.Random. ([XorShiftAdd](https://github.com/Tokeiya/XorShiftAddSharp/blob/master/XorShiftAddSharp/XorShiftAdd.cs))
- Xml documentations are available (but partially XD)

