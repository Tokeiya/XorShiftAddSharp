# XorShiftAddSharp

This is a pure C# implementation of a [XORSHIFT-ADD (XSadd)](http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/XSADD/index.html) pseudo-random number generator.



## Concept

- Small internal state size (128bit)
- Long period(2^128-1)
- Save and restore internal state anytime.
- Can Retrieve the arbitrary n-step jumped state.



## Overview

- There is the implementation of almost same semantics of "[xsadd.c](https://github.com/MersenneTwister-Lab/XSadd/blob/master/xsadd.c)" and "[xsadd.h](https://github.com/MersenneTwister-Lab/XSadd/blob/master/xsadd.h)". ([XorShiftAddCore](https://github.com/Tokeiya/XorShiftAddSharp/blob/master/XorShiftAddSharp/XorShiftAddCore.cs))
- There is the implementation of Inherited by System.Random. ([XorShiftAdd](https://github.com/Tokeiya/XorShiftAddSharp/blob/master/XorShiftAddSharp/XorShiftAdd.cs))
- Xml documentations are available (but currently partially)
- Compared tests from original library are passed.



## Change Log

### ver0.1

Initial release.



### ver 0.2

Changed XorShiftAdd psesudo-random generator's internal state from `Span<uint>` to `InternalState` struct.
Unfortunately this is a devastating changes from ver 0.1 .



### ver 0.2.1

Fix: XorShiftAddCore and XorShiftAdd class Jump method's  baseStep parameter can now be prefixed with "0x" or "0X".
(Previous Jump method could be accept prefix, but CalculateJumpPolynomial couldn't be accept prefix.)