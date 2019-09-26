// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"


#include "..\include\g3d.h"

#pragma managed(push, on)

namespace G3d
{
	class Test
	{
	public:
		static const char* name() {
			return "me";
		}
	};
}

#pragma managed(pop)

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

