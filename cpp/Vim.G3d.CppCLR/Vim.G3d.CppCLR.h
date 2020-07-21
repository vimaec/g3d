#pragma once

#include "..\include\g3d.h"

#include <msclr/marshal_cppstd.h>
using namespace msclr::interop;

using namespace System;

namespace Vim
{
    namespace G3d
    {
        public ref class ManagedG3d
        {
        private:

        public:
            g3d::G3d* g3d;

            ManagedG3d()
                : g3d(new g3d::G3d)
            { }

            ~ManagedG3d()
            {
                this->!ManagedG3d();
            }

            !ManagedG3d()
            {
                delete g3d;
                g3d = nullptr;
            }

            void Load(String^ filePath)
            {
                g3d->read_file(marshal_as<std::string>(filePath));
            }

            void Write(String^ filePath)
            {
                g3d->write_file(marshal_as<std::string>(filePath));
            }

            int Count()
            {
                return (int)g3d->attributes.size();
            }

            String^ AttributeName(int index)
            {
                return gcnew String(g3d->attributes[index].descriptor.to_string().c_str());
            }

            int AttributeElementCount(int index)
            {
                return (int)g3d->attributes[index].num_elements();
            }
        };
    }
}
