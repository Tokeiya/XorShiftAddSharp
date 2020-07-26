# XorShiftAddSharp

This is a pure C# implementation of a [XORSHIFT-ADD (XSadd)](http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/XSADD/index.html) pseudo-random number generator.



## Concept

- Small internal state size (128bit)
- Very long period(2^128-1)
- Save and restore internal state anytime.
- Can Retrieve the arbitrary n-step jumped state.



## Overview

- There is the implementation of almost same semantics of "[xsadd.c](https://github.com/MersenneTwister-Lab/XSadd/blob/master/xsadd.c)" and "[xsadd.h](https://github.com/MersenneTwister-Lab/XSadd/blob/master/xsadd.h)". ([XorShiftAddCore](https://github.com/Tokeiya/XorShiftAddSharp/blob/master/XorShiftAddSharp/XorShiftAddCore.cs))
- Implementation of Inherited by System.Random are available. ([XorShiftAdd](https://github.com/Tokeiya/XorShiftAddSharp/blob/master/XorShiftAddSharp/XorShiftAdd.cs))
- Xml documentations are available (but currently partially)
- Compared tests from original library are passed.