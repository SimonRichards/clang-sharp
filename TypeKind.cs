using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClangSharp {
    public enum TypeKind {
        Invalid = 0,
        Unexposed,
        Void,
        Bool,
        Char_U,
        UChar,
        Char16,
        Char32,
        UShort,
        UInt,
        ULong,
        ULongLong,
        UInt128,
        Char_S,
        SChar,
        WChar,
        Short,
        Int,
        Long,
        LongLong,
        Int128,
        Float,
        Double,
        LongDouble,
        NullPtr,
        Overload,
        Dependent,
        ObjCId,
        ObjCClass,
        ObjCSel,

        FirstBuiltin = Void,
        LastBuiltin = ObjCSel,

        Complex = 100,
        Pointer,
        BlockPointer,
        LValueReference,
        RValueReference,
        Record,
        Enum,
        Typedef,
        ObjCInterface,
        ObjCObjectPointer,
        FunctionNoProto,
        FunctionProto
    }
}
