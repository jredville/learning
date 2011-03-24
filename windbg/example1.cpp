// WindbgEx1.cpp : Defines the entry point for the console application.
//
#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
#include <tchar.h>
#include <Windows.h>
#include <stdio.h>
#include <stdlib.h>


// TODO: reference any additional headers you need in STDAFX.H
// and not in this file
int g_wow = 0;

CRITICAL_SECTION cs;

/*
The first thread enters a CS and runs away. The following shows how you can debug such a problem.

0:001> ~*kb

   0  Id: 894.848 Suspend: 1 Teb: 7ffde000 Unfrozen
ChildEBP RetAddr  Args to Child              
0012fde4 77f5c534 77f69f68 000007f4 00000000 SharedUserData!SystemCallStub+0x4
0012fde8 77f69f68 000007f4 00000000 00000000 ntdll!ZwWaitForSingleObject+0xc
0012fe70 77f5b2e6 00446dc8 0041531f 00446dc8 ntdll!RtlpWaitForCriticalSection+0x125
0012fe78 0041531f 00446dc8 00000000 00000002 ntdll!RtlEnterCriticalSection+0x46
0012ff74 0041579f 00000001 003235c0 00323608 WindbgEx1!main+0x8f
0012ffc0 77e814c7 00000000 00000002 7ffdf000 WindbgEx1!mainCRTStartup+0x1cf
0012fff0 00000000 0041458c 00000000 78746341 kernel32!BaseProcessStart+0x23

#  1  Id: 894.e9c Suspend: 1 Teb: 7ffdd000 Unfrozen
ChildEBP RetAddr  Args to Child              
0034ffc8 77f5f31f 00000005 00000004 00000001 ntdll!DbgBreakPoint
0034fff4 00000000 00000000 00000000 00000000 ntdll!DbgUiRemoteBreakin+0x2d
0:001> dt 00446dc8
cs
   +0x000 DebugInfo        : 0x77fc4240 
   +0x004 LockCount        : 1
   +0x008 RecursionCount   : 1
   +0x00c OwningThread     : 0x000005dc 
   +0x010 LockSemaphore    : 0x000007f4 
   +0x014 SpinCount        : 0
0:001> ~
   0  Id: 894.848 Suspend: 1 Teb: 7ffde000 Unfrozen
.  1  Id: 894.e9c Suspend: 1 Teb: 7ffdd000 Unfrozen

The owning thread (0x5dc) does not show up in the threads list => a thread has locked the critical section and exitted.
*/

// Acquire a critSec and run away.
DWORD WINAPI newThreadProc(LPVOID lpParameter)
{
    InitializeCriticalSection(&cs);
    EnterCriticalSection(&cs);
    return 0;
}

/*
Example1: application hang

This program hangs because a thread acquires a critical section and exits without releasing it and 
another thread waits forever on that CS.
*/
void Example1()
{
    DWORD tid = 0;
    HANDLE hThread = CreateThread(NULL, 0, newThreadProc, NULL, 0, &tid);   // let the thread execute immediately    

    WaitForSingleObject(hThread, INFINITE);
    EnterCriticalSection(&cs);
}

/*
Example2: Exception: Divide by zero

Commands you want to execute:
- .lastevent [o/p: Integer divide-by-zero],
OR:
- u [output: idiv ecx]
- r ecx [o/p: 0]
=> divide by zero	
*/

void Example2()
{
    int y = 71;
    y = y / (71 - y);
}

/*
Example3: Execute a command every time a breakpoint is hit.

bp WindbgEx1!Example3+0x3d "dd [ebp-0x14] L1; .echo hello world;g" will print value of i in every iteration
*/
void Example3()
{
    int k = 0;
    for (int i = 0; i < 100; ++i)
    {
        // bp WindbgEx1!Example3+0x3d "dd k L1; g" will print value of k in every iteration
        k = rand();
    }
}

/*
Example4: Exception: null pointer access

Run under windbg; the debugger will report a first chance exception and this asm instruction in x86:
- mov  dword ptr [eax],0x50 ds:0023:00000000=????????
- 0:000> r eax
eax=00000000

If you turn on source debugging, the line *i = 80 will be highlighted.
*/
void Example4()
{
    int* i = NULL;
    *i = 80;
}

/*
Example5: Exception: double deletion
*/
void Example5()
{
    char* str = new char[20];
    delete [] str;
    delete [] str;     // error!
    strcpy(str, "hi");
}

/*
Example6: Exception: stack overflow due to infinite recursion
If you run under the debugger, the debugger will get first and second chance stack overflow exceptions.
*/
void Example6_1();
void Example6()
{
    Example6_1();
}

void Example6_1()
{
    Example6();
}

void Example7()
{
    int i = 5, j = 7;
    j = 5 + 7;
}

int _tmain(int argc, const char* argv[])
{
    if (argc > 1 && argv)
    {
        int which = atoi(argv[1]);
        switch (which)
        {
        case 1:
            Example1();
            break;

        case 2:
            Example2();
            break;

        case 3:
            Example3();
            break;

        case 4:
            Example4();
            break;

        case 5:
            Example5();
            break;

        case 6:
            Example6();
            break;

        case 7:
            Example7();
            break;
        }
    }
	return 0;
}
