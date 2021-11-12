#pragma once

#include "..\include\vim.h"

#include <msclr/marshal_cppstd.h>
using namespace msclr::interop;

using namespace System;

namespace Vim
{
    public ref class ManagedVim
    {
    private:

    public:
        Vim::Scene* VimScene = nullptr;

        ManagedVim()
            : VimScene(new Vim::Scene)
        { }

        ~ManagedVim()
        {
            this->!ManagedVim();
        }

        !ManagedVim()
        {
            delete VimScene;
            VimScene = nullptr;
        }

        void Load(String^ filePath)
        {
            VimScene->ReadFile(marshal_as<std::string>(filePath));
        }

        int GetStringCount()
        {
            return (int)VimScene->mStrings.size();
        }

        String^ GetString(int index)
        {
            return gcnew String((char*)VimScene->mStrings[index]);
        }

        int GetNodeCount()
        {
            return -1;
        }

        int GetNodeParentIndex(int index)
        {
            return -1;
        }

        int GetNodeInstanceIndex(int index)
        {
            return -1;
        }

        int GetNodeGeometryIndex(int index)
        {
            return -1;
        }
    };
}
