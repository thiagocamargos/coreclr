// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Threading;

class Gen<T> 
{
	public static void Target(object p)
	{		
			ManualResetEvent evt = (ManualResetEvent) p;
			Interlocked.Increment(ref Test.Xcounter);
			evt.Set();
	}
	public static void ThreadPoolTest()
	{
		ManualResetEvent[] evts = new ManualResetEvent[Test.nThreads];
		WaitHandle[] hdls = new WaitHandle[Test.nThreads];

		for (int i=0; i<Test.nThreads; i++)
		{
			evts[i] = new ManualResetEvent(false);
			hdls[i] = (WaitHandle) evts[i];
		}

		Gen<T> obj = new Gen<T>();

		for (int i = 0; i < Test.nThreads; i++)
		{	
			WaitCallback cb = new WaitCallback(Gen<T>.Target);
			ThreadPool.QueueUserWorkItem(cb,evts[i]);
		}

		WaitHandle.WaitAll(hdls);
		Test.Eval(Test.Xcounter==Test.nThreads);
		Test.Xcounter = 0;
	}
}

public class Test
{
	public static int nThreads = 50;
	public static int counter = 0;
	public static int Xcounter = 0;
	public static bool result = true;
	public static void Eval(bool exp)
	{
		counter++;
		if (!exp)
		{
			result = exp;
			Console.WriteLine("Test Failed at location: " + counter);
		}
	
	}
	
	public static int Main()
	{
		Gen<int>.ThreadPoolTest();
		Gen<double>.ThreadPoolTest();
		Gen<string>.ThreadPoolTest();
		Gen<object>.ThreadPoolTest(); 
		Gen<Guid>.ThreadPoolTest(); 

		Gen<int[]>.ThreadPoolTest(); 
		Gen<double[,]>.ThreadPoolTest();
		Gen<string[][][]>.ThreadPoolTest(); 
		Gen<object[,,,]>.ThreadPoolTest();
		Gen<Guid[][,,,][]>.ThreadPoolTest();

		if (result)
		{
			Console.WriteLine("Test Passed");
			return 100;
		}
		else
		{
			Console.WriteLine("Test Failed");
			return 1;
		}
	}
}		


