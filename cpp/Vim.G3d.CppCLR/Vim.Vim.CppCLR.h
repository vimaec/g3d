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
            return (int)VimScene->mNodes.size();
        }

        int GetNodeParentIndex(int index)
        {
            return (int)VimScene->mNodes[index].mParent;
        }

        int GetNodeInstanceIndex(int index)
        {
            return (int)VimScene->mNodes[index].mInstance;
        }

        int GetNodeGeometryIndex(int index)
        {
            return (int)VimScene->mNodes[index].mGeometry;
        }
    };
}
