using System;

namespace Vim.G3d
{
    [Flags]
    public enum AttributeDescriptorErrors
    {
        None = 0,
        UnexpectedNumberOfTokens = 1,
        PrefixError = 1 << 1,
        AssociationError = 1 << 2,
        SemanticError = 1 << 3,
        IndexError = 1 << 4,
        DataTypeError = 1 << 5,
        DataArityError = 1 << 6,
    }
}
