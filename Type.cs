namespace ClangSharp {
    public class Type {

        internal Interop.Type Native { get; private set; }

        internal Type(Interop.Type native) {
            Native = native;
        }

        public Type ArrayElementType
        {
            get { return new Type(Interop.clang_getArrayElementType(Native)); }
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

        public Kind TypeKind {
            get { return Native.kind; }
        }

        public string TypeKindSpelling {
            get {
                return Interop.clang_getTypeKindSpelling(TypeKind).ManagedString;
            }
        }

        public bool IsConstQualifiedType
        {
            get { return Interop.clang_isConstQualifiedType(Native) != 0; }
        }

        public bool IsRestrictQualifiedType
        {
            get { return Interop.clang_isRestrictQualifiedType(Native) != 0; }
        }

        public bool IsVolatileQualifiedType
        {
            get { return Interop.clang_isVolatileQualifiedType(Native) != 0; }
        }

        public bool IsFunctionTypeVariadic
        {
            get { return Interop.clang_isFunctionTypeVariadic(Native) != 0; }
        }

        public int NumArgTypes
        {
            get { return Interop.clang_getNumArgTypes(Native); }
        }

        public Type GetArgType(uint i)
        {
            return new Type(Interop.clang_getArgType(Native, i));
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
            FunctionProto,
            ConstantArray,
            Vector,
            IncompleteArray,
            VariableArray,
            DependentSizedArray,
            MemberPointer
        }

        public string Spelling {
            get {
                switch (TypeKind) {
                    case Kind.Void:
                        return "void";
                    case Kind.Bool:
                        return "bool";
                    case Kind.UChar:
                    case Kind.CharU:
                        return "unsigned char";
                    case Kind.Char16:
                        return "char16_t";
                    case Kind.Char32:
                        return "char32_t";
                    case Kind.UShort:
                        return "unsigned short int";
                    case Kind.UInt:
                        return "unsigned int";
                    case Kind.ULong:
                        return "unsigned long int";
                    case Kind.ULongLong:
                        return "unsigned long long int";
                    case Kind.UInt128:
                        return "uint128_t";
                    case Kind.CharS:
                        return "char";
                    case Kind.SChar:
                        return "signed char";
                    case Kind.WChar:
                        return "wchar_t";
                    case Kind.Short:
                        return "short int";
                    case Kind.Int:
                        return "int";
                    case Kind.Long:
                        return "long int";
                    case Kind.LongLong:
                        return "long long int";
                    case Kind.Int128:
                        return "int128_t";
                    case Kind.Float:
                        return "float";
                    case Kind.Double:
                        return "double";
                    case Kind.LongDouble:
                        return "long double";
                    case Kind.NullPtr:
                        return "nullptr";
                    case Kind.Pointer:
                        return Pointee.Spelling + "*";
                    default:
                        return Declaration.Spelling;
                }
            }
        }
    }
}