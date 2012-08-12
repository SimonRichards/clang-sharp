using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClangSharp;

namespace ClangSharp {
    public class Type {

        internal Interop.Type Native { get; private set; }

        internal Type(Interop.Type native) {
            Native = native;
        }

        public Type Canonical {
            get { return new Type(Interop.clang_getCanonicalType(Native)); }
        }

        public Type Pointee {
            get { return new Type(Interop.clang_getPointeeType(Native)); }
        }

        public Type Result {
            get { return new Type(Interop.clang_getResultType(Native)); }
        }

        public Cursor Declaration {
            get { return new Cursor(Interop.clang_getTypeDeclaration(Native)); }
        }

        public static string TypeKindSpelling(Kind kind) {
            return Interop.clang_getTypeKindSpelling(kind).ManagedString;
        }

        public bool Equals(Type other) {
            return Interop.clang_equalTypes(Native, other.Native) != 0;
        }

        public override bool Equals(object obj) {
            return obj is Type && Equals((Type)obj);
        }

        public override int GetHashCode() {
            return Native.GetHashCode();
        }

        public enum Kind {
            Invalid,
            Unexposed,
            Void,
            Bool,
            CharU,
            UChar,
            Char16,
            Char32,
            UShort,
            UInt,
            ULong,
            ULongLong,
            UInt128,
            CharS,
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
            FirstBuiltin = 2,
            LastBuiltin = 29,
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
}
