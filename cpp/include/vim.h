/*
    VIM Data Format
    Copyright 2019, VIMaec LLC
    Copyright 2018, Ara 3D, Inc.
    Usage licensed under terms of MIT Licenese.
*/
#ifndef __VIM_H__
#define __VIM_H__

#include <vector>
#include <sstream>
#include <unordered_map>
#include <tuple>
#include <stdexcept>

#include "g3d.h"

namespace Vim
{
    class SerializableProperty
    {
    public:
        /// <summary>
        /// The index of the entity that the property is matched to 
        /// </summary>
        int mEntityId;

        /// <summary>
        /// The string index of the property name
        /// </summary>
        int mName;

        /// <summary>
        /// The string index of the property value
        /// </summary>
        int mValue;
    };

    class EntityTable
    {
    public:
        std::string mName;

        std::unordered_map<std::string, std::vector<int>> mIndexColumns;
        std::unordered_map<std::string, std::vector<int>> mStringColumns;
        std::unordered_map<std::string, std::vector<double>> mNumericColumns;
        std::vector<SerializableProperty> mProperties;
    };

    inline std::vector<std::string> split(const std::string& str, const std::string& delim)
    {
        std::vector<std::string> tokens;
        size_t prev = 0, pos = 0;
        do
        {
            pos = str.find(delim, prev);
            if (pos == std::string::npos)
            {
                pos = str.length();
            }

            std::string token = str.substr(prev, pos - prev);
            if (!token.empty())
            {
                tokens.push_back(token);
            }

            prev = pos + delim.length();
        } while (pos < str.length() && prev < str.length());

        return tokens;
    }

    enum class VimErrorCodes
    {
        Success = 0,
        Failed = -1,
        NoVersionInfo = -2,
        FileNotRecognized = -3,
        GeometryLoadingException = -4,
        AssetLoadingException = -5,
        EntityLoadingException = -6
    };

    class Scene
    {
    public:
        bfast::Bfast mBfast;
        bfast::Bfast mGeometryBFast;
        bfast::Bfast mAssetsBFast;
        bfast::Bfast mEntitiesBFast;
        std::vector<const bfast::byte*> mStrings;
        g3d::G3d mGeometry;
        std::unordered_map<std::string, EntityTable> mEntityTables;
        std::unordered_map<std::string, std::string> mHeader;

        uint32_t mVersionMajor = 0xffffffff;
        uint32_t mVersionMinor = 0xffffffff;
        uint32_t mVersionPatch = 0xffffffff;

        VimErrorCodes ReadFile(std::string fileName)
        {
            try
            {
                mBfast = bfast::Bfast::read_file(fileName);
            }
            catch (std::exception& e)
            {
                e;
                return VimErrorCodes::FileNotRecognized;
            }

            for (auto i = 0; i < mBfast.buffers.size(); ++i)
            {
                auto& b = mBfast.buffers[i];
                if (b.name == "header")
                {
                    std::vector<std::string> versionParts;

                    std::string header = (const char*)(b.data.begin());
                    std::vector<std::string> tokens = split(header, "\n");

                    for (size_t i = 0; i < tokens.size(); i ++)
                    {
                        std::vector<std::string> keyValue = split(tokens[i], "=");

                        if (keyValue.size() == 2)
                        {
                            mHeader[keyValue[0]] = keyValue[1];
                        }
                    }

                    if (mHeader.end() != mHeader.find("vim"))
                    {
                        versionParts = split(mHeader["vim"], ".");
                    }
                    else
                    {
                        // No vim version found
                        return VimErrorCodes::NoVersionInfo;
                    }

                    if (versionParts.size() > 0) mVersionMajor = std::stoi(versionParts[0]);
                    if (versionParts.size() > 1) mVersionMinor = std::stoi(versionParts[1]);
                    if (versionParts.size() > 2) mVersionPatch = std::stoi(versionParts[2]);
                }
                else if (b.name == "geometry")
                {
                    try
                    {
                        mGeometryBFast = bfast::Bfast::unpack(b.data);
                        mGeometry = std::move(g3d::G3d(mGeometryBFast));
                    }
                    catch (std::exception& e)
                    {
                        return Vim::VimErrorCodes::GeometryLoadingException;
                    }
                }
                else if (b.name == "assets")
                {
                    try
                    {
                        mAssetsBFast = bfast::Bfast::unpack(b.data);
                    }
                    catch (std::exception& e)
                    {
                        return Vim::VimErrorCodes::AssetLoadingException;
                    }
                }
                else if (b.name == "strings")
                {
                    const bfast::byte* data = b.data.begin();
                    size_t count = 0;
                    while (data < b.data.end())
                    {
                        count++;
                        data += strlen((const char*)data) + 1;
                    }

                    mStrings.resize(count);
                    count = 0;
                    data = b.data.begin();
                    while (data < b.data.end())
                    {
                        mStrings[count++] = data;
                        data += strlen((const char*)data) + 1;
                    }
                }
                else if (b.name == "entities")
                {
                    try
                    {
                        mEntitiesBFast = bfast::Bfast::unpack(b.data);
                        for (auto j = 0; j < mEntitiesBFast.buffers.size(); ++j)
                        {
                            auto& entityBuffer = mEntitiesBFast.buffers[j];
                            EntityTable entityTable = { entityBuffer.name };
                            bfast::Bfast tableBFast = bfast::Bfast::unpack(entityBuffer.data);

                            for (auto k = 0; k < tableBFast.buffers.size(); ++k)
                            {
                                auto& tableBuffer = tableBFast.buffers[k];

                                if (tableBuffer.name == "properties")
                                {
                                    entityTable.mProperties = std::vector<SerializableProperty>((SerializableProperty*)tableBuffer.data.begin(), (SerializableProperty*)tableBuffer.data.end());
                                }
                                else
                                {
                                    size_t index = tableBuffer.name.find_first_of(':');
                                    std::string type = tableBuffer.name.substr(0, index);
                                    std::string name = tableBuffer.name.substr(index + 1);

                                    if (type == "numeric")
                                    {
                                        entityTable.mNumericColumns[name] = std::vector<double>((double*)tableBuffer.data.begin(), (double*)tableBuffer.data.end());
                                    }
                                    else if (type == "index")
                                    {
                                        entityTable.mIndexColumns[name] = std::vector<int>((int*)tableBuffer.data.begin(), (int*)tableBuffer.data.end());
                                    }
                                    else if (type == "string")
                                    {
                                        entityTable.mStringColumns[name] = std::vector<int>((int*)tableBuffer.data.begin(), (int*)tableBuffer.data.end());
                                    }
                                }
                            }

                            mEntityTables[entityTable.mName] = entityTable;
                        }
                    }
                    catch (std::exception& e)
                    {
                        return Vim::VimErrorCodes::EntityLoadingException;
                    }
                }
            }
            return VimErrorCodes::Success;
        }
    };

}

#endif
